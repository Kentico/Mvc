using System;
using System.Web;
using System.Web.Mvc;

using CMS.SiteProvider;

using Kentico.Membership;

using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.WsFederation;
using Owin;

[assembly: OwinStartup(typeof(MVCApp.App_Start.KenticoMembershipStartup))]
namespace MVCApp.App_Start
{
    public class KenticoMembershipStartup
    {
        public void Configuration(IAppBuilder app)
        {
            // Register Kentico Membership identity implementation
            app.CreatePerOwinContext(() => UserManager.Initialize(app, new UserManager(new UserStore(SiteContext.CurrentSiteName))));
            app.CreatePerOwinContext<SignInManager>(SignInManager.Create);

            // Configure the sign in cookie
            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(urlHelper.Action("Login", "Account")),
                Provider = new CookieAuthenticationProvider
                {
                    // Redirect to logon page with return url
                    OnApplyRedirect = context => context.Response.Redirect(urlHelper.Action("Login", "Account") + new Uri(context.RedirectUri).Query)
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseWsFederationAuthentication(
                new WsFederationAuthenticationOptions
                {
                    MetadataAddress = "https://login.windows.net/cfa42587-015d-4a23-a74d-37a09b34923a/FederationMetadata/2007-06/FederationMetadata.xml",
                    Wtrealm = "http://MVCExternalAuthExplicit",
                });

            // Uncomment and supply Facebook application ID and secret to make the Facebook external authentication available on login page
            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: ""
            //);

            // Uncomment and supply Google client ID and secret to make the Google external authentication available on login page
            //app.UseGoogleAuthentication(
            //    clientId: "",
            //    clientSecret: ""
            //);
        }
    }
}
