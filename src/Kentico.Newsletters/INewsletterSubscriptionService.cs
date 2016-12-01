using System;

using CMS.Newsletters;
using CMS.ContactManagement;

using Kentico.Core.DependencyInjection;

namespace Kentico.Newsletters
{
    /// <summary>
    /// Provides methods for managing subscriptions to newsletters for one site.
    /// </summary>
    public interface INewsletterSubscriptionService : IService
    {
        /// <summary>
        /// Subscribes <paramref name="contact"/> to the specified newsletter. 
        /// </summary>
        /// <example> 
        /// This sample shows how to call the <see cref="Subscribe"/> method when an email is provided.
        /// <code>
        /// {
        ///     IContactProvider contactProvider = Service&lt;IContactProvider&gt;.Entry();
        ///     ContactInfo contact = contactProvider.GetContactForSubscribing(email);
        ///     Subscribe(contact, newsletter, subscriptionSettings);
        /// }
        /// </code>
        /// </example>
        /// <param name="contact">Subscriber to be subscribed</param>
        /// <param name="newsletter">Newsletter to subscribe to</param>
        /// <param name="subscriptionSettings">Subscription configuration</param>
        /// <returns>True if subscriber was subscribed by current call, false when subscriber had already been subscribed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="contact"/> or <paramref name="newsletter"/> is null.</exception>
        bool Subscribe(ContactInfo contact, NewsletterInfo newsletter, NewsletterSubscriptionSettings subscriptionSettings);


        /// <summary>
        /// Confirms subscription to a newsletter when double opt-in is required.
        /// </summary>
        /// <param name="subscriptionHash">Subscription confirmation hash</param>
        /// <param name="sendConfirmationEmail">Indicates if confirmation email should be sent. Confirmation email may also be sent if newsletter settings requires so</param>
        /// <param name="datetime">Date time when the hash was created. Subscription must be confirmed within certain period of time driven by site settings</param>
        /// <returns>Subscription confirmation result</returns>
        ApprovalResult ConfirmSubscription(string subscriptionHash, bool sendConfirmationEmail, DateTime datetime);


        /// <summary>
        /// Unsubscribes subscriber from the specified newsletter.
        /// If subscriber is already unsubscribed, nothing happens.
        /// </summary>
        /// <param name="email">Subscriber's e-mail address</param>
        /// <param name="newsletterGuid">Newsletter unique identifier</param>
        /// <param name="issueGuid">Issue unique identifier</param>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="newsletterGuid"/> or <paramref name="issueGuid"/> is not valid.</exception>
        void Unsubscribe(string email, Guid newsletterGuid, Guid issueGuid);


        /// <summary>
        /// Unsubscribes subscriber from all marketing materials.
        /// If subscriber is already unsubscribed, nothing happens.
        /// </summary>
        /// <param name="email">Subscriber's e-mail address</param>
        /// <param name="newsletterGuid">Newsletter unique identifier</param>
        /// <param name="issueGuid">Issue unique identifier</param>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="newsletterGuid"/> or <paramref name="issueGuid"/> is not valid.</exception>
        void UnsubscribeFromAll(string email, Guid newsletterGuid, Guid issueGuid);


        /// <summary>
        /// Validates <paramref name="email"/> against the given <paramref name="hash"/>.
        /// </summary>
        /// <param name="email">Email address to be validated</param>
        /// <param name="hash">Email hash</param>
        /// <exception cref="ArgumentNullException"><paramref name="email"/> or <paramref name="hash"/> is null.</exception>
        /// <returns>True, if given <paramref name="email"/> is valid; otherwise, false</returns>
        bool ValidateEmail(string email, string hash);
    }
}