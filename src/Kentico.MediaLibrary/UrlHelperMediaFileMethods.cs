using System;
using System.Web.Mvc;

using CMS.Core;
using Kentico.Web.Mvc;
using Kentico.Web.Mvc.Internal;

namespace Kentico.MediaLibrary
{
    /// <summary>
    /// Provides methods to generate fully qualified URLs to media files.
    /// </summary>
    public static class UrlHelperMediaFileMethods
    {
        /// <summary>
        /// Generates a fully qualified URL to the specified media file.
        /// </summary>
        /// <param name="instance">The object that provides methods to build URLs to Kentico content.</param>
        /// <param name="mediaFileGuid">The media file GUID.</param>
        /// <param name="siteName">The site's code name.</param>
        /// <returns>The fully qualified URL to the image.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> or <paramref name="siteName"/> is null.</exception>
        public static string MediaFile(this ExtensionPoint<UrlHelper> instance, Guid mediaFileGuid, string siteName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (string.IsNullOrEmpty(siteName))
            {
                throw new ArgumentNullException(nameof(siteName));
            }

            return GenerateMediaFileUrl(instance, mediaFileGuid, siteName, SizeConstraint.Empty);
        }


        /// <summary>
        /// Generates a fully qualified URL to the specified media file.
        /// </summary>
        /// <param name="instance">The object that provides methods to build URLs to Kentico content.</param>
        /// <param name="mediaFileGuid">The media file GUID.</param>
        /// <param name="siteName">The site's code name.</param>
        /// <param name="constraint">The size constraint that is enforced on image when resizing.</param>
        /// <returns>The fully qualified URL to the image.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> or <paramref name="siteName"/> is null.</exception>
        public static string MediaFile(this ExtensionPoint<UrlHelper> instance, Guid mediaFileGuid, string siteName, SizeConstraint constraint)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (string.IsNullOrEmpty(siteName))
            {
                throw new ArgumentNullException(nameof(siteName));
            }

            return GenerateMediaFileUrl(instance, mediaFileGuid, siteName, constraint);
        }


        private static string GenerateMediaFileUrl(this ExtensionPoint<UrlHelper> instance, Guid mediaFileGuid, string siteName, SizeConstraint constraint)
        {
            var mediaFileUrlProvider = Service<IMediaFileUrlProvider>.Entry();

            var url = mediaFileUrlProvider.GetMediaFileUrl(mediaFileGuid, siteName);
            var queryStringBuilder = new QueryStringBuilder().AppendSizeConstraint(constraint);
            url += queryStringBuilder.ToString();

            return instance.Target.Content(url);
        }
    }
}