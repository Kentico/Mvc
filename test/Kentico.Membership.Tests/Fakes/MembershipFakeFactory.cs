using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;
using CMS.Membership;
using CMS.Tests;

namespace Kentico.Membership.Tests
{
    /// <summary>
    /// By using this class in fakes, we gain access to internal properties of FakeClassStructureInfo.
    /// </summary>
    /// <typeparam name="T">Info type.</typeparam>
    internal class InternalsVisibleFakeClassStructure<T> : FakeClassStructureInfo<T>
    {
        public new void RegisterColumn(string colName, Type colType)
        {
            base.RegisterColumn(colName, colType);
        }
    }


    internal class MembershipFakeFactory
    {
        public const string TEST_PASSWORD = "test",
                            SECURITY_STAMP = "C56A4180-65AA-42EC-A945-5FD21DEC0538",
                            USERNAME_ENABLED_WITH_EMAIL = "EnabledUserWithEmail",
                            USERNAME_DISABLED_WITH_EMAIL = "DisabledUserWithEmail",
                            USERNAME_ENABLED_NO_EMAIL = "EnabledUserWithoutEmail",
                            USERNAME_DISABLED_NO_EMAIL = "DisabledUserWithoutEmail",
                            USERNAME_DUPLICATE_EMAIL1 = "DuplicateEmailUser",
                            USERNAME_DUPLICATE_EMAIL2 = "DuplicateEmailUser2",
                            USERNAME_WITH_PASSWORD = "TestUserWithPassword",
                            USERNAME_NO_PASSWORD = "TestUserWithoutPassword",
                            USERNAME_WITH_SECURITY_STAMP = "TestUserWithSecurityStamp",
                            USERNAME_WITHOUT_SECURITY_STAMP = "TestUserWithoutSecurityStamp",
                            USERNAME_NONEXISTENT = "NonExistentUser",
                            USERNAME_EXTERNAL = "ExternalUser",
                            USERNAME_EXTERNAL_WITH_SECURITY_STAMP = "ExternalUserWithSecurityStamp",
                            ROLE_ADMIN = "TestRoleAdmin",
                            ROLE_MEMBER = "TestRoleMember",
                            EXTERNAL_IDENTITY_KEY = "externalLogin",
                            EXTERNAL_IDENTITY_KEY_WITH_SECURITY_STAMP = "externalLoginWithSecurityStamp",     
                            EXTERNAL_PROVIDER = "externalProvider";

        public UserInfo UserEnabledWithEmail,
                        UserDisabledWithEmail,
                        UserEnabledWithoutEmail,
                        UserDisabledWithoutEmail,
                        UserDuplicateEmail1,
                        UserDuplicateEmail2,
                        UserExternal,
                        UserExternalWithSecurityStamp,
                        UserWithoutPassword,
                        UserWithPassword,
                        UserWithSecurityStamp,
                        UserWithoutSecurityStamp;

        public RoleInfo AdminRole,
                        MemberRole;

        public ExternalLoginInfo ExternalLogin,
                                 ExternalLoginWithSecurityStamp;

        public User NonExistentUser = new User()
        {
            UserName = USERNAME_NONEXISTENT
        };


        public UserInfo[] GetUsers()
        {
            // This UserPassword faking has to stay in this method due to limitations in CMS faking
            var classStructureInfo = new InternalsVisibleFakeClassStructure<UserInfo>();
            classStructureInfo.RegisterColumn("UserPassword", typeof(string));
            UserInfo.TYPEINFO.ClassStructureInfo = classStructureInfo;

            return InitUsers();
        }


        public IEnumerable<UserInfo> GetIntegrationUsers()
        {
            return InitUsers().ToList().Select(x => { x.UserID = 0; return x; });
        }


