using System;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Kentico.Membership;

namespace LearningKit.Controllers
{
    public class PasswordResetController : Controller
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
        /// Allows visitors to submit their email address and request a password reset.
        /// </summary>
        public ActionResult RequestPasswordReset()
        {
            return View();
        }

        /// <summary>
        /// Generates a password reset request for the specified email address.
        /// Accepts the email address posted via the RequestPasswordResetViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RequestPasswordReset(RequestPasswordResetViewModel model)
        {
            // Validates the received email address based on the view model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gets the user entity for the specified email address
            Kentico.Membership.User user = UserManager.FindByEmail(model.Email);

            if (user != null)
            {
                // Generates a password reset token for the user
                string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                // Prepares the URL of the password reset link (targets the "ResetPassword" action)
                // Fill in the name of your controller
                string resetUrl = Url.Action("ResetPassword", "PasswordReset", new { userId = user.Id, token }, Request.Url.Scheme);

                // Creates and sends the password reset email to the user's address
                await UserManager.SendEmailAsync(user.Id, "Password reset request",
                    String.Format("To reset your account's password, click <a href=\"{0}\">here</a>", resetUrl));
            }

            // Displays a view asking the visitor to check their email and click the password reset link
            return View("CheckYourEmail");
        }

        /// <summary>
        /// Handles the links that users click in password reset emails.
        /// If the request parameters are valid, displays a form where users can reset their password.
        /// </summary>
        public ActionResult ResetPassword(int? userId, string token)
        {
            try
            {
                // Verifies the parameters of the password reset request
                // True if the token is valid for the specified user, false if the token is invalid or has expired
                // By default, the generated tokens are single-use and expire in 1 day
                if (UserManager.VerifyUserToken(userId.Value, "ResetPassword", token))
                {
                    // If the password request is valid, displays the password reset form
                    var model = new ResetPasswordViewModel
                    {
                        UserId = userId.Value,
                        Token = token
                    };

                    return View(model);
                }

                // If the password request is invalid, returns a view informing the user
                return View("ResetPasswordInvalid");
            }
            catch (InvalidOperationException)
            {
                // An InvalidOperationException occurs if a user with the given ID is not found
                // Returns a view informing the user that the password reset request is not valid
                return View("ResetPasswordInvalid");
            }
        }

        /// <summary>
        /// Resets the user's password based on the posted data.
        /// Accepts the user ID, password reset token and the new password via the ResetPasswordViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            // Validates the received password data based on the view model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Changes the user's password if the provided reset token is valid
                if (UserManager.ResetPassword(model.UserId, model.Token, model.Password).Succeeded)
                {
                    // If the password change was successful, displays a view informing the user
                    return View("ResetPasswordSucceeded");
                }

                // Occurs if the reset token is invalid
                // Returns a view informing the user that the password reset failed
                return View("ResetPasswordInvalid");
            }
            catch (InvalidOperationException)
            {
                // An InvalidOperationException occurs if a user with the given ID is not found
                // Returns a view informing the user that the password reset failed
                return View("ResetPasswordInvalid");
            }
        }
    }
}