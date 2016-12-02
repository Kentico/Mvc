using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class NewsletterSubscribeViewModel
{
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

    /// <summary>
    /// Indicates whether the newsletter requires double-opt in for subscription.
    /// Allows the view to display appropriate information to newly subscribed users.
    /// </summary>
    [Bindable(false)]
    public bool RequireDoubleOptIn
    {
        get;
        set;
    }
}