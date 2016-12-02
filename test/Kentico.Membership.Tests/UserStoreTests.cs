using System;
using System.Linq;
using System.Threading.Tasks;

using CMS.Tests;
using CMS.Membership;
using CMS.DataEngine;

using NUnit.Framework;
using Microsoft.AspNet.Identity;

namespace Kentico.Membership.Tests
{
    [TestFixture]
    public class UserStoreTests : UnitTests
    {
        private readonly MembershipFakeFactory mMembershipFakeFactory = new MembershipFakeFactory();
        private UserStore mUserStore;


        [SetUp]
        public void SetUp()
        {
            Fake<UserSettingsInfo, UserSettingsInfoProvider>().WithData();
            Fake<UserInfo, UserInfoProvider>().WithData(mMembershipFakeFactory.GetUsers());
            Fake<SettingsKeyInfo, SettingsKeyInfoProvider>().WithData(mMembershipFakeFactory.GetSharedUsersSetting());
            Fake<ExternalLoginInfo, ExternalLoginInfoProvider>().WithData(mMembershipFakeFactory.GetExternalLogins());

            mUserStore = new UserStore("NonExistingSiteForTestingPurposes");
        }


        [Test]
        public void UpdateAsync_Null_NullException()
        {
            Assert.That(async () => await mUserStore.UpdateAsync(null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public void UpdateAsync_NonExistentUser_NotFoundException()
        {
            Assert.That(async () => await mUserStore.UpdateAsync(mMembershipFakeFactory.NonExistentUser), Throws.Exception.With.Message.EqualTo("general.usernotfound"));
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase(MembershipFakeFactory.USERNAME_NONEXISTENT)]
        public async Task FindByNameAsync_NonExistentUser_Null(string searchString)
        {
            Assert.IsNull(await mUserStore.FindByNameAsync(searchString));
        }


        [TestCase(MembershipFakeFactory.USERNAME_ENABLED_WITH_EMAIL)]
        [TestCase(MembershipFakeFactory.USERNAME_DISABLED_WITH_EMAIL)]
        [TestCase(MembershipFakeFactory.USERNAME_ENABLED_NO_EMAIL)]
        [TestCase(MembershipFakeFactory.USERNAME_DISABLED_NO_EMAIL)]
        [TestCase(MembershipFakeFactory.USERNAME_DUPLICATE_EMAIL1)]
        public async Task FindByNameAsync_ReturnsCorrectUser(string searchString)
        {
            var user = await mUserStore.FindByNameAsync(searchString);
            Assert.AreEqual(searchString, user.UserName);
        }


        [TestCase(MembershipFakeFactory.USERNAME_WITH_PASSWORD, true)]
        [TestCase(MembershipFakeFactory.USERNAME_NO_PASSWORD, false)]
        public async Task HasPasswordAsync_TestCases(string userName, bool hasPassword)
        {
            var user = new User(UserInfoProvider.GetUserInfo(userName));

            Assert.AreEqual(hasPassword, await mUserStore.HasPasswordAsync(user));
        }


        [Test]
        public void HasPasswordAsync_Null_NullException()
        {
            Assert.That(async () => await mUserStore.HasPasswordAsync(null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public void HasPasswordAsync_NonExistentUser_UserNotFoundException()
        {
            Assert.That(async () => await mUserStore.HasPasswordAsync(mMembershipFakeFactory.NonExistentUser), Throws.Exception.With.Message.EqualTo("general.usernotfound"));
        }


        [Test]
        public void GetPasswordHashAsync_Null_NullException()
        {
            Assert.That(async () => await mUserStore.GetPasswordHashAsync(null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public void GetPasswordHashAsync_NonExistentUser_NotFoundException()
        {
            Assert.That(async () => await mUserStore.GetPasswordHashAsync(mMembershipFakeFactory.NonExistentUser), Throws.Exception.With.Message.EqualTo("general.usernotfound"));
        }


        [TestCase(MembershipFakeFactory.USERNAME_WITH_PASSWORD, MembershipFakeFactory.TEST_PASSWORD)]
        [TestCase(MembershipFakeFactory.USERNAME_NO_PASSWORD, "")]
        public async Task GetPasswordHashAsync_TestCases(string userName, string userPassword)
        {
            var user = new User(UserInfoProvider.GetUserInfo(userName));

            Assert.AreEqual(userPassword, await mUserStore.GetPasswordHashAsync(user));
        }


        [Test]
        public void SetPasswordHashAsync_Null_NullException()
        {
            Assert.That(async () => await mUserStore.SetPasswordHashAsync(null, MembershipFakeFactory.TEST_PASSWORD), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public async Task SetPasswordHashAsync_NonExistentUser_NoException()
        {
            await mUserStore.SetPasswordHashAsync(mMembershipFakeFactory.NonExistentUser, MembershipFakeFactory.TEST_PASSWORD);

            CMSAssert.All(
                () => Assert.AreEqual(MembershipFakeFactory.TEST_PASSWORD, mMembershipFakeFactory.NonExistentUser.PasswordHash),
                () => Assert.IsNull(UserInfoProvider.GetUserInfo(mMembershipFakeFactory.NonExistentUser.UserName)));
        }


        [Test]
        public async Task SetPasswordHashAsync_User_NoException()
        {
            var userInfo = UserInfoProvider.GetUserInfo(MembershipFakeFactory.USERNAME_NO_PASSWORD);

            var user = new User(userInfo);

            await mUserStore.SetPasswordHashAsync(user, MembershipFakeFactory.TEST_PASSWORD);

            CMSAssert.All(
                () => Assert.AreEqual(MembershipFakeFactory.TEST_PASSWORD, user.PasswordHash),
                () => Assert.AreEqual(MembershipFakeFactory.TEST_PASSWORD, userInfo.GetValue("UserPassword")));
        }


        [Test]
        public async Task FindByIdAsync_NonExistentUser_Null()
        {
            Assert.IsNull(await mUserStore.FindByIdAsync(0));
        }


        [Test]
        public async Task FindByIdAsync_UserID_ReturnsCorrectUser()
        {
            var user = await mUserStore.FindByIdAsync(1);
            Assert.AreEqual(MembershipFakeFactory.USERNAME_ENABLED_WITH_EMAIL, user.UserName);
        }


        [Test]
        public async Task GetAccessFailedCountAsync_User_ReturnsZero()
        {
            Assert.AreEqual(0, await mUserStore.GetAccessFailedCountAsync(new User()));
        }


        [Test]
        public async Task GetLockoutEnabledAsync_User_ReturnsTrue()
        {
            Assert.AreEqual(true, await mUserStore.GetLockoutEnabledAsync(new User()));
        }


        [Test]
        public async Task GetLockoutEndDateAsync_EnabledUser_MinValue()
        {
            var user = new User { Enabled = true };

            var lockoutDate = await mUserStore.GetLockoutEndDateAsync(user);

            Assert.AreEqual(DateTimeOffset.MinValue, lockoutDate);
        }


        [Test]
        public async Task GetLockoutEndDateAsync_DisabledUser_MaxValue()
        {
            var user = new User { Enabled = false };

            var lockoutDate = await mUserStore.GetLockoutEndDateAsync(user);

            Assert.AreEqual(DateTimeOffset.MaxValue, lockoutDate);
        }


        [Test]
        public async Task GetEmailAsync_User_UserEmail()
        {
            var user = new User { Email = "test@email.com" };

            var email = await mUserStore.GetEmailAsync(user);

            Assert.AreEqual("test@email.com", email);
        }


        [Test]
        public async Task GetEmailConfirmedAsync_EnabledUser_True()
        {
            var user = new User { Enabled = true };

            var confirmed = await mUserStore.GetEmailConfirmedAsync(user);

            Assert.AreEqual(true, confirmed);
        }


        [Test]
        public async Task GetEmailConfirmedAsync_DisabledUser_False()
        {
            var user = new User { Enabled = false };

            var confirmed = await mUserStore.GetEmailConfirmedAsync(user);

            Assert.AreEqual(false, confirmed);
        }


        [Test]
        public async Task FindByEmailAsync_UserEmail_User()
        {
            var user = await mUserStore.FindByEmailAsync(mMembershipFakeFactory.UserEnabledWithEmail.Email);

            Assert.AreEqual(MembershipFakeFactory.USERNAME_ENABLED_WITH_EMAIL, user.UserName);
        }


        [Test]
        public async Task FindByEmailAsync_NonexistentUser_Null()
        {
            Assert.IsNull(await mUserStore.FindByEmailAsync("nonexistent@email.com"));
        }


        [Test]
        public async Task FindByEmailAsync_EmptyEmail_Null()
        {
            Assert.IsNull(await mUserStore.FindByEmailAsync(String.Empty));
        }


        [Test]
        public async Task FindByEmailAsync_NotUniqueEmail_Null()
        {
            Assert.IsNull(await mUserStore.FindByEmailAsync(mMembershipFakeFactory.UserDuplicateEmail1.Email));
        }


        [Test]
        public void UserDeleteAsync_UserIsNull_ArgumentNullExceptionThrown()
        {
            Assert.That(async () => await mUserStore.DeleteAsync(null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public void RemoveFromRoleAsync_UserIsNull_ArgumentNullExceptionThrown()
        {
            Assert.That(async () => await mUserStore.RemoveFromRoleAsync(null, "whatever"), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [TestCase("")]
        [TestCase(null)]
        public void RemoveFromRoleAsync_RoleNameIsNullOrEmpty_ArgumentExceptionThrown(string roleName)
        {
            Assert.That(async () => await mUserStore.RemoveFromRoleAsync(new User(), roleName), Throws.Exception.TypeOf<ArgumentException>().And.Message.Contains("roleName"));
        }


        [Test]
        public void AddToRoleAsync_UserIsNull_ArgumentNullExceptionThrown()
        {
            Assert.That(async () => await mUserStore.AddToRoleAsync(null, "whatever"), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [TestCase("")]
        [TestCase(null)]
        public void AddToRoleAsync_RoleNameIsNullOrEmpty_ArgumentExceptionThrown(string roleName)
        {
            Assert.That(async () => await mUserStore.AddToRoleAsync(new User(), roleName), Throws.Exception.TypeOf<ArgumentException>().And.Message.Contains("roleName"));
        }


        [Test]
        public async Task GetLoginsAsync_ExternalUser_LoginFound()
        {
            var logins = await mUserStore.GetLoginsAsync(new User(mMembershipFakeFactory.UserExternal));

            Assert.AreEqual(1, logins.Count);
        }


        [Test]
        public async Task GetLoginsAsync_NonExistingUser_NoLoginFound()
        {
            var logins = await mUserStore.GetLoginsAsync(mMembershipFakeFactory.NonExistentUser);
            Assert.IsFalse(logins.Any());
        }


        [Test]
        public async Task GetLoginsAsync_InternalUser_NoLoginFound()
        {
            var logins = await mUserStore.GetLoginsAsync(new User(mMembershipFakeFactory.UserDisabledWithEmail));
            Assert.IsFalse(logins.Any());
        }


        [Test]
        public void GetLoginsAsync_Null_ArgumentExceptionThrown()
        {
            Assert.That(async () => await mUserStore.GetLoginsAsync(null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public async Task FindAsync_ExternalLogin_ExternalUser()
        {
            var login = new UserLoginInfo(mMembershipFakeFactory.ExternalLogin.LoginProvider, mMembershipFakeFactory.ExternalLogin.IdentityKey);
            var user = await mUserStore.FindAsync(login);

            Assert.AreEqual(MembershipFakeFactory.USERNAME_EXTERNAL, user.UserName);
        }


        [Test]
        public async Task FindAsync_NonExistingLogin_Null()
        {
            Assert.IsNull(await mUserStore.FindAsync(new UserLoginInfo("fakeProvider", "fakeKey")));
        }


        [Test]
        public void FindAsync_Null_ArgumentExceptionThrown()
        {
            Assert.That(async () => await mUserStore.FindAsync(null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("login"));
        }


        [Test]
        public void SetSecurityStampAsync_Null_ArgumentNullException()
        {
            Assert.That(async () => await mUserStore.SetSecurityStampAsync(null, String.Empty), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public void SetSecuritStampAsync_NonExistentUser_NotFoundException()
        {
            Assert.That(async () => await mUserStore.SetEmailAsync(mMembershipFakeFactory.NonExistentUser, String.Empty), Throws.Exception.With.Message.EqualTo("general.usernotfound"));
        }


        [TestCase(MembershipFakeFactory.USERNAME_WITH_SECURITY_STAMP, "cd9f2586-d346-4655-ae64-633d156822ba")]
        public async Task SetSecurityStampAsync_NewStamp_StampIsChanged(string userName, string newStamp)
        {
            var userInfo = UserInfoProvider.GetUserInfo(userName);
            var user = new User(userInfo);

            await mUserStore.SetSecurityStampAsync(user, newStamp);

            Assert.AreEqual(newStamp, user.SecurityStamp);
        }


        [Test]
        public async Task GetSecurityStampAsync_UserWithoutSecurityStamp_StringEmpty()
        {
            var userInfo = UserInfoProvider.GetUserInfo(MembershipFakeFactory.USERNAME_WITHOUT_SECURITY_STAMP);
            var user = new User(userInfo);

            await mUserStore.GetSecurityStampAsync(user);

            Assert.AreEqual(String.Empty, user.SecurityStamp);
        }


        [Test]
        public async Task GetSecurityStampAsync_User_UserStamp()
        {
            var userInfo = UserInfoProvider.GetUserInfo(MembershipFakeFactory.USERNAME_WITH_SECURITY_STAMP);
            var user = new User(userInfo);

            var stamp = await mUserStore.GetSecurityStampAsync(user);

            Assert.AreEqual(MembershipFakeFactory.SECURITY_STAMP, stamp);
        }
    }


    [TestFixture, Category.IsolatedIntegration]
    public class UserStoreIntegrationTests : IsolatedIntegrationTests
    {
        private readonly MembershipFakeFactory mMembershipFakeFactory = new MembershipFakeFactory();
        private UserStore mUserStore;
        private string[] allRoleNames = { MembershipFakeFactory.ROLE_ADMIN, MembershipFakeFactory.ROLE_MEMBER };

        [SetUp]
        public void SetUp()
        {
            mMembershipFakeFactory.GetIntegrationUsers().ToList().ForEach(x => UserInfoProvider.SetUserInfo(x));
            mMembershipFakeFactory.GetIntegrationRoles().ToList().ForEach(x => RoleInfoProvider.SetRoleInfo(x));
            mMembershipFakeFactory.GetIntegrationExternalLogins().ToList().ForEach(x => ExternalLoginInfoProvider.SetExternalLoginInfo(x));
            SettingsKeyInfoProvider.SetValue("cmssitesharedaccounts", 0, "True");

            mUserStore = new UserStore("NonExistingSiteForTestingPurposes");
        }


        [Test]
        public async Task GetRolesAsync_UserInAllRoles_AllRoleNames()
        {
            var userInfo = mMembershipFakeFactory.UserEnabledWithEmail;
            UserRoleInfoProvider.AddUserToRole(userInfo, mMembershipFakeFactory.AdminRole);
            UserRoleInfoProvider.AddUserToRole(userInfo, mMembershipFakeFactory.MemberRole);

            var user = new User(userInfo);
            var roles = await mUserStore.GetRolesAsync(user);

            CMSAssert.All(
                () => Assert.AreEqual(2, roles.Count),
                async () => Assert.IsTrue(await mUserStore.IsInRoleAsync(user, MembershipFakeFactory.ROLE_ADMIN), "Admin role missing"),
                async () => Assert.IsTrue(await mUserStore.IsInRoleAsync(user, MembershipFakeFactory.ROLE_MEMBER), "Member role missing"));
        }


        [Test]
        public async Task GetRolesAsync_DisabledUserWithoutRoles_EmptyRolesCollection()
        {
            var user = new User(mMembershipFakeFactory.UserDisabledWithEmail);
            var roles = await mUserStore.GetRolesAsync(user);

            Assert.AreEqual(0, roles.Count);
        }


        [Test]
        public void IsInRoleAsync_UserInAllRoles_ExpectedResult()
        {
            var userInfo = mMembershipFakeFactory.UserEnabledWithEmail;
            UserRoleInfoProvider.AddUserToRole(userInfo, mMembershipFakeFactory.AdminRole);
            UserRoleInfoProvider.AddUserToRole(userInfo, mMembershipFakeFactory.MemberRole);

            var user = new User(userInfo);

            CMSAssert.All(
                async () => Assert.IsTrue(await mUserStore.IsInRoleAsync(user, MembershipFakeFactory.ROLE_ADMIN), "Admin role missing"),
                async () => Assert.IsTrue(await mUserStore.IsInRoleAsync(user, MembershipFakeFactory.ROLE_MEMBER), "Member role missing"));
        }


        [TestCase(MembershipFakeFactory.ROLE_ADMIN)]
        [TestCase(MembershipFakeFactory.ROLE_MEMBER)]
        [TestCase("nonexistingRole")]
        public async Task IsInRoleAsync_DisabledUserWithoutRoles_False(string roleName)
        {
            var user = new User(mMembershipFakeFactory.UserDisabledWithEmail);

            Assert.IsFalse(await mUserStore.IsInRoleAsync(user, roleName));
        }


        [Test]
        public async Task IsInRoleAsync_NullRole_False()
        {
            var user = new User(mMembershipFakeFactory.UserDisabledWithEmail);

            Assert.IsFalse(await mUserStore.IsInRoleAsync(user, null));
        }


        [Test]
        public void IsInRoleAsync_UserRole_ArgumentException()
        {
            Assert.That(async () => await mUserStore.IsInRoleAsync(null, MembershipFakeFactory.ROLE_ADMIN), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public async Task UpdateAsync_CompleteUserUpdate_UserPropertiesUpdated()
        {
            var userUpdate = mMembershipFakeFactory.GetNewUser();
            userUpdate.Id = mMembershipFakeFactory.UserEnabledWithEmail.UserID;

            await mUserStore.UpdateAsync(userUpdate);

            var updatedUser = await mUserStore.FindByIdAsync(userUpdate.Id);

            CMSAssert.All(
                () => Assert.AreEqual(userUpdate.Email, updatedUser.Email),
                () => Assert.AreEqual(userUpdate.Enabled, updatedUser.Enabled),
                () => Assert.AreEqual(userUpdate.LastName, updatedUser.LastName),
                () => Assert.AreEqual(userUpdate.UserName, updatedUser.UserName),
                () => Assert.AreEqual(userUpdate.FirstName, updatedUser.FirstName));
        }


        [Test]
        public async Task UpdateAsync_EmptyUser_AllPropertiesAreSet()
        {
            string userName = "newName";
            await mUserStore.UpdateAsync(new User {
                Id = mMembershipFakeFactory.UserEnabledWithEmail.UserID,
                UserName = userName
            });

            var updatedUser = await mUserStore.FindByIdAsync(mMembershipFakeFactory.UserEnabledWithEmail.UserID);

            CMSAssert.All(
                () => Assert.That(updatedUser.Email, Is.Empty),
                () => Assert.IsFalse(updatedUser.Enabled),
                () => Assert.That(updatedUser.LastName, Is.Empty),
                () => Assert.AreEqual(userName, updatedUser.UserName),
                () => Assert.That(updatedUser.FirstName, Is.Empty));
        }


        [Test]
        public async Task CreateAsync_CompleteUser_UserPropertiesAreSet()
        {
            var userModel = mMembershipFakeFactory.GetNewUser();
            await mUserStore.CreateAsync(userModel);

            var userFromDB = await mUserStore.FindByIdAsync(userModel.Id);

            CMSAssert.All(
                () => Assert.AreEqual(userModel.Email, userFromDB.Email),
                () => Assert.AreEqual(userModel.Enabled, userFromDB.Enabled),
                () => Assert.AreEqual(userModel.LastName, userFromDB.LastName),
                () => Assert.AreEqual(userModel.UserName, userFromDB.UserName),
                () => Assert.AreEqual(userModel.FirstName, userFromDB.FirstName));
        }


        [Test]
        public void CreateAsync_EmptyUser_ThrowsException()
        {
            var userModel = new User();

            Assert.CatchAsync(typeof(CodeNameNotValidException), () => mUserStore.CreateAsync(userModel));
        }


        [Test]
        public async Task DeleteAsync_ExistingUser_UserInfoDeleted()
        {
            var user = new User(mMembershipFakeFactory.UserDisabledWithEmail);

            // Ensure that user exists
            var userInfo = UserInfoProvider.GetUserInfo(user.Id);

            await mUserStore.DeleteAsync(user);

            CMSAssert.All(
                () => Assert.NotNull(userInfo),
                () => Assert.IsNull(UserInfoProvider.GetUserInfo(userInfo.UserID)));
        }


        [Test]
        public async Task AddToRoleAsync_UserIsNotInRole_UserRoleInfoCreated()
        {
            var user = new User(mMembershipFakeFactory.UserEnabledWithEmail);
            await mUserStore.AddToRoleAsync(user, MembershipFakeFactory.ROLE_MEMBER);

            CMSAssert.All(
                () => Assert.AreEqual(1, UserRoleInfoProvider.GetUserRoles().Count),
                () => Assert.IsNotNull(UserRoleInfoProvider.GetUserRoleInfo(user.Id, mMembershipFakeFactory.MemberRole.RoleID)));
        }


        [Test]
        public void AddToRoleAsync_RoleNotExists_InvalidOperationExceptionThrown()
        {
            Assert.That(async () => await mUserStore.AddToRoleAsync(new User(), "nonExisting"), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("general.rolenotfound"));
        }


        [Test]
        public void AddToRoleAsync_UserIsInRole_DoesNotThrow()
        {
            var user = new User(mMembershipFakeFactory.UserEnabledWithEmail);
            var role = mMembershipFakeFactory.AdminRole;
            UserRoleInfoProvider.AddUserToRole(mMembershipFakeFactory.UserEnabledWithEmail, role);

            CMSAssert.All(
                () => Assert.IsNotNull(UserRoleInfoProvider.GetUserRoleInfo(user.Id, role.RoleID)),
                () => Assert.DoesNotThrow(() => mUserStore.AddToRoleAsync(user, role.RoleName).Wait()));
        }


        [Test]
        public async Task RemoveFromRoleAsync_UserIsInRole_UserRoleInfoDeletedUserAndRoleExists()
        {
            var role = mMembershipFakeFactory.AdminRole;
            var user = new User(mMembershipFakeFactory.UserEnabledWithEmail);
            UserRoleInfoProvider.AddUserToRole(mMembershipFakeFactory.UserEnabledWithEmail, mMembershipFakeFactory.AdminRole);

            await mUserStore.RemoveFromRoleAsync(user, role.RoleName);

            CMSAssert.All(
                () => Assert.AreEqual(0, UserRoleInfoProvider.GetUserRoles().Count),
                () => Assert.IsNull(UserRoleInfoProvider.GetUserRoleInfo(user.Id, role.RoleID)),
                () => Assert.IsNotNull(RoleInfoProvider.GetRoleInfo(role.RoleID)),
                () => Assert.IsNotNull(UserInfoProvider.GetUserInfo(user.Id)));
        }


        [Test]
        public void RemoveFromRoleAsync_UserIsNotInRole_DoesNotThrow()
        {
            var user = new User(mMembershipFakeFactory.UserEnabledWithEmail);
            CMSAssert.All(
                () => Assert.IsNull(UserRoleInfoProvider.GetUserRoleInfo(user.Id, mMembershipFakeFactory.AdminRole.RoleID)),
                () => Assert.DoesNotThrow(() => mUserStore.RemoveFromRoleAsync(user, MembershipFakeFactory.ROLE_ADMIN).Wait()));
        }


        [Test]
        public void RemoveFromRoleAsync_RoleNotExists_InvalidOperationExceptionThrown()
        {
            Assert.That(async () => await mUserStore.RemoveFromRoleAsync(new User(), "nonExisting"), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("general.rolenotfound"));
        }



        [Test]
        public void SetEmailConfirmedAsync_Null_NullException()
        {
            Assert.That(async () => await mUserStore.SetEmailConfirmedAsync(null, true), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public void SetEmailConfirmedAsync_NonExistentUser_NoException()
        {
            Assert.DoesNotThrow(() => mUserStore.SetEmailConfirmedAsync(mMembershipFakeFactory.NonExistentUser, true).Wait());
        }


        [TestCase(MembershipFakeFactory.USERNAME_ENABLED_NO_EMAIL, false)]
        [TestCase(MembershipFakeFactory.USERNAME_DISABLED_NO_EMAIL, true)]
        [TestCase(MembershipFakeFactory.USERNAME_ENABLED_WITH_EMAIL, false)]
        [TestCase(MembershipFakeFactory.USERNAME_DISABLED_WITH_EMAIL, true)]
        public async Task SetEmailConfirmedAsync_EmailIsConfirmed_UserEnabled(string userName, bool confirmed)
        {
            var userInfo = UserInfoProvider.GetUserInfo(userName);
            var user = new User(userInfo);

            await mUserStore.SetEmailConfirmedAsync(user, confirmed);

            Assert.AreEqual(confirmed, user.Enabled);
        }


        [Test]
        public void SetEmailAsync_Null_ArgumentNullException()
        {
            Assert.That(async () => await mUserStore.SetEmailAsync(null, String.Empty), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public void SetEmailAsync_NonExistentUser_NotFoundException()
        {
            Assert.That(async () => await mUserStore.SetEmailAsync(mMembershipFakeFactory.NonExistentUser, String.Empty), Throws.Exception.With.Message.EqualTo("general.usernotfound"));
        }


        [TestCase(MembershipFakeFactory.USERNAME_ENABLED_NO_EMAIL, "changed@email.ccc")]
        [TestCase(MembershipFakeFactory.USERNAME_ENABLED_WITH_EMAIL, "aaa")]
        [TestCase(MembershipFakeFactory.USERNAME_ENABLED_NO_EMAIL, "")]
        public async Task SetEmailAsync_NewEmail_EmailIsChanged(string userName, string newEmail)
        {
            var userInfo = UserInfoProvider.GetUserInfo(userName);
            var user = new User(userInfo);

            await mUserStore.SetEmailAsync(user, newEmail);

            Assert.AreEqual(newEmail, userInfo.Email);
        }


        [Test]
        public async Task SetEmailAsync_Null_EmptyEmail()
        {
            var userInfo = UserInfoProvider.GetUserInfo(MembershipFakeFactory.USERNAME_ENABLED_WITH_EMAIL);
            var user = new User(userInfo);

            await mUserStore.SetEmailAsync(user, null);

            Assert.AreEqual(String.Empty, userInfo.Email);
        }


        [Test]
        public async Task RemoveLoginAsync_ExternalUserWithExternalLogin_LoginRemoved()
        {
            var login = new UserLoginInfo(mMembershipFakeFactory.ExternalLogin.LoginProvider, mMembershipFakeFactory.ExternalLogin.IdentityKey);
            var user = new User(mMembershipFakeFactory.UserExternal);
            await mUserStore.RemoveLoginAsync(user, login);
            var logins = await mUserStore.GetLoginsAsync(user);

            Assert.IsFalse(logins.Any());
        }


        [Test]
        public void RemoveLoginAsync_UserNull_ArgumentNullException()
        {
            var login = new UserLoginInfo(mMembershipFakeFactory.ExternalLogin.LoginProvider, mMembershipFakeFactory.ExternalLogin.IdentityKey);
            Assert.That(async () => await mUserStore.RemoveLoginAsync(null, login), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public void RemoveLoginAsync_LoginNull_ArgumentNullException()
        {
            var user = new User(mMembershipFakeFactory.UserExternal);
            Assert.That(async () => await mUserStore.RemoveLoginAsync(user, null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("login"));
        }


        [Test]
        public void RemoveLoginAsync_NonExistingUser_NoException()
        {
            var login = new UserLoginInfo(mMembershipFakeFactory.ExternalLogin.LoginProvider, mMembershipFakeFactory.ExternalLogin.IdentityKey);
            Assert.DoesNotThrow(() => mUserStore.RemoveLoginAsync(mMembershipFakeFactory.NonExistentUser, login).Wait());
        }


        [Test]
        public void RemoveLoginAsync_NonExistingLogin_NoExceptionNoLoginRemoved()
        {
            var user = new User(mMembershipFakeFactory.UserExternal);
            var login = new UserLoginInfo("fakeProvider", "fakeKey");
            CMSAssert.All(
                () => Assert.DoesNotThrow(() => mUserStore.RemoveLoginAsync(user, login).Wait()),
                async () => Assert.AreEqual(1, (await mUserStore.GetLoginsAsync(user)).Count));
        }


        [Test]
        public async Task AddLoginAsync_CorrectUserAndLogin_LoginAdded()
        {
            var login = new UserLoginInfo("newLoginProvider", "newIdentityKey");
            var user = new User(mMembershipFakeFactory.UserExternal);
            await mUserStore.AddLoginAsync(user, login);
            var logins = await mUserStore.GetLoginsAsync(user);

            Assert.AreEqual(2, logins.Count);
        }


        [Test]
        public void AddLoginAsync_UserNull_ArgumentNullException()
        {
            var login = new UserLoginInfo(mMembershipFakeFactory.ExternalLogin.LoginProvider, mMembershipFakeFactory.ExternalLogin.IdentityKey);
            Assert.That(async () => await mUserStore.AddLoginAsync(null, login), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("user"));
        }


        [Test]
        public void AddLoginAsync_LoginNull_ArgumentNullException()
        {
            var user = new User(mMembershipFakeFactory.UserExternal);
            Assert.That(async () => await mUserStore.AddLoginAsync(user, null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("login"));
        }


        [Test]
        public void AddLoginAsync_NewUser_BrokenForeignKeyConstraintException()
        {
            var login = new UserLoginInfo(mMembershipFakeFactory.ExternalLogin.LoginProvider, mMembershipFakeFactory.ExternalLogin.IdentityKey);
            Assert.That(async () => await mUserStore.AddLoginAsync(new User(), login), Throws.Exception.Message.Contains("FOREIGN KEY constraint"));
        }
    }
}
