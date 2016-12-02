using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Personas;

using DancingGoat.Models.Articles;

namespace DancingGoat.Models.Home
{
    public class IndexViewModel
    {
        public BannerViewModel Banner { get; set; }


        public IEnumerable<ArticleViewModel> LatestArticles { get; set; }


        public IEnumerable<HomeSectionViewModel> HomeSections { get; set; }


        public IEnumerable<Cafe> CompanyCafes { get; set; }
    }
}