using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Kentico.Membership
{
    /// <summary>
    /// Manages sign in operations for users.
    /// </summary>
    public class SignInManager : SignInManager<User, int>
    {
        /// <summary>
        /// Creates the instance of <see cref="SignInManager"/>.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="authenticationManager">Authentication manager.</param>
        public SignInManager(UserManager userManager, Microsoft.Owin.Security.IAuthenticationManager authenticationManager)
        : base(userManager, authenticationManager)
        {
        }


        /// <summary>
        /// Factory method that creates the <see cref="SignInManager"/> instance.
        /// </summary>
        /// <param name="options">Identity factory options.</param>
        /// <param name="context">OWIN context.</param>
        public static SignInManager Create(IdentityFactoryOptions<SignInManager> options, IOwinContext context)
        {
            return new SignInManager(context.GetUserManager<UserManager>(), context.Authentication);
        }
    }
}
