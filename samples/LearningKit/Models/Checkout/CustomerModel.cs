using Kentico.Ecommerce;

namespace LearningKit.Models.Checkout
{
    //DocSection:CustomerModel
    public class CustomerModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        public string OrganizationID { get; set; }
        public string TaxRegistrationID { get; set; }
        public bool IsCompanyAccount { get; set; }
        
        /// <summary>
        /// Creates a customer model.
        /// </summary>
        /// <param name="customer">Customer details.</param>
        public CustomerModel(Customer customer)
        {
            if (customer == null)
            {
                return;
            }
            
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Email = customer.Email;
            PhoneNumber = customer.PhoneNumber;
            Company = customer.Company;
            OrganizationID = customer.OrganizationID;
            TaxRegistrationID = customer.TaxRegistrationID;
            IsCompanyAccount = customer.IsCompanyAccount;
        }
        
        /// <summary>
        /// Creates an empty customer model.
        /// </summary>
        public CustomerModel()
        {
        }
        
        /// <summary>
        /// Applies the model to a customer wrapper.
        /// </summary>
        /// <param name="customer">Customer details to which the model is applied.</param>
        public void ApplyToCustomer(Customer customer)
        {
            customer.FirstName = FirstName;
            customer.LastName = LastName;
            customer.Email = Email;
            customer.PhoneNumber = PhoneNumber;
            customer.Company = Company;
            customer.OrganizationID = OrganizationID;
            customer.TaxRegistrationID = TaxRegistrationID;
        }
    }
    //EndDocSection:CustomerModel
}