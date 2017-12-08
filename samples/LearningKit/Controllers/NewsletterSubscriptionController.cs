//DocSection:Using
using System;
using System.Web.Mvc;

using CMS.Core;
using CMS.ContactManagement;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.Helpers;

using Kentico.Newsletters;
//EndDocSection:Using

namespace LearningKit.Controllers
{
    public class NewsletterSubscriptionController : Controller
    {
        /// <summary>
        /// Provides a NewsletterSubscriptionService instance for managing newsletter subscriptions and unsubscription for all types of email feeds.
        /// For real-world projects, we recommend using a dependency injection container to initialize service instances.
        /// The recommended lifetime scope for NewsletterSubscriptionService is a shared instance (singleton).
        /// </summary>
        private INewsletterSubscriptionService SubscriptionService
        {
            get
            {
                //DocSection:SubscriptionService              
                var subscriptionService = new NewsletterSubscriptionService();
                //EndDocSection:SubscriptionService
                
                return subscriptionService;
            }
        }

        //DocSection:Subscribe
        /// <summary>
        /// Basic action that displays the newsletter subscription form.
        /// </summary>
        public ActionResult Subscribe()
        {
            return View();
        }
        
        /// <summary>
        /// Handles creation of new marketing email recipients when the subscription form is submitted.
        /// Accepts an email address parameter posted from the subscription form.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subscribe(NewsletterSubscribeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If the subscription data is not valid, displays the subscription form with error messages
                return View(model);
            }
            
            // Gets the default Kentico contact provider
            IContactProvider contactProvider = Service.Resolve<IContactProvider>();
            // Either gets an existing contact by email or creates a new contact object with the given email
            ContactInfo contact = contactProvider.GetContactForSubscribing(model.Email);
            
            // Gets a newsletter
            // Fill in the code name of your newsletter object in Kentico
            NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo("SampleNewsletter", SiteContext.CurrentSiteID);
            
            // Prepares settings that configure the subscription behavior
            var subscriptionSettings = new NewsletterSubscriptionSettings()
            {
                RemoveAlsoUnsubscriptionFromAllNewsletters = true, // Subscription removes the given email address from the global opt-out list for all marketing emails (if present)
                SendConfirmationEmail = true, // Allows sending of confirmation emails for the subscription
                AllowOptIn = true // Allows handling of double opt-in subscription for newsletters that have it enabled
            };
            
            // Subscribes the contact with the specified email address to the given newsletter
            SubscriptionService.Subscribe(contact, newsletter, subscriptionSettings);
            
            // Passes information to the view, indicating whether the newsletter requires double opt-in for subscription
            model.RequireDoubleOptIn = newsletter.NewsletterEnableOptIn;
            
            // Displays a view to inform the user that the subscription was successful
            return View("SubscribeSuccess", model);
        }
        //EndDocSection:Subscribe

        //DocSection:Unsubscribe
        /// <summary>
        /// Handles marketing email unsubscription requests.
        /// </summary>
        public ActionResult Unsubscribe(MarketingEmailUnsubscribeModel model)
        {
            // Verifies that the unsubscription request contains all required parameters
            if (ModelState.IsValid)
            {
                // Confirms whether the hash in the unsubscription request is valid for the given email address
                // Provides protection against forged unsubscription requests
                if (SubscriptionService.ValidateEmail(model.Email, model.Hash))
                {
                    // Unsubscribes the specified email address (either from all marketing emails or one specific email feed)
                    if (model.UnsubscribeFromAll)
                    {
                        SubscriptionService.UnsubscribeFromAll(model.Email, model.NewsletterGuid, model.IssueGuid);
                    }
                    else
                    {
                        SubscriptionService.Unsubscribe(model.Email, model.NewsletterGuid, model.IssueGuid);
                    }
                    
                    // Displays a view to inform the user that they were unsubscribed
                    return View("UnsubscribeSuccess");
                }
            }
            
            // If the unsubscription was not successful, displays a view to inform the user
            // Failure can occur if the request does not provide all required parameters or if the hash is invalid
            return View("UnsubscribeFailure");
        }
        //EndDocSection:Unsubscribe

        //DocSection:ConfirmSubscription
        /// <summary>
        /// Handles confirmation requests for newsletter subscriptions (when using double opt-in).
        /// </summary>
        public ActionResult ConfirmSubscription(NewsletterConfirmSubscriptionViewModel model)
        {
            // Verifies that the confirmation request contains the required hash parameter
            if (!ModelState.IsValid)
            {
                // If the hash is missing, returns a view informing the user that the subscription confirmation was not successful
                ModelState.AddModelError(String.Empty, "The confirmation link is invalid.");
                return View(model);
            }
            
            // Attempts to parse the date and time parameter from the request query string
            // Uses the date and time formats required by the Kentico API
            DateTime parsedDateTime = DateTimeHelper.ZERO_TIME;
            if (!string.IsNullOrEmpty(model.DateTime) && !DateTimeUrlFormatter.TryParse(model.DateTime, out parsedDateTime))
            {
                // Returns a view informing the user that the subscription confirmation was not successful
                ModelState.AddModelError(String.Empty, "The confirmation link is invalid.");
                return View(model);
            }
            
            // Attempts to confirm the subscription specified by the request's parameters
            ApprovalResult result = SubscriptionService.ConfirmSubscription(model.SubscriptionHash, false, parsedDateTime);
            
            switch (result)
            {
                // The confirmation was successful or the recipient was already approved
                // Displays a view informing the user that the subscription is active
                case ApprovalResult.Success:
                case ApprovalResult.AlreadyApproved:
                    return View(model);
                
                // The confirmation link has expired
                // Expiration occurs after a certain number of hours from the time of the original subscription
                // You can set the expiration interval in Kentico (Settings -> On‑line marketing -> Email marketing -> Double opt-in interval)
                case ApprovalResult.TimeExceeded:
                    ModelState.AddModelError(String.Empty, "Your confirmation link has expired. Please subscribe to the newsletter again.");
                    break;
                
                // The subscription specified in the request's parameters does not exist
                case ApprovalResult.NotFound:
                    ModelState.AddModelError(String.Empty, "The subscription that you are attempting to confirm does not exist.");
                    break;
                
                // The confirmation failed
                default:
                    ModelState.AddModelError(String.Empty, "The confirmation of your newsletter subscription did not succeed.");
                    break;
            }           
            
            // If the subscription confirmation was not successful, displays a view informing the user
            return View(model);
        }
        //EndDocSection:ConfirmSubscription
    }
}