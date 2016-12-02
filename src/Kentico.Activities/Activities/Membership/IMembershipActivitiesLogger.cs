using CMS.Activities;

using Kentico.Core.DependencyInjection;

namespace Kentico.Activities
{
    /// <summary>
    /// Provides methods for membership activities logging.
    /// </summary>
    public interface IMembershipActivitiesLogger : IService
    {
        /// <summary>
        /// Logs login activity for given user.
        /// </summary>
        /// <remarks>
        /// This method should be called whenever the user is authenticated to ensure logging of correct <see cref="ActivityInfo"/>.
        /// </remarks>
        /// <param name="userName">User name of the authenticated user</param>
        void LogLoginActivity(string userName);


        /// <summary>
        /// Logs user registration activity for given user.
        /// </summary>
        /// <remarks>
        /// This method should be called whenever the user is registered to ensure logging of correct <see cref="ActivityInfo"/>.
        /// </remarks>
        /// <param name="userName">User name of the registered user</param>
        void LogRegisterActivity(string userName);
    }
}