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

        public SearchResult Result
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
