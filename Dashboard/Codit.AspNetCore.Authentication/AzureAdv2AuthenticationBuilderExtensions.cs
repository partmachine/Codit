using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Caching.Memory;

namespace Codit.AspNetCore.Authentication
{

    public static class AzureAdv2AuthenticationBuilderExtensions
    {
        public const string ObjectIdentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
            => builder.AddAzureAdv2(_ => { });

        public static AuthenticationBuilder AddAzureAdv2(this AuthenticationBuilder builder, Action<AzureADOptions> configureOptions)
        {
            builder.AddAzureAD(configureOptions);

            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureADv2Options>();

            return builder;
        }       

        public class ConfigureAzureADv2Options : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureADv2Options _azureOptions;

            public AzureADv2Options GetAzureAdOptions() => _azureOptions;

            public ConfigureAzureADv2Options(IOptions<AzureADv2Options> azureOptions)
            {
                _azureOptions = azureOptions.Value;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}common/v2.0";
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;
                options.RequireHttpsMetadata = false;
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                var allScopes = $"{_azureOptions.Scopes} {_azureOptions.GraphScopes}".Split(new[] { ' ' });
                foreach (var scope in allScopes) { options.Scope.Add(scope); }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Ensure that User.Identity.Name is set correctly after login
                    NameClaimType = "name",

                    // Instead of using the default validation (validating against a single issuer value, as we do in line of business apps),
                    // we inject our own multitenant validation logic
                    ValidateIssuer = false,

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                        // If your authentication logic is based on users then add your logic here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Home/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = async (context) =>
                    {
                        var code = context.ProtocolMessage.Code;
                        var identifier = context.Principal.FindFirst(ObjectIdentifierClaimType).Value;
                        var memoryCache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
                        var graphScopes = _azureOptions.GraphScopes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        var cca = new ConfidentialClientApplication(
                            _azureOptions.ClientId,
                            _azureOptions.BaseUrl + _azureOptions.CallbackPath,
                            new ClientCredential(_azureOptions.ClientSecret),
                            new SessionTokenCache(identifier, memoryCache).GetCacheInstance(),
                            null);
                        var result = await cca.AcquireTokenByAuthorizationCodeAsync(code, graphScopes);

                        // Check whether the login is from the MSA tenant. 
                        // The sample uses this attribute to disable UI buttons for unsupported operations when the user is logged in with an MSA account.
                        var currentTenantId = context.Principal.FindFirst(TenantIdClaimType).Value;
                        if (currentTenantId == "9188040d-6c67-4c5b-b112-36a304b66dad")
                        {
                            
                        }

                        context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                    },
                    // If your application needs to do authenticate single users, add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                };
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
