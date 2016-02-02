using System.Collections.Generic;

using CMS.DocumentEngine.Types;

using DancingGoat.Models.Articles;

namespace DancingGoat.Models.Home
{
    public class IndexViewModel
    {
        public IEnumerable<ArticleViewModel> LatestArticles 
        {
            get;
            set;
        }


        public IEnumerable<Cafe> CompanyCafes
        {
            get;
            set;
        }
        

        public string OurStory
        {
            get;
            set;
        }
    }
}