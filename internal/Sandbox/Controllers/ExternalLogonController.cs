using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Kentico.Membership;

namespace Sandbox.Controllers
{
    [Authorize]
    public class ExternalLogonController : Controller
    {
        private SignInManager _signInManager;
        private UserManager _userManager;

        public SignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<SignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }


        public UserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<UserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RequestLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("LoginCallback", "ExternalLogon", new { ReturnUrl = returnUrl }));
        }


        [AllowAnonymous]
        public async Task<ActionResult> LoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return View("LoginFailure");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Create user
                    var userCreation = await CreateExternalUser(loginInfo);
                    result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
                    if (userCreation.Succeeded && result == SignInStatus.Success)
                    {
                        // Redirect to return URL
                        return RedirectToLocal(returnUrl);
                    }
                    // Show errors
                    // TODO: Send loginInfo as a view model and let user change the personal information that caused the error.
                    //       The form should lead to another action that still can get original loginInfo from the AuthenticationManager.GetExternalLoginInfoAsync() method.
                    AddErrors(userCreation);
                    return View();
            }
        }


        #region Helpers

        private async Task<IdentityResult> CreateExternalUser(ExternalLoginInfo loginInfo)
        {
            // Create user
            var user = new User
            {
                UserName = loginInfo.DefaultUserName ?? loginInfo.Email,
                Email = loginInfo.Email,
                Enabled = true,
                IsExternal = true
                //TODO: Add other information from the claims in loginInfo
            };
            var result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                // Add external login information
                result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
            }
            // TODO: Add user to roles based on claims in in loginInfo
            return result;
        }


        // The rest of the file is the same as in the original code generated from current (25.1.2016) MVC project template.

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }


            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}