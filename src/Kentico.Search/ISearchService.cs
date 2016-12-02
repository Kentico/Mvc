using System;

using Kentico.Core.DependencyInjection;

namespace Kentico.Search
{
    /// <summary>
    /// Provides access to Kentico Smart search.
    /// </summary>
    public interface ISearchService : IService
    {
        /// <summary>
        /// Performs full-text search for the requested page.
        /// </summary>
        /// <param name="options">Options that specify what should be searched for.</param>
        /// <returns>
        /// Search result for the specified search <paramref name="options" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        SearchResult Search(SearchOptions options);
    }
}