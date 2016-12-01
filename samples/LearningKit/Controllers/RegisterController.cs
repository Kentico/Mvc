using System;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Kentico.Membership;

using CMS.EventLog;

namespace LearningKit.Controllers
{
    public class RegisterController : Controller
    {
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
        /// Basic action that displays the registration form.
        /// </summary>
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handles creation of new users when the registration form is submitted.
        /// Accepts parameters posted from the registration form via the RegisterViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            // Validates the received user data based on the view model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Prepares a new user entity using the posted registration data
            Kentico.Membership.User user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Enabled = true // Enables the new user directly
            };

            // Attempts to create the user in the Kentico database
            IdentityResult registerResult = IdentityResult.Failed();
            try
            {
                registerResult = await UserManager.CreateAsync(user, model.Password);
            }
            catch (Exception ex)
            {
                // Logs an error into the Kentico event log if the creation of the user fails
                EventLogProvider.LogException("MvcApplication", "UserRegistration", ex);
                ModelState.AddModelError(String.Empty, "Registration failed");
            }

            // If the registration was not successful, displays the registration form with an error message
            if (!registerResult.Succeeded)
            {
                foreach (string error in registerResult.Errors)
                {
                    ModelState.AddModelError(String.Empty, error);
                }
                return View(model);
            }

            // If the registration was successful, signs in the user and redirects to a different action
            await SignInManager.SignInAsync(user, true, false);
            return RedirectToAction("Index", "Home");
        }
    }
}