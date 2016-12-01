using System;
using System.Linq;
using System.Threading.Tasks;
using CMS.Tests;
using CMS.Membership;
using CMS.DataEngine;

using NUnit.Framework;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Kentico.Membership.Tests
{
    public class TestIdentityFactoryProvider : IdentityFactoryProvider<UserManager>
    {
        public TestIdentityFactoryProvider()
        {
            OnCreate = ((options, context) =>
            {
                var manager = new UserManager(new UserStore("NonExistingSiteForTestingPurposes"));
                manager.PasswordValidator = new PasswordValidator();
                manager.UserLockoutEnabledByDefault = false;
                manager.EmailService = new EmailService();
                manager.UserValidator = new UserValidator<User, int>(manager);
                return manager;
            });
        }
    }


    [TestFixture, Category.IsolatedIntegration]
    public class OwinIntegrationTests : IsolatedIntegrationTests
    {
        private readonly MembershipFakeFactory mMembershipFakeFactory = new MembershipFakeFactory();
        private OwinContext mOwinContext;
        private SignInManager mSignInManager;

        public static object[] UsersWithPasswords = new object[]
        {
            new object[] { MembershipFakeFactory.USERNAME_WITH_PASSWORD, MembershipFakeFactory.TEST_PASSWORD },
            new object[] { MembershipFakeFactory.USERNAME_NO_PASSWORD, String.Empty }
        };


        [SetUp]
        public async Task SetUp()
        {
            mMembershipFakeFactory.GetIntegrationUsers().ToList().ForEach(x => UserInfoProvider.SetUserInfo(x));
            mMembershipFakeFactory.GetIntegrationRoles().ToList().ForEach(x => RoleInfoProvider.SetRoleInfo(x));
            mMembershipFakeFactory.GetIntegrationExternalLogins().ToList().ForEach(x => ExternalLoginInfoProvider.SetExternalLoginInfo(x));
            SettingsKeyInfoProvider.SetValue("cmssitesharedaccounts", 0, "True");

            mOwinContext = new OwinContext();
            var options = new IdentityFactoryOptions<UserManager>
            {
                Provider = new TestIdentityFactoryProvider(),
                DataProtectionProvider = new DpapiDataProtectionProvider()
            };
            var middleware = new IdentityFactoryMiddleware<UserManager, IdentityFactoryOptions<UserManager>>(null, options);
            await middleware.Invoke(mOwinContext);
            var manager = mOwinContext.GetUserManager<UserManager>();

            mSignInManager = new SignInManager(mOwinContext.GetUserManager<UserManager>(), mOwinContext.Authentication);
        }

        
        [Test, TestCaseSource(nameof(UsersWithPasswords))]
        public async Task PasswordSignInAsync_CorrectCredentials_AdminUser_UserSignedIn(string userName, string password)
        {
            var user = UserInfoProvider.GetUserInfo(userName);
            UserRoleInfoProvider.AddUserToRole(user, mMembershipFakeFactory.AdminRole);
            var result = await mSignInManager.PasswordSignInAsync(user.UserName, password, false, false);
            var claims = mOwinContext.Authentication.AuthenticationResponseGrant.Identity.Claims;

            CMSAssert.All(
                () => Assert.AreEqual(SignInStatus.Success, result),
                () => Assert.AreEqual(5, claims.Count()),
                () => Assert.IsTrue(claims.Any(x => x.Value == user.UserID.ToString()), "ID claim missing"),
                () => Assert.IsTrue(claims.Any(x => x.Value == user.UserName), "Name claim missing"),
                () => Assert.IsTrue(claims.Any(x => x.Value == user.UserSecurityStamp), "Security stamp claim missing"),
                () => Assert.IsTrue(claims.Any(x => x.Value == MembershipFakeFactory.ROLE_ADMIN), "Role claim missing"));
        }


        [Test]
        public async Task PasswordSignInAsync_ClaimsContainCorrectSecurityStamp()
        {
            var user = UserInfoProvider.GetUserInfo(MembershipFakeFactory.USERNAME_WITH_SECURITY_STAMP);
            var result = await mSignInManager.PasswordSignInAsync(user.UserName, String.Empty, false, false);
            var claims = mOwinContext.Authentication.AuthenticationResponseGrant.Identity.Claims;

            CMSAssert.All(
                () => Assert.AreEqual(SignInStatus.Success, result),
                () => Assert.AreEqual(4, claims.Count()),
                () => Assert.IsTrue(claims.Any(x => x.Value == MembershipFakeFactory.SECURITY_STAMP), "Security stamp in claims is incorrect."));
        }


        [Test, TestCaseSource(nameof(UsersWithPasswords))]
        public async Task PasswordSignInAsync_UserDisabled_UserNotSignedIn(string userName, string password)
        {
            var user = UserInfoProvider.GetUserInfo(userName);
            user.Enabled = false;
            UserInfoProvider.SetUserInfo(user);

            var result = await mSignInManager.PasswordSignInAsync(user.UserName, password, false, false);

            CMSAssert.All(
                () => Assert.AreEqual(SignInStatus.LockedOut, result),
                () => Assert.IsNull(mOwinContext.Authentication.AuthenticationResponseGrant));
        }

        
        [Test, TestCaseSource(nameof(UsersWithPasswords))]
        public async Task PasswordSignInAsync_IncorrectPassword_UserNotSignedIn(string userName, string password)
        {
            var user = UserInfoProvider.GetUserInfo(userName);

            var result = await mSignInManager.PasswordSignInAsync(user.UserName, "incorrectPassword", false, false);

            CMSAssert.All(
                () => Assert.AreEqual(SignInStatus.Failure, result),
                () => Assert.IsNull(mOwinContext.Authentication.AuthenticationResponseGrant));
        }

        
        [Test, TestCaseSource(nameof(UsersWithPasswords))]
        public async Task SignOut_CorrectCredentials_AdminUser_UserSignedInAndOut(string userName, string password)
        {
            var user = UserInfoProvider.GetUserInfo(userName);
            UserRoleInfoProvider.AddUserToRole(user, mMembershipFakeFactory.AdminRole);

            var signInResult = await mSignInManager.PasswordSignInAsync(user.UserName, password, false, false);
            var signedInClaimsCount = mOwinContext.Authentication.AuthenticationResponseGrant.Identity.Claims.Count();
            mOwinContext.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            CMSAssert.All(
                () => Assert.AreEqual(SignInStatus.Success, signInResult),
                () => Assert.AreEqual(5, signedInClaimsCount),
                () => Assert.IsNull(mOwinContext.Authentication.AuthenticationResponseGrant));
        }


        [Test, TestCaseSource(nameof(UsersWithPasswords))]
        public async Task SignInAsync_AdminUser_UserSignedInUnconditionally(string userName, string password)
        {
            var userInfo = UserInfoProvider.GetUserInfo(userName);
            UserRoleInfoProvider.AddUserToRole(userInfo, mMembershipFakeFactory.AdminRole);
            var user = new User(userInfo);

            await mSignInManager.SignInAsync(user, false, false);
            var claims = mOwinContext.Authentication.AuthenticationResponseGrant.Identity.Claims;

            CMSAssert.All(
                () => Assert.AreEqual(5, claims.Count()),
                () => Assert.IsTrue(claims.Any(x => x.Value == user.Id.ToString()), "ID claim missing"),
                () => Assert.IsTrue(claims.Any(x => x.Value == user.UserName), "Name claim missing"),
                () => Assert.IsTrue(claims.Any(x => x.Value == user.SecurityStamp), "Security stamp claim missing"),
                () => Assert.IsTrue(claims.Any(x => x.Value == MembershipFakeFactory.ROLE_ADMIN), "Role claim missing"));
        }


        [Test]
        public async Task SignInAsync_ClaimsContainCorrectSecurityStamp()
        {
            var userInfo = UserInfoProvider.GetUserInfo(MembershipFakeFactory.USERNAME_WITH_SECURITY_STAMP);
            var user = new User(userInfo);

            await mSignInManager.SignInAsync(user, false, false);
            var claims = mOwinContext.Authentication.AuthenticationResponseGrant.Identity.Claims;

            CMSAssert.All(
                () => Assert.AreEqual(4, claims.Count()),
                () => Assert.IsTrue(claims.Any(x => x.Value == MembershipFakeFactory.SECURITY_STAMP), "Security stamp in claims is incorrect."));
        }


        [Test]
        public async Task ExternalSignInAsync_ExternalUser_UserSignedIn()
        {
            var externalLoginInfo = new Microsoft.AspNet.Identity.Owin.ExternalLoginInfo
            {
                Login = new UserLoginInfo(mMembershipFakeFactory.ExternalLogin.LoginProvider, mMembershipFakeFactory.ExternalLogin.IdentityKey)
            };

            var result = await mSignInManager.ExternalSignInAsync(externalLoginInfo, false);
            var claims = mOwinContext.Authentication.AuthenticationResponseGrant.Identity.Claims;

            CMSAssert.All(
                () => Assert.AreEqual(SignInStatus.Success, result),
                () => Assert.AreEqual(4, claims.Count()),
                () => Assert.IsTrue(claims.Any(x => x.Value == mMembershipFakeFactory.UserExternal.UserID.ToString()), "ID claim missing"),
                () => Assert.IsTrue(claims.Any(x => x.Value == mMembershipFakeFactory.UserExternal.UserName), "Name claim missing"),
                () => Assert.IsTrue(claims.Any(x => x.Value == mMembershipFakeFactory.UserExternal.UserSecurityStamp), "Security stamp claim missing"));
        }


        [Test]
        public async Task ExternalSignInAsync_ClaimsContainCorrectSecurityStamp()
        {
            var externalLoginInfo = new Microsoft.AspNet.Identity.Owin.ExternalLoginInfo
            {
                Login = new UserLoginInfo(mMembershipFakeFactory.ExternalLoginWithSecurityStamp.LoginProvider, mMembershipFakeFactory.ExternalLoginWithSecurityStamp.IdentityKey)
            };

            var result = await mSignInManager.ExternalSignInAsync(externalLoginInfo, false);
            var claims = mOwinContext.Authentication.AuthenticationResponseGrant.Identity.Claims;

            CMSAssert.All(
                () => Assert.AreEqual(SignInStatus.Success, result),
                () => Assert.AreEqual(4, claims.Count()),
                () => Assert.IsTrue(claims.Any(x => x.Value == MembershipFakeFactory.SECURITY_STAMP), "Security stamp in claims is incorrect."));
        }


        [Test]
        public async Task ExternalSignInAsync_ExternalDisabledUser_UserNotSignedIn()
        {
            mMembershipFakeFactory.UserExternal.Enabled = false;
            UserInfoProvider.SetUserInfo(mMembershipFakeFactory.UserExternal);

            var externalLoginInfo = new Microsoft.AspNet.Identity.Owin.ExternalLoginInfo
            {
                Login = new UserLoginInfo(mMembershipFakeFactory.ExternalLogin.LoginProvider, mMembershipFakeFactory.ExternalLogin.IdentityKey)
            };

            var result = await mSignInManager.ExternalSignInAsync(externalLoginInfo, false);

            CMSAssert.All(
                () => Assert.AreEqual(SignInStatus.LockedOut, result),
                () => Assert.IsNull(mOwinContext.Authentication.AuthenticationResponseGrant));
        }


        [Test]
        public async Task CreateAsync_UserNameAlreadyTaken_WontUpdatePasswordOfExistingUser()
        {
            var existingUser = mMembershipFakeFactory.UserWithPassword;
            var newUser = new User
            {
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                UserName = existingUser.UserName,
                Email = existingUser.Email,
                Enabled = true
            };

            var userManager = mOwinContext.GetUserManager<UserManager>();

            var identityResult = await userManager.CreateAsync(newUser, "THIS IS A PASSWORD FOR THE NEW USER");
            existingUser = UserInfoProvider.GetUserInfo(existingUser.UserID);

            CMSAssert.All(
                () => Assert.False(identityResult.Succeeded, 
                        "Creation of user with already taken UserName should fail."),

                () => Assert.AreEqual(MembershipFakeFactory.TEST_PASSWORD, existingUser.GetValue("UserPassword"), 
                        "Password of existing user should be unchanged."));

        }
    }
}
