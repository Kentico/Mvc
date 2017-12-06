using System.Collections.Generic;
using System.Linq;

using CMS.DataProtection;

namespace LearningKit.Models.PrivacyPage
{
    public class ConsentListingModel
    {
        public IEnumerable<Consent> Consents { get; set; } = Enumerable.Empty<Consent>();
    }
}