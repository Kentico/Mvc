using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

//DocSection:Usings
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataProtection;
using CMS.Helpers;

using Kentico.ContactManagement;
//EndDocSection:Usings

using LearningKit.Models.PrivacyPage;

namespace LearningKit.Controllers
{
    public class PrivacyPageController : Controller
    {
        //DocSection:Constructor
        private readonly IContactTrackingService contactTrackingService;
        private readonly IConsentAgreementService consentAgreementService;
        private readonly ICurrentCookieLevelProvider currentCookieLevelProvider;

        public PrivacyPageController()
        {
            consentAgreementService = Service.Resolve<IConsentAgreementService>();
            contactTrackingService = new ContactTrackingService();
            currentCookieLevelProvider = Service.Resolve<ICurrentCookieLevelProvider>();
        }
        //EndDocSection:Constructor

        //DocSection:Index
        /// <summary>
        /// Loads and displays consents for which visitors have given agreements.
        /// </summary>
        public ActionResult Index()
        {
            // Gets the current visitor's contact
            ContactInfo currentContact = contactTrackingService.GetCurrentContactAsync(User.Identity.Name).Result;

            var consentListingModel = new ConsentListingModel();
            
            // Does not attempt to load consent data if the current contact is not available
            // This occurs if contact tracking is disabled, or for visitors who have not given an agreement with the tracking consent
            if (currentContact != null)
            {
                // Gets all consents for which the current contact has given an agreement
                consentListingModel.Consents = consentAgreementService.GetAgreedConsents(currentContact);
            }

            return View(consentListingModel);
        }
        //EndDocSection:Index

        //DocSection:Revoke
        /// <summary>
        /// Revokes the current contact's agreement with the specified consent.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revoke(int consentId)
        {
            // Gets the related consent object
            ConsentInfo consent = ConsentInfoProvider.GetConsentInfo(consentId);

            // Gets the current visitor's contact
            ContactInfo currentContact = contactTrackingService.GetCurrentContactAsync(User.Identity.Name).Result;

            // For the tracking consent, lowers the cookie level to the site's default in order to disable tracking
            if (consent.ConsentName == "SampleTrackingConsent")
            {
                currentCookieLevelProvider.SetCurrentCookieLevel(currentCookieLevelProvider.GetDefaultCookieLevel());
            }

            // Revokes the consent agreement
            consentAgreementService.Revoke(currentContact, consent);

            return RedirectToAction("Index");
        }
        //EndDocSection:Revoke

        //DocSection:Details
        /// <summary>
        /// Display details of the specified consent.
        /// </summary>
        public ActionResult ConsentDetails(int consentId)
        {
            // Gets a list of consents for which the current contact has given an agreement            
            ContactInfo currentContact = contactTrackingService.GetCurrentContactAsync(User.Identity.Name).Result;
            IEnumerable<Consent> consents = consentAgreementService.GetAgreedConsents(currentContact);

            // Gets the consent matching the identifier for which the details were requested
            // Using this approach to get objects of the 'Consent' class is necessary to ensure that the correct consent text
            // is displayed, either from the current consent text or the archived consent version for which the agreement was given
            Consent consent = consents.FirstOrDefault(c => c.Id == consentId);

            // Displays the privacy page (consent list) if the specified consent identifier is not valid
            if (consent == null)
            {
                return View("Index");
            }

            // Sets the consent's details into the view model
            var model = new ConsentDetailsModel
            {
                ConsentDisplayName = consent.DisplayName,
                ConsentShortText = consent.GetConsentText("en-US").ShortText,
                ConsentFullText = consent.GetConsentText("en-US").FullText
            };

            return View(model);
        }
        //EndDocSection:Details
    }
}