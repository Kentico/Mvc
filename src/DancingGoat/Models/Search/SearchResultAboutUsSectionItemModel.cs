using System.Web;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultAboutUsSectionItemModel : SearchResultPageItemModel
    {
        public SearchResultAboutUsSectionItemModel(SearchFields fields, AboutUsSection aboutUsSection)
            :base(fields, aboutUsSection)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Url = urlHelper.Action("Index", "About");
        }
    }
}