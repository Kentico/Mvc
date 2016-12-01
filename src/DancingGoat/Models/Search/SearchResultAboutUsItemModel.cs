using System.Web;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultAboutUsItemModel : SearchResultPageItemModel
    {
        public SearchResultAboutUsItemModel(SearchFields fields, AboutUs aboutUs) :base(fields, aboutUs)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Url = urlHelper.Action("Index", "About");
        }
    }
}