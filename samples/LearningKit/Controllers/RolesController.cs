using System;
using System.Web.Mvc;
using System.Threading.Tasks;

//DocSection:Using
using System.Web;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Kentico.Membership;
//EndDocSection:Using

namespace LearningKit.Controllers
{
    public class RolesController : Controller
    {
        //DocSection:Properties
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
        /// Gets the Kentico.Membership.User representation of the currently signed in user.
        /// You can use the object to access the user's ID, which is required by the role management methods.
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return UserManager.FindByName(User.Identity.Name);
            }
        }
        //EndDocSection:Properties

        [Authorize]
        public ActionResult ManageRoles()
        {
            return View(CurrentUser);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageRoles(string action)
        {
            switch (action)
            {
                case "add":
                    try
                    {
                        //DocSection:AddRole
                        // Attempts to assign the current user to the "KenticoRole" and "CMSBasicUsers" roles
                        IdentityResult addResult = await UserManager.AddToRolesAsync(CurrentUser.Id, "KenticoRole", "CMSBasicUsers");
                        //EndDocSection:AddRole
                    }
                    catch (Exception exception)
                    {
                        // Adds error messages onto the role management page (for example if the roles do not exist in the system)
                        ModelState.AddModelError("", string.Format("Exception: {0}", exception.Message));
                    }

                    return View(CurrentUser);

                case "remove":
                    //DocSection:RemoveRole
                    // Attempts to remove the "KenticoRole" and "CMSBasicUsers" roles from the current user
                    IdentityResult removeResult = await UserManager.RemoveFromRolesAsync(CurrentUser.Id, "KenticoRole", "CMSBasicUsers");
                    //EndDocSection:RemoveRole

                    return View(CurrentUser);

                default:
                    return View(CurrentUser);
            }
        }

        //DocSection:RoleAuthorize
        // Allows the "RestrictedPage" action only for signed in users who belong to the "KenticoRole" role
        [Authorize(Roles = "KenticoRole")]
        public ActionResult RestrictedPage()
        {
            return View();
        }
        //EndDocSection:RoleAuthorize
    }
}