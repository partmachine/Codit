﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace Codit.AspNetCore.Microsoft.Graph
{
    public class GraphClient
    {
        private const string ObjectIdentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        GraphServiceClient _client;
        IHttpContextAccessor _accessor;
        string _userObjectId;
        string _email;

        public GraphClient(IGraphClientFactory factory, IHttpContextAccessor accessor)
        {
            _accessor = accessor;

            if (accessor.HttpContext.User.Identity.IsAuthenticated)
            {
                _userObjectId = accessor.HttpContext.User.FindFirst(c => c.Type == ObjectIdentifierClaimType).Value;
                _email = accessor.HttpContext.User.FindFirst(c => c.Type == "email")?.Value ?? "";
                _client = factory.CreateClient(_userObjectId);                
            }
        }

        public async Task<string> ReadMail()
        {
            try
            {
                var messages = await _client.Me.MailFolders.Inbox.Messages.Request().GetAsync();

                var mail = JsonConvert.SerializeObject(messages, Formatting.Indented);

                return mail;
            }
            catch (Exception e)
            {
                var m = e.Message;
            }

            return string.Empty;
        }

        // Load user's profile in formatted JSON.
        public async Task<string> GetUserJson()
        {            
            try
            {
                // Load user profile.
                var user = await _client.Users[_email].Request().GetAsync();
                return JsonConvert.SerializeObject(user, Formatting.Indented);
            }
            catch (ServiceException e)
            {
                switch (e.Error.Code)
                {
                    case "Request_ResourceNotFound":
                    case "ResourceNotFound":
                    case "ErrorItemNotFound":
                    case "itemNotFound":
                        return JsonConvert.SerializeObject(new { Message = $"User '{_email}' was not found." }, Formatting.Indented);
                    case "ErrorInvalidUser":
                        return JsonConvert.SerializeObject(new { Message = $"The requested user '{_email}' is invalid." }, Formatting.Indented);
                    case "AuthenticationFailure":
                        return JsonConvert.SerializeObject(new { e.Error.Message }, Formatting.Indented);
                    case "TokenNotFound":
                        await _accessor.HttpContext.ChallengeAsync();
                        return JsonConvert.SerializeObject(new { e.Error.Message }, Formatting.Indented);
                    default:
                        return JsonConvert.SerializeObject(new { Message = "An unknown error has occurred." }, Formatting.Indented);
                }
            }
        }

        // Load user's profile picture in base64 string.
        public async Task<string> GetPictureBase64()
        {
            try
            {
                // Load user's profile picture.
                var pictureStream = await GetPictureStream();

                // Copy stream to MemoryStream object so that it can be converted to byte array.
                var pictureMemoryStream = new MemoryStream();
                await pictureStream.CopyToAsync(pictureMemoryStream);

                // Convert stream to byte array.
                var pictureByteArray = pictureMemoryStream.ToArray();

                // Convert byte array to base64 string.
                var pictureBase64 = Convert.ToBase64String(pictureByteArray);

                return "data:image/jpeg;base64," + pictureBase64;
            }
            catch (Exception e)
            {
                switch (e.Message)
                {
                    case "ResourceNotFound":
                        // If picture not found, return the default image.
                        return "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4NCjwhRE9DVFlQRSBzdmcgIFBVQkxJQyAnLS8vVzNDLy9EVEQgU1ZHIDEuMS8vRU4nICAnaHR0cDovL3d3dy53My5vcmcvR3JhcGhpY3MvU1ZHLzEuMS9EVEQvc3ZnMTEuZHRkJz4NCjxzdmcgd2lkdGg9IjQwMXB4IiBoZWlnaHQ9IjQwMXB4IiBlbmFibGUtYmFja2dyb3VuZD0ibmV3IDMxMi44MDkgMCA0MDEgNDAxIiB2ZXJzaW9uPSIxLjEiIHZpZXdCb3g9IjMxMi44MDkgMCA0MDEgNDAxIiB4bWw6c3BhY2U9InByZXNlcnZlIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPg0KPGcgdHJhbnNmb3JtPSJtYXRyaXgoMS4yMjMgMCAwIDEuMjIzIC00NjcuNSAtODQzLjQ0KSI+DQoJPHJlY3QgeD0iNjAxLjQ1IiB5PSI2NTMuMDciIHdpZHRoPSI0MDEiIGhlaWdodD0iNDAxIiBmaWxsPSIjRTRFNkU3Ii8+DQoJPHBhdGggZD0ibTgwMi4zOCA5MDguMDhjLTg0LjUxNSAwLTE1My41MiA0OC4xODUtMTU3LjM4IDEwOC42MmgzMTQuNzljLTMuODctNjAuNDQtNzIuOS0xMDguNjItMTU3LjQxLTEwOC42MnoiIGZpbGw9IiNBRUI0QjciLz4NCgk8cGF0aCBkPSJtODgxLjM3IDgxOC44NmMwIDQ2Ljc0Ni0zNS4xMDYgODQuNjQxLTc4LjQxIDg0LjY0MXMtNzguNDEtMzcuODk1LTc4LjQxLTg0LjY0MSAzNS4xMDYtODQuNjQxIDc4LjQxLTg0LjY0MWM0My4zMSAwIDc4LjQxIDM3LjkgNzguNDEgODQuNjR6IiBmaWxsPSIjQUVCNEI3Ii8+DQo8L2c+DQo8L3N2Zz4NCg==";
                    case "EmailIsNull":
                        return JsonConvert.SerializeObject(new { Message = "Email address cannot be null." }, Formatting.Indented);
                    default:
                        return null;
                }
            }
        }

        public async Task<Stream> GetPictureStream()
        {
            if (_email == null) throw new Exception("EmailIsNull");

            Stream pictureStream = null;

            try
            {
                try
                {
                    // Load user's profile picture.
                    pictureStream = await _client.Users[_email].Photo.Content.Request().GetAsync();
                }
                catch (ServiceException e)
                {
                    if (e.Error.Code == "GetUserPhoto") // User is using MSA, we need to use beta endpoint
                    {
                        // Set Microsoft Graph endpoint to beta, to be able to get profile picture for MSAs 
                        _client.BaseUrl = "https://graph.microsoft.com/beta";

                        // Get profile picture from Microsoft Graph
                        pictureStream = await _client.Users[_email].Photo.Content.Request().GetAsync();

                        // Reset Microsoft Graph endpoint to v1.0
                        _client.BaseUrl = "https://graph.microsoft.com/v1.0";
                    }
                    else if (e.Error.Code == "ErrorItemNotFound")
                    {
                        throw new Exception("ResourceNotFound");
                    }
                }
            }
            catch (ServiceException e)
            {
                switch (e.Error.Code)
                {
                    case "Request_ResourceNotFound":
                    case "ResourceNotFound":
                    case "ErrorItemNotFound":
                    case "itemNotFound":
                    case "ErrorInvalidUser":
                        // If picture not found, return the default image.
                        throw new Exception("ResourceNotFound");
                    case "TokenNotFound":
                        await _accessor.HttpContext.ChallengeAsync();
                        return null;
                    default:
                        return null;
                }
            }

            return pictureStream;
        }
        public async Task<Stream> GetMyPictureStream()
        {
            Stream pictureStream = null;

            try
            {
                try
                {
                    // Load user's profile picture.
                    pictureStream = await _client.Me.Photo.Content.Request().GetAsync();
                }
                catch (ServiceException e)
                {
                    if (e.Error.Code == "GetUserPhoto") // User is using MSA, we need to use beta endpoint
                    {
                        // Set Microsoft Graph endpoint to beta, to be able to get profile picture for MSAs 
                        _client.BaseUrl = "https://graph.microsoft.com/beta";

                        // Get profile picture from Microsoft Graph
                        pictureStream = await _client.Me.Photo.Content.Request().GetAsync();

                        // Reset Microsoft Graph endpoint to v1.0
                        _client.BaseUrl = "https://graph.microsoft.com/v1.0";
                    }
                    else if (e.Error.Code == "ErrorItemNotFound")
                    {
                        throw new Exception("ResourceNotFound");
                    }
                }
            }
            catch (ServiceException e)
            {
                switch (e.Error.Code)
                {
                    case "Request_ResourceNotFound":
                    case "ResourceNotFound":
                    case "ErrorItemNotFound":
                    case "ItemNotFound":
                    case "ErrorInvalidUser":
                        // If picture not found, return the default image.
                        throw new Exception("ResourceNotFound");
                    case "TokenNotFound":
                        await _accessor.HttpContext.ChallengeAsync();
                        return null;
                    default:
                        return null;
                }
            }

            return pictureStream;
        }

        // Send an email message from the current user.
        public async Task SendEmail(IHostingEnvironment hostingEnvironment, string recipients)
        {
            if (recipients == null) return;

            var attachments = new MessageAttachmentsCollectionPage();

            try
            {
                // Load user's profile picture.
                var pictureStream = await GetMyPictureStream();

                // Copy stream to MemoryStream object so that it can be converted to byte array.
                var pictureMemoryStream = new MemoryStream();
                await pictureStream.CopyToAsync(pictureMemoryStream);

                // Convert stream to byte array and add as attachment.
                attachments.Add(new FileAttachment
                {
                    ODataType = "#microsoft.graph.fileAttachment",
                    ContentBytes = pictureMemoryStream.ToArray(),
                    ContentType = "image/png",
                    Name = "me.png"
                });
            }
            catch (Exception e)
            {
                switch (e.Message)
                {
                    case "ResourceNotFound":
                        break;
                    default:
                        throw;
                }
            }

            // Prepare the recipient list.
            var splitRecipientsString = recipients.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var recipientList = splitRecipientsString.Select(recipient => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = recipient.Trim()
                }
            }).ToList();

            // Build the email message.
            var email = new Message
            {
                Body = new ItemBody
                {
                    Content = System.IO.File.ReadAllText(hostingEnvironment.WebRootPath + "/email_template.html"),
                    ContentType = BodyType.Html,
                },
                Subject = "Sent from the Microsoft Graph Connect sample",
                ToRecipients = recipientList,
                Attachments = attachments
            };

            await _client.Me.SendMail(email, true).Request().PostAsync();
        }
    }
}
