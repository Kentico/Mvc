using System;

using CMS.Core;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.ContactManagement;

namespace Kentico.Newsletters
{
    /// <summary>
    /// Provides methods for managing subscriptions to newsletters for one site.
    /// </summary>
    public class NewsletterSubscriptionService : INewsletterSubscriptionService
    {
        #region "Properties"

        /// <summary>
        /// Gets site ID.
        /// </summary>
        protected int SiteId
        {
            get
            {
                return SiteContext.CurrentSiteID;
            }
        }


        /// <summary>
        /// Gets site name.
        /// </summary>
        protected string SiteName
        {
            get
            {
                return SiteContext.CurrentSiteName;
            }
        }

        #endregion


        #region "Public Methods"

        /// <summary>
        /// Subscribes <paramref name="contact"/> to the specified <paramref name="newsletter"/> using <paramref name="subscriptionSettings"/>.
        /// If a given contact hasn't been saved into database yet the method does so.
        /// </summary>
        /// <param name="contact">Subscriber to be subscribed</param>
        /// <param name="newsletter">Newsletter to subscribe to</param>
        /// <param name="subscriptionSettings">Subscription configuration</param>
        /// <returns>True if contact was subscribed by current call, false when contact had already been subscribed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="contact"/> or <paramref name="newsletter"/> is null.</exception>
        public virtual bool Subscribe(ContactInfo contact, NewsletterInfo newsletter, NewsletterSubscriptionSettings subscriptionSettings)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            if (newsletter == null)
            {
                throw new ArgumentNullException(nameof(newsletter));
            }

            var subscriptionService = Service<ISubscriptionService>.Entry();

            if (!subscriptionService.IsSubscribed(contact, newsletter))
            {
                if (contact.ContactID <= 0)
                {
                    ContactInfoProvider.SetContactInfo(contact);
                }

                subscriptionService.Subscribe(contact, newsletter, subscriptionSettings);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Confirms subscription to a newsletter when double opt-in is required.
        /// </summary>
        /// <param name="subscriptionHash">Subscription confirmation hash</param>
        /// <param name="sendConfirmationEmail">Indicates if confirmation email should be sent. Confirmation email may also be sent if newsletter settings requires so</param>
        /// <param name="datetime">Date time when the hash was created. Subscription must be confirmed within certain period of time driven by site settings</param>
        /// <returns>Subscription confirmation result</returns>
        public virtual ApprovalResult ConfirmSubscription(string subscriptionHash, bool sendConfirmationEmail, DateTime datetime)
        {
            if (subscriptionHash == null)
            {
                throw new ArgumentNullException(nameof(subscriptionHash));
            }

            return ConfirmSubscriptionInternal(subscriptionHash, sendConfirmationEmail, datetime);
        }


        /// <summary>
        /// Unsubscribes subscriber from the specified newsletter.
        /// If subscriber is already unsubscribed, nothing happens.
        /// </summary>
        /// <param name="email">Subscriber's e-mail address</param>
        /// <param name="newsletterGuid">Newsletter unique identifier</param>
        /// <param name="issueGuid">Issue unique identifier</param>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="newsletterGuid"/> or <paramref name="issueGuid"/> is not valid.</exception>
        public virtual void Unsubscribe(string email, Guid newsletterGuid, Guid issueGuid)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (newsletterGuid == Guid.Empty)
            {
                throw new ArgumentException("Guid.Empty is not a valid id", nameof(newsletterGuid));
            }

            if (issueGuid == Guid.Empty)
            {
                throw new ArgumentException("Guid.Empty is not a valid id", nameof(issueGuid));
            }

            UnsubscribeInternal(email, newsletterGuid, issueGuid, false);
        }


        /// <summary>
        /// Unsubscribes subscriber from all marketing materials.
        /// If subscriber is already unsubscribed, nothing happens.
        /// </summary>
        /// <param name="email">Subscriber's e-mail address</param>
        /// <param name="newsletterGuid">Newsletter unique identifier</param>
        /// <param name="issueGuid">Issue unique identifier</param>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="newsletterGuid"/> or <paramref name="issueGuid"/> is not valid.</exception>
        public virtual void UnsubscribeFromAll(string email, Guid newsletterGuid, Guid issueGuid)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (newsletterGuid == Guid.Empty)
            {
                throw new ArgumentException("Guid.Empty is not a valid id", nameof(newsletterGuid));
            }

