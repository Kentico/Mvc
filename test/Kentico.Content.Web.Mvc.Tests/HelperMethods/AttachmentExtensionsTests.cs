using System;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Content.Web.Mvc.Tests
{
    /// <summary>
    /// Unit tests for class <see cref="HelperMethods.AttachmentExtensions"/>.
    /// </summary>
    public class AttachmentExtensionsTests
    {
        [TestFixture]
        public class GetPathTests : UnitTests
        {
            private readonly Guid ATTACHMENT_GUID = Guid.Parse("00000000-0000-0000-0000-000000000001");


            [SetUp]
            public void SetUp()
            {
                Fake<AttachmentInfo>();
                Fake<AttachmentHistoryInfo>();
            }


            [Test]
            public void Attachment_NullAttachment_ThrowsException()
            {
                Assert.That(
                    () => ((Attachment) null).GetPath(), Throws.Exception.TypeOf<ArgumentNullException>()
                );
            }


            [TestCase(null, "~/getattachment/{0}/attachment")]
            [TestCase("", "~/getattachment/{0}/attachment")]
            [TestCase("Můj název přílohy", "~/getattachment/{0}/muj-nazev-prilohy")]
            [TestCase("attachment", "~/getattachment/{0}/attachment")]
            [TestCase("ATTACHMENT", "~/getattachment/{0}/attachment")]
            [TestCase("long attachment name", "~/getattachment/{0}/long-attachment-name")]
            [TestCase("long  /\\  attachment  ?+?   name", "~/getattachment/{0}/long-attachment-name")]
            [TestCase("long attachment name  ++++  ???", "~/getattachment/{0}/long-attachment-name")]
            [TestCase("long.attachment_name", "~/getattachment/{0}/long.attachment_name")]
            [TestCase("long...attachment___name", "~/getattachment/{0}/long...attachment___name")]
            public void GetPath_FileName_ResultPathDoesNotContainSpecialCharacters(string fileName, string expectedUrl)
            {
                var attachment = CreateAttachment(fileName);
                string attachmentPath = attachment.GetPath();

                Assert.That(attachmentPath, Is.EqualTo(string.Format(expectedUrl, ATTACHMENT_GUID)));
            }


            [Test]
            public void GetPath_FileNameAndVariantName_ResultPathContainsVariantQueryString()
            {
                var attachment = CreateAttachment("attachmentName");
                string attachmentPath = attachment.GetPath("mobile");

                Assert.That(attachmentPath, Is.EqualTo(string.Format("~/getattachment/{0}/attachmentname?variant=mobile", ATTACHMENT_GUID)));
            }


            [TestCase(-1, "~/getattachment/{0}/attachment")]
            [TestCase(0, "~/getattachment/{0}/attachment")]
            [TestCase(1, "~/testPrefix/getattachment/{0}/attachment?uh=a5d8706e4bcd9fc858aae6c13a4a0f6c7e9ee1c51d469e80787befdce8e682d5")]
            public void GetPath_VersionedAttachment_ResultPathContainsPreviewHash(int attachmentHistoryId, string expectedUrl)
            {
                FakeVirtualContext();
                var attachment = CreateAttachmentHistory(attachmentHistoryId);
                string attachmentPath = attachment.GetPath();

                Assert.That(attachmentPath, Is.EqualTo(string.Format(expectedUrl, ATTACHMENT_GUID)));
            }


            [Test]
            public void GetPath_VersionedAttachmenVariant_ResultPathContainsVariantQueryStringAndHash()
            {
                FakeVirtualContext();
                var attachment = CreateAttachmentHistory(1);
                string attachmentPath = attachment.GetPath("mobile");

                Assert.That(attachmentPath, Is.EqualTo(string.Format("~/testPrefix/getattachment/{0}/attachment?variant=mobile&uh=a5d8706e4bcd9fc858aae6c13a4a0f6c7e9ee1c51d469e80787befdce8e682d5", ATTACHMENT_GUID)));
            }


            private static void FakeVirtualContext()
            {
                VirtualContext.SetItem(VirtualContext.PARAM_PREVIEW_LINK, "testPreviewLink");
                ValidationHelper.HashStringSalt = "78a60095-2b09-40c2-9a9d-e2cc0b511aca";
                VirtualContext.CurrentURLPrefix = "/testPrefix";
            }


            private Attachment CreateAttachmentHistory(int attachmentHistoryId)
            {
                var attachmentInfo = new AttachmentHistoryInfo
                {
                    AttachmentHistoryID = attachmentHistoryId,
                    AttachmentGUID = ATTACHMENT_GUID
                };

                return new Attachment(attachmentInfo);

            }


            private Attachment CreateAttachment(string fileName)
            {
                var attachmentInfo = new AttachmentInfo
                {
                    AttachmentName = fileName,
                    AttachmentGUID = ATTACHMENT_GUID
                };

                return new Attachment(attachmentInfo);
            }
        }
    }
}
