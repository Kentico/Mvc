using CMS.Activities;
using CMS.Activities.Loggers;

namespace Kentico.Activities
{
    /// <summary>
    /// Public service for activity logging in online marketing.
    /// </summary>
    public class MembershipActivitiesLogger : IMembershipActivitiesLogger
    {
        /// <summary>
        /// Logs login activity for given user.
        /// </summary>
        /// <remarks>
        /// This method should be called whenever the user is authenticated to ensure logging of correct <see cref="ActivityInfo"/>.
        /// </remarks>
        /// <param name="userName">User name of the authenticated user</param>
        public void LogLoginActivity(string userName)
        {
            MembershipActivityLogger.LogLogin(userName);
        }


        /// <summary>
        /// Logs user registration activity for given user.
        /// </summary>
        /// <remarks>
        /// This method should be called whenever the user is registered to ensure logging of correct <see cref="ActivityInfo"/>.
        /// </remarks>
        /// <param name="userName">User name of the registered user</param>
        public void LogRegisterActivity(string userName)
        {
            MembershipActivityLogger.LogRegistration(userName);
        }
    }
}
