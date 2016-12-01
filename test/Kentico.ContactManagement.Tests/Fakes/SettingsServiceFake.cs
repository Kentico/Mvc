using CMS.Core;

namespace Kentico.ContactManagement.Tests
{
    public class SettingsServiceFake : ISettingsService
    {
        /// <summary>
        /// Checks the specific settings. Is always true.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns>Always true</returns>
        public string this[string keyName] => "true";


        /// <summary>
        /// Checks if the settings service is available. Is always true.
        /// </summary>
        /// <returns>Always true</returns>
        public bool IsAvailable => true;
    }
}
