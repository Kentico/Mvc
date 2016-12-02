using System;

using CMS.Ecommerce;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents a customer.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Original Kentico customer info object from which the model gathers information. See <see cref="CustomerInfo"/> for detailed information.
        /// </summary>
        internal readonly CustomerInfo OriginalCustomer;


        /// <summary>
        /// Get the customer's identifier.
        /// </summary>
        public int ID => OriginalCustomer.CustomerID;


        /// <summary>
        /// Customer's first name.
        /// </summary>
        public string FirstName
        {
            get
            {
                return OriginalCustomer.CustomerFirstName;
            }
            set
            {
                OriginalCustomer.CustomerFirstName = value;
            }
        }


        /// <summary>
        /// Customer's last name.
        /// </summary>
        public string LastName
        {
            get
            {
                return OriginalCustomer.CustomerLastName;
            }
            set
            {
                OriginalCustomer.CustomerLastName = value;
            }
        }


        /// <summary>
        /// Customer's e-mail.
        /// </summary>
        public string Email
        {
            get
            {
                return OriginalCustomer.CustomerEmail;
            }
            set
            {
                OriginalCustomer.CustomerEmail = value;
            }
        }


        /// <summary>
        /// Customer's phone number.
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                return OriginalCustomer.CustomerPhone;
            }
            set
            {
                OriginalCustomer.CustomerPhone = value;
            }
        }


        /// <summary>
        /// Customer's company name.
        /// </summary>
        public string Company
        {
            get
            {
                return OriginalCustomer.CustomerCompany;
            }
            set
            {
                OriginalCustomer.CustomerCompany = value;
            }
        }


        /// <summary>
        /// Customer's organization ID.
        /// </summary>
        public string OrganizationID
        {
            get
            {
                return OriginalCustomer.CustomerOrganizationID;
            }
            set
            {
                OriginalCustomer.CustomerOrganizationID = value;
            }
        }


        /// <summary>
        /// Customer's tax registration ID.
        /// </summary>
        public string TaxRegistrationID
        {
            get
            {
                return OriginalCustomer.CustomerTaxRegistrationID;
            }
            set
            {
                OriginalCustomer.CustomerTaxRegistrationID = value;
            }
        }


        /// <summary>
        /// Indicates if the customer represents a company account (Company name, Tax reg. ID or Organization ID is submitted).
        /// </summary>
        public bool IsCompanyAccount => (Company != null) || (TaxRegistrationID != null) || (OrganizationID != null);


        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        public Customer()
            : this(new CustomerInfo())
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        /// <param name="originalCustomer"><see cref="CustomerInfo"/> object representing an original Kentico customer info object from which the model is created.</param>
        public Customer(CustomerInfo originalCustomer)
        {
            if (originalCustomer == null)
            {
                throw new ArgumentNullException(nameof(originalCustomer));
            }

            OriginalCustomer = originalCustomer;
        }


        /// <summary>
        /// Validates the customer.
        /// </summary>
        /// <returns>Instance of the <see cref="CustomerValidator"/> with validation summary.</returns>
        public CustomerValidator Validate()
        {
            var validator = new CustomerValidator(this);
            validator.Validate();
            return validator;
        }


        /// <summary>
        /// Saves the customer into the database.
        /// </summary>
        /// <param name="validate">Specifies whether the validation should be performed.</param>
        public void Save(bool validate = true)
        {
            if (validate)
            {
                var result = Validate();
                if (result.CheckFailed)
                {
                    throw new InvalidOperationException();
                }
            }

            CustomerInfoProvider.SetCustomerInfo(OriginalCustomer);
        }
    }
}
