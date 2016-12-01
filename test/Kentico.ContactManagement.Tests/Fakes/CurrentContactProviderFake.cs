using System;
using CMS.Base;
using CMS.ContactManagement;
using CMS.SiteProvider;

namespace Kentico.ContactManagement.Tests
{
    internal class CurrentContactProviderFake : ICurrentContactProvider
    {
        public ContactInfo CurrentContact
        {
            get;
            set;
        }


        public int GetCurrentContactFlag
        {
            private set;
            get;
        }


        public int GetExistingContactFlag
        {
            private set;
            get;
        }


        public int SetCurrentContactFlag
        {
            private set;
            get;
        }

        public ContactInfo GetCurrentContact(IUserInfo currentUser, bool forceUserMatching)
        {
            GetCurrentContactFlag++;
            return CurrentContact;
        }

        public ContactInfo GetExistingContact(IUserInfo currentUser, bool forceUserMatching)
        {
            return CurrentContact;
        }

        public void SetCurrentContact(ContactInfo contact)
        {
            SetCurrentContactFlag++;
            CurrentContact = contact;
        }
    }
}
