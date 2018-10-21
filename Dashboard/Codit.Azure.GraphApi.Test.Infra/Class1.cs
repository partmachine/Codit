using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Codit.Azure.GraphApi.Test.Infra
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
            get { return string.Format("{0}/{1}/", AADInstance, Domain); }
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

        // The resource identifier for the Microsoft Graph
        public static string GraphResource
        {
            get { return "https://graph.windows.net/"; }
        }

        // The Microsoft Graph version, can be "v1.0" or "beta"
        public static string GraphVersion
        {
            get { return "v1.6"; }
        }

        // Get an access token for the Microsoft Graph using ADAL
        public static async Task<string> GetAccessToken()
        {
            var platformParameters = new PlatformParameters(PromptBehavior.Auto);

            var cac = new ClientCredential(ClientId, ClientSecret);

            // Create the authentication context (ADAL)
            var authenticationContext = new AuthenticationContext(Authority);

            // Get the access token
            var authenticationResult = await authenticationContext.AcquireTokenAsync(GraphResource,
                ClientId, RedirectUri, platformParameters); 

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
}
