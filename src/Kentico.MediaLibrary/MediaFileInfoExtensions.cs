using System;

using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.SiteProvider;

namespace Kentico.MediaLibrary
{
    /// <summary>
    /// Provides <see cref="MediaFileInfo"/> extension methods for generating URLs to media files.
    /// </summary>
    public static class MediaFileInfoExtensions
    {
        /// <summary>
        /// Returns a direct URL to the media file.
        /// </summary>
        /// <param name="mediaFile">Media file for which the URL is generated.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="mediaFile"/> is null.</exception>
        /// <remarks>
        /// Generates various URL types depending on the media library location:
        /// <list type="table">
        ///   <listheader>
        ///     <term>URL</term>
        ///     <description>Location</description>
        ///   </listheader>
        ///   <item>
        ///     <term>~/mediaLibraryFolder/filename.extension</term>
        ///     <description>Media file is located in a local media library on the current site.</description>
        ///   </item>
        ///   <item>
        ///     <term>http://domain.com/mediaLibraryFolder/filename.extension</term>
        ///     <description>Media file is located in a local media library on a different site.</description>
        ///   </item>
        ///   <item>
        ///     <term>http://CDN/mediaLibraryFolder/filename.extension</term>
        ///     <description>Media file is located in a media library stored in CDN.</description>
        ///   </item>
        ///   <item>
        ///     <term>~/getmedia/fileGUID/fileName</term>
        ///     <description>Media file is located in a media library which is located outside of the CMS application on the current site.</description>
        ///   </item>
        ///   <item>
        ///     <term>http://domain.com/getmedia/fileGUID/fileName</term>
        ///     <description>Media file is located in a media library which is located outside of the CMS application on a different site.</description>
        ///   </item>
        /// </list>
        /// </remarks>
        public static string GetUrl(this MediaFileInfo mediaFile)
        {
            if (mediaFile == null)
            {
                throw new ArgumentNullException(nameof(mediaFile));
            }

            return GetMediaFileUrl(mediaFile, GetDirectPath);
        }


        /// <summary>
        /// Returns a permanent URL to the media file.
        /// </summary>
        /// <param name="mediaFile">Media file which the URL will be generated for.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="mediaFile" /> is null.</exception>
        /// <remarks>
        /// Generates various URL types depending on the media library location:
        /// <list type="table">
        ///   <listheader>
        ///     <term>URL</term>
        ///     <description>Location</description>
        ///   </listheader>
        ///   <item>
        ///     <term>~/getmedia/fileGUID/fileName</term>
        ///     <description>Media file is located in a media library on the current site.</description>
        ///   </item>
        ///   <item>
        ///     <term>http://domain.com/getmedia/fileGUID/fileName</term>
        ///     <description>Media file is located in a media library on a different site.</description>
        ///   </item>
        /// </list>
        /// </remarks>
        public static string GetPermanentUrl(this MediaFileInfo mediaFile)
        {
            if (mediaFile == null)
            {
                throw new ArgumentNullException(nameof(mediaFile));
            }

            return GetMediaFileUrl(mediaFile, GetPermanentUrl);
        }


        /// <summary>
        /// Returns a direct path for the given media file.
        /// The method can return the following URL types:
        ///   ~/mediaLibraryFolder/filename.extension
        ///   ~/getazurefile.aspx?path=, ~/getmazonfile.aspx?path= ...
        ///   http://CDN/mediaLibraryFolder/filename.extension
        /// </summary>
        private static string GetDirectPath(MediaFileInfo mediaFile, SiteInfo mediaFileSite)
        {
            var mediaLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo(mediaFile.FileLibraryID);

            if ((mediaLibrary != null) && (mediaFileSite != null))
            {
                return MediaFileInfoProvider.GetMediaFileUrl(mediaFileSite.SiteName, mediaLibrary.LibraryFolder, mediaFile.FilePath);
            }

            return null;
        }


        /// <summary>
        /// Returns a permanent URL for the given media file.
        /// </summary>
        private static string GetPermanentUrl(MediaFileInfo mediaFile, SiteInfo mediaFileSite)
        {
            return $"~/getmedia/{mediaFile.FileGUID:D}/{mediaFile.FileName}";
        }


        /// <summary>
        /// Returns a relative or an absolute URL for the given media file depending on the media library location.
        /// Media library locations:
        ///  - current site -> relative URL
        ///  - different site / CDN -> absolute URL
        /// </summary>
        /// <param name="mediaFile">Media file for which the URL is generated.</param>
        /// <param name="getUrlMethod">The method which generates a relative URL for the given <paramref name="mediaFile"/>.</param>
        private static string GetMediaFileUrl(MediaFileInfo mediaFile, Func<MediaFileInfo, SiteInfo, string> getUrlMethod)
        {
            var mediaFileSite = SiteInfoProvider.GetSiteInfo(mediaFile.FileSiteID);

            if (mediaFileSite == null)
            {
                return null;
            }

            string relativeUrl = getUrlMethod(mediaFile, mediaFileSite);

            if (!String.IsNullOrEmpty(relativeUrl))
            {
                bool mediaFileIsOnCurrentSite = (SiteContext.CurrentSiteName == mediaFileSite.SiteName);

                if (!mediaFileIsOnCurrentSite)
                {
                    return GetAbsoluteUrl(relativeUrl, mediaFileSite);
                }

                return relativeUrl;
            }

            return null;
        }


        /// <summary>
        /// Returns an absolute URL for the given relative URL.
        /// </summary>
        private static string GetAbsoluteUrl(string relativeUrl, SiteInfo site)
        {
            if (site.SiteIsContentOnly)
            {
                // Public CDN URLs are already in an absolute form
                if (!relativeUrl.StartsWith("http"))
                {
                    return URLHelper.CombinePath(relativeUrl, '/', site.SitePresentationURL, null);
                }

                return relativeUrl;
            }
            else
            {
                return URLHelper.GetAbsoluteUrl(relativeUrl, site.DomainName);
            }
        }
    }
}
