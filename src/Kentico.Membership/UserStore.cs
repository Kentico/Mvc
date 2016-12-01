using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using CMS.DataEngine;
using CMS.Membership;
using CMS.Helpers;
using CMS.SiteProvider;

using Microsoft.AspNet.Identity;

namespace Kentico.Membership
{
    /// <summary>
    /// Implements basic user management operations.
    /// </summary>
    public class UserStore : IUserPasswordStore<User, int>,
                             IUserLockoutStore<User, int>,
                             IUserTwoFactorStore<User, int>,
                             IUserRoleStore<User, int>,
                             IUserEmailStore<User, int>,
                             IUserLoginStore<User, int>,
                             IUserSecurityStampStore<User, int>
    {
        private readonly string mSiteName;
        private int mSiteId;


        /// <summary>
        /// Returns <see cref="SiteInfo.SiteID"/> that belongs to <see cref="SiteInfo"/> managed by <see cref="UserStore"/>.
        /// </summary>
        private int SiteID
        {
            get
            {
                return mSiteId == 0 ? mSiteId = SiteInfoProvider.GetSiteID(mSiteName) : mSiteId;
            }
        }


        /// <summary>
        /// Creates store to manage users for given site.
        /// </summary>
        /// <param name="siteName">Site name that represents <see cref="SiteInfo"/> for which users are managed.</param>
        public UserStore(string siteName)
        {
            if (String.IsNullOrWhiteSpace(siteName))
            {
                throw new ArgumentException($"{nameof(siteName)} is null or empty.", nameof(siteName));
            }

            mSiteName = siteName;
        }


        /// <summary>
        /// Inserts new user to the database.
        /// </summary>
        /// <param name="user">User.</param>
        public Task CreateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userInfo = new UserInfo()
            {
                UserName = user.UserName,
                FullName = UserInfoProvider.GetFullName(user.FirstName, null, user.LastName),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Enabled = user.Enabled,
                UserGUID = user.GUID,
                PasswordFormat = UserInfoProvider.NewPasswordFormat,
                UserPasswordLastChanged = DateTime.Now,
                IsExternal = user.IsExternal,
                UserSecurityStamp = user.SecurityStamp
            };

            userInfo.UserNickName = userInfo.GetFormattedUserName(true);

            userInfo.SetValue("UserPassword", user.PasswordHash);

            UserInfoProvider.SetUserInfo(userInfo);
            UserInfoProvider.AddUserToSite(userInfo.UserName, mSiteName);

            user.Id = userInfo.UserID;

            return Task.FromResult(0);
        }


        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="user">User.</param>
        public Task DeleteAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            UserInfoProvider.DeleteUser(user.Id);
            return Task.FromResult(0);
        }


        /// <summary>
        /// Finds the user by user's ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        public Task<User> FindByIdAsync(int userId)
        {
            var userInfo = UserInfoProvider.GetUserInfo(userId);
            if (userInfo == null)
            {
                return Task.FromResult((User)null);
            }

            var user = new User(UserInfoProvider.CheckUserBelongsToSite(userInfo, mSiteName));

            return Task.FromResult(user);
        }


        /// <summary>
        /// Finds the user by user's username.
        /// </summary>
        /// <param name="userName">Username.</param>
        public Task<User> FindByNameAsync(string userName)
        {
            var userInfo = UserInfoProvider.GetUserInfo(userName);
            if (userInfo == null)
            {
                return Task.FromResult((User)null);
            }

            var user = new User(UserInfoProvider.CheckUserBelongsToSite(userInfo, mSiteName));

            return Task.FromResult(user);
        }


        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">User.</param>
        public Task UpdateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userInfo = UserInfoProvider.GetUserInfo(user.Id);
            if (userInfo == null)
            {
                throw new Exception(ResHelper.GetString("general.usernotfound"));
            }

            userInfo.UserName = user.UserName;
            userInfo.Email = user.Email;
            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            userInfo.FullName = UserInfoProvider.GetFullName(user.FirstName, null, user.LastName);
            userInfo.UserNickName = userInfo.GetFormattedUserName(true);
            userInfo.Enabled = user.Enabled;
            userInfo.UserSecurityStamp = user.SecurityStamp;

            UserInfoProvider.SetUserInfo(userInfo);

            return Task.FromResult(0);
        }


        /// <summary>
        /// Performs tasks to dispose the user store.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Disposes the user store.
        /// </summary>
        /// <param name="disposing">Describes whether or not should the managed resources be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
        }


        /// <summary>
        /// Gets the password hash for the user.
        /// </summary>
        /// <param name="user">User.</param>
        public Task<string> GetPasswordHashAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            UserInfo userInfo = UserInfoProvider.GetUserInfo(user.UserName);
            if (userInfo == null)
            {
                throw new Exception(ResHelper.GetString("general.usernotfound"));
            }

            return Task.FromResult(ValidationHelper.GetString(userInfo.GetValue("UserPassword"), string.Empty));
        }


        /// <summary>
        /// Returns true if the user has the password set.
        /// </summary>
        /// <param name="user">User.</param>
        public Task<bool> HasPasswordAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            UserInfo userInfo = UserInfoProvider.GetUserInfo(user.UserName);
            if (userInfo == null)
            {
                throw new Exception(ResHelper.GetString("general.usernotfound"));
            }

            return Task.FromResult(!String.IsNullOrEmpty(ValidationHelper.GetString(userInfo.GetValue("UserPassword"), string.Empty)));
        }


        /// <summary>
        /// Sets the password hash for the user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="passwordHash">Password hash.</param>
        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            UserInfo userInfo = UserInfoProvider.GetUserInfo(user.UserName);
            if (userInfo != null)
            {
                userInfo.SetValue("UserPassword", passwordHash);
            }

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }


        /// <summary>
        /// Returns the current number of failed access attempts.
        /// </summary>
        /// <param name="user">User.</param>
        public Task<int> GetAccessFailedCountAsync(User user)
        {
            return Task.FromResult(0);
        }


        /// <summary>
        /// Returns whether the user can be locked out.
        /// </summary>
        /// <param name="user">User.</param>
        public Task<bool> GetLockoutEnabledAsync(User user)
        {
            return Task.FromResult(true);
        }


        /// <summary>
        /// Returns the DateTimeOffset that represents the end of the user's lockout, any time
        /// in the past should be considered not locked out.
        /// </summary>
        /// <param name="user">User.</param>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            if (user.Enabled)
            {
                return Task.FromResult(DateTimeOffset.MinValue);
            }
            else
            {
                return Task.FromResult(DateTimeOffset.MaxValue);
            }
        }


        /// <summary>
        /// Used to record when an attempt to access the user has failed.
        /// </summary>
        /// <param name="user">User.</param>
        public Task<int> IncrementAccessFailedCountAsync(User user)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Used to reset the access failed count, typically after the account is successfully accessed.
        /// </summary>
        /// <param name="user">User.</param>
        public Task ResetAccessFailedCountAsync(User user)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets whether the user can be locked out.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="enabled">Whether the user can be locked out.</param>
        public Task SetLockoutEnabledAsync(User user, bool enabled)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Locks a user out until the specified end date (set to a past date, to unlock a user).
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="lockoutEnd">DateTimeOffset that represents the end of a user's lockout.</param>
        public Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns whether two factor authentication is enabled for the user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <remarks>Not enabled in current implementation.</remarks>
        public Task<bool> GetTwoFactorEnabledAsync(User user)
        {
            return Task.FromResult(false);
        }


        /// <summary>
        /// Sets whether two factor authentication is enabled for the user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="enabled">Whether users should go through two-factor authentication.</param>
        /// <remarks>Not used in current implementation.</remarks>
        public Task SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns all role names for the given user.
        /// </summary>
        /// <param name="user">User entity.</param>
        /// <returns>List of role names.</returns>
        public Task<IList<string>> GetRolesAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(UserInfoProvider.GetRolesForUser(user.UserName, mSiteName).ToList() as IList<string>);
        }


        /// <summary>
        /// Checks whether user is in role with given role name.
        /// </summary>
        /// <param name="user">User entity.</param>
        /// <param name="roleName">Role name.</param>
        /// <returns>True if user is in role, false otherwise.</returns>
        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(UserInfoProvider.IsUserInRole(user.UserName, roleName, mSiteName));
        }


        /// <summary>
        /// Removes <see cref="User"/> from <see cref="Role"/>.
        /// </summary>
        /// <param name="user">User entity.</param>
        /// <param name="roleName">Role name.</param>
        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty", nameof(roleName));
            }
            
            var userRole = UserRoleInfoProvider.GetUserRoles()
                                                   .WhereEquals("UserID", user.Id)
                                                   .WhereEquals("RoleID", GetRoleByRoleName(roleName, SiteID).RoleID)
                                                   .FirstObject;
            UserRoleInfoProvider.DeleteUserRoleInfo(userRole);

            return Task.FromResult(0);
        }


        /// <summary>
        /// Adds <see cref="User"/> to <see cref="Role"/>.
        /// </summary>
        /// <param name="user">User entity.</param>
        /// <param name="roleName">Role name.</param>
        public Task AddToRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty", nameof(roleName));
            }
            
            UserRoleInfoProvider.AddUserToRole(user.Id, GetRoleByRoleName(roleName, SiteID).RoleID);

            return Task.FromResult(0);
        }


        /// <summary>
        /// Sets the user email.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="email">Email.</param>
        public Task SetEmailAsync(User user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            UserInfo userInfo = UserInfoProvider.GetUserInfo(user.UserName);

            if (userInfo == null)
            {
                throw new Exception(ResHelper.GetString("general.usernotfound"));
            }

            userInfo.Email = email;
            UserInfoProvider.SetUserInfo(userInfo);

            return Task.FromResult(0);
        }


        /// <summary>
        /// Gets the user email.
        /// </summary>
        /// <param name="user">User.</param>
        public Task<string> GetEmailAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }


        /// <summary>
        /// Returns true if the user email is confirmed.
        /// </summary>
        /// <param name="user">User.</param>
        public Task<bool> GetEmailConfirmedAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Enabled);
        }


        /// <summary>
        /// Enables user if her email was confirmed.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="confirmed">Indicates if the user email is confirmed.</param>
        public Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Enabled = confirmed;
            SetSecurityStampAsync(user, Guid.NewGuid().ToString());

            return Task.FromResult(0);
        }


        /// <summary>
        /// Returns the user associated with the given email.
        /// </summary>
        /// <param name="email">Email.</param>
        public Task<User> FindByEmailAsync(string email)
        {
            var users = UserInfoProvider.GetUsers()
                                        .Where("Email", QueryOperator.Equals, email)
                                        .ToList()
                                        .Where(u => UserInfoProvider.CheckUserBelongsToSite(u, mSiteName) != null);

            if (users.Count() != 1)
            {
                return Task.FromResult((User)null);
            }
            return Task.FromResult(new User(users.First()));
        }


        /// <summary>
        /// Adds external login to user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="login">External login information.</param>
        public Task AddLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            ExternalLoginInfoProvider.SetExternalLoginInfo(new ExternalLoginInfo
            {
                UserID = user.Id,
                LoginProvider = login.LoginProvider,
                IdentityKey = login.ProviderKey
            });

            return Task.FromResult(0);
        }


        /// <summary>
        /// Removes external login to user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="login">External login information.</param>
        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var info = ExternalLoginInfoProvider.GetExternalLogins()
                                                .WhereEquals("LoginProvider", login.LoginProvider)
                                                .WhereEquals("IdentityKey", login.ProviderKey)
                                                .WhereEquals("UserID", user.Id);
            ExternalLoginInfoProvider.DeleteExternalLoginInfo(info);

            return Task.FromResult(0);
        }


        /// <summary>
        /// Returns all external logins of user.
        /// </summary>
        /// <param name="user">User.</param>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<UserLoginInfo> logins = ExternalLoginInfoProvider.GetExternalLogins()
                                                                   .WhereEquals("UserID", user.Id)
                                                                   .Select(x => new UserLoginInfo(x.LoginProvider, x.IdentityKey))
                                                                   .ToList();
            return Task.FromResult(logins);
        }


        /// <summary>
        /// Returns user based on given external login.
        /// </summary>
        /// <param name="login">External login information.</param>
        public Task<User> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var loginInfo = ExternalLoginInfoProvider.GetExternalLogins()
                                                .WhereEquals("LoginProvider", login.LoginProvider)
                                                .WhereEquals("IdentityKey", login.ProviderKey)
                                                .FirstObject;

            if (loginInfo != null)
            {
                return FindByIdAsync(loginInfo.UserID);
            }

            return Task.FromResult<User>(null);
        }


        /// <summary>
        /// Sets user's security stamp.
        /// </summary>
        /// <param name="user">User to which stamp should be assigned.</param>
        /// <param name="stamp">Stamp to be assigned.</param>
        public Task SetSecurityStampAsync(User user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }


        /// <summary>
        /// Returns user's security stamp. 
        /// </summary>
        /// <param name="user">User whose stamp should be returned.</param>
        public Task<string> GetSecurityStampAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            UserInfo userInfo = UserInfoProvider.GetUserInfo(user.UserName);
            if (userInfo == null)
            {
                throw new Exception(ResHelper.GetString("general.usernotfound"));
            }

            return Task.FromResult(userInfo.UserSecurityStamp);
        }


        #region "Private members"

        private RoleInfo GetRoleByRoleName(string roleName, int siteId)
        {
            var role = RoleInfoProvider.GetRoleInfo(roleName, siteId);
            if (role == null)
            {
                throw new InvalidOperationException(ResHelper.GetString("general.rolenotfound"));
            }

            return role;
        }

        #endregion
    }
}
