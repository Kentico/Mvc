using System;

namespace CMS.DocumentEngine.Types 
{
    /// <summary>
    /// Custom Article members.
    /// </summary>
    public partial class Article
    {
        public DateTime PublicationDate
        {
            get
            {
                return GetDateTimeValue("DocumentPublishFrom", GetDateTimeValue("DocumentCreatedWhen", DateTime.MinValue));
            }
        }
    }
}