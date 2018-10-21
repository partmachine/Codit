// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using Codit.AspNetCore.Authentication.AzureADv2.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication
{
    /// <summary>
    /// Extension methods to add Azure Active Directory Authentication to your application.
    /// </summary>
    public static class AzureADv2AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds JWT Bearer authentication to your app for Azure Active Directory Applications.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">The <see cref="Action{AzureADv2Options}"/> to configure the
        /// <see cref="AzureADv2Options"/>.
        /// </param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddAzureADBearer(this AuthenticationBuilder builder, Action<AzureADv2Options> configureOptions) =>
            builder.AddAzureADBearer(
                AzureADv2Defaults.BearerAuthenticationScheme,
                AzureADv2Defaults.JwtBearerAuthenticationScheme,
                configureOptions);

        /// <summary>
        /// Adds JWT Bearer authentication to your app for Azure Active Directory Applications.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="scheme">The identifier for the virtual scheme.</param>
        /// <param name="jwtBearerScheme">The identifier for the underlying JWT Bearer scheme.</param>
        /// <param name="configureOptions">The <see cref="Action{AzureADv2Options}"/> to configure the
        /// <see cref="AzureADv2Options"/>.
        /// </param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddAzureADBearer(
            this AuthenticationBuilder builder,
            string scheme,
            string jwtBearerScheme,
            Action<AzureADv2Options> configureOptions)
        {

            builder.AddPolicyScheme(scheme, displayName: null, configureOptions: o =>
            {
                o.ForwardDefault = jwtBearerScheme;
            });

            builder.Services.Configure(TryAddJwtBearerSchemeMapping(scheme, jwtBearerScheme));

            builder.Services.TryAddSingleton<IConfigureOptions<AzureADv2Options>, AzureADv2OptionsConfiguration>();

            builder.Services.TryAddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfiguration>();

            builder.Services.Configure(scheme, configureOptions);
            builder.AddJwtBearer(jwtBearerScheme, o => { });

            return builder;
        }

        /// <summary>
        /// Adds Azure Active Directory Authentication to your application.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">The <see cref="Action{AzureADv2Options}"/> to configure the
        /// <see cref="AzureADv2Options"/>
        /// </param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddAzureADv2(this AuthenticationBuilder builder, Action<AzureADv2Options> configureOptions) =>
            builder.AddAzureADv2(
                AzureADv2Defaults.AuthenticationScheme,
                AzureADv2Defaults.OpenIdScheme,
                AzureADv2Defaults.CookieScheme,
                AzureADv2Defaults.DisplayName,
                configureOptions);

        /// <summary>
        /// Adds Azure Active Directory Authentication to your application.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="scheme">The identifier for the virtual scheme.</param>
        /// <param name="openIdConnectScheme">The identifier for the underlying Open ID Connect scheme.</param>
        /// <param name="cookieScheme">The identifier for the underlying cookie scheme.</param>
        /// <param name="displayName">The display name for the scheme.</param>
        /// <param name="configureOptions">The <see cref="Action{AzureADv2Options}"/> to configure the
        /// <see cref="AzureADv2Options"/>
        /// </param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddAzureADv2(
            this AuthenticationBuilder builder,
            string scheme,
            string openIdConnectScheme,
            string cookieScheme,
            string displayName,
            Action<AzureADv2Options> configureOptions)
        {
            AddAdditionalMvcApplicationParts(builder.Services);
            builder.AddPolicyScheme(scheme, displayName, o =>
            {
                o.ForwardDefault = cookieScheme;
                o.ForwardChallenge = openIdConnectScheme;
            });

            builder.Services.Configure(TryAddOpenIDCookieSchemeMappings(scheme, openIdConnectScheme, cookieScheme));

            builder.Services.TryAddSingleton<IConfigureOptions<AzureADv2Options>, AzureADv2OptionsConfiguration>();

            builder.Services.TryAddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsConfiguration>();

            builder.Services.TryAddSingleton<IConfigureOptions<CookieAuthenticationOptions>, CookieOptionsConfiguration>();

            builder.Services.Configure(scheme, configureOptions);

            builder.Services.AddMemoryCache();

            builder.AddOpenIdConnect(openIdConnectScheme, null, o => { });
            builder.AddCookie(cookieScheme, null, o => { });

            

            return builder;
        }

        private static Action<AzureADv2SchemeOptions> TryAddJwtBearerSchemeMapping(string scheme, string jwtBearerScheme)
        {
            return TryAddMapping;

            void TryAddMapping(AzureADv2SchemeOptions o)
            {
                if (o.JwtBearerMappings.ContainsKey(scheme))
                {
                    throw new InvalidOperationException($"A scheme with the name '{scheme}' was already added.");
                }
                foreach (var mapping in o.JwtBearerMappings)
                {
                    if (mapping.Value.JwtBearerScheme == jwtBearerScheme)
                    {
                        throw new InvalidOperationException(
                            $"The JSON Web Token Bearer scheme '{jwtBearerScheme}' can't be associated with the Azure Active Directory scheme '{scheme}'. " +
                            $"The JSON Web Token Bearer scheme '{jwtBearerScheme}' is already mapped to the Azure Active Directory scheme '{mapping.Key}'");
                    }
                }
                o.JwtBearerMappings.Add(scheme, new AzureADv2SchemeOptions.JwtBearerSchemeMapping
                {
                    JwtBearerScheme = jwtBearerScheme
                });
            };
        }

        private static Action<AzureADv2SchemeOptions> TryAddOpenIDCookieSchemeMappings(string scheme, string openIdConnectScheme, string cookieScheme)
        {
            return TryAddMapping;

            void TryAddMapping(AzureADv2SchemeOptions o)
            {
                if (o.OpenIDMappings.ContainsKey(scheme))
                {
                    throw new InvalidOperationException($"A scheme with the name '{scheme}' was already added.");
                }
                foreach (var mapping in o.OpenIDMappings)
                {
                    if (mapping.Value.CookieScheme == cookieScheme)
                    {
                        throw new InvalidOperationException(
                            $"The cookie scheme '{cookieScheme}' can't be associated with the Azure Active Directory scheme '{scheme}'. " +
                            $"The cookie scheme '{cookieScheme}' is already mapped to the Azure Active Directory scheme '{mapping.Key}'");
                    }

                    if (mapping.Value.OpenIdConnectScheme == openIdConnectScheme)
                    {
                        throw new InvalidOperationException(
                            $"The Open ID Connect scheme '{openIdConnectScheme}' can't be associated with the Azure Active Directory scheme '{scheme}'. " +
                            $"The Open ID Connect scheme '{openIdConnectScheme}' is already mapped to the Azure Active Directory scheme '{mapping.Key}'");
                    }
                }
                o.OpenIDMappings.Add(scheme, new AzureADv2SchemeOptions.AzureADOpenIDSchemeMapping
                {
                    OpenIdConnectScheme = openIdConnectScheme,
                    CookieScheme = cookieScheme
                });
            };
        }

        private static void AddAdditionalMvcApplicationParts(IServiceCollection services)
        {
            var additionalParts = GetAdditionalParts();
            var mvcBuilder = services
                .AddMvc()
                .AddRazorPagesOptions(o => o.AllowAreas = true)
                .ConfigureApplicationPartManager(apm =>
                {
                    foreach (var part in additionalParts)
                    {
                        if (!apm.ApplicationParts.Any(ap => HasSameName(ap.Name, part.Name)))
                        {
                            apm.ApplicationParts.Add(part);
                        }
                    }

                    apm.FeatureProviders.Add(new AzureADv2AccountControllerFeatureProvider());
                });

            bool HasSameName(string left, string right) => string.Equals(left, right, StringComparison.Ordinal);
        }

        private static IEnumerable<ApplicationPart> GetAdditionalParts()
        {
            var thisAssembly = typeof(AzureADv2AuthenticationBuilderExtensions).Assembly;
            var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(thisAssembly, throwOnError: true);

            foreach (var reference in relatedAssemblies)
            {
                yield return new CompiledRazorAssemblyPart(reference);
            }
        }
    }
}
