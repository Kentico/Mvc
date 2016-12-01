using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sandbox.Models.Account
{
    /// <summary>
    /// View model for registration purposes.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Username.
        /// </summary>
        [Required(ErrorMessage = "User name cannot be empty.")]
        [DisplayName("User name")]
        [MaxLength(100, ErrorMessage = "User name must be no longer than 100 characters.")]
        public string UserName
        {
            get;
            set;
        }


        /// <summary>
        /// Email.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email cannot be empty.")]
        [DisplayName("Email address")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(254, ErrorMessage = "Email address must be no longer than 254 characters.")]
        public string Email
        {
            get;
            set;
        }


        /// <summary>
        /// Password.
        /// </summary>
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password name cannot be empty.")]
        [DisplayName("Password")]
        [MaxLength(100, ErrorMessage = "Password must be no longer than 100 characters.")]
        public string Password
        {
            get;
            set;
        }


        /// <summary>
        /// Password confirmation.
        /// </summary>
        [DataType(DataType.Password)]
        [DisplayName("Password confirmation")]
        [MaxLength(100, ErrorMessage = "Password must be no longer than 100 characters.")]
        [Compare("Password", ErrorMessage = "The passwords does not match.")]
        public string PasswordConfirmation
        {
            get;
            set;
        }


        /// <summary>
        /// First name.
        /// </summary>
        [DisplayName("First name")]
        [Required(ErrorMessage = "First name name cannot be empty.")]
        [MaxLength(100, ErrorMessage = "First name must be no longer than 100 characters.")]
        public string FirstName
        {
            get;
            set;
        }


        /// <summary>
        /// Last name.
        /// </summary>
        [DisplayName("Last name")]
        [Required(ErrorMessage = "Last name name cannot be empty.")]
        [MaxLength(100, ErrorMessage = "Last name must be no longer than 100 characters.")]
        public string LastName
        {
            get;
            set;
        }
    }
}