using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "The User name cannot be empty.")]
    [DisplayName("User name")]
    [MaxLength(100, ErrorMessage = "The User name cannot be longer than 100 characters.")]
    public string UserName
    {
        get;
        set;
    }

    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "The Email address cannot be empty.")]
    [DisplayName("Email address")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [MaxLength(254, ErrorMessage = "The Email address cannot be longer than 254 characters.")]
    public string Email
    {
        get;
        set;
    }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "The Password cannot be empty.")]
    [DisplayName("Password")]
    [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
    public string Password
    {
        get;
        set;
    }

    [DataType(DataType.Password)]
    [DisplayName("Password confirmation")]
    [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
    [Compare("Password", ErrorMessage = "The entered passwords do not match.")]
    public string PasswordConfirmation
    {
        get;
        set;
    }

    [DisplayName("First name")]
    [Required(ErrorMessage = "The First name cannot be empty.")]
    [MaxLength(100, ErrorMessage = "The First name cannot be longer than 100 characters.")]
    public string FirstName
    {
        get;
        set;
    }

    [DisplayName("Last name")]
    [Required(ErrorMessage = "The Last name cannot be empty.")]
    [MaxLength(100, ErrorMessage = "The Last name cannot be longer than 100 characters.")]
    public string LastName
    {
        get;
        set;
    }
}