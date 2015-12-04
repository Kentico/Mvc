using System.Text;
using System.Web.Mvc;

using CMS.Base;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides methods to resolve relative URLs in HTML fragments.
    /// </summary>
    public static class HtmlHelperResolveUrlsMethods
    {
        /// <summary>
        /// Resolves relative URLs in the HTML fragment so that they can be used by the browser.
        /// </summary>
        /// <param name="instance">The object that provides methods to render HTML fragments.</param>
        /// <param name="html">An HTML fragment with potential relative URLs.</param>
        /// <returns>An HTML fragment where relative URLs are replaced with URLs that can be used by the browser.</returns>
        public static MvcHtmlString ResolveUrls(this ExtensionPoint<HtmlHelper> instance, string html)
        {
            var urlHelper = new UrlHelper(instance.Target.ViewContext.RequestContext);
            var applicationPath = urlHelper.Content("~/").TrimEnd('/');

            var pathIndex = html.IndexOfCSafe("~/");
            if (pathIndex >= 1)
            {
                var builder = new StringBuilder((int)(html.Length * 1.1));
                var lastIndex = 0;
                
                while (pathIndex >= 1)
                {
                    if ((html[pathIndex - 1] == '(') || (html[pathIndex - 1] == '"') || (html[pathIndex - 1] == '\''))
                    {
                        // Add previous content
                        if (lastIndex < pathIndex)
                        {
                            builder.Append(html, lastIndex, pathIndex - lastIndex);
                        }

                        // Add application path and move to the next location
                        builder.Append(applicationPath);
                        lastIndex = pathIndex + 1;
                    }

                    pathIndex = html.IndexOfCSafe("~/", pathIndex + 2);
                }

                // Add the rest of the content
                if (lastIndex < html.Length)
                {
                    builder.Append(html, lastIndex, html.Length - lastIndex);
                }

                html = builder.ToString();
            }

            return new MvcHtmlString(html);
        }
    }
}
