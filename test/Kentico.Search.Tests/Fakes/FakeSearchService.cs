using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Search;

namespace Kentico.Search.Tests
{
    public class FakeSearchService : SearchService
    {
        private string _dataSetFilePath = null;


        public DataSet RawResults
        {
            get
            {
                return mRawResults;
            }
        }


        public SearchParameters Parameters
        {
            get;
            private set;
        }


        public FakeSearchService(string[] indexes, string culture, string sitename, bool combineWithDefaultCulture, string dataSetFilePath)
            : base(indexes, culture, sitename, combineWithDefaultCulture)
        {
            _dataSetFilePath = dataSetFilePath;
        }


        /// <summary>
        /// Mocks Kentico Smart search results.
        /// </summary>
        internal override DataSet Search(SearchParameters parameters)
        {
            Parameters = parameters;
            mRawResults = GetFakeSearchResults();

            if (mRawResults != null)
            {
                SearchContext.CurrentSearchResults = new SafeDictionary<string, DataRow>(mRawResults.Tables[0].AsEnumerable().ToDictionary(x => x["id"]));
                parameters.NumberOfResults = SearchContext.CurrentSearchResults.Count;
            }

            return mRawResults;
        }


        /// <summary>
        /// Gets mock collection of attachments.
        /// </summary>
        internal override Dictionary<Guid, AttachmentInfo> GetPageAttachments(ICollection<Guid> attachmentGuids)
        {
            var attachments = new List<AttachmentInfo>();

            foreach (Guid guid in attachmentGuids)
            {
                attachments.Add(new AttachmentInfo() { AttachmentGUID = guid });
            }

            return attachments.ToDictionary(x => x.AttachmentGUID);
        }


        /// <summary>
        /// Gets the fake search results dataset from the specified XML file.
        /// </summary>
        private DataSet GetFakeSearchResults()
        {
            if (String.IsNullOrEmpty(_dataSetFilePath))
            {
                return null;
            }

            DataSet ds = new DataSet();
            ds.ReadXml(_dataSetFilePath, XmlReadMode.InferTypedSchema);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                // Empty image GUIDs are represented in the original Smart search dataset by DBNull
                if (row["image"].ToString() == "")
                {
                    row["image"] = DBNull.Value;
                }
            }

            return ds;
        }
    }
}
