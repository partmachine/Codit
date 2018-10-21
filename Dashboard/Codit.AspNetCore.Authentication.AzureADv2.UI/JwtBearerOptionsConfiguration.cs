// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using System;
using Codit.AspNetCore.Authentication.AzureADv2.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication
{
    internal class JwtBearerOptionsConfiguration : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IOptions<AzureADv2SchemeOptions> _schemeOptions;
        private readonly IOptionsMonitor<AzureADv2Options> _azureADOptions;

        public JwtBearerOptionsConfiguration(
            IOptions<AzureADv2SchemeOptions> schemeOptions,
            IOptionsMonitor<AzureADv2Options> azureADOptions)
        {
            _schemeOptions = schemeOptions;
            _azureADOptions = azureADOptions;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            var azureADScheme = GetAzureADScheme(name);
            var azureADOptions = _azureADOptions.Get(azureADScheme);
            if (name != azureADOptions.JwtBearerSchemeName)
            {
                return;
            }

            options.Audience = azureADOptions.ClientId;
            options.Authority = new Uri(new Uri(azureADOptions.Instance), azureADOptions.TenantId).ToString();
        }

        public void Configure(JwtBearerOptions options)
        {
        }

        private string GetAzureADScheme(string name)
        {
            foreach (var mapping in _schemeOptions.Value.JwtBearerMappings)
            {
                if (mapping.Value.JwtBearerScheme == name)
                {
                    return mapping.Key;
                }
            }

            return null;
        }
    }
}
