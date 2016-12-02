using CMS.DataEngine;

namespace Kentico.ContactManagement.Tests
{
    public class LicenseServiceFake : ILicenseService
    {
        public bool HasLicence
        {
            get;
            set;
        }


        public LicenseServiceFake()
        {
            HasLicence = true;
        }


        /// <summary>
        /// Checks the license based on feature and perform action based on given arguments.
        /// </summary>
        /// <param name="feature">Feature to check</param>
        /// <param name="domain">Domain to check. If null, function tries to get domain from HttpContext</param>
        /// <param name="throwError">Indicates whether throw error after false check</param>
        /// <returns>
        /// True if not changed through <see cref="LicenseServiceFake.HasLicence"/> property.
        /// </returns>
        public bool CheckLicense(FeatureEnum feature, string domain = null, bool throwError = true)
        {
            return HasLicence;
        }


        public bool IsFeatureAvailable(FeatureEnum feature, string domain = null)
        {
            return HasLicence;
        }
    }
}
