using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CMS.Tests;
using CMS.Membership;
using CMS.DataEngine;

using NUnit.Framework;

namespace Kentico.Membership.Tests
{
    [TestFixture, Category.IsolatedIntegration]
    public class UserTests : IsolatedIntegrationTests
    {
        private readonly MembershipFakeFactory mMembershipFakeFactory = new MembershipFakeFactory();

        [SetUp]
        public void SetUp()
        {
            mMembershipFakeFactory.GetIntegrationUsers().ToList().ForEach(x => UserInfoProvider.SetUserInfo(x));
            mMembershipFakeFactory.GetIntegrationRoles().ToList().ForEach(x => RoleInfoProvider.SetRoleInfo(x));
            SettingsKeyInfoProvider.SetValue("cmssitesharedaccounts", 0, "True");
        }


        [Test]
        public void Roles_UserInAllRoles_AllRoleNames()
        {
            var userInfo = mMembershipFakeFactory.UserEnabledWithEmail;
            UserRoleInfoProvider.AddUserToRole(userInfo, mMembershipFakeFactory.AdminRole);
            UserRoleInfoProvider.AddUserToRole(userInfo, mMembershipFakeFactory.MemberRole);

            var user = new User(userInfo);

            CMSAssert.All(
                () => Assert.AreEqual(2, user.Roles.Count()),
                () => Assert.IsTrue(user.Roles.Contains(MembershipFakeFactory.ROLE_ADMIN), "Admin role missing"),
                () => Assert.IsTrue(user.Roles.Contains(MembershipFakeFactory.ROLE_MEMBER), "Member role missing"));
        }


        [Test]
        public void Roles_DisabledUserInAllRoles_AllRoleNames()
        {
            var userInfo = mMembershipFakeFactory.UserDisabledWithEmail;
            UserRoleInfoProvider.AddUserToRole(userInfo, mMembershipFakeFactory.AdminRole);
            UserRoleInfoProvider.AddUserToRole(userInfo, mMembershipFakeFactory.MemberRole);

            var user = new User(userInfo);

            CMSAssert.All(
                () => Assert.AreEqual(2, user.Roles.Count()),
                () => Assert.IsTrue(user.Roles.Contains(MembershipFakeFactory.ROLE_ADMIN), "Admin role missing"),
                () => Assert.IsTrue(user.Roles.Contains(MembershipFakeFactory.ROLE_MEMBER), "Member role missing"));
        }


        [Test]
        public void Roles_DisabledUserWithoutRoles_EmptyRolesCollection()
        {
            var user = new User(mMembershipFakeFactory.UserDisabledWithEmail);

            Assert.AreEqual(0, user.Roles.Count());
        }
    }
}
