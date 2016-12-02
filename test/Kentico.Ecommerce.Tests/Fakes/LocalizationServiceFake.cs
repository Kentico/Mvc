using System;

using CMS.Core;

namespace Kentico.Ecommerce.Tests.Fakes
{
    public class LocalizationServiceFake : ILocalizationService
    {
        public string GetString(string resourceKey, string culture = null, bool useDefaultCulture = true)
        {
            return "{0}";
        }


        public string GetFileString(string resourceKey, string culture = null, bool useDefaultCulture = true)
        {
            return "{0}";
        }


        public string GetAPIString(string resourceKey, string culture, string defaultValue)
        {
            return "{0}";
        }


        public string LocalizeExpression(string expression, string culture = null, bool encode = false, Func<string, string, bool, string> getStringMethod = null, bool useDefaultCulture = true)
        {
            return "{0}";
        }


        public void ClearCache(bool logTasks)
        {
            
        }
    }
}