            if (issueGuid == Guid.Empty)
            {
                throw new ArgumentException("Guid.Empty is not a valid id", nameof(issueGuid));
            }

            UnsubscribeInternal(email, newsletterGuid, issueGuid, true);
        }


        /// <summary>
        /// Validates <paramref name="email"/> against the given <paramref name="hash"/>.
        /// </summary>
        /// <param name="email">Email address to be validated</param>
        /// <param name="hash">Email hash</param>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> or <paramref name="hash"/> is null.</exception>
        /// <returns>True, if given <paramref name="email"/> is valid; otherwise, false</returns>
        public virtual bool ValidateEmail(string email, string hash)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrEmpty(hash))
            {
                throw new ArgumentNullException(nameof(hash));
            }

            var emailHashValidatorService = Service<IEmailHashValidator>.Entry();
            bool emailIsValid = emailHashValidatorService.ValidateEmailHash(hash, email);

            return emailIsValid;
        }

        #endregion


        #region "Protected and Private Methods"

        /// <summary>
        /// Confirms subscription to a newsletter when double opt-in is required.
        /// </summary>
        /// <param name="subscriptionHash">Subscription confirmation hash</param>
        /// <param name="sendConfirmationEmail">Indicates if confirmation email should be sent. Confirmation email may also be sent if newsletter settings requires so</param>
        /// <param name="datetime">Date time when the hash was created. Subscription must be confirmed within certain period of time driven by site settings</param>
        /// <returns>Subscription confirmation result</returns>
        private ApprovalResult ConfirmSubscriptionInternal(string subscriptionHash, bool sendConfirmationEmail, DateTime datetime)
        {
            var approvalService = Service<ISubscriptionApprovalService>.Entry();

            return approvalService.ApproveSubscription(subscriptionHash, sendConfirmationEmail, SiteName, datetime);
        }


        /// <summary>
        /// Unsubscribes email from a newsletter of all marketing materials.
        /// </summary>
        /// <param name="email">Subscriber's e-mail address</param>
        /// <param name="newsletterGuid">Newsletter to unsubscribe email from</param>
        /// <param name="issueGuid">Issue unique identifier. When issue guid is present, number of unsubscriptions for this issue is increased</param>
        /// <param name="unsubscribeFromAll">If true, subscriber is unsubscribed from all marketing materials</param>
        /// <returns>Returns true if unsubscription was successful.</returns>
        /// <exception cref="ArgumentException"><paramref name="newsletterGuid"/> or <paramref name="issueGuid"/> is not valid.</exception>
        private void UnsubscribeInternal(string email, Guid newsletterGuid, Guid? issueGuid, bool unsubscribeFromAll)
        {
            // Gets information about newsletter and issue
            NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(newsletterGuid, SiteId);
            int? issueId = null;

            if (newsletter == null)
            {
                throw new ArgumentException("Newsletter with the given newsletter guid does not exist.", nameof(newsletterGuid));
            }

            if (issueGuid.HasValue)
            {
                IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueGuid.Value, SiteId);

                if (issue == null)
                {
                    throw new ArgumentException("Issue with the given issue guid does not exist.", nameof(issueGuid));
                }

                issueId = issue.IssueID;
            }

            // Creates required Services
            var subscriptionService = Service<ISubscriptionService>.Entry();
            var unsubscriptionProvider = Service<IUnsubscriptionProvider>.Entry();

            if (unsubscribeFromAll)
            {
                // Unsubscribes if not already unsubscribed
                if (!unsubscriptionProvider.IsUnsubscribedFromAllNewsletters(email))
                {
                    subscriptionService.UnsubscribeFromAllNewsletters(email, issueId);
                }

                return;
            }

            // Unsubscribes if not already unsubscribed
            if (!unsubscriptionProvider.IsUnsubscribedFromSingleNewsletter(email, newsletter.NewsletterID))
            {
                subscriptionService.UnsubscribeFromSingleNewsletter(email, newsletter.NewsletterID, issueId);
            }
        }

        #endregion
    }
}