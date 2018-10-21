// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Options;

namespace Codit.AspNetCore.Authentication.AzureADv2.UI
{
    internal class AzureADv2OptionsConfiguration : IConfigureNamedOptions<AzureADv2Options>
    {
        private readonly IOptions<AzureADv2SchemeOptions> _schemeOptions;

        public AzureADv2OptionsConfiguration(IOptions<AzureADv2SchemeOptions> schemeOptions)
        {
            _schemeOptions = schemeOptions;
        }

        public void Configure(string name, AzureADv2Options options)
        {
            // This can be called because of someone configuring JWT or someone configuring
            // Open ID + Cookie.
            if (_schemeOptions.Value.OpenIDMappings.TryGetValue(name, out var webMapping))
            {
                options.OpenIdConnectSchemeName = webMapping.OpenIdConnectScheme;
                options.CookieSchemeName = webMapping.CookieScheme;
                return;
            }
            if (_schemeOptions.Value.JwtBearerMappings.TryGetValue(name, out var mapping))
            {
                options.JwtBearerSchemeName = mapping.JwtBearerScheme;
                return;
            }
        }

        public void Configure(AzureADv2Options options)
        {
        }
    }
}
