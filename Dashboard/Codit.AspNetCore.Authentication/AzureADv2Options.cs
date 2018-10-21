using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codit.AspNetCore.Authentication
{
    public class AzureADv2Options : AzureADOptions
    {
        public string BaseUrl { get; set; }

        public string Scopes { get; set; } = "openid email profile offline_access";

        public string GraphScopes { get; set; } = "User.Read User.ReadBasic.All";
    }
}
