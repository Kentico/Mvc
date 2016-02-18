using CMS.Helpers;

namespace Kentico.Search.Tests
{
    public class FakeCultureHelper : CultureHelper
    {
        public FakeCultureHelper(string defaultCulture)
        {
            DefaultUICultureCodeInternal = defaultCulture;
        }
    }
}
