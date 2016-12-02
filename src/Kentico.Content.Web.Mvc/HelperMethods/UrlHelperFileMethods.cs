using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using Kentico.Web.Mvc;

namespace Kentico.Content.Web.Mvc
{
    /// <summary>
    /// Provides methods to generate fully qualified URLs to files.
    /// </summary>
    public static class UrlHelperFileMethods
    {
        /// <summary>
        /// Generates a fully qualified URL to the specified image path.
        /// </summary>
        /// <param name="instance">The object that provides methods to build URLs to Kentico content.</param>
        /// <param name="path">The virtual path of the image.</param>
        /// <param name="constraint">The size constraint that is enforced on image when resizing.</param>
        /// <returns>The fully qualified URL to the image.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is null, empty or contains query string parameters.</exception>
        public static string ImageUrl(this ExtensionPoint<UrlHelper> instance, string path, SizeConstraint constraint)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("The image path needs to be provided.");
            }

            var absolutePath = instance.Target.Content(path);
            var queryString = GetSizeConstraintQueryString(constraint);

            return BuildUrl(absolutePath, queryString);
        }


        /// <summary>
        /// Generates a fully qualified URL to the specified file path.
        /// </summary>
        /// <param name="instance">The object that provides methods to build URLs to Kentico content.</param>
        /// <param name="path">The virtual path of the file.</param>
        /// <param name="options">Options that affect the file URL.</param>
        /// <returns>The fully qualified URL to the file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is null, empty or contains query string parameters.</exception>
        public static string FileUrl(this ExtensionPoint<UrlHelper> instance, string path, FileUrlOptions options)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("The file path needs to be provided.");
            }

            var absolutePath = instance.Target.Content(path);
            var queryString = GetUrlOptionsQueryString(options);

            return BuildUrl(absolutePath, queryString);
        }


        private static string GetSizeConstraintQueryString(SizeConstraint constraint)
        {
            var sizeConstraintParameters = new NameValueCollection();
            sizeConstraintParameters.AddSizeConstraint(constraint);

            return sizeConstraintParameters.ToQueryString();
        }


        private static string GetUrlOptionsQueryString(FileUrlOptions options)
        {
            if ((options == null) || !options.AttachmentContentDisposition)
            {
                return String.Empty;
            }
            var queryStringParameters = new NameValueCollection { { "disposition", "attachment" } };

            return queryStringParameters.ToQueryString();
        }


        private static string BuildUrl(string absolutePath, string query)
        {
            if (String.IsNullOrEmpty(query))
            {
                return absolutePath;
            }

            if (absolutePath.Contains("?"))
            {
                query = query.Replace("?", "&");
            }

            return absolutePath + query;
        }
    }
}
