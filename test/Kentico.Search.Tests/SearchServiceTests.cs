using System;
using System.Data;
using System.Linq;
using NUnit.Framework;

using CMS.Search;
using CMS.Tests;
using CMS.DocumentEngine;
using CMS.Membership;
using CMS.Helpers;

namespace Kentico.Search.Tests
{
    [TestFixture]
    public class SearchServiceTests : UnitTests
    {
        [SetUp]
        public void SetUp()
        {
            MembershipContext.AuthenticatedUser = new CurrentUserInfo();
        }


        [TearDown]
        public void TearDown()
        {
            MembershipContext.AuthenticatedUser = null;
        }


        [TestCase("index1")]
        [TestCase("index1;index2")]
        [Description("Test whether index is passed correctly to the SearchParameters object.")]
        public void Search_CheckParameters_Indexes(string searchIndexes)
        {
            var parameters = Search(indexes: searchIndexes).Parameters;

            CMSAssert.All(
                () => Assert.AreEqual(parameters.SearchIndexes, searchIndexes)
            );
        }


        [TestCase("query", "en-us", "en-us", true)]
        [TestCase("query", "en-us", "en-us", false)]
        [TestCase("query", "cs-cz", "en-us", true)]
        [TestCase("query", "cs-cz", "en-us", false)]
        [TestCase("query", "en-us", "cs-cz", true)]
        [TestCase("query", "en-us", "cs-cz", false)]
        [Description("Test whether search query, culture, default culture and combineWithDefaultCulture parameters are passed correctly to the SearchParameters object.")]
        public void Search_CheckParameters_Query(string searchQuery, string culture, string defaultCulture, bool combineWithDefaultCulture)
        {
            var parameters = Search(
                query: searchQuery,
                culture: culture,
                defaultCulture: defaultCulture,
                combineWithDefaultCulture: combineWithDefaultCulture
            )
            .Parameters;

            var documentCondition = new DocumentSearchCondition(null, culture, defaultCulture, combineWithDefaultCulture);
            var condition = new SearchCondition(documentCondition: documentCondition);
            var searchExpression = SearchSyntaxHelper.CombineSearchCondition(searchQuery, condition);

            CMSAssert.All(
                () => Assert.AreEqual(parameters.SearchFor, searchExpression),
                () => Assert.AreEqual(parameters.CurrentCulture, culture),
                () => Assert.AreEqual(parameters.DefaultCulture, defaultCulture),
                () => Assert.AreEqual(parameters.CombineWithDefaultCulture, combineWithDefaultCulture)
            );
        }


        [TestCase(0, 3)]
        [TestCase(1, 3)]
        [TestCase(2, 3)]
        [Description("Test whether paging parameters are passed correctly to the SearchParameters object.")]
        public void Search_CheckParameters_Paging(int page, int pageSize)
        {
            var parameters = Search(
                page: page,
                pageSize: pageSize
            )
            .Parameters;

            CMSAssert.All(
                () => Assert.AreEqual(parameters.StartingPosition, page * pageSize),
                () => Assert.AreEqual(parameters.DisplayResults, pageSize)
            );
        }


        [Test]
        [Description("Test whether parameters which have their values predefined are passed correctly to the SearchParameters object.")]
        public void Search_CheckParameters_DefaultValues()
        {
            var parameters = Search().Parameters;

            CMSAssert.All(
                () => Assert.AreEqual(parameters.Path, "/%"),
                () => Assert.AreEqual(parameters.ClassNames, null),
                () => Assert.AreEqual(parameters.CheckPermissions, false),
                () => Assert.AreEqual(parameters.SearchInAttachments, false),
                () => Assert.AreEqual(parameters.NumberOfProcessedResults, 100),
                () => Assert.AreEqual(parameters.User, MembershipContext.AuthenticatedUser)
            );
        }


        [TestCase("")]
        [TestCase(null)]
        public void Search_EmptyQuery_ReturnsNull(string query)
        {
            var searchResults = Search(query: query);

            CMSAssert.All(
                () => Assert.IsNull(searchResults.RawResults),
                () => Assert.AreEqual(0, searchResults.NumberOfResults));
        }


        [Test]
        public void Search_CustomTableIndex_ReturnsAllData()
        {
            var searchResults = Search(resultsXmlPath: @".\Data\SearchResults_CustomTable.xml");
            var results = searchResults.Results;
            var fakeResultRows = searchResults.RawResults.Tables[0].AsEnumerable();

            CMSAssert.All(
                // Test that returned result collection is equal to the fake collection
                () => Assert.That(results.Select(x => x.Title), Is.EqualTo(fakeResultRows.Select(r => r["title"]))),
                () => Assert.That(results.Select(x => x.Content), Is.EqualTo(fakeResultRows.Select(r => r["content"]))),
                () => Assert.That(results.Select(x => x.Date.ToString()), Is.EqualTo(fakeResultRows.Select(r => r["created"]))),
                () => Assert.That(results.Select(x => x.ObjectType), Is.EqualTo(fakeResultRows.Select(r => r["type"]))),
                () => Assert.AreEqual(searchResults.NumberOfResults, results.Count()));
        }


