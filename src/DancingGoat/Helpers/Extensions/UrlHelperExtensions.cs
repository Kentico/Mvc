using System;
using System.Web.Mvc;

using Kentico.Web.Mvc;

using DancingGoat.Models.Articles;

namespace DancingGoat.Helpers
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Generates a fully qualified URL to the action method handling the detail of the given article.
        /// </summary>
        /// <param name="urlHelper">URL Helper</param>
        /// <param name="article">Article model to generate URL for.</param>
        public static string ForArticle(this UrlHelper urlHelper, ArticleViewModel article)
        {
            return urlHelper.Action("Show", "Articles", new
            {
                id = article.NodeID,
                pageAlias = article.NodeAlias
            });
        }
    }
}