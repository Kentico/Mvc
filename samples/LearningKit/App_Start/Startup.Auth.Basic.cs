using System;
using System.Web;
using System.Web.Mvc;

using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Owin;

using CMS.Helpers;
using CMS.SiteProvider;

using Kentico.Membership;

// Assembly attribute that sets the OWIN startup class
// This example sets the Startup class from the 'LearningKit.App_Start' namespace, not 'LearningKit.App_Start.Basic' used below
// The active Startup class is defined in Startup.Auth.cs and additionally demonstrates registration of external authentication services
[assembly: OwinStartup(typeof(LearningKit.App_Start.Startup))]

namespace LearningKit.App_Start.Basic
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
        }
    }
}