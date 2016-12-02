using System.Collections.Generic;

namespace DancingGoat.Models.MediaGallery
{
    public class MediaGalleryViewModel
    {
        public string SiteName { get; set; }
        public string MediaGalleryName { get; set; }
        public IEnumerable<MediaFileViewModel> MediaFiles { get; set; } = new List<MediaFileViewModel>();


        public MediaGalleryViewModel(string siteName, string mediaGalleryName)
        {
            SiteName = siteName;
            MediaGalleryName = mediaGalleryName;
        }
    }
}