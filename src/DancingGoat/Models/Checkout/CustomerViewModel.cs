using System.ComponentModel.DataAnnotations;

using Kentico.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    public class CustomerViewModel
    {
        [Required]
        [Display(Name = "General.Firstname")]
        [MaxLength(100, ErrorMessage = "General.MaxlengthExceeded")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "General.Lastname")]
        [MaxLength(100, ErrorMessage = "General.MaxlengthExceeded")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "General.RequireEmail")]
        [Display(Name = "General.EmailAddress")]
        [EmailAddress(ErrorMessage = "General.CorrectEmailFormat")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.News.LongEmail")]
        public string Email { get; set; }


        [Display(Name = "General.Phone")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(26, ErrorMessage = "General.MaxlengthExceeded")]
        public string PhoneNumber { get; set; }


        [Display(Name = "com.companyname")]
        [MaxLength(200, ErrorMessage = "General.MaxlengthExceeded")]
        public string Company { get; set; }


        [Display(Name = "com.customer.organizationid")]
        [MaxLength(50, ErrorMessage = "General.MaxlengthExceeded")]
        public string OrganizationID { get; set; }


        [Display(Name = "com.customer.taxregistrationid")]
        [MaxLength(50, ErrorMessage = "General.MaxlengthExceeded")]
        public string TaxRegistrationID { get; set; }


        public bool IsCompanyAccount { get; set; }


        public CustomerViewModel(Customer customer)
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


        public CustomerViewModel()
        {
        }


        public void ApplyToCustomer(Customer customer, bool emailCanBeChanged)
        {
            customer.FirstName = FirstName;
            customer.LastName = LastName;
            customer.PhoneNumber = PhoneNumber;
            customer.Company = Company;
            customer.OrganizationID = OrganizationID;
            customer.TaxRegistrationID = TaxRegistrationID;

            if (emailCanBeChanged)
            {
                customer.Email = Email;
            }
        }
    }
}