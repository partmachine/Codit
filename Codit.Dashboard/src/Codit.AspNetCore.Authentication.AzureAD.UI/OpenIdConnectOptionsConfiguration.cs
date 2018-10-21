// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Codit.AspNetCore.Authentication.AzureADv2.UI
{
    internal class OpenIdConnectOptionsConfiguration : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private const string ObjectIdentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";

        private readonly IOptions<AzureADv2SchemeOptions> _schemeOptions;
        private readonly IOptionsMonitor<AzureADv2Options> _AzureADv2Options;

        public OpenIdConnectOptionsConfiguration(IOptions<AzureADv2SchemeOptions> schemeOptions, IOptionsMonitor<AzureADv2Options> AzureADv2Options)
        {
            _schemeOptions = schemeOptions;
            _AzureADv2Options = AzureADv2Options;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            var azureADScheme = GetAzureADScheme(name);
            var AzureADv2Options = _AzureADv2Options.Get(azureADScheme);
            if (name != AzureADv2Options.OpenIdConnectSchemeName)
            {
                return;
            }

            options.ClientId = AzureADv2Options.ClientId;
            options.ClientSecret = AzureADv2Options.ClientSecret;
            options.Authority = $"{AzureADv2Options.Instance}common/v2.0";
            options.CallbackPath = AzureADv2Options.CallbackPath ?? options.CallbackPath;
            options.SignedOutCallbackPath = AzureADv2Options.SignedOutCallbackPath ?? options.SignedOutCallbackPath;
            options.SignInScheme = AzureADv2Options.CookieSchemeName;
            options.UseTokenLifetime = true;
            options.RequireHttpsMetadata = true;           
            options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

            var allScopes = $"{AzureADv2Options.Scopes} {AzureADv2Options.GraphScopes}".Split(new[] { ' ' }).Distinct();
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
                OnRedirectToIdentityProvider = context =>
                {

                    return Task.CompletedTask;
                },
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
                    var graphScopes = AzureADv2Options.GraphScopes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    var cca = new ConfidentialClientApplication(
                        AzureADv2Options.ClientId,
                        AzureADv2Options.BaseUrl + AzureADv2Options.CallbackPath,
                        new ClientCredential(AzureADv2Options.ClientSecret),
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

        private string GetAzureADScheme(string name)
        {
            foreach (var mapping in _schemeOptions.Value.OpenIDMappings)
            {
                if (mapping.Value.OpenIdConnectScheme == name)
                {
                    return mapping.Key;
                }
            }

            return null;
        }

        public void Configure(OpenIdConnectOptions options)
        {
        }
    }
}
