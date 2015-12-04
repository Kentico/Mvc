using System.Collections.Generic;

using CMS.DocumentEngine.Types;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of stories about company's strategy, history and philosophy.
    /// </summary>
    public interface IAboutUsRepository
    {
        /// <summary>
        /// Returns the story that describes company's strategy and history.
        /// </summary>
        /// <returns>The story that describes company's strategy and history, if found; otherwise, null.</returns>
        AboutUs GetOurStory();


        /// <summary>
        /// Returns an enumerable collection of stories about company's philosophy ordered by a position in the content tree.
        /// </summary>
        /// <returns>An enumerable collection of stories about company's philosophy ordered by a position in the content tree.</returns>
        IEnumerable<AboutUsSection> GetSideStories();
    }
}