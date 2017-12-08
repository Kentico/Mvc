using System;
using System.Linq;
using CMS.Activities;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.Tests;
using Tests.Activities;

using NUnit.Framework;

namespace Kentico.Newsletters.Tests
{
    public class NewsletterSubscriptionServiceTests
    {
        #region "Utility methods"

        private static SiteInfo CreateSiteInfo()
        {
            return new SiteInfo
            {
                DisplayName = "Test site",
                SiteName = "TestSite",
                SiteIsContentOnly = true,
                Status = SiteStatusEnum.Running,
                DomainName = "my.cool.web"
            };
        }


        private static EmailTemplateInfo CreateEmailTemplateInfo(int siteId)
        {
            return new EmailTemplateInfo
            {
                TemplateName = "MyCustomNewsletterEmailTemplate",
                TemplateDisplayName = "My Custom Newsletter Email Template",
                TemplateSiteID = siteId,
                TemplateCode = "<!DOCTYPE html><html><head><title>Email title</title></head><body>Email body</body></html>",
                TemplateType = EmailTemplateTypeEnum.Issue,
                TemplateInlineCSS = true
            };
        }


        private static NewsletterInfo CreateNewsletterInfo(int siteId, int emailTemplateId)
        {
            return new NewsletterInfo
            {
                NewsletterName = "MyCustomNewsletter",
                NewsletterDisplayName = "My Custom Newsletter",
                NewsletterSiteID = siteId,
                NewsletterSource = String.Empty,
                NewsletterSubscriptionTemplateID = emailTemplateId,
                NewsletterUnsubscriptionTemplateID = emailTemplateId,
                NewsletterOptInTemplateID = emailTemplateId,
                NewsletterSenderName = "John West",
                NewsletterSenderEmail = "john@west.east",
                NewsletterEnableOptIn = true,
                NewsletterType = EmailCommunicationTypeEnum.Newsletter
            };
        }


        private static NewsletterSubscriptionSettings CreateNewsletterSubscriptionSettings(bool allowOptIn)
        {
            return new NewsletterSubscriptionSettings
            {
                RemoveAlsoUnsubscriptionFromAllNewsletters = true,
                AllowOptIn = allowOptIn,
                SendConfirmationEmail = false
            };
        }


        private static ContactInfo GetContact(string email)
        {
            IContactProvider contactProvider = Service.Resolve<IContactProvider>();
            return contactProvider.GetContactForSubscribing(email);
        }

        #endregion


        [TestFixture]
        public class Subscribe : IsolatedIntegrationTests
        {
            private SiteInfo mSite;
            private NewsletterInfo mNewsletter;
            private NewsletterSubscriptionSettings mNewsletterSubscriptionSettings;
            private NewsletterSubscriptionService mNewsletterSubscriptionService;


            [SetUp]
            public void SetUp()
            {
                mSite = CreateSiteInfo();
                SiteInfoProvider.SetSiteInfo(mSite);

                var emailTemplate = CreateEmailTemplateInfo(mSite.SiteID);
                EmailTemplateInfoProvider.SetEmailTemplateInfo(emailTemplate);

                mNewsletter = CreateNewsletterInfo(mSite.SiteID, emailTemplate.TemplateID);
                NewsletterInfoProvider.SetNewsletterInfo(mNewsletter);

                mNewsletterSubscriptionSettings = CreateNewsletterSubscriptionSettings(false);
                mNewsletterSubscriptionService = new NewsletterSubscriptionService();
                Service.Use<IActivityLogService, ActivityLogServiceInMemoryFake>();

                SiteContext.CurrentSite = mSite;
            }


            [Test]
            public void Subscribe_Contact_ContactIsSubscribed()
            {
                var email = "a-guid-" + Guid.NewGuid() + "@domain.com";
                var contact = GetContact(email);

                var result = mNewsletterSubscriptionService.Subscribe(contact, mNewsletter, mNewsletterSubscriptionSettings);
                var expectedSubscriber = SubscriberInfoProvider.GetSubscriberByEmail(email, mSite.SiteID);

                CMSAssert.All(
                   () => Assert.IsTrue(result, "New subscription is expected."),
                   () => Assert.NotNull(expectedSubscriber, $"No subscriber for email '{email}' was created.")
                );
            }


            [Test]
            public void Subscribe_NewContact_ContactIsStoredToDBAndSubscribed()
            {
                var email = "a-guid-" + Guid.NewGuid() + "@domain.com";
                var contact = new ContactInfo()
                {
                    ContactEmail = email,
                };

                var result = mNewsletterSubscriptionService.Subscribe(contact, mNewsletter, mNewsletterSubscriptionSettings);

                var expectedSubscriber = SubscriberInfoProvider.GetSubscriberByEmail(email, mSite.SiteID);
                var subscribedContact = ContactInfoProvider.GetContacts().WhereEquals("ContactEmail", email);

                CMSAssert.All(
                   () => Assert.IsTrue(result, "New subscription is expected."),
                   () => Assert.NotNull(subscribedContact, $"No contact for email '{email}' was created."),
                   () => Assert.NotNull(expectedSubscriber, $"No subscriber for email '{email}' was created.")
                );
            }


