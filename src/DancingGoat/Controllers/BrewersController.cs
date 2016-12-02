using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DancingGoat.Models.Brewers;
using DancingGoat.Models.Products;
using DancingGoat.Repositories;
using DancingGoat.Repositories.Filters;
using DancingGoat.Services;

namespace DancingGoat.Controllers
{
    public class BrewersController : Controller
    {
        private readonly IBrewerRepository mBrewerRepository;
        private readonly ICalculationService mCalculationService;


        public BrewersController(IBrewerRepository brewerRepository, ICalculationService calculationService)
        {
            mBrewerRepository = brewerRepository;
            mCalculationService = calculationService;
        }


        // GET: Brewers
        public ActionResult Index()
        {
            var item = GetFilteredBrewers(null);
            var filter = new BrewerFilterViewModel(mBrewerRepository);
            filter.Load();

            var model = new ProductListViewModel
            {
                Filter = filter,
                Items = item
            };

            return View(model);
        }


        // POST: Filter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(BrewerFilterViewModel filter)
        {
            if (!Request.IsAjaxRequest())
            {
                return HttpNotFound();
            }

            var items = GetFilteredBrewers(filter);

            return PartialView("BrewersList", items);
        }


        private IEnumerable<ProductListItemViewModel> GetFilteredBrewers(IRepositoryFilter filter)
        {
            var brewers = mBrewerRepository.GetBrewers(filter);

            var items = brewers.Select(
                brewer => new ProductListItemViewModel(
                    brewer,
                    mCalculationService.CalculateListingPrice(brewer.SKU),
                    brewer.Product.PublicStatus?.PublicStatusDisplayName));
            return items;
        }
    }
}