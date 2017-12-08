using System.Web.Mvc;

using CMS.Core;
using CMS.DataProtection;
using CMS.OnlineForms.Types;

using Kentico.ContactManagement;

using LearningKit.Models.Form;

namespace LearningKit.Controllers
{
    public class FeedbackFormConsentController : Controller
    {
        private readonly IContactTrackingService contactTrackingService;
        private readonly IFormConsentAgreementService formConsentAgreementService;
        private readonly ConsentInfo consent;

        /// <summary>
        /// Constructor.
        /// You can use a dependency injection container to initialize the services.
        /// </summary>
        public FeedbackFormConsentController()
        {
            contactTrackingService = new ContactTrackingService();
            formConsentAgreementService = Service.Resolve<IFormConsentAgreementService>();

            // Gets the related consent
            // Fill in the code name of the appropriate consent object in Kentico
            consent = ConsentInfoProvider.GetConsentInfo("SampleFormConsent");
        }

        /// <summary>
        /// Basic action that displays the feedback form.
        /// </summary>
        public ActionResult Fill()
        {
            var model = new FeedbackFormMessageConsentModel
            {
                // Adds the consent text to the form model
                ConsentShortText = consent.GetConsentText("en-US").ShortText,
                ConsentIsAgreed = false
            };

            return View("FormFillConsent", model);
        }

        /// <summary>
        /// Inserts the form data to the connected database when the feedback form is submitted.
        /// Accepts parameters posted from the feedback form via the FeedbackFormMessageConsentModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendFeedback(FeedbackFormMessageConsentModel model)
        {
            // Validates the received form data based on the view model
            if (!ModelState.IsValid)
            {
                model.ConsentShortText = consent.GetConsentText("en-US").ShortText;

                return View("FormFillConsent", model);
            }

            // Inserts the collected form data to the connected database
            InsertFeedbackFormItem(model);

            return View("FormSendSuccessConsent");
        }

        // Inserts the collected data to the connected database (helper method)
        private void InsertFeedbackFormItem(FeedbackFormMessageConsentModel feedback)
        {
            // Creates a form item containing the collected data
            var item = new FeedbackFormItemConsent
            {
                UserName = feedback.FirstName,
                UserLastName = feedback.LastName,
                UserEmail = feedback.Email,
                UserFeedback = feedback.MessageText,
            };

            // Creates a consent agreement if the consent checkbox was selected
            if (feedback.ConsentIsAgreed)
            {
                // Gets the current contact
                var contact = contactTrackingService.GetCurrentContactAsync(User.Identity.Name).Result;

                // Creates an agreement for the specified consent and contact
                var agreement = formConsentAgreementService.Agree(contact, consent, item);

                // Adds the GUID of the agreement into the form's consent field
                item.ConsentAgreement = agreement.ConsentAgreementGuid;
            }

            // Inserts the form data into the database
            item.Insert();
        }
    }
}