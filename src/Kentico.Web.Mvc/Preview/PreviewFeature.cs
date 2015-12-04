using CMS.Helpers;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Represents a feature that provides information about preview for the current request.
    /// </summary>
    internal sealed class PreviewFeature : IPreviewFeature
    {
        private readonly bool mEnabled;
        private readonly string mCultureName;


        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewFeature"/> class from the Kentico virtual context.
        /// </summary>
        public PreviewFeature()
        {
            mEnabled = VirtualContext.IsPreviewLinkInitialized;
            if (mEnabled)
            {
                mCultureName = GetVirtualContextItem<string>("Culture");
            }
        }


        /// <summary>
        /// Gets a value indicating whether preview is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return mEnabled;
            }
        }


        /// <summary>
        /// Gets the culture name in the format languagecode2-country/regioncode2.
        /// </summary>
        /// <value>The culture name in the format languagecode2-country/regioncode2, if preview information with culture is available; otherwise, null.</value>
        public string CultureName
        {
            get
            {
                return mCultureName;
            }
        }


        private T GetVirtualContextItem<T>(string key)
        {
            object value = VirtualContext.GetItem(key);
            if (value != null)
            {
                return (T)value;
            }

            return default(T);
        }
    }
}