            [Test]
            public void Subscribe_Contact_OnlyOnceIsSubscribed()
            {
                var email = "a-guid-" + Guid.NewGuid() + "@domain.com";
                var contact = GetContact(email);

                // Subscribe contact twice
                mNewsletterSubscriptionService.Subscribe(contact, mNewsletter, mNewsletterSubscriptionSettings);
                var result = mNewsletterSubscriptionService.Subscribe(contact, mNewsletter, mNewsletterSubscriptionSettings);
                var subscribers = SubscriberNewsletterInfoProvider.GetSubscriberNewsletters().TypedResult;
                var contacts = ContactInfoProvider.GetContacts();

                CMSAssert.All(
                   () => Assert.IsFalse(result, "No new subscription is expected."),
                   () => Assert.AreEqual(1, subscribers.Count(), "One subscription only should be present.")
                );
            }
        }


        [TestFixture]
        public class ConfirmSubscription : IsolatedIntegrationTests
        {
            private SiteInfo mSite;
            private NewsletterInfo mNewsletter;
            private NewsletterSubscriptionSettings mNewsletterSubscriptionSettings;
            private NewsletterSubscriptionService mNewsletterSubscriptionService;


            [SetUp]
            public void SetUp()
            {
                mSite = CreateSiteInfo();
                SiteInfoProvider.SetSiteInfo(mSite);

                var emailTemplate = CreateEmailTemplateInfo(mSite.SiteID);
                EmailTemplateInfoProvider.SetEmailTemplateInfo(emailTemplate);

                mNewsletter = CreateNewsletterInfo(mSite.SiteID, emailTemplate.TemplateID);
                NewsletterInfoProvider.SetNewsletterInfo(mNewsletter);

                mNewsletterSubscriptionSettings = CreateNewsletterSubscriptionSettings(true);
                mNewsletterSubscriptionService = new NewsletterSubscriptionService();
                Service.Use<IActivityLogService, ActivityLogServiceInMemoryFake>();

                SiteContext.CurrentSite = mSite;
            }


            [Test]
            public void ConfirmSubscription_SubscriptionIsNotConfirmedByDefault()
            {
                var email = "a-guid-" + Guid.NewGuid() + "@domain.com";
                var contact = GetContact(email);

                mNewsletterSubscriptionService.Subscribe(contact, mNewsletter, mNewsletterSubscriptionSettings);

                var expectedSubscriber = SubscriberInfoProvider.GetSubscriberByEmail(email, mSite.SiteID);

                var expectedSubscriberNewsletter = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(expectedSubscriber.SubscriberID, mNewsletter.NewsletterID);

                Assert.IsFalse(expectedSubscriberNewsletter.SubscriptionApproved, $"Subscription must not be approved when double opt-in is enabled.");
            }


            [Test]
            public void ConfirmSubscription_Hash_DateTime_SubscriptionIsConfirmed()
            {
                var email = "a-guid-" + Guid.NewGuid() + "@domain.com";
                var dateTime = DateTime.Now;
                var contact = GetContact(email);

                mNewsletterSubscriptionService.Subscribe(contact, mNewsletter, mNewsletterSubscriptionSettings);

                // Get the expected subscriber and binding to newsletter so that hash for their subscription confirmation can be faked
                // (the newsletters API allows to retrieve current hash, but not the corresponding timestamp which is necessary to pass the validation).
                var expectedSubscriber = SubscriberInfoProvider.GetSubscriberByEmail(email, mSite.SiteID);
                var confirmationHash = SecurityHelper.GenerateConfirmationEmailHash(mNewsletter.NewsletterGUID + "|" + expectedSubscriber.SubscriberGUID, dateTime);

                var expectedSubscriberNewsletter = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(expectedSubscriber.SubscriberID, mNewsletter.NewsletterID);
                expectedSubscriberNewsletter.SubscriptionApprovalHash = confirmationHash;
                SubscriberNewsletterInfoProvider.SetSubscriberNewsletterInfo(expectedSubscriberNewsletter);

                var approvalResult = mNewsletterSubscriptionService.ConfirmSubscription(expectedSubscriberNewsletter.SubscriptionApprovalHash, false, dateTime);

                // Retrieve the subscriber-newsletter binding again so that approval state can be validated
                expectedSubscriberNewsletter = SubscriberNewsletterInfoProvider.GetSubscriberNewsletterInfo(expectedSubscriber.SubscriberID, mNewsletter.NewsletterID);

                Assert.AreEqual(ApprovalResult.Success, approvalResult, "Subscription confirmation was not successful.");
                Assert.IsTrue(expectedSubscriberNewsletter.SubscriptionApproved, $"Subscription is not approved after confirmation.");
            }
        }
    }
}
