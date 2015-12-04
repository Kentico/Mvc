using System;

namespace DancingGoat.Models.Pager
{
    public class PagerModel
    {
        public IPagedDataSource DataSource
        {
            get;
            set;
        }


        public Func<int, string> CreateUrlForPageIndex
        {
            get; 
            set;
        }
    }
}