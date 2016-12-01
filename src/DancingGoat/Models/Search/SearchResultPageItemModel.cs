using CMS.DataEngine;
using CMS.DocumentEngine;

using Kentico.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultPageItemModel : SearchResultItemModel
    {
        public string Type { get; set; }


        public SearchResultPageItemModel(SearchFields fields, TreeNode treeNode)
            : base(fields)
        {
            var className = treeNode.ClassName;
            var dataClassInfo = DataClassInfoProvider.GetDataClassInfo(className);

            Type = dataClassInfo.ClassDisplayName;
        }
    }
}