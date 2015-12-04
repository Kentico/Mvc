using CMS.DocumentEngine.Types;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of contact information.
    /// </summary>
    public interface IContactRepository
    {
        /// <summary>
        /// Returns company's contact information.
        /// </summary>
        /// <returns>Company's contact information, if found; otherwise, null.</returns>
        Contact GetCompanyContact();
    }
}