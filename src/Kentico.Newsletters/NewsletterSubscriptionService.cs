using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;

namespace Kentico.Newsletters
{
    /// <summary>
    /// Provides methods for managing subscriptions to newsletters for one site.
    /// </summary>
    public class NewsletterSubscriptionService
    {
        #region "Constants & Variables"

        private readonly string mSiteName;
        private int mSiteId;
		private NewsletterSubscriptionSettings mSubscriptionSettings;

		#endregion


		#region "Properties"

		private int SiteId
        {
            get
            {
                if (mSiteId == 0)
                {
                    mSiteId = SiteInfoProvider.GetSiteID(mSiteName);
                }

                return mSiteId;
            }
        }

        #endregion


        #region "Public Methods"

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsletterSubscriptionService"/> class.
        /// </summary>
        /// <param name="siteName">The code name of the site which this service will work with</param>
		/// <param name="subscriptionSettings">Subscription configuration</param>
        public NewsletterSubscriptionService(string siteName, NewsletterSubscriptionSettings subscriptionSettings)
        {
            mSiteName = siteName;
			mSubscriptionSettings = subscriptionSettings;
        }


        /// <summary>
        /// Subscribes e-mail address to the specified newsletter. 
        /// If no subscriber with the given e-mail is found, a new subscriber is created.
        /// </summary>
        /// <remarks>
        /// Action fails when:
        /// <list type="bullet">
        /// <item>
        /// <description>Newsletter with the given name does not exist.</description>
        /// </item>
        /// <item>
        /// <description>Current license does not allow creating new subscribers.</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="email">Subscriber's e-mail address</param>
        /// <param name="newsletterName">Name of the newsletter</param>
        /// <returns>Returns true if subscription was successful.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> or <paramref name="newsletterName"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="newsletterName"/> is not valid.</exception>
        public virtual bool Subscribe(string email, string newsletterName)
        {
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }

            if (newsletterName == null)
            {
                throw new ArgumentNullException("newsletterName");
            }

            // Gets information about newsletter on current site
            NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(newsletterName, SiteId);
            if (newsletter == null)
            {
                throw new ArgumentException("Newsletter object with the given newsletter name does not exist.", "newsletterName");
            }

            // Creates new transaction for saving subscriber's information
            using (var tr = new CMSTransactionScope())
            {
                // Saves subscriber into the database
                SubscriberInfo subscriber = SaveSubscriber(email);

                if (subscriber != null)
                {
                    // Assigns subscriber to the newsletter
                    if (SubscribeToNewsletter(subscriber, newsletter))
                    {
                        // Saves changes
                        tr.Commit();
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Unsubscribes subscriber from the specified newsletter.
        /// If subscriber is already unsubscribed, nothing happens.
        /// </summary>
        /// <param name="email">Subscriber's e-mail address</param>
        /// <param name="newsletterGuid">Newsletter unique identifier</param>
        /// <param name="issueGuid">Issue unique identifier</param>
        /// <returns>Returns true if unsubscription was successful.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="newsletterGuid"/> or <paramref name="issueGuid"/> is not valid.</exception>
        public virtual bool Unsubscribe(string email, Guid newsletterGuid, Guid issueGuid)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            if (newsletterGuid == Guid.Empty)
            {
                throw new ArgumentException("Guid.Empty is not a valid id", "newsletterGuid");
            }

            if (issueGuid == Guid.Empty)
            {
                throw new ArgumentException("Guid.Empty is not a valid id", "issueGuid");
            }

            return UnsubscribeInternal(email, newsletterGuid, issueGuid, false);
        }


        /// <summary>
        /// Unsubscribes subscriber from all marketing materials.
        /// If subscriber is already unsubscribed, nothing happens
        /// </summary>
        /// <param name="email">Subscriber's e-mail address</param>
        /// <param name="newsletterGuid">Newsletter unique identifier</param>
        /// <param name="issueGuid">Issue unique identifier</param>
        /// <returns>Returns true if unsubscription was successful.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="newsletterGuid"/> or <paramref name="issueGuid"/> is not valid.</exception>
        public virtual bool UnsubscribeFromAll(string email, Guid newsletterGuid, Guid issueGuid)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            if (newsletterGuid == Guid.Empty)
            {
                throw new ArgumentException("Guid.Empty is not a valid id", "newsletterGuid");
            }

            if (issueGuid == Guid.Empty)
            {
                throw new ArgumentException("Guid.Empty is not a valid id", "issueGuid");
            }

            return UnsubscribeInternal(email, newsletterGuid, issueGuid, true);
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
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(hash))
            {
                throw new ArgumentNullException("hash");
            }

            var emailHashValidatorService = Service<IEmailHashValidator>.Entry();
            bool emailIsValid = emailHashValidatorService.ValidateEmailHash(hash, email);

            return emailIsValid;
        }

