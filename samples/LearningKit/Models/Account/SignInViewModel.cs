using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class SignInViewModel
{    
    [Required(ErrorMessage = "The User name cannot be empty.")]
    [DisplayName("User name")]
    [MaxLength(100, ErrorMessage = "The User name cannot be longer than 100 characters.")]
    public string UserName
    {
        get;
        set;
    }
    
    [DataType(DataType.Password)]
    [DisplayName("Password")]
    [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
    public string Password
    {
        get;
        set;
    }
    
    [DisplayName("Stay signed in")]
    public bool SignInIsPersistent
    {
        get;
        set;
    }
}