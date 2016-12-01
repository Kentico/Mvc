//DocSection:Using
using System.Collections.Generic;
using System.Web.Mvc;

using CMS.MediaLibrary;
using CMS.SiteProvider;
//EndDocSection:Using

namespace LearningKit.Controllers
{
    public class MediaLibraryController : Controller
    {
        /// <summary>
        /// Retrieves media files with the .jpg extension from the 'SampleMediaLibrary'.
        /// </summary>
        public ActionResult ShowMediaFiles()
        {
            //DocSection:GetMediaFiles
            // Creates an instance of the 'SampleMediaLibrary' media library for the current site
            MediaLibraryInfo mediaLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo("SampleMediaLibrary", SiteContext.CurrentSiteName);
            
            // Gets a collection of media files with the .jpg extension from the media library
            IEnumerable<MediaFileInfo> mediaLibraryFiles = MediaFileInfoProvider.GetMediaFiles()
                .WhereEquals("FileLibraryID", mediaLibrary.LibraryID)
                .WhereEquals("FileExtension", ".jpg");
            //EndDocSection:GetMediaFiles

            return View(mediaLibraryFiles);
        }
    }
}
