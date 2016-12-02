//DocSection:Using
using System;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Kentico.Membership;

using CMS.EventLog;
using CMS.SiteProvider;
//EndDocSection:Using

namespace LearningKit.Controllers
{
    public class AccountController : Controller
    {
        //DocSection:SignIn
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
        /// Basic action that displays the sign-in form.
        /// </summary>
        public ActionResult SignIn()
        {
            return View();
        }        
        
        /// <summary>
        /// Handles authentication when the sign-in form is submitted. Accepts parameters posted from the sign-in form via the SignInViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> SignIn(SignInViewModel model, string returnUrl)
        {
            // Validates the received user credentials based on the view model
            if (!ModelState.IsValid)
            {
                // Displays the sign-in form if the user credentials are invalid
                return View();
            }
            
            // Attempts to authenticate the user against the Kentico database
            SignInStatus signInResult = SignInStatus.Failure;
            try
            {
                signInResult = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.SignInIsPersistent, false);
            }
            catch (Exception ex)
            {
                // Logs an error into the Kentico event log if the authentication fails
                EventLogProvider.LogException("MvcApplication", "SignIn", ex);
            }
            
            // If the authentication was not successful, displays the sign-in form with an "Authentication failed" message 
            if (signInResult != SignInStatus.Success)
            {
                ModelState.AddModelError(String.Empty, "Authentication failed");
                return View();
            }
            
            // If the authentication was successful, redirects to the return URL when possible or to a different default action
            string decodedReturnUrl = Server.UrlDecode(returnUrl);
            if (!string.IsNullOrEmpty(decodedReturnUrl) && Url.IsLocalUrl(decodedReturnUrl))
            {
                return Redirect(decodedReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        //EndDocSection:SignIn


        //DocSection:Signout
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
        
        /// <summary>
        /// Action for signing out users. The Authorize attribute allows the action only for users who are already signed in.
        /// </summary>
        [Authorize]
        public ActionResult SignOut()
        {
            // Signs out the current user
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            
            // Redirects to a different action after the sign-out
            return RedirectToAction("Index", "Home");
        }
        //EndDocSection:Signout

        //DocSection:EditUser
        /// <summary>
        /// Provides access to user related API which will automatically save changes to the UserStore.
        /// </summary>
        public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }
        
        /// <summary>
        /// Displays a form where user information can be changed.
        /// </summary>
        public ActionResult EditUser()
        {
            // Finds the user based on their current user name
            User user = UserManager.FindByName(User.Identity.Name);
            
            return View(user);
        }
        
        /// <summary>
        /// Saves the entered changes of the user details to the database.
        /// </summary>
        /// <param name="returnedUser">User that is changed.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> EditUser(User returnedUser)
        {
            // Finds the user based on their current user name
            User user = UserManager.FindByName(User.Identity.Name);
            
            // Assigns the names based on the entered data
            user.FirstName = returnedUser.FirstName;
            user.LastName = returnedUser.LastName;
            
            // Saves the user details into the database
            UserStore userStore = new UserStore(SiteContext.CurrentSiteName);
            await userStore.UpdateAsync(user);
            
            return RedirectToAction("Index", "Home");
        }
        //EndDocSection:EditUser
    }
}