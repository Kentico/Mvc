using System;
using System.Linq;

using CMS.Tests;
using CMS.Membership;
using CMS.DataEngine;
using CMS.Helpers;

using NUnit.Framework;
using Microsoft.Owin.Builder;
using Microsoft.AspNet.Identity;

namespace Kentico.Membership.Tests
{
    class FakeUserManager : UserManager
    {
        public FakeUserManager(IUserStore<User, int> store)
        : base(store)
        {
        }


        public IdentityResult CallProtectedUpdatePassword(User user, string newPassword)
        {
            return UpdatePassword(Store as IUserPasswordStore<User, int>, user, newPassword).Result;
        }


        public bool CallProtectedVerifyPassword(User user, string newPassword)
        {
            return VerifyPasswordAsync(Store as IUserPasswordStore<User, int>, user, newPassword).Result;
        }
    }


    [TestFixture]
    public class UserManagerTests : UnitTests
    {
        private readonly MembershipFakeFactory mMembershipFakeFactory = new MembershipFakeFactory();
        private FakeUserManager manager;
        private IInfoProviderFake<SettingsKeyInfo, SettingsKeyInfoProvider> settingsFake;

        [SetUp]
        public void SetUp()
        {
            Fake<UserSettingsInfo, UserSettingsInfoProvider>().WithData();
            Fake<UserInfo, UserInfoProvider>().WithData(mMembershipFakeFactory.GetUsers());
            settingsFake = Fake<SettingsKeyInfo, SettingsKeyInfoProvider>().WithData(mMembershipFakeFactory.GetSharedUsersSetting());

            manager = UserManager.Initialize(new AppBuilder(), new FakeUserManager(new UserStore("NonExistingSiteForTestingPurposes")));
        }


        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("incorrectPassword", false)]
        [TestCase(MembershipFakeFactory.TEST_PASSWORD, true)]
        public void VerifyPassword_UserWithPassword_ExpectedResults(string password, bool expectedResult)
        {
            var user = new User(mMembershipFakeFactory.UserWithPassword);

            Assert.AreEqual(expectedResult, manager.CallProtectedVerifyPassword(user, password));
        }


        [TestCase("", true)]
        [TestCase(null, true)]
        [TestCase("somePassword", false)]
        public void VerifyPassword_UserWithNoPassword_ExpectedResults(string password, bool expectedResult)
        {
            var user = new User(mMembershipFakeFactory.UserWithoutPassword);

            Assert.AreEqual(expectedResult, manager.CallProtectedVerifyPassword(user, password));
        }


        [Test]
        public void VerifyPassword_UserNull_False()
        {
            Assert.IsFalse(manager.CallProtectedVerifyPassword(null, null));
        }


        [Test]
        public void VerifyPassword_UserIsExternal_False()
        {
            var user = new User(mMembershipFakeFactory.UserExternal);

            Assert.IsFalse(manager.CallProtectedVerifyPassword(user, ""));
        }


        [Test]
        public void VerifyPassword_UserIsDomain_False()
        {
            var user = new User(mMembershipFakeFactory.UserDomain);

            Assert.IsFalse(manager.CallProtectedVerifyPassword(user, ""));
        }


