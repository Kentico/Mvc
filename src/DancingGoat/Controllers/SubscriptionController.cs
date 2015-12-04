using System;
using System.Web.Mvc;

using CMS.Helpers;

using Kentico.Newsletters;
using DancingGoat.Models.Subscription;

namespace DancingGoat.Web.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly NewsletterSubscriptionService mService;


        public SubscriptionController(NewsletterSubscriptionService subscriptionService)
        {
            mService = subscriptionService;
        }


        // POST: Subscription/Subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Subscribe(SubscribeModel model)
        {
            if (ModelState.IsValid)
            {
                if (mService.Subscribe(model.Email, "DancingGoatMvcNewsletter"))
                {
                    model.SubscriptionSaved = true;
                }
                else
                {
                    ModelState.AddModelError("Email", ResHelper.GetString("DancingGoatMvc.News.SubscribeError"));
                }
            }

            return PartialView("_Subscribe", model);
        }


        // GET: Subscription/Unsubscribe
        [ValidateInput(false)]
        public ActionResult Unsubscribe(UnsubscriptionModel model)
        {
            bool unsubscribed = false;
            string invalidUrlMessage = ResHelper.GetString("DancingGoatMvc.News.InvalidUnsubscriptionLink");

            if (ModelState.IsValid)
            {
                bool emailIsValid = mService.ValidateEmail(model.Email, model.Hash);

                if (emailIsValid)
                {
                    try
                    {
                        if (model.UnsubscribeFromAll)
                        {
                            unsubscribed = mService.UnsubscribeFromAll(model.Email, model.NewsletterGuid, model.IssueGuid);
                        }
                        else
                        {
                            unsubscribed = mService.Unsubscribe(model.Email, model.NewsletterGuid, model.IssueGuid);
                        }
                    }
                    catch (ArgumentException)
                    {
                        model.UnsubscriptionResult = invalidUrlMessage;
                    }
                }
                else
                {
                    model.UnsubscriptionResult = invalidUrlMessage;
                }
            }
            else
            {
                model.UnsubscriptionResult = invalidUrlMessage;
            }

            model.IsError = !unsubscribed;
            if (unsubscribed)
            {
                // Return a successful message
                model.UnsubscriptionResult = ResHelper.GetString(model.UnsubscribeFromAll ? "DancingGoatMvc.News.UnsubscribedAll" : "DancingGoatMvc.News.Unsubscribed");
            }
            else if (String.IsNullOrEmpty(model.UnsubscriptionResult))
            {
                // Return a general error message unless a specific error message is already defined
                model.UnsubscriptionResult = ResHelper.GetString("DancingGoatMvc.News.UnsubscribeError");
            }

            return View(model);
        }


        // GET: Subscription/Show
        public ActionResult Show()
        {
            var model = new SubscribeModel();
            return PartialView("_Subscribe", model);
        }
    }
}