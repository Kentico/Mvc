using Kentico.Search;
using System;

namespace LearningKit.Models.Search
{
    public class SearchResultItemModel
    {
        public string Title { get; set; }
        
        public string Content { get; set; }
        
        public DateTime Date { get; set; }
        
        public string ObjectType { get; set; }
        
        public string ImagePath { get; set; }
        
        public string Url { get; set; }
        
        public SearchResultItemModel(SearchFields fields)
        {
            Title = fields.Title;
            Content = fields.Content;
            Date = fields.Date;
            ImagePath = fields.ImagePath;
            ObjectType = fields.ObjectType;
        }
    }
}