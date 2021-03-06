﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Codit.AspNetCore.Authentication.AzureADv2.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers
{

    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                AzureADv2Defaults.AuthenticationScheme);
        }

        [Authorize]
        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl }, AzureADv2Defaults.CookieScheme, AzureADv2Defaults.OpenIdScheme, AzureADv2Defaults.AuthenticationScheme);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View();
        }
    }
}