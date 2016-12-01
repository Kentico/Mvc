using System;
using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Helpers;
using CMS.Personas;

using DancingGoat.Infrastructure;
using DancingGoat.Models.Articles;
using DancingGoat.Models.Home;
using DancingGoat.Repositories;

using Kentico.ContactManagement;


namespace DancingGoat.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPromotedContentRepository mHighlightRepository;
        private readonly IHomeRepository mHomeRepository;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;
        private readonly IContactTrackingService mContactTrackingService;


        public HomeController(IPromotedContentRepository repository, 
            IHomeRepository homeRepository,
            IOutputCacheDependencies outputCacheDependencies,
            IContactTrackingService contactTrackingService)
        {
            mHighlightRepository = repository;
            mHomeRepository = homeRepository;
            mOutputCacheDependencies = outputCacheDependencies;
            mContactTrackingService = contactTrackingService;
        }


        // GET: Home
        [OutputCache(CacheProfile = "Default", VaryByParam = "none", VaryByCustom = DancingGoatApplication.CACHE_VARY_BY_PERSONA)]
        public ActionResult Index()
        {
            var viewModel = new IndexViewModel
            {
                Banner = GetBannerViewModel(),
                LatestArticles = mHighlightRepository.GetNewestArticles(5).Select(ArticleViewModel.GetViewModel),
                HomeSections = mHomeRepository.GetHomeSections().Select(HomeSectionViewModel.GetViewModel),
                CompanyCafes = mHighlightRepository.GetPromotedCompanyCafes(4)
            };

            mOutputCacheDependencies.AddDependencyOnPages<Article>();
            mOutputCacheDependencies.AddDependencyOnPages<HomeSection>();
            mOutputCacheDependencies.AddDependencyOnPages<Cafe>();

            return View(viewModel);
        }


        private BannerViewModel GetBannerViewModel()
        {
            var currentContact = mContactTrackingService.GetCurrentContactAsync(User.Identity.Name).Result;
            var currentPersonaName = currentContact.GetPersona()?.PersonaName;

            var banner = new BannerViewModel();

            // This functionality is based on Personas.
            // By default, you have no personas in database, i.e. the default banner data will be used.
            // To try this personalization feature, you need to create two personas with following names:
            //     1. "Tony, the Cafe owner"
            //     2. "Martina, the Coffee geek"
            // and assign them score and rules. When your contact matching one of the personas, 
            // personalized banner will be displayed.
            if (String.Equals(currentPersonaName, "Tony_TheCafeOwner", StringComparison.InvariantCultureIgnoreCase))
            {
                banner.BackgroundImagePath = Url.Content("~/Content/Images/banner-b2b.jpg");
                banner.Heading = ResHelper.GetString("DancingGoatMvc.Banner.Tony.Heading");
                banner.Text = ResHelper.GetString("DancingGoatMvc.Banner.Tony.Text");
            }
            else if (String.Equals(currentPersonaName, "Martina_TheCoffeeGeek", StringComparison.InvariantCultureIgnoreCase))
            {
                banner.BackgroundImagePath = Url.Content("~/Content/Images/banner-b2c.jpg");
                banner.Heading = ResHelper.GetString("DancingGoatMvc.Banner.Martina.Heading");
                banner.Text = ResHelper.GetString("DancingGoatMvc.Banner.Martina.Text");
            }
            else
            {
                banner.BackgroundImagePath = Url.Content("~/Content/Images/banner-default.jpg");
                banner.Heading = ResHelper.GetString("DancingGoatMvc.Banner.Default.Heading");
                banner.Text = ResHelper.GetString("DancingGoatMvc.Banner.Default.Text");
            }

            return banner;
        }
    }
}
