using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.Tests;

using Microsoft.AspNet.Identity;
using NUnit.Framework;

namespace Kentico.Membership.Tests
{
    [TestFixture]
    public class EmailServiceTests : UnitTests
    {
        private const string SITE_NAME = "TestSite";
        private EmailService service;


        [SetUp]
        public void SetUp()
        {
            service = new EmailService();
        }


        [Test]
        public void CreateEmailMessage_Null_ReturnsNull()
        {
            Assert.IsNull(service.CreateEmailMessage(null));
        }


        [Test]
        public void CreateEmailMessage_NoreplyAddressNotDefined_ReturnsNull()
        {
            Fake<SettingsKeyInfo, SettingsKeyInfoProvider>().WithData(new SettingsKeyInfo { KeyName = "CMSNoreplyEmailAddress", KeyValue = "" });

            var message = service.CreateEmailMessage(new IdentityMessage());

            Assert.IsNull(message);
        }


        [Test]
        public void CreateEmailMessage_EmptyMessage_ReturnsCorrectEmail()
        {
            var from = "noreply@email.com";
            var im = new IdentityMessage();

            Fake<SettingsKeyInfo, SettingsKeyInfoProvider>().WithData(new SettingsKeyInfo { KeyName = "CMSNoreplyEmailAddress", KeyValue = from });

            var message = service.CreateEmailMessage(im);

            CMSAssert.All(
                () => Assert.IsInstanceOf<EmailMessage>(message),
                () => Assert.AreEqual(from, message.From),
                () => Assert.AreEqual(im.Destination, message.Recipients),
                () => Assert.AreEqual(im.Body, message.Body),
                () => Assert.AreEqual(im.Subject, message.Subject),
                () => Assert.AreEqual(EmailFormatEnum.Html, message.EmailFormat));
        }


        [Test]
        public void CreateEmailMessage_FilledMessage_ReturnsCorrectEmail()
        {
            var from = "noreply@email.com";
            var im = new IdentityMessage
            {
                Destination = "test@test.com",
                Body = "Hi! This is <strong>test message</strong>.",
                Subject = "Test subject"
            };

            Fake<SettingsKeyInfo, SettingsKeyInfoProvider>().WithData(new SettingsKeyInfo { KeyName = "CMSNoreplyEmailAddress", KeyValue = from });

            var message = service.CreateEmailMessage(im);

            CMSAssert.All(
                () => Assert.IsInstanceOf<EmailMessage>(message),
                () => Assert.AreEqual(from, message.From),
                () => Assert.AreEqual(im.Destination, message.Recipients),
                () => Assert.AreEqual(im.Body, message.Body),
                () => Assert.AreEqual(im.Subject, message.Subject),
                () => Assert.AreEqual(EmailFormatEnum.Html, message.EmailFormat));
        }


        [Test]
        public void SendAsync_Null_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => service.SendAsync(null).Wait());
        }


        [TestFixture, Category.IsolatedIntegration]
        public class UserStoreIntegrationTests : IsolatedIntegrationTests
        {
            private EmailService service;


            [SetUp]
            public void SetUp()
            {
                service = new EmailService();
            }


            [Test]
            public void SendAsync_Message_DoesNotThrow()
            {
                var message = new IdentityMessage
                {
                    Destination = "test@test.com",
                    Body = "Hi! This is <strong>test message</strong>.",
                    Subject = "Test subject"
                };

                Assert.DoesNotThrow(() => service.SendAsync(message).Wait());
            }
        }
    }
}
