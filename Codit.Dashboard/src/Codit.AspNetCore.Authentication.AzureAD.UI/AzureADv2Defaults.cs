// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;


using Microsoft.AspNetCore.Authentication;

namespace Codit.AspNetCore.Authentication.AzureADv2.UI
{
    /// <summary>
    /// Constants for different Azure Active Directory authentication components.
    /// </summary>
    public static class AzureADv2Defaults
    {        
        /// <summary>
        /// The scheme name for Open ID Connect when using
        /// <see cref="AzureADv2AuthenticationBuilderExtensions.AddAzureADv2(AuthenticationBuilder, System.Action{AzureADv2Options})"/>.
        /// </summary>
        public static readonly string OpenIdScheme = "AzureADOpenID";

        /// <summary>
        /// The scheme name for cookies when using
        /// <see cref="AzureADv2AuthenticationBuilderExtensions.AddAzureADv2(AuthenticationBuilder, System.Action{AzureADv2Options})"/>.
        /// </summary>
        public static readonly string CookieScheme = "AzureADCookie";

        /// <summary>
        /// The default scheme for Azure Active Directory Bearer.
        /// </summary>
        public static readonly string BearerAuthenticationScheme = "AzureADBearer";

        /// <summary>
        /// The scheme name for JWT Bearer when using
        /// <see cref="AzureADv2AuthenticationBuilderExtensions.AddAzureADBearer(AuthenticationBuilder, System.Action{AzureADv2Options})"/>.
        /// </summary>
        public static readonly string JwtBearerAuthenticationScheme = "AzureADJwtBearer";

        /// <summary>
        /// The default scheme for Azure Active Directory.
        /// </summary>
        public static readonly string AuthenticationScheme = "AzureAD";

        /// <summary>
        /// The display name for Azure Active Directory.
        /// </summary>
        public static readonly string DisplayName = "Azure Active Directory";
    }
}
