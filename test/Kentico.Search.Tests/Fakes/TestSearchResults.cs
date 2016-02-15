using System.Collections.Generic;
using System.Data;

using CMS.Search;

namespace Kentico.Search.Tests
{
    public class TestSearchResults
    {
        public DataSet RawResults
        {
            get;
            set;
        }


        public IEnumerable<SearchResultItem> Results
        {
            get;
            set;
        }


        public int NumberOfResults
        {
            get;
            set;
        }


        public SearchParameters Parameters
        {
            get;
            set;
        }
    }
}
