using System.Web.Mvc;

//DocSection:Usings
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataProtection;
using CMS.Helpers;

using Kentico.ContactManagement;
//EndDocSection:Usings

using LearningKit.Models.TrackingConsent;

namespace LearningKit.Controllers
{
    public class TrackingConsentController : Controller
    {
        //DocSection:Constructor
        private readonly IConsentAgreementService consentAgreementService;
        private readonly IContactTrackingService contactTrackingService;
        private readonly ICurrentCookieLevelProvider currentCookieLevelProvider;
        
        public TrackingConsentController()
        {
            consentAgreementService = Service.Resolve<IConsentAgreementService>();
            contactTrackingService = new ContactTrackingService();
            currentCookieLevelProvider = Service.Resolve<ICurrentCookieLevelProvider>();
        }
        //EndDocSection:Constructor

        //DocSection:DisplayConsent
        public ActionResult DisplayConsent()
        {
            // Gets the related tracking consent
            // Fill in the code name of the appropriate consent object in Kentico
            ConsentInfo consent = ConsentInfoProvider.GetConsentInfo("SampleTrackingConsent");

            // Gets the current contact
            ContactInfo contact = contactTrackingService.GetCurrentContactAsync(User.Identity.Name).Result;

            // Sets the default cookie level for contacts who have revoked the tracking consent
            // Required for scenarios where one contact uses multiple browsers
            if ((contact != null) && !consentAgreementService.IsAgreed(contact, consent))
            {
                var defaultCookieLevel = currentCookieLevelProvider.GetDefaultCookieLevel();
                currentCookieLevelProvider.SetCurrentCookieLevel(defaultCookieLevel);
            }
            
            var consentModel = new TrackingConsentViewModel
            {
                // Adds the consent's short text to the model
                ShortText = consent.GetConsentText("en-US").ShortText,

                // Checks whether the current contact has given an agreement for the tracking consent
                IsAgreed = (contact != null) && consentAgreementService.IsAgreed(contact, consent)
            };

            return View("Consent", consentModel);
        }
        //EndDocSection:DisplayConsent
        
        //DocSection:AcceptConsent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Accept()
        {
            // Gets the related tracking consent
            ConsentInfo consent = ConsentInfoProvider.GetConsentInfo("SampleTrackingConsent");

            // Sets the visitor's cookie level to 'All' (enables contact tracking)
            currentCookieLevelProvider.SetCurrentCookieLevel(CookieLevel.All);

            // Gets the current contact and creates a consent agreement
            ContactInfo contact = contactTrackingService.GetCurrentContactAsync(User.Identity.Name).Result;
            consentAgreementService.Agree(contact, consent);

            return RedirectToAction("DisplayConsent");
        }
        //EndDocSection:AcceptConsent
        
        //DocSection:RevokeConsent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revoke()
        {
            // Gets the related tracking consent
            ConsentInfo consent = ConsentInfoProvider.GetConsentInfo("SampleTrackingConsent");

            // Gets the current contact and revokes the tracking consent agreement
            ContactInfo contact = contactTrackingService.GetCurrentContactAsync(User.Identity.Name).Result;
            consentAgreementService.Revoke(contact, consent);

            // Sets the visitor's cookie level to the site's default cookie level (disables contact tracking)
            int defaultCookieLevel = currentCookieLevelProvider.GetDefaultCookieLevel();
            currentCookieLevelProvider.SetCurrentCookieLevel(defaultCookieLevel);

            return RedirectToAction("DisplayConsent");
        }
        //EndDocSection:RevokeConsent
    }
}