        private UserInfo[] InitUsers()
        {
            UserWithPassword = new UserInfo
            {
                UserID = 10,
                UserName = USERNAME_WITH_PASSWORD,
                Enabled = true,
            };
            UserWithPassword.SetValue("UserPassword", TEST_PASSWORD);

            return new[] {
                UserEnabledWithEmail = new UserInfo { UserID = 1, UserName = USERNAME_ENABLED_WITH_EMAIL, Enabled = true, Email = "EnabledUserWithEmail@email.com" },
                UserDisabledWithEmail = new UserInfo { UserID = 2, UserName = USERNAME_DISABLED_WITH_EMAIL, Enabled = false, Email = "DisabledUserWithEmail@email.com" },
                UserEnabledWithoutEmail = new UserInfo { UserID = 3, UserName = USERNAME_ENABLED_NO_EMAIL, Enabled = true },
                UserDisabledWithoutEmail = new UserInfo { UserID = 4, UserName = USERNAME_DISABLED_NO_EMAIL, Enabled = false },
                UserDuplicateEmail1 = new UserInfo { UserID = 5, UserName = USERNAME_DUPLICATE_EMAIL1, Enabled = true, Email = "DuplicateEmailUser@email.com" },
                UserDuplicateEmail2 = new UserInfo { UserID = 6, UserName = USERNAME_DUPLICATE_EMAIL2, Enabled = false, Email = "DuplicateEmailUser@email.com" },
                UserWithoutPassword = new UserInfo { UserID = 7, UserName = USERNAME_NO_PASSWORD, Enabled = true },
                UserExternal = new UserInfo { UserID = 8, UserName = USERNAME_EXTERNAL, Enabled = true, IsExternal = true },
                UserExternalWithSecurityStamp = new UserInfo { UserID = 9, UserName = USERNAME_EXTERNAL_WITH_SECURITY_STAMP, Enabled = true, IsExternal = true, UserSecurityStamp = SECURITY_STAMP },
                UserWithPassword,
                UserWithSecurityStamp = new UserInfo { UserID = 11, UserName = USERNAME_WITH_SECURITY_STAMP, Enabled = true, UserSecurityStamp = SECURITY_STAMP },
                UserWithoutSecurityStamp = new UserInfo { UserID = 12, UserName = USERNAME_WITHOUT_SECURITY_STAMP, Enabled = true }           
            };
        }


        public User GetNewUser()
        {
            return new User
            {
                Email = "test@t.com",
                Enabled = false,
                LastName = "test LastName",
                UserName = "testUserName",
                FirstName = "test FirstName"
            };
        }


        public RoleInfo[] GetRoles()
        {
            return new[]
            {
                AdminRole = new RoleInfo { RoleName = ROLE_ADMIN, RoleID = 1234, RoleDisplayName = ROLE_ADMIN, SiteID = 0 },
                MemberRole = new RoleInfo { RoleName = ROLE_MEMBER, RoleID = 1235, RoleDisplayName = ROLE_MEMBER, SiteID = 0 }
            };
        }


        public IEnumerable<RoleInfo> GetIntegrationRoles()
        {
            return GetRoles().ToList().Select(x => { x.RoleID = 0; return x; });
        }


        public SettingsKeyInfo GetSharedUsersSetting()
        {
            return new SettingsKeyInfo { KeyName = "cmssitesharedaccounts", KeyValue = "true" };
        }


        public ExternalLoginInfo[] GetExternalLogins()
        {
            if (UserExternal == null || UserExternalWithSecurityStamp == null)
            {
                throw new Exception("[MembershipFakeFactory.GetExternalLogins]: Users must be initialized first.");
            }

            return new[]
            {
                ExternalLogin = new ExternalLoginInfo { ExternalLoginID = 1, UserID = UserExternal.UserID, IdentityKey = EXTERNAL_IDENTITY_KEY, LoginProvider = EXTERNAL_PROVIDER },
                ExternalLoginWithSecurityStamp = new ExternalLoginInfo { ExternalLoginID = 2, UserID = UserExternalWithSecurityStamp.UserID, IdentityKey = EXTERNAL_IDENTITY_KEY_WITH_SECURITY_STAMP, LoginProvider = EXTERNAL_PROVIDER }
            };
        }


        public IEnumerable<ExternalLoginInfo> GetIntegrationExternalLogins()
        {
            return GetExternalLogins().ToList().Select(x => { x.ExternalLoginID = 0; return x; });
        }
    }
}
