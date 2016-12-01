using System;

using CMS.DataEngine;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.MediaLibrary.Tests
{
    /// <summary>
    /// Unit tests for class <see cref="MediaFileInfoExtensions"/>.
    /// </summary>
    public class MediaFileInfoExtensionsTests
    {
        [TestFixture]
        public class GetUrlTests : MediaFileInfoExtensionsBaseTests
        {
            [SetUp]
            public void SetUp()
            {
                FakeData();

                SiteContext.CurrentSite = site;

                Fake<SettingsKeyInfo, SettingsKeyInfoProvider>().WithData(
                    new SettingsKeyInfo { KeyID = 1, KeyName = "CMSUseMediaLibrariesSiteFolder", KeyType = "boolean", KeyValue = "false", SiteID = 1 },
                    new SettingsKeyInfo { KeyID = 2, KeyName = "CMSFilesFriendlyURLExtension", KeyType = "string", KeyValue = "", SiteID = 1 }
                );
            }


            [Test]
            public void GetUrl_NullMediaFile_ThrowsException()
            {
                Assert.That(() => ((MediaFileInfo)null).GetUrl(), Throws.Exception.TypeOf<ArgumentNullException>());
            }


            [Test]
            public void GetUrl_CurrentSite_ReturnsRelativeUrl()
            {
                Assert.AreEqual($"~/{SITE_NAME}/media/{LIBRARY_FOLDER}/{MEDIA_FILE_PATH}", mediaFile.GetUrl());
            }


            [Test]
            public void GetUrl_DifferentSite_ReturnsAbsoluteUrl()
            {
                var differentSite = new SiteInfo()
                {
                    SiteName = "site2",
                    SiteIsContentOnly = true
                };

                SiteContext.CurrentSite = differentSite;

                Assert.AreEqual($"{SITE_PRESENTATION_URL}/{SITE_NAME}/media/{LIBRARY_FOLDER}/{MEDIA_FILE_PATH}", mediaFile.GetUrl());
            }


            [Test]
            public void GetUrl_FileInCDNOnDifferentSite_ReturnsCDNUrl()
            {
                var differentSite = new SiteInfo()
                {
                    SiteName = "site2",
                    SiteIsContentOnly = true
                };

                SiteContext.CurrentSite = differentSite;
                MediaFileURLProvider.ProviderObject = new MediaFileURLProviderFake();

                Assert.AreEqual(MediaFileURLProviderFake.CDN_File_Url, mediaFile.GetUrl());
            }


            private class MediaFileURLProviderFake : MediaFileURLProvider
            {
                public static string CDN_File_Url = "http://cdn.com/mediaFile.jpg";

                protected override string GetMediaFileUrlInternal(MediaFileInfo fileInfo, string siteName, string libraryFolder, string filePath)
                {
                    return CDN_File_Url;
                }
            }
        }


        [TestFixture]
        public class GetPermanentUrlTests : MediaFileInfoExtensionsBaseTests
        {
            [SetUp]
            public void SetUp()
            {
                FakeData();
            }


            [Test]
            public void GetPermanentUrl_NullMediaFile_ThrowsException()
            {
                Assert.That(() => ((MediaFileInfo)null).GetPermanentUrl(), Throws.Exception.TypeOf<ArgumentNullException>());
            }


            [Test]
            public void GetPermanentUrl_CurrentSite_ReturnsRelativeUrl()
            {
                SiteContext.CurrentSite = site;

                Assert.AreEqual($"~/getmedia/{MEDIA_FILE_GUID}/{MEDIA_FILE_NAME}", mediaFile.GetPermanentUrl());
            }

            [Test]
            public void GetPermanentUrl_DifferentSite_ReturnsAbsoluteUrl()
            {
                var differentSite = new SiteInfo()
                {
                    SiteName = "site2",
                    SiteIsContentOnly = true
                };

                SiteContext.CurrentSite = differentSite;

                Assert.AreEqual($"{SITE_PRESENTATION_URL}/getmedia/{MEDIA_FILE_GUID}/{MEDIA_FILE_NAME}", mediaFile.GetPermanentUrl());
            }

        }


        public class MediaFileInfoExtensionsBaseTests : UnitTests
        {
            public readonly Guid MEDIA_FILE_GUID = new Guid("019bc7cd-4907-476f-984e-952f3b4a23c7");
            public const string MEDIA_FILE_NAME = "fileName";
            public const string MEDIA_FILE_PATH = "fileName.jpg";
            public const string LIBRARY_FOLDER = "libraryFolder";
            public const string SITE_NAME = "site1";
            public const string SITE_PRESENTATION_URL = "http://presentation.url";
            public const string SITE_DOMAIN = "domain.com";

            public MediaFileInfo mediaFile;
            public SiteInfo site;


            public void FakeData()
            {
                Fake<MediaLibraryInfo, MediaLibraryInfoProvider>().WithData(
                    new MediaLibraryInfo()
                    {
                        LibraryID = 1,
                        LibraryFolder = LIBRARY_FOLDER
                    }
                );

                Fake<SiteInfo>();
                site = new SiteInfo()
                {
                    SiteID = 1,
                    SiteName = SITE_NAME,
                    DomainName = SITE_DOMAIN,
                    SitePresentationURL = SITE_PRESENTATION_URL,
                    SiteIsContentOnly = true
                };
                Fake<SiteInfo, SiteInfoProvider>().WithData(site);

                Fake<MediaFileInfo>();
                mediaFile = new MediaFileInfo
                {
                    FileGUID = MEDIA_FILE_GUID,
                    FileName = MEDIA_FILE_NAME,
                    FileExtension = ".jpg",
                    FilePath = MEDIA_FILE_PATH,
                    FileSiteID = 1,
                    FileLibraryID = 1
                };
            }
        }
    }
}
