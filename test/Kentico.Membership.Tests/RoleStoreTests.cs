using System;
using System.Linq;
using System.Threading.Tasks;
using CMS.Tests;
using CMS.Membership;
using CMS.DataEngine;
using CMS.SiteProvider;

using NUnit.Framework;

namespace Kentico.Membership.Tests
{
    [TestFixture, Category.IsolatedIntegration]
    public class RoleStoreIntegrationTests : IsolatedIntegrationTests
    {
        private RoleStore store;
        private Role role;

        [SetUp]
        public async Task Init()
        {
            // Creates a new site object
            SiteInfo newSite = new SiteInfo
            {
                DisplayName = "New site",
                SiteName = "NewSite",
                Status = SiteStatusEnum.Running,
                DomainName = "127.0.0.1",
            };

            if (SiteInfoProvider.GetSiteInfo(newSite.SiteName) == null)
            {
                SiteInfoProvider.SetSiteInfo(newSite);
            }

            SiteContext.CurrentSite = newSite;

            var roleInfo = new RoleInfo
            {
                RoleDisplayName = "Role for tests",
                RoleName = "RoleForTests",
                SiteID = SiteContext.CurrentSiteID,
            };

            store = new RoleStore();
            role = new Role(roleInfo);
            await store.CreateAsync(role);
        }


        [Test]
        public void CreateRole_CodeNameIsInvalid_CodeNameNotValidExceptionIsThrown()
        {
            var roleInfo = new RoleInfo
            {
                RoleDisplayName = "Display Name",
                RoleName = "Not valid code name",
                SiteID = SiteContext.CurrentSiteID,
            };

            var roleWithNotValidCodeName = new Role(roleInfo);

            Assert.That(async () => await store.CreateAsync(roleWithNotValidCodeName), Throws.Exception.TypeOf<CodeNameNotValidException>());
        }


        [Test]
        public async Task CreateRole_FromRoleInfo_RoleGetsCorrectID()
        {
            var roleInfo = new RoleInfo
            {
                RoleDisplayName = "Display Name",
                RoleName = "ValidCodeName",
                SiteID = SiteContext.CurrentSiteID,
            };

            var role = new Role(roleInfo);
            await store.CreateAsync(role);

            CMSAssert.All(
                () => Assert.AreEqual(roleInfo.RoleName, role.Name),
                () => Assert.AreEqual(roleInfo.RoleDisplayName, role.DisplayName),
                () => Assert.AreNotEqual(0, role.Id));
        }


        [Test]
        public async Task CreateRole_NewRoleWithValidCodeName_RoleInfoIsCreatedWithCorrectParams()
        {
            var role = new Role
            {
                DisplayName = "DisplayNameTest",
                Name = "ValidCodeName",
            };
            await store.CreateAsync(role);

            var createdRoleInfo = RoleInfoProvider.GetRoleInfo(role.Id);
            CMSAssert.All(
                () => Assert.IsNotNull(createdRoleInfo),
                () => Assert.AreEqual(role.DisplayName, createdRoleInfo.RoleDisplayName),
                () => Assert.AreEqual(role.Name, createdRoleInfo.RoleName));
        }


        [Test]
        public async Task RoleDeletion_OneRoleIsCreatedWithID_AfterDeletingRole_NoRoleInfoExists()
        {
            int idToBeDeleted = role.Id;

            await store.DeleteAsync(role);

            CMSAssert.All(
                () => Assert.AreNotEqual(0, idToBeDeleted),
                () => Assert.AreEqual(0, RoleInfoProvider.GetRoles().Count));
        }


        [Test]
        public async Task RoleUpdate_RoleCodeNameAndDisplayNameAreChanged_RoleInfoHasCodeNameAndDisplayNameChanged()
        {
            var newDisplayName = "newDisp";
            var newName = "newName";

            // Update role
            role.Name = newName;
            role.DisplayName = newDisplayName;
            await store.UpdateAsync(role);

            // Get update info
            var updateRoleInfo = RoleInfoProvider.GetRoleInfo(role.Id);

            CMSAssert.All(
                () => Assert.AreEqual(newDisplayName, updateRoleInfo.RoleDisplayName),
                () => Assert.AreEqual(newName, updateRoleInfo.RoleName));
        }


        [Test]
        public void UpdateRole_RoleDoesNotExists_ExceptionIsThrown()
        {
            var roleInfo = new RoleInfo
            {
                RoleDisplayName = "NonExisting",
                RoleName = "NonExisting",
                SiteID = SiteContext.CurrentSiteID,
            };

            // Create role
            var role = new Role(roleInfo);

            Assert.That(async () => await store.UpdateAsync(role), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("general.rolenotfound"));
        }
    }


    [TestFixture]
    public class RoleStoreTests : UnitTests
    {
        private readonly MembershipFakeFactory mMembershipFakeFactory = new MembershipFakeFactory();
        RoleStore store;
        RoleInfo adminRole, memberRole;

        [SetUp]
        public void SetUp()
        {
            var provider = Fake<RoleInfo, RoleInfoProvider>();
            var roles = mMembershipFakeFactory.GetRoles();

            adminRole = roles.First(x => x.RoleName == MembershipFakeFactory.ROLE_ADMIN);
            memberRole = roles.First(x => x.RoleName == MembershipFakeFactory.ROLE_MEMBER);

            provider.WithData(roles);

            store = new RoleStore();
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase("NonExistingRole")]
        public async Task FindByNameAsync_NonExistingNames_ReturnsNullWhenNotFound(string searchString)
        {
            Assert.IsNull(await store.FindByNameAsync(searchString));
        }


        [TestCase("TestRoleAdmin")]
        [TestCase("TestRoleMember")]
        public async Task FindByNameAsync_ExistingNames_ReturnsRoleWhenFound(string searchString)
        {
            var role = await store.FindByNameAsync(searchString);

            Assert.AreEqual(role.Name, searchString);
        }


        [Test]
        public async Task FindByIdAsync_NonExistingID_ReturnsNullWhenRoleNotFound()
        {
            Assert.IsNull(await store.FindByIdAsync(-1));
        }


        [Test]
        public async Task FindByIdAsync_MembershipRoleID_ReturnsRoleWhenFound()
        {
            var role = await store.FindByIdAsync(memberRole.RoleID);

            CMSAssert.All(
                () => Assert.AreEqual(role.Id, memberRole.RoleID),
                () => Assert.AreEqual(role.Name, memberRole.RoleName));
        }


        [Test]
        public void CreateRole_RoleIsNull_ArgumentNullExceptionIsThrown()
        {
            Assert.That(async () => await store.CreateAsync(null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("role"));
        }


        [Test]
        public void UpdateRole_RoleIsNull_ArgumentNullExceptionIsThrown()
        {
            Assert.That(async () => await store.UpdateAsync(null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("role"));
        }


        [Test]
        public void DeleteRole_RoleIsNull_ArgumentNullExceptionIsThrown()
        {
            Assert.That(async () => await store.DeleteAsync(null), Throws.Exception.TypeOf<ArgumentNullException>().And.Message.Contains("role"));
        }
    }
}
