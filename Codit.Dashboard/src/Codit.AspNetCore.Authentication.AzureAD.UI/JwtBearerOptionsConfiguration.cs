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
        private readonly IOptionsMonitor<AzureADv2Options> _AzureADv2Options;

        public JwtBearerOptionsConfiguration(
            IOptions<AzureADv2SchemeOptions> schemeOptions,
            IOptionsMonitor<AzureADv2Options> AzureADv2Options)
        {
            _schemeOptions = schemeOptions;
            _AzureADv2Options = AzureADv2Options;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            var azureADScheme = GetAzureADScheme(name);
            var AzureADv2Options = _AzureADv2Options.Get(azureADScheme);
            if (name != AzureADv2Options.JwtBearerSchemeName)
            {
                return;
            }

            options.Audience = AzureADv2Options.ClientId;
            options.Authority = new Uri(new Uri(AzureADv2Options.Instance), AzureADv2Options.TenantId).ToString();
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
