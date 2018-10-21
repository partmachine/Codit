using Microsoft.Graph;
using System;
using System.Net.Http.Headers;

namespace Codit.Azure.Microsoft.Graph
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGraphClientFactory
    {
        GraphServiceClient CreateClient(string userId);
    }

    /// <summary>
    /// 
    /// </summary>
    public class GraphClientFactory : IGraphClientFactory
    {
        private readonly IGraphAuthProvider _authProvider;
        private GraphServiceClient _graphClient;

        public GraphClientFactory(IGraphAuthProvider authProvider)
        {
            _authProvider = authProvider;
        }

        // Get an authenticated Microsoft Graph Service client.
        public GraphServiceClient CreateClient(string userId)
        {
            _graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(
                async requestMessage =>
                {
                    // Passing tenant ID to the sample auth provider to use as a cache key
                    var accessToken = await _authProvider.GetUserAccessTokenAsync(userId);

                    // Append the access token to the request
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // This header identifies the sample in the Microsoft Graph service. If extracting this code for your project please remove.
                    requestMessage.Headers.Add("SampleID", "aspnetcore-connect-sample");
                }));

            return _graphClient;
        }
    }
    
}
