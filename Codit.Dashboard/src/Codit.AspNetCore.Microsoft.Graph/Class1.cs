using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Codit.AspNetCore.Microsoft.Graph
{
    public static class MicrosoftGraphServiceCollectionExtensions
    {
        public static IServiceCollection AddMicrosoftGraph(this IServiceCollection services)
        {
            services.AddSingleton<IGraphAuthProvider,GraphAuthProvider>();
            services.AddSingleton<IGraphClientFactory, GraphClientFactory>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient(typeof(GraphClient));

            return services;
        }
    }
}
