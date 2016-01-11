using CMS.DocumentEngine.Types;
using CMS.Helpers;
using DancingGoat.Helpers;
using DancingGoat.Repositories;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCDemo_TestDemoApp.Helpers
{
    class AzureSearchHelper
    {
        private static SearchServiceClient _searchClient;
        private static SearchIndexClient _indexClient;
        private static string _indexName;

        public static string errorMessage;

        static AzureSearchHelper()
        {
            try
            {
                string searchServiceName = ValidationHelper.GetString(ConfigurationManager.AppSettings["AzureSearchName"], "");
                string apiKey = ValidationHelper.GetString(ConfigurationManager.AppSettings["AzureSearchAPIKey"], "");
                _indexName = ValidationHelper.GetString(ConfigurationManager.AppSettings["AzureSearchIndexName"], "");

                // Create an HTTP reference to the catalog index
                _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
                _indexClient = _searchClient.Indexes.GetClient(_indexName);
            }
            catch (Exception e)
            {
                errorMessage = e.Message.ToString();
            }
        }


        #region IndexActions
        public string CreateIndex()
        {
            // Create the Azure Search index based on the included schema
            try
            {
                //Create the index definition
                var definition = new Index()
                {
                    Name = _indexName,
                    Fields = new[]
                    {
                    new Field ( "DocumentID", DataType.String) { IsKey = true,  IsSearchable = false, IsFilterable = false, IsSortable = false, IsFacetable = false, IsRetrievable = true},
                    new Field ( "NodeAliasPath", DataType.String) { IsKey = false,  IsSearchable = false, IsFilterable = false, IsSortable = false, IsFacetable = false, IsRetrievable = true},
                    new Field ( "QuoteAuthor", DataType.String) { IsKey = false,  IsSearchable = true, IsFilterable = true, IsSortable = true, IsFacetable = true, IsRetrievable = true},
                    new Field ( "QuoteText", DataType.String) { IsKey = false, IsSearchable = true,  IsFilterable = true, IsSortable = true,  IsFacetable = true, IsRetrievable = true},
                    new Field ( "QuoteDate", DataType.String) { IsKey = false, IsSearchable = true,  IsFilterable = true, IsSortable = true,  IsFacetable = true, IsRetrievable = true}
                    }
                };

                definition.Suggesters = new List<Suggester> {

                    new Suggester()
                    {
                        Name = "quoteauthor",
                        SearchMode = SuggesterSearchMode.AnalyzingInfixMatching,
                        SourceFields = new List<string> { "QuoteAuthor" }
                    }
                };

                List<ScoringProfile> lstScoringProfiles = new List<ScoringProfile>();
                TextWeights twauthor = new TextWeights();
                twauthor.Weights.Add("QuoteAuthor", 100);
                TextWeights twtext = new TextWeights();
                twtext.Weights.Add("QuoteText", 100);

                lstScoringProfiles.Add(new ScoringProfile()
                {
                    Name = "QuoteAuthor",
                    TextWeights = twauthor
                });
                lstScoringProfiles.Add(new ScoringProfile()
                {
                    Name = "QuoteText",
                    TextWeights = twtext
                });

                definition.ScoringProfiles = lstScoringProfiles;



                _searchClient.Indexes.Create(definition);

                

                return "Index created!";
            }
            catch (Exception ex)
            {
                return "There was an issue creating the index: {0}\r\n" + ex.Message.ToString();
            }
        }


        public string LoadIndex()
        {
            try
            {
                //Get the quotes
                var quotes = QuoteMVCProvider.GetQuoteMVCs();

                //Build up a json post of the quote data
                List<Quote> lstQuotes = new List<Quote>();

                foreach (QuoteMVC quote in quotes)
                {
                    Quote qt = new Quote();
                    qt.DocumentID = quote.DocumentID.ToString();
                    qt.NodeAliasPath = quote.NodeAliasPath;
                    qt.QuoteAuthor = quote.GetValue("QuoteAuthor").ToString();
                    qt.QuoteText = quote.GetValue("QuoteText").ToString().Replace("'", "''").Replace("\"", "''");
                    qt.QuoteDate = quote.GetValue("QuoteDate").ToString();

                    lstQuotes.Add(qt);
                }

                _indexClient.Documents.Index(IndexBatch.Create(lstQuotes.Select(qt => IndexAction.Create(qt))));


                return "Index loaded!";
            }
            catch (Exception ex)
            {
                return "There was an issue loading the index: {0}\r\n" + ex.Message.ToString();
            }
        }

        public string DeleteIndex()
        {
            try
            {
                //Delete the index
                _searchClient.Indexes.Delete(_indexName);
                return "Index deleted!";
            }
            catch (Exception ex)
            {
                return "There was an issue deleting the index: {0}\r\n" + ex.Message.ToString();
            }
        }
        #endregion

        #region Search Actions

        public DocumentSuggestResponse Suggest(string searchText, bool blnFuzzy)
        {
            // Execute search based on query string
            try
            {
                //Build the SearchParameter object
                SearchParameters sp = new SearchParameters();
                sp.SearchMode = SearchMode.All;
                SuggestParameters sugp = new SuggestParameters();
                sugp.UseFuzzyMatching = blnFuzzy;
                return _indexClient.Documents.Suggest(searchText, "quoteauthor", sugp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }
            return null;
        }

        public DocumentSearchResponse Search(string searchText, bool blnHighlights, bool blnFacets, bool blnScore, string strFilter, string strScoringProfile)
        {
            // Execute search based on query string
            try
            {
                //Build the SearchParameter object
                SearchParameters sp = new SearchParameters();
                sp.SearchMode = SearchMode.All;

                //Check if highlights should be returned
                if (blnHighlights)
                {
                    sp.HighlightFields = new List<string> { "QuoteAuthor", "QuoteText" };
                    sp.HighlightPreTag = "<span class='highlight'>";
                    sp.HighlightPostTag = "</span>";
                }
                //Check if facets shoudl be returned
                if (blnFacets)
                {
                    sp.Facets = new List<string> { "QuoteAuthor" };
                }
                //Check if the results should be filtered
                if (strFilter != "")
                {
                    sp.Filter = "QuoteAuthor eq '" + strFilter + "'";
                }
                //Check if there is a scoring profile specified
                if(strScoringProfile != "")
                {
                    sp.ScoringProfile = strScoringProfile;
                }

                return _indexClient.Documents.Search(searchText, sp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }
            return null;
        }

        #endregion
    }
}
