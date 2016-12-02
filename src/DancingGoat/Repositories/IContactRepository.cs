using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of contact information.
    /// </summary>
    public interface IContactRepository : IRepository
    {
        /// <summary>
        /// Returns company's contact information.
        /// </summary>
        /// <returns>Company's contact information, if found; otherwise, null.</returns>
        Contact GetCompanyContact();
    }
}