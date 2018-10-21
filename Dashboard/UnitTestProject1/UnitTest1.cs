using Codit.Azure.GraphApi;
using Codit.Azure.GraphApi.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Refit;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using System.Linq;
using System.Security;
using System.Net;

namespace UnitTestProject1
{
    public static class AzureClient
    {
        // The Azure AD instance where you domain is hosted
        public static string AADInstance
        {
            get { return "https://login.microsoftonline.com"; }
        }

        // The Office 365 domain (e.g. contoso.microsoft.com)
        public static string Domain
        {
            get { return "renevangeffenlive.onmicrosoft.com"; }
        }

        // The authority for authentication; combining the AADInstance
        // and the domain.
        public static string Authority
        {
            get { return string.Format("{0}/{1}/v2.0", AADInstance, Domain); }
        }

        public static string TenantId
        {
            get { return "5f84c940-0e0c-4131-a617-f89dd7caca05"; }
        }

        // The client Id of your native Azure AD application
        public static string ClientId
        {
            get { return "5f84c940-0e0c-4131-a617-f89dd7caca05"; }
        }

        public static string ClientSecret { get { return "7NJLKcGuahdqyf3DwRz01WlHfk8m5ks46Qz+GUG43FM="; } }

        // The redirect URI specified in the Azure AD application
        public static Uri RedirectUri
        {
            get { return new Uri("https://localhost:44303/signin-oidc"); }
        }
        static string[] _scopes = { "api://2997b735-e278-4408-881f-90bea0c85207/access_as_user", "user.read" };    // The resource identifier for the Microsoft Graph
        public static string[] Scopes
        {
            get { return new[] { "api://2997b735-e278-4408-881f-90bea0c85207/access_as_user", "user.read" };
            }
        }

        // The Microsoft Graph version, can be "v1.0" or "beta"
        public static string GraphVersion
        {
            get { return "v1.6"; }
        }

        // Get an access token for the Microsoft Graph using ADAL
        public static async Task<string> GetAccessToken()
        {
            var cred = new NetworkCredential("","Lonestar1");
            PublicClientApplication app = new PublicClientApplication(ClientId,Authority);

            var accounts = await app.GetAccountsAsync();

            var authenticationResult = await app.AcquireTokenByUsernamePasswordAsync(_scopes, "administrator@renevangeffenlive.onmicrosoft.com ", cred.SecurePassword);

            //await authenticationContext.AcquireTokenAsync(GraphResource,
            //    cac);

            return authenticationResult.AccessToken;
        }

        // Prepare an HttpClient with the an authorization header (access token)
        public static HttpClient GetHttpClient(string accessToken)
        {
            // Create the HTTP client with the access token
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",
                accessToken);
            return httpClient;
        }
    }

    class AzureAdHttpClientHandler : HttpClientHandler
    {
        private readonly Func<Task<string>> getToken;

        public AzureAdHttpClientHandler(Func<Task<string>> getToken)
        {
            if (getToken == null) throw new ArgumentNullException(nameof(getToken));
            this.getToken = getToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // See if the request has an authorize header
            var auth = request.Headers.Authorization;
            if (auth != null)
            {
                var token = await getToken().ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestApiGeneration()
        {
            var token =  await AzureClient.GetAccessToken();

            ApiQueryParams apiversion = new ApiQueryParams { Version = "1.6" };
           

            var api = RestService.For<IGraphApi>(new HttpClient(new AzureAdHttpClientHandler(AzureClient.GetAccessToken)) { BaseAddress = new Uri("https://graph.windows.net/") });
            try
            {
                var users = await api.GetUsers(AzureClient.Domain, apiversion);

            }
            catch (ApiException e)
            {
                var error = JsonConvert.DeserializeObject<ODataError>(e.Content);
            }
        }
    }
}
