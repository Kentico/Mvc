using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CMS.Membership;
using CMS.SiteProvider;
using CMS.Helpers;

using Microsoft.AspNet.Identity;

namespace Kentico.Membership
{
    /// <summary>
    /// Identity role store implementation.
    /// </summary>
    public class RoleStore : IRoleStore<Role, int>
    {
        /// <summary>
        /// Performs tasks to dispose the role store.
        /// </summary>
        public void Dispose()
        {
        }


        /// <summary>
        /// Returns instance of <see cref="Role"/>.
        /// </summary>
        /// <param name="roleId">ID of the role.</param>
        public Task<Role> FindByIdAsync(int roleId)
        {
            var roleInfo = RoleInfoProvider.GetRoleInfo(roleId);
            if (roleInfo == null)
            {
                return Task.FromResult((Role)null);
            }

            return Task.FromResult(new Role(roleInfo));
        }


        /// <summary>
        /// Returns instance of <see cref="Role"/>.
        /// </summary>
        /// <param name="roleName">Name of role.</param>
        public Task<Role> FindByNameAsync(string roleName)
        {
            var roleInfo = RoleInfoProvider.GetRoleInfo(roleName, SiteContext.CurrentSiteID);
            if (roleInfo == null)
            {
                return Task.FromResult((Role)null);
            }

            return Task.FromResult(new Role(roleInfo));
        }


        /// <summary>
        /// Stores <see cref="Role"/> in the database.
        /// </summary>
        /// <param name="role">Role.</param>
        public Task CreateAsync(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var roleInfo = new RoleInfo
            {
                RoleDisplayName = String.IsNullOrEmpty(role.DisplayName) ? role.Name : role.DisplayName,
                RoleName = role.Name,
                SiteID = SiteContext.CurrentSiteID,
            };

            // Insert role and after set ID to role
            RoleInfoProvider.SetRoleInfo(roleInfo);
            role.Id = roleInfo.RoleID;

            return Task.FromResult(0);
        }


        /// <summary>
        /// Deletes <see cref="Role"/> from the database.
        /// </summary>
        /// <param name="role">Role.</param>
        public Task DeleteAsync(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            RoleInfoProvider.DeleteRoleInfo(role.Id);

            return Task.FromResult(0);
        }


        /// <summary>
        /// Updates <see cref="Role"/> in the database.
        /// </summary>
        /// <param name="role">Role.</param>
        public Task UpdateAsync(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var roleToUpdate = RoleInfoProvider.GetRoleInfo(role.Id);
            if (roleToUpdate == null)
            {
                throw new InvalidOperationException(ResHelper.GetString("general.rolenotfound"));
            }

            roleToUpdate.RoleName = role.Name;
            roleToUpdate.RoleDisplayName = role.DisplayName;
            RoleInfoProvider.SetRoleInfo(roleToUpdate);

            return Task.FromResult(0);
        }
    }
}
