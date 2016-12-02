using System;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

using CMS.EmailEngine;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.EventLog;

namespace Kentico.Membership
{
    /// <summary>
    /// Exposes method for sending messages to users using email.
    /// </summary>
    public class EmailService : IIdentityMessageService
    {
        /// <summary>
        /// Sends the given message (email).
        /// </summary>
        /// <param name="message">Message.</param>
        public Task SendAsync(IdentityMessage message)
        {
            var em = CreateEmailMessage(message);

            if (em == null)
            {
                return Task.FromResult(0);
            }

            EmailSender.SendEmail(em);

            return Task.FromResult(0);
        }


        /// <summary>
        /// Creates new instance of <see cref="EmailMessage"/> from the
        /// instance of <see cref="IdentityMessage"/>.
        /// </summary>
        /// <param name="message">Instance of <see cref="IdentityMessage"/>.</param>
        public EmailMessage CreateEmailMessage(IdentityMessage message)
        {
            if (message == null)
            {
                return null;
            }

            string from = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress");

            if (String.IsNullOrEmpty(from))
            {
                EventLogProvider.LogEvent(EventType.ERROR, "EmailService", "EmailSenderNotSpecified");
                return null;
            }

            return new EmailMessage
            {
                From = from,
                Recipients = message.Destination,
                Subject = message.Subject,
                EmailFormat = EmailFormatEnum.Html,
                Body = message.Body
            };
        }
    }
}
