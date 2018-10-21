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
        private readonly IOptionsMonitor<AzureADv2Options> _azureADOptions;

        public OpenIdConnectOptionsConfiguration(IOptions<AzureADv2SchemeOptions> schemeOptions, IOptionsMonitor<AzureADv2Options> azureADOptions)
        {
            _schemeOptions = schemeOptions;
            _azureADOptions = azureADOptions;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            var azureADScheme = GetAzureADScheme(name);
            var azureADOptions = _azureADOptions.Get(azureADScheme);
            if (name != azureADOptions.OpenIdConnectSchemeName)
            {
                return;
            }

            options.ClientId = azureADOptions.ClientId;
            options.ClientSecret = azureADOptions.ClientSecret;
            options.Authority = $"{azureADOptions.Instance}common/v2.0";
            options.CallbackPath = azureADOptions.CallbackPath ?? options.CallbackPath;
            options.SignedOutCallbackPath = azureADOptions.SignedOutCallbackPath ?? options.SignedOutCallbackPath;
            options.SignInScheme = azureADOptions.CookieSchemeName;
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
