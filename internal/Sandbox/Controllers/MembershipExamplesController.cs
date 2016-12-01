using System;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Kentico.Membership;

using CMS.EventLog;
using CMS.Membership;
using CMS.SiteProvider;

// Usings to remove
using Sandbox.Models.Account;

namespace Sandbox.Controllers
{
    public class MembershipExamplesController : Controller
    {
        #region "Login example"

        //using System;
        //using System.Web;
        //using System.Web.Mvc;
        //using System.Threading.Tasks;

        //using Microsoft.AspNet.Identity.Owin;

        //using Kentico.Membership;

        //using CMS.EventLog;

        public SignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<SignInManager>();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            // Validate received data.
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Try to log in.
            SignInStatus signInResult = SignInStatus.Failure;
            try
            {
                signInResult = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.StaySignedIn, false);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("UserController", "Login", ex);
            }

            // Report failure.
            if (signInResult != SignInStatus.Success)
            {
                ModelState.AddModelError(String.Empty, "Authentication failed");
                return View();
            }

            // Logged in successfully.
            // Redirect to return url or home.
            var decodedReturnUrl = Server.UrlDecode(returnUrl);
            if (!string.IsNullOrEmpty(decodedReturnUrl) && Url.IsLocalUrl(decodedReturnUrl))
            {
                return Redirect(decodedReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion


        #region "Logout example"

        //using System;
        //using System.Web;
        //using System.Web.Mvc;

        //using Microsoft.AspNet.Identity;

        //using Microsoft.Owin.Security;

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Index", "Home");
        }

        #endregion


        #region "Registration basic"

        //using System;
        //using System.Web;
        //using System.Web.Mvc;
        //using System.Threading.Tasks;

        //using Microsoft.AspNet.Identity;
        //using Microsoft.AspNet.Identity.Owin;

        //using Kentico.Membership;

        //using CMS.EventLog;

        /*public SignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<SignInManager>();
            }
        }*/

        public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }


        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            // Validate received data.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Create new user entity.
            Kentico.Membership.User user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Enabled = true
            };

            // Try to register the user.
            IdentityResult registerResult = IdentityResult.Failed();
            try
            {
                registerResult = await UserManager.CreateAsync(user, model.Password);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Membership", "Register", ex);
                ModelState.AddModelError(String.Empty, "Registration failed");
            }

            // Report failure with all errors.
            if (!registerResult.Succeeded)
            {
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError(String.Empty, error);
                }
                return View(model);
            }

            // Log in as new user and redirect home.
            await SignInManager.SignInAsync(user, true, false);
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterUserWithDefaultRole(RegisterViewModel model)
        {
            // Validate received data.
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            // Create new user entity.
            Kentico.Membership.User user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Enabled = true
            };

            // Try to register the user with some default role.
            IdentityResult registerResult = IdentityResult.Failed();
            try
            {
                var roleStore = new RoleStore();
                string defaultRoleName = "testRegistrationRole";
                registerResult = await UserManager.CreateAsync(user, model.Password);

                var role = await roleStore.FindByNameAsync(defaultRoleName);
                if (role == null)
                {
                    var roleInfo = new RoleInfo
                    {
                        RoleDisplayName = defaultRoleName,
                        RoleName = defaultRoleName,
                        SiteID = SiteContext.CurrentSiteID
                    };

                    await roleStore.CreateAsync(new Role(roleInfo));
                }

                registerResult = await UserManager.AddToRoleAsync(user.Id, defaultRoleName);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Membership", "Register", ex);
                ModelState.AddModelError(String.Empty, "Registration failed");
            }

            // Report failure with all errors.
            if (!registerResult.Succeeded)
            {
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError(String.Empty, error);
                }
                return View(model);
            }

            // Log in as new user and redirect home.
            await SignInManager.SignInAsync(user, true, false);
            return RedirectToAction("Index", "Home");
        }

        #endregion


        #region "Registration with email confirmation"

        //using System;
        //using System.Web;
        //using System.Web.Mvc;
        //using System.Threading.Tasks;

        //using Microsoft.AspNet.Identity;
        //using Microsoft.AspNet.Identity.Owin;

        //using Kentico.Membership;

        //using CMS.EventLog;

        /*public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }*/

        public ActionResult RegisterWithEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterWithEmailConfirmation(RegisterViewModel model)
        {
            // Validate received data.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Create new user entity that is not enabled.
            var user = new User
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };

            // Try to register the user.
            IdentityResult registerResult = IdentityResult.Failed();
            try
            {
                registerResult = await UserManager.CreateAsync(user, model.Password);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Membership", "Register", ex);
                ModelState.AddModelError(String.Empty, "Registration failed");
            }

            // Report failure with all errors.
            if (!registerResult.Succeeded)
            {
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError(String.Empty, error);
                }
                return View(model);
            }

            // Create and send email.
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var encodedCode = HttpUtility.UrlEncode(code);

            var confirmationUrl = Url.Action("ConfirmEmail", "MembershipExamples", new { userId = user.Id, code = encodedCode }, protocol: Request.Url.Scheme); // Change the controller name on this line.
            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + confirmationUrl + "\">here</a>");

            // Display view asking user to check his email.
            return View("CheckYourEmail");
        }

        public async Task<ActionResult> ConfirmEmail(int userId, string code)
        {
            var decodedCode = HttpUtility.UrlDecode(code);

            // Confirm email and enable user.
            var result = await UserManager.ConfirmEmailAsync(userId, decodedCode);

            if (result.Succeeded)
            {
                return View();
            }
            return View("EmailConfirmationFailed");
        }

        #endregion


        #region "Roles"

        //using System;
        //using System.Web;
        //using System.Web.Mvc;

        //using Microsoft.AspNet.Identity;
        //using Microsoft.AspNet.Identity.Owin;

        //using Kentico.Membership;

        //using CMS.Membership;
        //using CMS.SiteProvider;

        /*public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }*/


        [Authorize]
        public ActionResult YourAccount()
        {
            return View(UserManager.FindByName(User.Identity.Name));
        }


        [Authorize(Roles = "CMSGlobalAdministrator")]
        public ActionResult AllUsers()
        {
            return View(UserInfoProvider.GetUsers().OnSite(SiteContext.CurrentSiteName));
        }


        private ManageUserRolesViewModel PopulateRoleLists(ManageUserRolesViewModel model, Kentico.Membership.User user)
        {
            // Populate user roles
            model.UserRoles = UserManager.FindById(user.Id).Roles;

            // Populate site roles
            var siteRoles = RoleInfoProvider.GetRoles().OnSite(SiteContext.CurrentSiteID, false).Select( x => new RoleInfo(x));
            List<string> siteList = new List<string>();

            foreach (var role in siteRoles)
            {
                siteList.Add(role.RoleName);
            }

            model.SiteRoles = siteList;

            return model;
        }


        public ActionResult ManageUserRoles(int userId)
        {
            ViewBag.CurrentUserName = User.Identity.Name;

            var model = new ManageUserRolesViewModel();

            var user = UserManager.FindById(userId);

            if (user == null)
            {
                ModelState.AddModelError("", string.Format("User with ID {0} does not exist!", userId));
                return View();
            }

            model.User = user;
            model = PopulateRoleLists(model, user);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageUserRoles(int userId, ManageUserRolesViewModel model)
        {
            ViewBag.CurrentUserName = User.Identity.Name;

            var user = UserManager.FindById(userId);

            if (user == null)
            {
                ModelState.AddModelError("", string.Format("User with ID {0} does not exist!", userId));
                return View();
            }

            model.User = user;
            model = PopulateRoleLists(model, user);

            // Validate received data.
            if (ModelState.IsValid)
            {
                var roleName = model.RoleName;

                if (string.IsNullOrEmpty(model.RoleName))
                {
                    return View(model);
                }

                if (model.action == ManageUserRolesViewModel.Actions.AddRole)
                { // Add user to specified role
                    try
                    { // Add user to role
                        await UserManager.AddToRoleAsync(user.Id, roleName);
                        model = PopulateRoleLists(model, user);
                    }
                    catch (Exception exception)
                    {
                        ModelState.AddModelError("", string.Format("Exception: {0}", exception.Message));
                        return View(model);
                    }
                }
                else if (model.action == ManageUserRolesViewModel.Actions.RemoveRole)
                { // Remove user from specified role
                    try
                    { // Remove user from role
                        await UserManager.RemoveFromRoleAsync(user.Id, roleName);
                        model = PopulateRoleLists(model, user);
                    }
                    catch (Exception exception)
                    {
                        ModelState.AddModelError("", string.Format("Exception: {0}", exception.Message));
                        return View(model);
                    }
                }
            }

            return View(model);
        }

        #endregion
    }
}