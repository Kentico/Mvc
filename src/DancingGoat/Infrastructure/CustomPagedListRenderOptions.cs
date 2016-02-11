using PagedList.Mvc;

using CMS.Helpers;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Customized render option definitions for X.PagedList.
    /// </summary>
    /// <remarks>
    /// For more customization options see https://github.com/kpi-ua/X.PagedList/blob/master/src/X.PagedList.Mvc/PagedListRenderOptions.cs.
    /// </remarks>
    public class CustomPagedListRenderOptions : PagedListRenderOptions
    {
        public CustomPagedListRenderOptions() : base()
        {
            LinkToPreviousPageFormat = ResHelper.GetString("General.Previous").ToLower();
            LinkToNextPageFormat = ResHelper.GetString("General.Next").ToLower();
        }
    }
}