        #endregion


        #region "Private Methods"

        /// <summary>
        /// Creates a subscriber object. If the subscriber with the given e-mail address already exists, returns the existing subscriber. 
        /// </summary>
        /// <param name="email">Subscriber's e-mail address</param>
        /// <returns>Returns new subscriber object. If the subscriber with the given email already exists, returns the existing subscriber.</returns>
        private SubscriberInfo SaveSubscriber(string email)
        {
            // Gets information about subscriber based on email address and current site
            SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(email, SiteId);
            if (subscriber == null)
            {
                // Creates new subscriber
                subscriber = new SubscriberInfo
                {
                    SubscriberEmail = email,
                    SubscriberSiteID = SiteId
                };

                // Checks subscriber license limitation
                if (!SubscriberInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Subscribers, ObjectActionEnum.Insert))
                {
                    CoreServices.EventLog.LogEvent("W", "Newsletters", "SaveSubscriber", "Subscriber could not be added due to license limitations.");
                    return null;
                }

                // Saves subscriber info
                SubscriberInfoProvider.SetSubscriberInfo(subscriber);
            }

            return subscriber;
        }


        /// <summary>
        /// Subscribes subscriber to the newsletter.
        /// </summary>
        /// <param name="subscriber">Subscriber object</param>
        /// <param name="newsletter">Newsletter which the subscriber will be subscribed to</param>
        /// <returns>True if subscription was successful.</returns>
        private bool SubscribeToNewsletter(SubscriberInfo subscriber, NewsletterInfo newsletter)
        {
            // Creates new Service for subscriptions
            var subscriptionService = Service<ISubscriptionService>.Entry();

            try
            {
                subscriptionService.Subscribe(subscriber.SubscriberID, newsletter.NewsletterID, mSubscriptionSettings);

                return true;
            }
            catch (Exception exception)
            {
                Service<IEventLogService>.Entry().LogException("Newsletters", "Subscribe", exception);
            }

            return false;
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
        private bool UnsubscribeInternal(string email, Guid newsletterGuid, Guid? issueGuid, bool unsubscribeFromAll)
        {
            // Gets information about newsletter and issue
            NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(newsletterGuid, SiteId);
            int? issueId = null;

            if (newsletter == null)
            {
                throw new ArgumentException("Newsletter with the given newsletter guid does not exist.", "newsletterGuid");
            }

            if (issueGuid.HasValue)
            {
                IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueGuid.Value, SiteId);

                if (issue == null)
                {
                    throw new ArgumentException("Issue with the given issue guid does not exist.", "issue");
                }

                issueId = issue.IssueID;
            }

            // Creates required Services
            var subscriptionService = Service<ISubscriptionService>.Entry();
            var unsubscriptionProvider = Service<IUnsubscriptionProvider>.Entry();

            // Creates new transaction for saving subscriber's information
            using (var tr = new CMSTransactionScope())
            {
                try
                {
                    if (unsubscribeFromAll)
                    {
                        // Unsubscribes if not already unsubscribed
                        if (!unsubscriptionProvider.IsUnsubscribedFromAllNewsletters(email, newsletter.NewsletterSiteID))
                        {
                            subscriptionService.UnsubscribeFromAllNewsletters(email, SiteId, issueId);
                            tr.Commit();
                        }

                        return true;
                    }

                    // Unsubscribes if not already unsubscribed
                    if (!unsubscriptionProvider.IsUnsubscribedFromSingleNewsletter(email, newsletter.NewsletterID, newsletter.NewsletterSiteID))
                    {
                        subscriptionService.UnsubscribeFromSingleNewsletter(email, newsletter.NewsletterID, issueId);
                        tr.Commit();
                    }

                    return true;
                }
                catch (Exception exception)
                {
                    Service<IEventLogService>.Entry().LogException("Newsletters", "Unsubscribe", exception);
                }

                return false;
            }

            #endregion
        }
    }
}