using CMS.Membership;

using Microsoft.AspNet.Identity;

namespace Kentico.Membership
{
    /// <summary>
    /// Application identity role.
    /// </summary>
    public class Role : IRole<int>
    {
        /// <summary>
        /// Role ID.
        /// </summary>
        public int Id
        {
            get;
            set;
        }


        /// <summary>
        /// Role name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }


        /// <summary>
        /// Role display name.
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }


        /// <summary>
        /// Creates a new instance of <see cref="Role"/> based on <see cref="RoleInfo"/>.
        /// </summary>
        /// <param name="roleInfo">Role info (CMS entity).</param>
        public Role(RoleInfo roleInfo)
        {
            Id = roleInfo.RoleID;
            Name = roleInfo.RoleName;
            DisplayName = roleInfo.RoleDisplayName;
        }


        /// <summary>
        /// Creates empty role.
        /// </summary>
        public Role()
        { 
        }
    }
}
