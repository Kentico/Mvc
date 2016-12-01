using System.Collections.Generic;

namespace LearningKit.Models.Search
{
    //DocSection:SearchResultModel
    public class SearchResultModel
    {
        public string Query { get; set; }
        
        public IEnumerable<SearchResultItemModel> Items { get; set; }
    }
    //EndDocSection:SearchResultModel
}