        [Test]
        public void Search_PagesIndex_ReturnsAllData()
        {
            Fake<AttachmentInfo, AttachmentInfoProvider>().WithData();

            var searchResults = Search(resultsXmlPath: @".\Data\SearchResults_PagesWithImage.xml");
            var results = searchResults.Results;
            var fakeResultRows = searchResults.RawResults.Tables[0].AsEnumerable();

            CMSAssert.All(
                // Test that returned result collection is equal to the fake collection
                () => Assert.That(results.Select(x => x.Title), Is.EqualTo(fakeResultRows.Select(r => r["title"]))),
                () => Assert.That(results.Select(x => x.Content), Is.EqualTo(fakeResultRows.Select(r => r["content"]))),
                () => Assert.That(results.Select(x => x.Date.ToString()), Is.EqualTo(fakeResultRows.Select(r => r["created"]))),
                () => Assert.That(results.Select(x => x.ImageGuid.ToString()), Is.EqualTo(fakeResultRows.Select(r => r["image"]))),
                () => Assert.That(results.Select(x => x.ImageAttachment.GUID.ToString()), Is.EqualTo(fakeResultRows.Select(r => r["image"]))),
                () => Assert.That(results.Select(x => x.NodeId), Is.EqualTo(fakeResultRows.Select(r => r["nodeid"]))),
                () => Assert.That(results.Select(x => x.ObjectType), Is.EqualTo(fakeResultRows.Select(r => r["type"]))),
                () => Assert.That(results.Select(x => x.PageTypeCodeName), Is.EqualTo(fakeResultRows.Select(r => r["ClassName"]))),
                () => Assert.That(results.Select(x => x.PageTypeDispayName), Is.EqualTo(fakeResultRows.Select(r => r["ClassDisplayName"]))),
                () => Assert.AreEqual(searchResults.NumberOfResults, results.Count()));
        }


        [Test]
        public void Search_PagesIndex_ReturnsDataWithoutAttachment()
        {
            Fake<AttachmentInfo, AttachmentInfoProvider>().WithData();

            var searchResults = Search(resultsXmlPath: @".\Data\SearchResults_PagesWithoutImage.xml");
            var results = searchResults.Results;
            var fakeResultRows = searchResults.RawResults.Tables[0].AsEnumerable();

            Attachment emptyAttachment = null;

            CMSAssert.All(
                // Test that returned result collection is equal to the fake collection
                () => Assert.That(results.Select(x => x.Title), Is.EqualTo(fakeResultRows.Select(r => r["title"]))),
                () => Assert.That(results.Select(x => x.Content), Is.EqualTo(fakeResultRows.Select(r => r["content"]))),
                () => Assert.That(results.Select(x => x.Date.ToString()), Is.EqualTo(fakeResultRows.Select(r => r["created"]))),
                () => Assert.That(results.Select(x => x.ImageGuid), Is.EqualTo(fakeResultRows.Select(r => Guid.Empty))),
                () => Assert.That(results.Select(x => x.ImageAttachment), Is.EqualTo(fakeResultRows.Select(r => emptyAttachment))),
                () => Assert.That(results.Select(x => x.NodeId), Is.EqualTo(fakeResultRows.Select(r => r["nodeid"]))),
                () => Assert.That(results.Select(x => x.ObjectType), Is.EqualTo(fakeResultRows.Select(r => r["type"]))),
                () => Assert.That(results.Select(x => x.PageTypeCodeName), Is.EqualTo(fakeResultRows.Select(r => r["ClassName"]))),
                () => Assert.That(results.Select(x => x.PageTypeDispayName), Is.EqualTo(fakeResultRows.Select(r => r["ClassDisplayName"]))),
                () => Assert.AreEqual(searchResults.NumberOfResults, results.Count()));
        }


        /// <summary>
        /// Performs search using the fake search service and populating default values to make sure that search works correctly when modifying just some parameters.
        /// </summary>
        private static TestSearchResults Search(
            string indexes = "index1;index2",
            string culture = "en-us",
            string defaultCulture = "en-us",
            string siteName = "site",
            bool combineWithDefaultCulture = true,
            string query = "query",
            int page = 0,
            int pageSize = 5,
            string resultsXmlPath = null
            )
        {
            var testResults = new TestSearchResults();
            int numberOfResults;

            // Mock the method CultureHelper.GetDefaultCultureCode()
            CultureHelper.HelperObject = new FakeCultureHelper(defaultCulture);

            var searchService = new FakeSearchService(indexes.Split(';'), culture, siteName, combineWithDefaultCulture, resultsXmlPath);
            testResults.Results = searchService.Search(query, page, pageSize, out numberOfResults);
            testResults.NumberOfResults = numberOfResults;
            testResults.RawResults = searchService.RawResults;
            testResults.Parameters = searchService.Parameters;

            CultureHelper.HelperObject = null;

            return testResults;
        }
    }
}
