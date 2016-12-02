using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

using CMS.Activities;
using CMS.Core;

using NUnit.Framework;

using CMS.Search;
using CMS.Tests;
using CMS.Membership;
using CMS.Helpers;
using CMS.IO;

namespace Kentico.Search.Tests
{
    [TestFixture]
    public class SearchServiceTests : UnitTests
    {
        [SetUp]
        public void SetUp()
        {
            ObjectFactory<IActivityLogService>.SetObjectTypeTo<ActivityLogServiceFake>();
            Fake<UserInfo, UserInfoProvider>()
                .WithData(new UserInfo
                {
                    UserName = "public"
                });

            // Set current thread's culture to English in order the test to be independent on environment where it runs. 
            // Thread's culture influences, for example, how dates are converted to string and vice versa. Test data stored in XML files contains dates in English format.
            FakeCurrentCulture();
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
            var parameters = PageSearch(indexes: searchIndexes).Parameters;

            Assert.AreEqual(parameters.SearchIndexes, searchIndexes);
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
            var parameters = PageSearch(
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


        [TestCase(1, 3)]
        [TestCase(2, 3)]
        [TestCase(3, 3)]
        [Description("Test whether paging parameters are passed correctly to the SearchParameters object.")]
        public void Search_CheckParameters_Paging(int pageNumber, int pageSize)
        {
            var parameters = PageSearch(
                pageNumber: pageNumber,
                pageSize: pageSize
            )
            .Parameters;

            CMSAssert.All(
                () => Assert.AreEqual(parameters.StartingPosition, (pageNumber - 1) * pageSize),
                () => Assert.AreEqual(parameters.DisplayResults, pageSize)
            );
        }


        [Test]
        [Description("Test whether parameters which have their values predefined are passed correctly to the SearchParameters object.")]
        public void Search_CheckParameters_DefaultValues()
        {
            var parameters = PageSearch().Parameters;

            CMSAssert.All(
                () => Assert.AreEqual(parameters.Path, "/%"),
                () => Assert.AreEqual(parameters.ClassNames, null),
                () => Assert.AreEqual(parameters.CheckPermissions, false),
                () => Assert.AreEqual(parameters.SearchInAttachments, false),
                () => Assert.AreEqual(parameters.NumberOfProcessedResults, 100),
                () => Assert.AreEqual(parameters.User, MembershipContext.AuthenticatedUser)
            );
        }


        [Test]
        public void Search_NoOptions_ThrowsException()
        {
            var searchService = new FakeSearchService(null, new FakePagesActivityLogger());

            Assert.That(() => searchService.Search(null),
                              Throws.Exception.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void Search_ValidSearchOptions_ReturnsNoData()
        {
            var searchResults = PageSearch();
            var results = searchResults.Result;

            CMSAssert.All(
                // Test that results.Items is an empty collection, cannot be null.
                () => Assert.That(results.Items, Is.Empty),
                () => Assert.AreEqual(results.TotalNumberOfResults, 0));
        }


        [Test]
        public void Search_CustomTableIndex_ReturnsAllData()
        {
            var currentFolder = Path.GetDirectoryName(GetType().Assembly.Location);
            var xmlFilePath = Path.Combine(currentFolder, @".\Data\SearchResults_CustomTable.xml");

            var searchResults = GeneralSearch(
                                    service: new FakeSearchService(xmlFilePath, new FakePagesActivityLogger()),
                                    searchIndexNames: new[] { "index" },
                                    query: "query",
                                    culture: "en-us",
                                    combineWithDefaultCulture: true,
                                    pageNumber: 1,
                                    pageSize: 5);

            var results = searchResults.Result.Items;
            var fakeResultRows = searchResults.RawResults.Tables[0].AsEnumerable();

            CMSAssert.All(
                // Test that returned result collection is equal to the fake collection
                () => Assert.That(results.Select(x => x.Fields.Title), Is.EqualTo(fakeResultRows.Select(r => r["title"]))),
                () => Assert.That(results.Select(x => x.Fields.Content), Is.EqualTo(fakeResultRows.Select(r => r["content"]))),
                () => Assert.That(results.Select(x => x.Fields.Date.ToString(CultureHelper.EnglishCulture)), Is.EqualTo(fakeResultRows.Select(r => r["created"]))),
                () => Assert.That(results.Select(x => x.Fields.ObjectType), Is.EqualTo(fakeResultRows.Select(r => r["type"]))),
                () => Assert.AreEqual(searchResults.Result.TotalNumberOfResults, results.Count()));
        }


        [Test]
        public void Search_PagesIndex_ReturnsAllData()
        {
            var currentFolder = Path.GetDirectoryName(GetType().Assembly.Location);
            var searchResults = PageSearch(resultsXmlPath: Path.Combine(currentFolder, @".\Data\SearchResults_PagesWithImage.xml"));
            var results = searchResults.Result.Items;
            var fakeResultRows = searchResults.RawResults.Tables[0].AsEnumerable();

            CMSAssert.All(
                // Test that returned result collection is equal to the fake collection
                () => Assert.That(results.Select(x => x.Fields.Title), Is.EqualTo(fakeResultRows.Select(r => r["title"]))),
                () => Assert.That(results.Select(x => x.Fields.Content), Is.EqualTo(fakeResultRows.Select(r => r["content"]))),
                () => Assert.That(results.Select(x => x.Fields.Date.ToString(CultureHelper.EnglishCulture)), Is.EqualTo(fakeResultRows.Select(r => r["created"]))),
                () => Assert.That(results.Select(x => x.Fields.ImagePath), Is.EqualTo(fakeResultRows.Select(r => r["image"]))),
                () => Assert.That(results.Select(x => x.Fields.ObjectType), Is.EqualTo(fakeResultRows.Select(r => r["type"]))),
                () => Assert.AreEqual(searchResults.Result.TotalNumberOfResults, results.Count()));
        }


        [Test]
        public void Search_PagesIndex_ReturnsDataWithoutAttachment()
        {
            var currentFolder = Path.GetDirectoryName(GetType().Assembly.Location);
            var searchResults = PageSearch(resultsXmlPath: Path.Combine(currentFolder, @".\Data\SearchResults_PagesWithoutImage.xml"));
            var results = searchResults.Result.Items;
            var fakeResultRows = searchResults.RawResults.Tables[0].AsEnumerable();

            CMSAssert.All(
                // Test that returned result collection is equal to the fake collection
                () => Assert.That(results.Select(x => x.Fields.Title), Is.EqualTo(fakeResultRows.Select(r => r["title"]))),
                () => Assert.That(results.Select(x => x.Fields.Content), Is.EqualTo(fakeResultRows.Select(r => r["content"]))),
                () => Assert.That(results.Select(x => x.Fields.Date.ToString(CultureHelper.EnglishCulture)), Is.EqualTo(fakeResultRows.Select(r => r["created"]))),
                () => Assert.That(results.Select(x => x.Fields.ImagePath), Is.EqualTo(fakeResultRows.Select(r => String.Empty))),
                () => Assert.That(results.Select(x => x.Fields.ObjectType), Is.EqualTo(fakeResultRows.Select(r => r["type"]))),
                () => Assert.AreEqual(searchResults.Result.TotalNumberOfResults, results.Count()));
        }


        [Test]
        public void Search_WithQuery_LogsInternalSearchActivity()
        {
            // Arrange
            var logger = (ActivityLogServiceFake)Service.Entry<IActivityLogService>();
            var searchPhrase = "Search phrase";

            // Act
            PageSearch(query: searchPhrase);

            // Assert
            CMSAssert.All(
                () => Assert.That(logger.LoggedActivities, Has.Count.EqualTo(1)),
                () => Assert.That(logger.LoggedActivities[0].ActivityType, Is.EqualTo(PredefinedActivityType.INTERNAL_SEARCH)),
                () => Assert.That(logger.LoggedActivities[0].ActivityValue, Is.EqualTo(searchPhrase))
                );
        }


        /// <summary>
        /// Performs search using the fake search service and populating default values to make sure that search works correctly when modifying just some parameters.
        /// </summary>
        private static TestSearchResults PageSearch(
            string indexes = "index1;index2",
            string culture = "en-us",
            string defaultCulture = "en-us",
            bool combineWithDefaultCulture = true,
            string query = "query",
            int pageNumber = 1,
            int pageSize = 5,
            string resultsXmlPath = null
            )
        {

            // Mock the method CultureHelper.GetDefaultCultureCode()
            CultureHelper.HelperObject = new FakeCultureHelper(defaultCulture);

            var results = GeneralSearch(
                                service: new FakeSearchService(resultsXmlPath, new FakePagesActivityLogger()),
                                searchIndexNames: indexes.Split(';'),
                                query: query,
                                culture: culture,
                                combineWithDefaultCulture: combineWithDefaultCulture,
                                pageNumber: pageNumber,
                                pageSize: pageSize);

            CultureHelper.HelperObject = null;

            return results;
        }


        private static TestSearchResults GeneralSearch(FakeSearchService service, IEnumerable<string> searchIndexNames, string query, string culture, bool combineWithDefaultCulture, int pageNumber, int pageSize)
        {
            var testResults = new TestSearchResults();

            var options = new SearchOptions(query, searchIndexNames)
            {
                CultureName = culture,
                CombineWithDefaultCulture = combineWithDefaultCulture,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var searchResult = service.Search(options);
            testResults.Result = searchResult;
            testResults.RawResults = service.RawResults;
            testResults.Parameters = service.Parameters;

            return testResults;
        }


        private void FakeCurrentCulture()
        {
            Thread.CurrentThread.CurrentCulture = CultureHelper.EnglishCulture;
        }
    }
}
