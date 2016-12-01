using System.Collections.Generic;
using System.Linq;

using CMS.MediaLibrary;
using CMS.SiteProvider;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of media files.
    /// </summary>
    public class KenticoMediaFileRepository : IMediaFileRepository
    {
        private readonly string mMediaLibraryName;
        private MediaLibraryInfo mLibrary;


        private MediaLibraryInfo Library
        {
            get
            {
                if (mLibrary == null)
                {
                    mLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo(mMediaLibraryName, SiteName);
                }

                return mLibrary;
            }
        }


        /// <summary>
        /// Site's code name retrieved from <see cref="SiteContext.CurrentSiteName"/>.
        /// </summary>
        public string SiteName => SiteContext.CurrentSiteName;

        
        /// <summary>
        /// Returns display name of media library which stores the media files provided by the repository.
        /// </summary>
        public string MediaLibraryDisplayName => Library.LibraryDisplayName;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoMediaFileRepository"/> class that returns media files from the specified media library.
        /// </summary>
        /// <param name="mediaLibraryName">The code name of a media library.</param>
        public KenticoMediaFileRepository(string mediaLibraryName)
        {
            mMediaLibraryName = mediaLibraryName;
        }


        /// <summary>
        /// Returns all media files in the media library.
        /// </summary>
        public IEnumerable<MediaFileInfo> GetMediaFiles()
        {
            return MediaFileInfoProvider.GetMediaFiles()
                .WhereEquals("FileLibraryID", Library.LibraryID)
                .ToList();
        }
    }
}