using System.Web.Mvc;
using CMS.ContactManagement;
using Kentico.ContactManagement;

namespace LearningKit.Controllers
{
    public class PersonalizationController : Controller
    {
        /// <summary>
        /// Gets the current contact, if contact tracking is enabled for the connected Kentico instance.
        /// </summary>
        private ContactInfo CurrentContact
        {
            get
            {
                IContactTrackingService contactService = new ContactTrackingService();
                return contactService.GetCurrentContactAsync(User.Identity.Name).Result;
            }
        }

        /// <summary>
        /// Displays a page with a personalized greeting.
        /// The content depends on whether the current contact belongs to the "YoungCustomers" contact group.
        /// Caches the action's output for 10 minutes, with different cache versions defined by the "contactdata" custom string.
        /// The "contactdata" string ensures separate caching for each combination of the following contact variables: contact groups, persona, gender
        /// </summary>
        [OutputCache(Duration = 600, VaryByCustom = "contactdata")]
        public ActionResult PersonalizedGreeting()
        {
            return View(CurrentContact);
        }
    }
}