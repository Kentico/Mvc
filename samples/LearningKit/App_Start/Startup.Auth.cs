using System;
using System.Web;
using System.Web.Mvc;

using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Owin;

using CMS.Helpers;
using CMS.SiteProvider;

using Kentico.Membership;

using Microsoft.Owin.Security.WsFederation;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;

// Assembly attribute that sets the OWIN startup class
[assembly: OwinStartup(typeof(LearningKit.App_Start.Startup))]

namespace LearningKit.App_Start
{
    public partial class Startup
    {
        // Cookie name prefix used by OWIN when creating authentication cookies
        private const string OWIN_COOKIE_PREFIX = ".AspNet.";


        public void Configuration(IAppBuilder app)
        {
            // Registers the Kentico.Membership identity implementation
            app.CreatePerOwinContext(() => UserManager.Initialize(app, new UserManager(new UserStore(SiteContext.CurrentSiteName))));
            app.CreatePerOwinContext<SignInManager>(SignInManager.Create);

            // Configures the authentication cookie
            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                // Fill in the name of your sign-in action and controller
                LoginPath = new PathString(urlHelper.Action("SignIn", "Account")),
                Provider = new CookieAuthenticationProvider
                {
                    // Sets the return URL for the sign-in page redirect (fill in the name of your sign-in action and controller)
                    OnApplyRedirect = context => context.Response.Redirect(urlHelper.Action("SignIn", "Account")
                                                 + new Uri(context.RedirectUri).Query)
                }
            });

            // Registers the authentication cookie with the 'Essential' cookie level
            // Ensures that the cookie is preserved when changing a visitor's allowed cookie level below 'Visitor'
            CookieHelper.RegisterCookie(OWIN_COOKIE_PREFIX + DefaultAuthenticationTypes.ApplicationCookie, CookieLevel.Essential);

            // Uses a cookie to temporarily store information about users signing in via external authentication services
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Registers a WS-Federation authentication service
            app.UseWsFederationAuthentication(
                    new WsFederationAuthenticationOptions
                    {
                    // Set any properties required by your authentication service
                    MetadataAddress = "placeholder", // Fill in the address of your service's WS-Federation metadata
                    Wtrealm = "",
                    // When using external services, Passive authentication mode may help avoid redirect loops for 401 responses
                    AuthenticationMode = AuthenticationMode.Passive
                    });

            // Registers an OpenID Connect authentication service
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    // Set any properties required by your authentication service
                    ClientId = "placeholder",
                    ClientSecret = "placeholder",
                    Authority = "placeholder",
                    AuthenticationMode = AuthenticationMode.Passive
                });

            // Registers the Facebook authentication service
            app.UseFacebookAuthentication(
                new FacebookAuthenticationOptions
                {
                    // Fill in the application ID and secret of your Facebook authentication application
                    AppId = "placeholder",
                    AppSecret = "placeholder"
                });

            // Registers the Google authentication service
            app.UseGoogleAuthentication(
                new GoogleOAuth2AuthenticationOptions
                {
                    // Fill in the client ID and secret of your Google authentication application
                    ClientId = "placeholder",
                    ClientSecret = "placeholder"
                });
        }        
    }
}