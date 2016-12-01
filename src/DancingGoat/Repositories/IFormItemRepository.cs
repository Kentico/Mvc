using DancingGoat.Models.Contacts;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of form records.
    /// </summary>
    public interface IFormItemRepository : IRepository
    {
        /// <summary>
        /// Saves a new form record from the specified "Contact us" form data.
        /// </summary>
        /// <param name="message">The "Contact us" form data.</param>
        void InsertContactUsFormItem(MessageModel message);
    }
}