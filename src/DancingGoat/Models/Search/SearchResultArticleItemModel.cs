using System.Web;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultArticleItemModel : SearchResultPageItemModel
    {
        public SearchResultArticleItemModel(SearchFields fields, Article article)
            : base(fields, article)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Url = urlHelper.Action("Show", "Articles", new { id = article.NodeID });
        }
    }
}