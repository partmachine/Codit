// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Codit.AspNetCore.Authentication.AzureADv2.UI.AzureAD.Controllers.Internal
{
    [NonController]
    [AllowAnonymous]
    [Area("AzureAD")]
    [Route("[area]/[controller]/[action]")]
    internal class AccountController : Controller
    {
        public AccountController(IOptionsMonitor<AzureADv2Options> options)
        {
            Options = options;
        }

        public IOptionsMonitor<AzureADv2Options> Options { get; }

        [HttpGet("{scheme?}")]
        public IActionResult SignIn([FromRoute] string scheme)
        {
            scheme = scheme ?? AzureADv2Defaults.AuthenticationScheme;
            var redirectUrl = Url.Content("~/");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                scheme);
        }

        [HttpGet("{scheme?}")]
        public IActionResult SignOut([FromRoute] string scheme)
        {
            scheme = scheme ?? AzureADv2Defaults.AuthenticationScheme;
            var options = Options.Get(scheme);
            var callbackUrl = Url.Page("/Account/SignedOut", pageHandler: null, values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                options.CookieSchemeName,
                options.OpenIdConnectSchemeName);
        }
    }
}