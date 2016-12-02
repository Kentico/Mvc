using System;
using System.Data;
using System.Linq;

using CMS.Base;
using CMS.DataEngine;
using CMS.Search;
using CMS.WebAnalytics;

namespace Kentico.Search.Tests
{
    public class FakeSearchService : SearchService
    {
        private readonly string mDataSetFilePath;


        public DataSet RawResults => rawResults;


        public SearchParameters Parameters
        {
            get;
            private set;
        }


        public FakeSearchService(string dataSetFilePath, PagesActivityLogger pagesActivityLogger)
        {
            mDataSetFilePath = dataSetFilePath;
            mPagesActivityLogger = pagesActivityLogger;
        }


        /// <summary>
        /// Mocks Kentico Smart search results.
        /// </summary>
        internal override DataSet SearchInternal(SearchParameters parameters)
        {
            Parameters = parameters;
            rawResults = GetFakeSearchResults();

            if (rawResults != null)
            {
                SearchContext.CurrentSearchResults = new SafeDictionary<string, DataRow>(rawResults.Tables[0].AsEnumerable().ToDictionary(x => x["id"]));
                parameters.NumberOfResults = SearchContext.CurrentSearchResults.Count;
            }

            return rawResults;
        }


        internal override string GetImagePath(string objectType, string id, string image)
        {
            return image;
        }


        internal override BaseInfo GetDataObject(string id, string objectType)
        {
            return null;
        }


        private DataSet GetFakeSearchResults()
        {
            if (String.IsNullOrEmpty(mDataSetFilePath))
            {
                return null;
            }

            DataSet ds = new DataSet();
            ds.ReadXml(mDataSetFilePath, XmlReadMode.InferTypedSchema);
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
