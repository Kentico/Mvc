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
    public class EmailRegisterController : Controller
    {
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
        public ActionResult RegisterWithEmailConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Creates new users when the registration form is submitted and sends the confirmation emails.
        /// Accepts parameters posted from the registration form via the RegisterViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> RegisterWithEmailConfirmation(RegisterViewModel model)
        {
            // Validates the received user data based on the view model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Prepares a new user entity using the posted registration data
            // The user is not enabled by default
            Kentico.Membership.User user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
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
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError(String.Empty, error);
                }
                return View(model);
            }

            // Generates a confirmation token for the new user
            // Accepts a user ID parameter, which is automatically set for the 'user' variable by the UserManager.CreateAsync method
            string token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            // Prepares the URL of the confirmation link for the user (targets the "ConfirmUser" action)
            // Fill in the name of your controller
            string confirmationUrl = Url.Action("ConfirmUser", "EmailRegister", new { userId = user.Id, token = token }, protocol: Request.Url.Scheme);

            // Creates and sends the confirmation email to the user's address
            await UserManager.SendEmailAsync(user.Id, "Confirm your new account", 
                String.Format("Please confirm your new account by clicking <a href=\"{0}\">here</a>", confirmationUrl));

            // Displays a view asking the visitor to check their email and confirm the new account
            return View("CheckYourEmail");
        }

        /// <summary>
        /// Action for confirming new user accounts. Handles the links that users click in confirmation emails.
        /// </summary>
        public async Task<ActionResult> ConfirmUser(int? userId, string token)
        {
            IdentityResult confirmResult;

            try
            {
                // Verifies the confirmation parameters and enables the user account if successful
                confirmResult = await UserManager.ConfirmEmailAsync(userId.Value, token);
            }
            catch (InvalidOperationException)
            {
                // An InvalidOperationException occurs if a user with the given ID is not found
                confirmResult = IdentityResult.Failed("User not found.");
            }

            if (confirmResult.Succeeded)
            {
                // If the verification was successful, displays a view informing the user that their account was activated
                return View();
            }

            // Returns a view informing the user that the email confirmation failed
            return View("EmailConfirmationFailed");
        }
    }
}