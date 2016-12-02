using System;

using CMS.Helpers;

using Kentico.Search;

namespace DancingGoat.Models.Search
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
            Content = HTMLHelper.StripTags(fields.Content, false);
            Date = fields.Date;
            ImagePath = fields.ImagePath;
            ObjectType = fields.ObjectType;
        }
    }
}