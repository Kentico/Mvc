using System.Web.Mvc;

using CMS.DocumentEngine.Types;

namespace DancingGoat.Helpers
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Generates a fully qualified URL to the action method handling the detail of given article.
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="article">Article object to generate URL for.</param>
        public static string ForArticle(this UrlHelper urlHelper, Article article)
        {
            return urlHelper.Action("Show", "Articles", new
            {
                id = article.NodeID,
                pageAlias = article.NodeAlias
            });
        }
    }
}