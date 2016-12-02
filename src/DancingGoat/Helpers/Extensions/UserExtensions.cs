using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Membership;
using CMS.Newsletters;
using Kentico.Membership;

namespace DancingGoat.Helpers.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Creates a <see cref="SubscriberInfo"/> object and fills its properties from the specified user object.
        /// </summary>
        /// <param name="user">Source user.</param>
        /// <param name="siteId">Identifier of the subscriber's site.</param>
        /// <returns>Instance of the <see cref="SubscriberInfo"/> class initialized from <paramref name="user"/>.</returns>
        public static SubscriberInfo ToSubscriberInfo(this User user, int siteId)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var subscriber = new SubscriberInfo
            {
                SubscriberEmail = user.Email,
                SubscriberFirstName = user.FirstName,
                SubscriberLastName = user.LastName,
                SubscriberFullName = user.FirstName + " " + user.LastName,
                SubscriberType = UserInfo.OBJECT_TYPE,
                SubscriberRelatedID = user.Id,
                SubscriberSiteID = siteId
            };

            return subscriber;
        }
    }
}