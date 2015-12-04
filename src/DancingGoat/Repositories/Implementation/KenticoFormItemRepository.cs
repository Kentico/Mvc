using CMS.OnlineForms.Types;

using DancingGoat.Models.Contacts;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of form records.
    /// </summary>
    public class KenticoFormItemRepository : IFormItemRepository
    {
        /// <summary>
        /// Saves a new form record from the specified "Contact us" form data.
        /// </summary>
        /// <param name="message">The "Contact us" form data.</param>
        public void InsertContactUsFormItem(MessageModel message)
        {
            var item = new DancingGoatMvcContactUsItem
            {
                UserFirstName = message.FirstName,
                UserLastName = message.LastName,
                UserEmail = message.Email,
                UserMessage = message.MessageText,
            };

            item.Insert();
        }
    }
}