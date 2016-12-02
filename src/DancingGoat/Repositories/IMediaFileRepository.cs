using System.Collections.Generic;

using CMS.MediaLibrary;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of media files.
    /// </summary>
    public interface IMediaFileRepository : IRepository
    {
        /// <summary>
        /// Site's code name.
        /// </summary>
        string SiteName { get; }


        /// <summary>
        /// Returns display name of media library which stores the media files provided by the repository.
        /// </summary>
        string MediaLibraryDisplayName { get; }


        /// <summary>
        /// Returns all media files in the media library.
        /// </summary>
        IEnumerable<MediaFileInfo> GetMediaFiles();
    }
}