using CMS.Membership;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sandbox.Models.Account
{
    /// <summary>
    /// View model for user management.
    /// </summary>
    public class ManageUserRolesViewModel
    {
        /// <summary>
        /// List of actions.
        /// </summary>
        public enum Actions
        {
            AddRole,
            RemoveRole
        }

        /// <summary>
        /// Action to perform.
        /// </summary>
        public Actions action { get; set; }


        /// <summary>
        /// Managed user.
        /// </summary>
        public Kentico.Membership.User User { get; set; }


        /// <summary>
        /// List of roles user is assigned to.
        /// </summary>
        public IEnumerable<string> UserRoles { get; set; }


        /// <summary>
        /// List of site roles.
        /// </summary>
        public IEnumerable<string> SiteRoles { get; set; }


        /// <summary>
        /// Selected role name.
        /// </summary>
        [DisplayName("Role name")]
        public string RoleName { get; set; }
    }
}