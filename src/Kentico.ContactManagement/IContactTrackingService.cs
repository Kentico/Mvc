using System.Threading.Tasks;

using CMS.ContactManagement;
using CMS.Membership;

using Kentico.Core.DependencyInjection;

namespace Kentico.ContactManagement
{
    /// <summary>
    /// Interface for service for contact tracking in contact management.
    /// </summary>
    public interface IContactTrackingService : IService
    {
        /// <summary>
        /// Gets <see cref="ContactInfo"/> from persistent storage and compares it with all the contacts that are related to given <paramref name="userName"/>.
        /// Resolves the conflict when the current <see cref="ContactInfo"/> is already in relationship with another user by <see cref="ContactInfo"/> merging.
        /// </summary>
        /// <remarks>
        /// This method should be called whenever the authenticated user changes to ensure correct <see cref="ContactInfo"/> is stored in persistent storage.
        /// Please note that this method has no effect if the feature is disabled or unavailable in the license.
        /// </remarks>
        /// <example>
        /// Following example shows how to use method <see cref="MergeUserContactsAsync"/>.
        /// <code>
        /// ...
        /// // Method handling the log in routine
        /// public async Task LoginUser(string userName)
        /// {
        ///     ...
        ///     IContactTrackingService contactTrackingService = someImplementation;
        ///     await contactTrackingService.MergeUserContactsAsync(userName);
        ///     ...
        /// }
        /// </code>
        /// </example>
		/// <param name="userName">User name of the <see cref="UserInfo"/> the current <see cref="ContactInfo"/> should be merged for</param>
        Task MergeUserContactsAsync(string userName);


        /// <summary>
        /// Returns <see cref="ContactInfo"/> from persistent storage. If <see cref="ContactInfo"/> is not found, the service tries to obtain it based on 
        /// <see cref="ContactMembershipInfo"/> with given <paramref name="userName"/>. If <see cref="ContactInfo"/> is still not found, a new one is created.
        /// After retrieval stores the result <see cref="ContactInfo"/> in persistent storage.
        /// </summary>
        /// <remarks>
        /// This method has no effect and returns <c>null</c> if the feature is disabled or unavailable in the license. Contact is not returned for crawler requests.
        /// </remarks>
        /// <example>
        /// <para>
        /// Following example shows how to use method <see cref="GetCurrentContactAsync"/> with current user.
        /// <code>
        /// ...
        /// IContactTrackingService contactTrackingService = someImplementation;
        /// // Tries to find contact assigned to the current user
        /// var contact = await contactTrackingService.GetCurrentContactAsync(HttpContext.Current.User.Identity.Name);
        /// ...
        /// </code>
        /// </para>
        /// <para>
        /// Following example shows how to use method <see cref="GetCurrentContactAsync"/> without user (the "public" one).
        /// <code>
        /// ...
        /// IContactTrackingService contactTrackingService = someImplementation;
        /// // Tries to obtain contact from persistent storage
        /// var contact = await contactTrackingService.GetCurrentContactAsync();
        /// ...
        /// </code>
        /// </para>
        /// </example>
        /// <param name="userName">User name of the <see cref="UserInfo"/> to get the <see cref="ContactInfo"/> for</param>
        /// <returns><see cref="ContactInfo"/> for the user from persistent storage. Returns <c>null</c> if the feature is unavailable or disabled.</returns>
        Task<ContactInfo> GetCurrentContactAsync(string userName = null);


        /// <summary>
        /// Returns <see cref="ContactInfo"/> from persistent storage. If <see cref="ContactInfo"/> is not found, the service tries to obtain it based on 
        /// <see cref="ContactMembershipInfo"/> with given <paramref name="userName"/>.
        /// </summary>
        /// <remarks>
        /// This method has no effect and returns <c>null</c> if the feature is disabled or unavailable in the license. Contact is not returned for crawler requests.
        /// </remarks>
        /// <param name="userName">User name of the <see cref="UserInfo"/> to get the <see cref="ContactInfo"/> for</param>
        /// <returns><see cref="ContactInfo"/> for the user from persistent storage. Returns <c>null</c> if the feature is unavailable or disabled.</returns>
        Task<ContactInfo> GetExistingContactAsync(string userName = null);
    }
}