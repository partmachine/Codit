// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace Codit.AspNetCore.Authentication.AzureADv2.UI
{
    internal class OpenIdConnectOptionsConfiguration : IConfigureNamedOptions<OpenIdConnectOptions>
    {
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
            options.Authority = new Uri(new Uri(AzureADv2Options.Instance), AzureADv2Options.TenantId).ToString();
            options.CallbackPath = AzureADv2Options.CallbackPath ?? options.CallbackPath;
            options.SignedOutCallbackPath = AzureADv2Options.SignedOutCallbackPath ?? options.SignedOutCallbackPath;
            options.SignInScheme = AzureADv2Options.CookieSchemeName;
            options.UseTokenLifetime = true;
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
