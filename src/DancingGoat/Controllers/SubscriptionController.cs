using System;
using System.Web;
using System.Web.Mvc;

using CMS.Core;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;

using DancingGoat.Models.Subscription;

using Kentico.Membership;
using Kentico.Newsletters;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DancingGoat.Web.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly INewsletterSubscriptionService mSubscriptionService;

        private NewsletterSubscriptionSettings mNewsletterSubscriptionSettings;

        private readonly IContactProvider mContactProvider = Service<IContactProvider>.Entry();

        private NewsletterSubscriptionSettings NewsletterSubscriptionSettings
        {
            get
            {
                return mNewsletterSubscriptionSettings ?? (mNewsletterSubscriptionSettings = new NewsletterSubscriptionSettings
                {
                    AllowOptIn = true,
                    RemoveAlsoUnsubscriptionFromAllNewsletters = true,
                    SendConfirmationEmail = true
                });
            }
        }

        private UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }


        public SubscriptionController(INewsletterSubscriptionService subscriptionService)
        {
            mSubscriptionService = subscriptionService;
        }


        // POST: Subscription/Subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Subscribe(SubscribeModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Subscribe", model);
            }

            var newsletter = NewsletterInfoProvider.GetNewsletterInfo("DancingGoatMvcNewsletter", SiteContext.CurrentSiteID);

            var contact = mContactProvider.GetContactForSubscribing(model.Email);
            bool newlySubscribed = mSubscriptionService.Subscribe(contact, newsletter, NewsletterSubscriptionSettings);

            string resultMessage;

            if (newlySubscribed)
            {
                // The subscription service is configured to use double opt-in, but newsletter must allow for it
                resultMessage = ResHelper.GetString(newsletter.NewsletterEnableOptIn ? "DancingGoatMvc.News.ConfirmationSent" : "DancingGoatMvc.News.Subscribed");
            }
            else
            {
                resultMessage = ResHelper.GetString("DancingGoatMvc.News.AlreadySubscribed");
            }

            return Content(resultMessage);
        }


        // POST: Subscription/SubscribeAuthenticated
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SubscribeAuthenticated()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Show();
            }

            var user = UserManager.FindByName(User.Identity.Name);
            var newsletter = NewsletterInfoProvider.GetNewsletterInfo("DancingGoatMvcNewsletter", SiteContext.CurrentSiteID);
            var contact = mContactProvider.GetContactForSubscribing(user.Email, user.FirstName, user.LastName);

            bool newlySubscribed = mSubscriptionService.Subscribe(contact, newsletter, NewsletterSubscriptionSettings);

            string resultMessage;

            if (newlySubscribed)
            {
                // The subscription service is configured to use double opt-in, but newsletter must allow for it
                resultMessage = ResHelper.GetString(newsletter.NewsletterEnableOptIn ? "DancingGoatMvc.News.ConfirmationSent" : "DancingGoatMvc.News.Subscribed");
            }
            else
            {
                resultMessage = ResHelper.GetString("DancingGoatMvc.News.AlreadySubscribed");
            }

            return Content(resultMessage);
        }


        // GET: Subscription/ConfirmSubscription
        [ValidateInput(false)]
        public ActionResult ConfirmSubscription(ConfirmSubscriptionModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionInvalidLink"));

                return View(model);
            }

            DateTime parsedDateTime = DateTimeHelper.ZERO_TIME;

            // Parse date and time from query string, if present
            if (!string.IsNullOrEmpty(model.DateTime) && !DateTime.TryParseExact(model.DateTime, SecurityHelper.EMAIL_CONFIRMATION_DATETIME_FORMAT, null, System.Globalization.DateTimeStyles.None, out parsedDateTime))
            {
                ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionInvalidDateTime"));

                return View(model);
            }

            var result = mSubscriptionService.ConfirmSubscription(model.SubscriptionHash, false, parsedDateTime);
            switch (result)
            {
                case ApprovalResult.Success:
                    model.ConfirmationResult = ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionSucceeded");
                    break;

                case ApprovalResult.AlreadyApproved:
                    model.ConfirmationResult = ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionAlreadyConfirmed");
                    break;

                case ApprovalResult.TimeExceeded:
                    ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionTimeExceeded"));
                    break;

                case ApprovalResult.NotFound:
                    ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionInvalidLink"));
                    break;

                default:
                    ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionFailed"));

                    break;
            }

            return View(model);
        }


        // GET: Subscription/Unsubscribe
        [ValidateInput(false)]
        public ActionResult Unsubscribe(UnsubscriptionModel model)
        {
            string invalidUrlMessage = ResHelper.GetString("DancingGoatMvc.News.InvalidUnsubscriptionLink");

            if (ModelState.IsValid)
            {
                bool emailIsValid = mSubscriptionService.ValidateEmail(model.Email, model.Hash);

                if (emailIsValid)
                {
                    try
                    {
                        if (model.UnsubscribeFromAll)
                        {
                            mSubscriptionService.UnsubscribeFromAll(model.Email, model.NewsletterGuid, model.IssueGuid);
                            model.UnsubscriptionResult = ResHelper.GetString("DancingGoatMvc.News.UnsubscribedAll");
                        }
                        else
                        {
                            mSubscriptionService.Unsubscribe(model.Email, model.NewsletterGuid, model.IssueGuid);
                            model.UnsubscriptionResult = ResHelper.GetString("DancingGoatMvc.News.Unsubscribed");
                        }
                    }
                    catch (ArgumentException)
                    {
                        ModelState.AddModelError(String.Empty, invalidUrlMessage);
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, invalidUrlMessage);
                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, invalidUrlMessage);
            }

            return View(model);
        }


        // GET: Subscription/Show
        public ActionResult Show()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Handle authenticated user
                return PartialView("_SubscribeAuthenticated");
            }
            var model = new SubscribeModel();

            return PartialView("_Subscribe", model);
        }
    }
}