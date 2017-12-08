using System;
using System.Collections.Generic;
using System.Web;

using CMS.Activities;

namespace Kentico.Ecommerce.Tests.Fakes
{
    internal class ActivityLogServiceFake : IActivityLogService
    {
        public IList<IActivityInfo> LoggedActivities
        {
            get;
            private set;
        }


        public ActivityLogServiceFake()
        {
            LoggedActivities = new List<IActivityInfo>();
        }


        public void Log(IActivityInitializer activityInitializer, HttpRequestBase currentRequest, bool loggingDisabledInAdministration = true)
        {
            var activity = new Activity();
            activity.ActivityType = activityInitializer.ActivityType;
            activityInitializer.Initialize(activity);
            LoggedActivities.Add(activity);
        }


        public void LogWithoutModifiersAndFilters(IActivityInitializer activityInitializer)
        {
            
        }


        public void RegisterFilter(IActivityLogFilter filter)
        {
            
        }


        public void RegisterModifier(IActivityModifier activityModifier)
        {
            
        }


        public void RegisterValidator(IActivityLogValidator activityLogValidator)
        {
            
        }

        private class Activity : IActivityInfo
        {
            public string ActivityCampaign
            {
                get;
                set;
            }


            public string ActivityUTMSource
            {
                get;
                set; 
            }


            public string ActivityUTMContent
            {
                get;
                set;
            }


            public int ActivityItemDetailID
            {
                get;
                set;
            }


            public int ActivityID
            {
                get;
                set;
            }


            public int ActivityContactID
            {
                get;
                set;
            }


            public Guid ActivityContactGUID
            {
                get;
                set;
            }


            public int ActivityNodeID
            {
                get;
                set;
            }


            public Guid ActivityGUID
            {
                get;
                set;
            }


            public string ActivityTitle
            {
                get;
                set;
            }


            public string ActivityType
            {
                get;
                set;
            }


            public string ActivityValue
            {
                get;
                set;
            }


            public string ActivityURL
            {
                get;
                set;
            }


            public DateTime ActivityCreated
            {
                get;
                set;
            }


            public int ActivitySiteID
            {
                get;
                set;
            }


            public int ActivityItemID
            {
                get;
                set;
            }


            public string ActivityIPAddress
            {
                get;
                set;
            }


            public string ActivityURLReferrer
            {
                get;
                set;
            }


            public string ActivityComment
            {
                get;
                set;
            }


            public string ActivityCulture
            {
                get;
                set;
            }


            public string ActivityABVariantName
            {
                get;
                set;
            }


            public string ActivityMVTCombinationName
            {
                get;
                set;
            }


            public long ActivityURLHash
            {
                get;
                set;
            }
        }
    }
}