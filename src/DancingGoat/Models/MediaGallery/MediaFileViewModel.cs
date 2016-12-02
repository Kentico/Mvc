using System;

using CMS.MediaLibrary;
using Kentico.MediaLibrary;

namespace DancingGoat.Models.MediaGallery
{
    public class MediaFileViewModel
    {
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }


        public static MediaFileViewModel GetViewModel(MediaFileInfo mediaFileInfo)
        {
            return new MediaFileViewModel
            {
                Guid = mediaFileInfo.FileGUID,
                Title = mediaFileInfo.FileTitle,
                Name = mediaFileInfo.FileName,
                Url = mediaFileInfo.GetUrl()
            };
        }
    }
}