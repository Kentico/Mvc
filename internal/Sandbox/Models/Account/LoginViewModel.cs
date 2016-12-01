using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sandbox.Models.Account
{
    /// <summary>
    /// View model for login purposes.
    /// </summary>
    public class LoginViewModel
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
        /// Password.
        /// </summary>
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [MaxLength(100, ErrorMessage = "Password must be no longer than 100 characters.")]
        public string Password
        {
            get;
            set;
        }


        /// <summary>
        /// Indicates if the sign in should be persistent.
        /// </summary>
        [DisplayName("Stay signed in")]
        public bool StaySignedIn
        {
            get;
            set;
        }
    }
}