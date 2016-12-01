using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using DancingGoat.Infrastructure;
using DancingGoat.Models.MediaGallery;
using DancingGoat.Repositories;

namespace DancingGoat.Controllers
{
    public class AboutController : Controller
    {
        private readonly IAboutUsRepository mAboutUsRepository;
        private readonly IMediaFileRepository mMediaFileRepository;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;


        public AboutController(IAboutUsRepository aboutUsRepository, IMediaFileRepository mediaFileRepository, IOutputCacheDependencies outputCacheDependencies)
        {
            mAboutUsRepository = aboutUsRepository;
            mMediaFileRepository = mediaFileRepository;
            mOutputCacheDependencies = outputCacheDependencies;
        }


        // GET: About
        [OutputCache(CacheProfile = "Default", VaryByParam = "none")]
        public ActionResult Index()
        {
            var sideStories = mAboutUsRepository.GetSideStories();

            mOutputCacheDependencies.AddDependencyOnPages<AboutUsSection>();

            return View(sideStories);
        }


        [ChildActionOnly]
        public ActionResult MediaGallery()
        {
            var mediaFiles = mMediaFileRepository.GetMediaFiles();
            var mediaGallery = new MediaGalleryViewModel(mMediaFileRepository.SiteName, mMediaFileRepository.MediaLibraryDisplayName);
            mediaGallery.MediaFiles = mediaFiles.Select(MediaFileViewModel.GetViewModel);

            return PartialView("_MediaGallery", mediaGallery);
        }
    }
}