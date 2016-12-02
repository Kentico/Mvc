using CMS.Helpers;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Class for customer validation.
    /// </summary>
    public class CustomerValidator
    {
        private readonly Customer mCustomer;


        /// <summary>
        /// Indicates if some validation failed.
        /// </summary>
        public bool CheckFailed => InvalidEmailFormat;

        
        /// <summary>
        /// True when email is in invalid format.
        /// </summary>
        public bool InvalidEmailFormat { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerValidator"/> class.
        /// </summary>
        /// <param name="customer"><see cref="Customer"/> object representing a customer that is validated</param>
        public CustomerValidator(Customer customer)
        {
            mCustomer = customer;
        }


        /// <summary>
        /// Validates the customer.
        /// </summary>
        /// <remarks>
        /// The following conditions must be met to pass the validation:
        /// 1) Email is in valid format.
        /// </remarks>
        public void Validate()
        {
            InvalidEmailFormat = !ValidationHelper.IsEmail(mCustomer.Email);
        }
    }
}
