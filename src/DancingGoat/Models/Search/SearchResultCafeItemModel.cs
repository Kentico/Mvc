using System.Web;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultCafeItemModel : SearchResultPageItemModel
    {
        public SearchResultCafeItemModel(SearchFields fields, Cafe cafe)
            :base(fields, cafe)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Url = urlHelper.Action("Index", "Cafes");
        }
    }
}