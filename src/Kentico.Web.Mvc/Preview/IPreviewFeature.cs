namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Represents a feature that provides information about preview for the current request.
    /// </summary>
    public interface IPreviewFeature
    {
        /// <summary>
        /// Gets a value indicating whether preview is enabled.
        /// </summary>
        bool Enabled
        {
            get;
        }
        

        /// <summary>
        /// Gets the culture name in the format languagecode2-country/regioncode2.
        /// </summary>
        /// <value>The culture name in the format languagecode2-country/regioncode2, if preview information with culture is available; otherwise, null.</value>
        string CultureName
        {
            get;
        }
    }
}
