using System;

using CMS.Membership;
using CMS.SiteProvider;
using CMS.Helpers;

using Microsoft.AspNet.Identity;

namespace Kentico.Membership
{
    /// <summary>
    /// Representation of user identity.
    /// </summary>
    public class User : IUser<int>
    {
        /// <summary>
        /// User ID.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// User name.
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// First name.
        /// </summary>
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Last name.
        /// </summary>
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Email.
        /// </summary>
        public string Email
        {
            get;
            set;
        }


        /// <summary>
        /// Password hash.
        /// </summary>
        public string PasswordHash
        {
            get;
            set;
        }


        /// <summary>
        /// Indicates if the user is enabled.
        /// This represents the lockout notion in ASP.NET Identity.
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }


        /// <summary>
        /// Guid.
        /// </summary>
        public Guid GUID
        {
            get;
            set;
        }


        /// <summary>
        /// A unique value that should change whenever user credentials have changed, e.g.:
        /// password reset, external login removal, etc. 
        /// </summary>
        public string SecurityStamp
        {
            get;
            set;
        }


        /// <summary>
        /// Roles of the user.
        /// </summary>
        public string[] Roles
        {
            get
            {
                return UserInfoProvider.GetRolesForUser(UserName, SiteContext.CurrentSiteName);
            }
        }


        /// <summary>
        /// Indicates that the user can be logged in only through an external authentication provider.
        /// </summary>
        public bool IsExternal
        {
            get;
            set;
        }


        /// <summary>
        /// Creates new user from <see cref="UserInfo"/>.
        /// </summary>
        /// <param name="userInfo">User info (CMS entity).</param>
        public User(UserInfo userInfo)
        {
            if (userInfo == null)
            {
                return;
            }

            Id = userInfo.UserID;
            UserName = userInfo.UserName;
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            Email = userInfo.Email;
            Enabled = userInfo.Enabled;
            GUID = userInfo.UserGUID;
            IsExternal = userInfo.IsExternal;
            PasswordHash = ValidationHelper.GetString(userInfo.GetValue("UserPassword"), string.Empty);
            SecurityStamp = userInfo.UserSecurityStamp;
        }


        /// <summary>
        /// Creates empty user.
        /// </summary>
        public User()
        {
        }
    }
}
