using System.Threading.Tasks;

using CMS.Base;
using CMS.ContactManagement;
using CMS.ContactManagement.Internal;
using CMS.Core;
using CMS.Membership;

using Kentico.Membership;

namespace Kentico.ContactManagement
{
    /// <summary>
    /// Public service for contact tracking in contact management.
    /// </summary>
    public class ContactTrackingService : IContactTrackingService
    {
        private readonly ICurrentContactProvider mCurrentContactProvider;
        private readonly ISiteService mSiteService;
        private readonly IContactProcessingChecker mContactProcessingChecker;


        /// <summary>
        /// Instantiates new instance of <see cref="ContactTrackingService"/>.
        /// </summary>
        public ContactTrackingService()
        {
            mCurrentContactProvider = Service.Resolve<ICurrentContactProvider>();
            mSiteService = Service.Resolve<ISiteService>();
            mContactProcessingChecker = Service.Resolve<IContactProcessingChecker>();
        }


        /// <summary>
        /// Gets <see cref="ContactInfo"/> from persistent storage and compares it with all the contacts that are related to given <paramref name="userName"/>.
        /// Resolves the conflict when the current <see cref="ContactInfo"/> is already in relationship with another user by <see cref="ContactInfo"/> merging.
        /// </summary>
        /// <remarks>
        /// This method should be called whenever the authenticated user changes to ensure correct <see cref="ContactInfo"/> is stored in persistent storage. 
        /// Please note that this method has no effect if the feature is disabled or unavailable in the license.
        /// </remarks>
        /// <param name="userName">User name of the <see cref="UserInfo"/> the current <see cref="ContactInfo"/> should be merged for</param>
        public async Task MergeUserContactsAsync(string userName)
        {
            await GetContactAsync(userName, true);
        }


        /// <summary>
        /// Returns <see cref="ContactInfo"/> from persistent storage. If <see cref="ContactInfo"/> is not found, the service tries to obtain it based on 
        /// <see cref="ContactMembershipInfo"/> with given <paramref name="userName"/>. If <see cref="ContactInfo"/> is still not found, a new one is created.
        /// After retrieval stores the result <see cref="ContactInfo"/> in persistent storage.
        /// </summary>
        /// <remarks>
        /// This method has no effect and returns <c>null</c> if the feature is disabled or unavailable in the license. Contact is not returned for crawler requests.
        /// </remarks>
        /// <param name="userName">User name of the <see cref="UserInfo"/> to get the <see cref="ContactInfo"/> for</param>
        /// <returns><see cref="ContactInfo"/> for the user from persistent storage. Returns <c>null</c> if the feature is unavailable or disabled.</returns>
        public async Task<ContactInfo> GetCurrentContactAsync(string userName = null)
        {
            return await GetContactAsync(userName, false);
        }


        /// <summary>
        /// Get a contact for a user and decide whether to merge matching activities.
        /// </summary>
        /// <param name="userName">User name of the user to get a contact for</param>
        /// <param name="forceMatching">Merge matched activities to contact</param>
        /// <returns><see cref="ContactInfo"/> returned for the user.</returns>
        private async Task<ContactInfo> GetContactAsync(string userName, bool forceMatching)
        {
            if (!mContactProcessingChecker.CanProcessContactInCurrentContext())
            {
                return null;
            }

            var userInfo = await GetCurrentUserInfoAsync(userName);
            var currentContact = mCurrentContactProvider.GetCurrentContact(userInfo, forceMatching);
            mCurrentContactProvider.SetCurrentContact(currentContact);

            return currentContact;
        }


        /// <summary>
        /// Returns <see cref="ContactInfo"/> from persistent storage. If <see cref="ContactInfo"/> is not found, the service tries to obtain it based on 
        /// <see cref="ContactMembershipInfo"/> with given <paramref name="userName"/>.
        /// </summary>
        /// <remarks>
        /// This method has no effect and returns <c>null</c> if the feature is disabled or unavailable in the license. Contact is not returned for crawler requests.
        /// </remarks>
        /// <param name="userName">User name of the <see cref="UserInfo"/> to get the <see cref="ContactInfo"/> for</param>
        /// <returns><see cref="ContactInfo"/> for the user from persistent storage. Returns <c>null</c> if the feature is unavailable or disabled.</returns>
        public async Task<ContactInfo> GetExistingContactAsync(string userName = null)
        {
            if (!mContactProcessingChecker.CanProcessContactInCurrentContext())
            {
                return null;
            }

            var userInfo = await GetCurrentUserInfoAsync(userName);
            return mCurrentContactProvider.GetExistingContact(userInfo, false);
        }


        /// <summary>
        /// Gets a <see cref="UserInfo"/> for given <paramref name="userName"/>. 
        /// </summary>
        /// <param name="userName">User name of the <see cref="UserInfo"/> to get info for</param>
        /// <returns><see cref="CurrentUserInfo"/> returned for the user.</returns>
        private async Task<UserInfo> GetCurrentUserInfoAsync(string userName)
        {
            var user = await FindUserAsync(userName);
            if (user != null)
            {
                return UserInfoProvider.GetUserInfo(user.Id);
            }

            return AuthenticationHelper.GlobalPublicUser;
        }


        /// <summary>
        /// Find a <see cref="UserInfo"/> based on MVC application <see cref="User"/>.
        /// </summary>
        /// <param name="userName">User name of MVC application <see cref="User"/></param>
        /// <returns>Found <see cref="User"/>.</returns>
        private async Task<User> FindUserAsync(string userName)
        {
            var userStore = new UserStore(mSiteService.CurrentSite.SiteName);
            return await userStore.FindByNameAsync(userName);
        }
    }
}