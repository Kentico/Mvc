using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using CMS.ContactManagement;

using Kentico.Web.Mvc;
using Kentico.ContactManagement;

namespace LearningKit
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Enables and configures the selected Kentico ASP.NET MVC integration features
            ApplicationConfig.RegisterFeatures(ApplicationBuilder.Current);
        }

        //DocSection:GetVaryByCustom
        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            // Defines caching requirements based on the on-line marketing data of the current contact
            if (arg == "contactdata")
            {
                // Gets the current contact
                ContactInfo currentContact = new ContactTrackingService().GetExistingContactAsync().Result;

                if (currentContact != null)
                {
                    // Ensures separate caching for each combination of the following contact variables: contact groups, persona, gender
                    // Note: This does NOT define separate caching for every contact
                    return String.Format("ContactData={0}|{1}|{2}",
                        String.Join("|", currentContact.ContactGroups.Select(c => c.ContactGroupName).OrderBy(x => x)),
                        currentContact.ContactPersonaID,
                        currentContact.ContactGender);
                }
            }
            
            return base.GetVaryByCustomString(context, arg);
        }
        //EndDocSection:GetVaryByCustom
    }
}