        [Test]
        public void VerifyPassword_PasswordFormatChanged_UserCanLogInWithOldPasswordHash()
        {
            settingsFake.IncludeData(new SettingsKeyInfo { KeyName = "CMSPasswordFormat", KeyValue = "sha2" });

            var user = new User(mMembershipFakeFactory.UserWithPassword);
            var passwordHash = UserInfoProvider.GetUserInfo(user.Id).GetValue("UserPassword");

            CMSAssert.All(
                () => Assert.IsTrue(manager.CallProtectedVerifyPassword(user, MembershipFakeFactory.TEST_PASSWORD)),
                () => Assert.AreNotEqual(UserInfoProvider.GetPasswordHash(MembershipFakeFactory.TEST_PASSWORD, "sha2", user.GUID.ToString()), passwordHash));
        }
    }


    [TestFixture, Category.IsolatedIntegration]
    public class UserManagerIntegrationTests : IsolatedIntegrationTests
    {
        private readonly MembershipFakeFactory mMembershipFakeFactory = new MembershipFakeFactory();
        private FakeUserManager manager;

        [SetUp]
        public void SetUp()
        {
            mMembershipFakeFactory.GetIntegrationUsers().ToList().ForEach(x => UserInfoProvider.SetUserInfo(x));
            SettingsKeyInfoProvider.SetValue("cmssitesharedaccounts", 0, "True");

            manager = UserManager.Initialize(new AppBuilder(), new FakeUserManager(new UserStore("NonExistingSiteForTestingPurposes")));
        }


        [TestCase("sha1", "newPassword")]
        [TestCase("sha1", MembershipFakeFactory.TEST_PASSWORD)]
        [TestCase("sha2", "newPassword")]
        [TestCase("sha2", MembershipFakeFactory.TEST_PASSWORD)]
        [TestCase("sha2salt", "newPassword")]
        [TestCase("sha2salt", MembershipFakeFactory.TEST_PASSWORD)]
        [TestCase("plainText", "newPassword")]
        [TestCase("plainText", MembershipFakeFactory.TEST_PASSWORD)]
        public void UpdatePassword_VariousPasswordHashMethods_PasswordCanBeVerified(string hashMethod, string password)
        {
            SettingsKeyInfoProvider.SetValue("CMSPasswordFormat", 0, hashMethod);

            var user = new User(mMembershipFakeFactory.UserWithPassword);
            var result = manager.CallProtectedUpdatePassword(user, password);
            var passwordHash = UserInfoProvider.GetUserInfo(user.Id).GetValue("UserPassword");

            CMSAssert.All(() => Assert.AreEqual(UserInfoProvider.GetPasswordHash(password, hashMethod, user.GUID.ToString()), passwordHash),
                          () => Assert.IsTrue(manager.CallProtectedVerifyPassword(user, password)));
        }


        [Test]
        public void UpdatePassword_PBKDF2PasswordHashMethod_PasswordCanBeVerified()
        {
            SettingsKeyInfoProvider.SetValue("CMSPasswordFormat", 0, "pbkdf2");

            var user = new User(mMembershipFakeFactory.UserWithPassword);
            var result = manager.CallProtectedUpdatePassword(user, MembershipFakeFactory.TEST_PASSWORD);
            var passwordHash = ValidationHelper.GetString(UserInfoProvider.GetUserInfo(user.Id).GetValue("UserPassword"), string.Empty);

            CMSAssert.All(() => Assert.IsTrue(SecurityHelper.VerifyPBKDF2Hash(MembershipFakeFactory.TEST_PASSWORD, passwordHash)),
                          () => Assert.IsTrue(manager.CallProtectedVerifyPassword(user, MembershipFakeFactory.TEST_PASSWORD)));
        }


        [TestCase("", false)]
        [TestCase("newPassword", true)]
        [TestCase(MembershipFakeFactory.TEST_PASSWORD, true)]
        public void UpdatePassword_UserWithPassword_PasswordCanBeVerified(string password, bool expectedResult)
        {
            var user = new User(mMembershipFakeFactory.UserWithPassword);
            var result = manager.CallProtectedUpdatePassword(user, password);

            CMSAssert.All(() => Assert.AreEqual(expectedResult, result.Succeeded),
                          () => Assert.AreEqual(expectedResult, manager.CallProtectedVerifyPassword(user, password)));
        }


        [TestCase("", false)]
        [TestCase("somePassword", true)]
        public void UpdatePassword_UserWithNoPassword_PasswordCanBeVerified(string password, bool expectedResult)
        {
            var user = new User(mMembershipFakeFactory.UserWithoutPassword);
            var result = manager.CallProtectedUpdatePassword(user, password);

            CMSAssert.All(() => Assert.AreEqual(expectedResult, result.Succeeded),
                          () => Assert.IsTrue(manager.CallProtectedVerifyPassword(user, password)));
        }


        [Test]
        public void UpdatePassword_UserNull_ArgumentNullException()
        {
            Assert.That(() => manager.CallProtectedUpdatePassword(null, null), Throws.Exception.InnerException.TypeOf<ArgumentNullException>().And.InnerException.Message.Contains("user"));
        }


        [Test]
        public void UpdatePassword_PasswordNull_ArgumentNullException()
        {
            var user = new User(mMembershipFakeFactory.UserWithoutPassword);
            Assert.That(() => manager.CallProtectedUpdatePassword(user, null), Throws.Exception.InnerException.TypeOf<ArgumentNullException>().And.InnerException.Message.Contains("item"));
        }


        [Test]
        public void UpdatePassword_UserWithSecurityStamp_SecurityStampIsUpdated()
        {
            var user = new User(mMembershipFakeFactory.UserWithSecurityStamp);
            var result = manager.CallProtectedUpdatePassword(user, MembershipFakeFactory.TEST_PASSWORD);

            CMSAssert.All(() => Assert.IsTrue(ValidationHelper.IsGuid(user.SecurityStamp)),
                          () => Assert.AreNotEqual(MembershipFakeFactory.SECURITY_STAMP, user.SecurityStamp));
        }
    }
}
