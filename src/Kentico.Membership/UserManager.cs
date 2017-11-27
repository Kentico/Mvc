using System;
using System.Threading.Tasks;

using CMS.Membership;
using CMS.Helpers;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Owin;

namespace Kentico.Membership
{
    /// <summary>
    /// Exposes user related API which will automatically save changes to the UserStore.
    /// </summary>
    public class UserManager : UserManager<User, int>
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserManager"/>.
        /// </summary>
        /// <param name="store">User store.</param>
        public UserManager(IUserStore<User, int> store)
        : base(store)
        {
        }


        /// <summary>
        /// Factory method that creates the user manager with <see cref="UserStore"/>.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="manager">Manager to be initialized.</param>
        public static T Initialize<T>(IAppBuilder app, T manager) where T : UserManager
        {
            var provider = app.GetDataProtectionProvider();
            if (provider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(provider.Create("Kentico.Membership"));
            }

            manager.PasswordValidator = new PasswordValidator();
            manager.UserLockoutEnabledByDefault = false;
            manager.EmailService = new EmailService();
            manager.UserValidator = new UserValidator<User, int>(manager);

            return manager;
        }


        /// <summary>
        /// Updates the user password.
        /// </summary>
        /// <param name="passwordStore">Unused implementation of UserPasswordStore.</param>
        /// <param name="user">User.</param>
        /// <param name="newPassword">New password in plain text format.</param>
        protected override async Task<IdentityResult> UpdatePassword(IUserPasswordStore<User, int> passwordStore, User user, string newPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = await PasswordValidator.ValidateAsync(newPassword);
            if (!result.Succeeded)
            {
                return result;
            }

            UserInfo userInfo = UserInfoProvider.GetUserInfo(user.Id);

            if (userInfo == null)
            {
                user.GUID = Guid.NewGuid();
                user.PasswordHash = UserInfoProvider.GetPasswordHash(newPassword, UserInfoProvider.NewPasswordFormat, user.GUID.ToString()); 
            }
            else
            {
                UserInfoProvider.SetPassword(userInfo, newPassword);
                user.PasswordHash = ValidationHelper.GetString(userInfo.GetValue("UserPassword"), string.Empty);
                await UpdateSecurityStampInternal(user);
            }

            return IdentityResult.Success;
        }


        /// <summary>
        /// Verifies the user password.
        /// </summary>
        /// <param name="store">Unused implementation of UserPasswordStore.</param>
        /// <param name="user">User.</param>
        /// <param name="password">Password in plain text format.</param>
        protected override Task<bool> VerifyPasswordAsync(IUserPasswordStore<User, int> store, User user, string password)
        {
            if (user == null)
            {
                return Task.FromResult(false);
            }

            var userInfo = UserInfoProvider.GetUserInfo(user.UserName);
            var result = !userInfo.IsExternal && !userInfo.UserIsDomain && !UserInfoProvider.IsUserPasswordDifferent(userInfo, password);

            return Task.FromResult(result);
        }


        /// <summary>
        /// Updates the security stamp if the store supports it.
        /// </summary>
        /// <param name="user">User whose stamp should be updated.</param>
        internal async Task UpdateSecurityStampInternal(User user)
        {
            if (SupportsUserSecurityStamp)
            {
                await GetSecurityStore().SetSecurityStampAsync(user, NewSecurityStamp());
            }
        }


        private IUserSecurityStampStore<User, int> GetSecurityStore()
        {
            var cast = Store as IUserSecurityStampStore<User, int>;
            if (cast == null)
            {
                throw new NotSupportedException("Current Store does not implement the IUserSecurityStore interface.");
            }
            return cast;
        }


        private string NewSecurityStamp()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
