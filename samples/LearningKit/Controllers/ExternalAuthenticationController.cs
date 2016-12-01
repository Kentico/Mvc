//DocSection:Using
using System;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Kentico.Membership;
//EndDocSection:Using

namespace LearningKit.Controllers
{
    public class ExternalAuthenticationController : Controller
    {
        //DocSection:Properties
        /// <summary>
        /// Provides access to the Kentico.Membership.SignInManager instance.
        /// </summary>
        public SignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<SignInManager>();
            }
        }
        
        /// <summary>
        /// Provides access to the Kentico.Membership.UserManager instance.
        /// </summary>
        public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }
        
        /// <summary>
        /// Provides access to the Microsoft.Owin.Security.IAuthenticationManager instance.
        /// </summary>
        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        //EndDocSection:Properties

        //DocSection:ExternalAuth
        /// <summary>
        /// Redirects authentication requests to an external service. 
        /// Posted parameters include the name of the requested authentication middleware instance and a return URL.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RequestSignIn(string provider, string returnUrl)
        {
            // Requests a redirect to the external sign-in provider
            // Sets a return URL targeting an action that handles the response
            // Fill in the name of your controller
            return new ChallengeResult(provider, Url.Action("SignInCallback", "ExternalAuthentication", new { ReturnUrl = returnUrl }));
        }
        
        /// <summary>
        /// Handles responses from external authentication services.
        /// </summary>
        [AllowAnonymous]
        public async Task<ActionResult> SignInCallback(string returnUrl)
        {
            // Extracts login info out of the external identity provided by the service
            ExternalLoginInfo loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            
            // If the external authentication fails, displays a view with appropriate information
            if (loginInfo == null)
            {
                return View("ExternalAuthenticationFailure");
            }
            
            // Attempts to sign in the user using the external login info
            SignInStatus result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                // Success occurs if the user already exists in the Kentico database and has signed in using the given external service
                case SignInStatus.Success:
                    // Redirects to the original return URL
                    return RedirectToLocal(returnUrl);
                
                // The 'LockedOut' status occurs if the user exists in Kentico, but is not enabled
                case SignInStatus.LockedOut:
                    // Returns a view informing the user about the locked account
                    return View("Lockout");
                
                case SignInStatus.Failure:
                default:
                    // Attempts to create a new user in Kentico if the authentication failed
                    IdentityResult userCreation = await CreateExternalUser(loginInfo);
                    
                    // Attempts to sign in again with the new user created based on the external authentication data
                    result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
                    
                    // Verifies that the user was created successfully and was able to sign in
                    if (userCreation.Succeeded && result == SignInStatus.Success)
                    {
                        // Redirects to the original return URL
                        return RedirectToLocal(returnUrl);
                    }
                    
                    // If the user creation was not successful, displays corresponding error messages
                    foreach (string error in userCreation.Errors)
                    {
                        ModelState.AddModelError(String.Empty, error);
                    }
                    return View();
                    
                    // Optional extension:
                    // Send the loginInfo data as a view model and create a form that allows adjustments of the user data.
                    // Allows visitors to resolve errors such as conflicts with existing usernames in Kentico.
                    // Then post the data to another action and attempt to create the user account again.
                    // The action can access the original loginInfo using the AuthenticationManager.GetExternalLoginInfoAsync() method.
            }
        }
        
        /// <summary>
        /// Creates users in Kentico based on external login data.
        /// </summary>
        private async Task<IdentityResult> CreateExternalUser(ExternalLoginInfo loginInfo)
        {
            // Prepares a new user entity based on the external login data
            Kentico.Membership.User user = new User
            {
                UserName = loginInfo.DefaultUserName ?? loginInfo.Email,
                Email = loginInfo.Email,
                Enabled = true, // The user is enabled by default
                IsExternal = true // IsExternal must always be true for users created via external authentication
                // Set any other required user properties using the data available in loginInfo
            };
            
            // Attempts to create the user in the Kentico database
            IdentityResult result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                // If the user was created successfully, creates a mapping between the user and the given authentication provider
                result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
            }
            
            return result;
        }
        
        /// <summary>
        /// Redirects to a specified return URL if it belongs to the MVC website or to the site's home page.
        /// </summary>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            return RedirectToAction("Index", "Home");
        }
        
        /// <summary>
        /// Custom type that processes the result of an action method.
        /// </summary>
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
            }
            
            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            
            // Called to run the action result using the specified controller context
            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                
                // Adds information into the response environment that causes the specified authentication middleware to challenge the caller
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        //EndDocSection:ExternalAuth
    }
}