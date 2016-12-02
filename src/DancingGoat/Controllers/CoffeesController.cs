using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DancingGoat.Models.Coffees;
using DancingGoat.Models.Products;
using DancingGoat.Repositories;
using DancingGoat.Repositories.Filters;
using DancingGoat.Services;

namespace DancingGoat.Controllers
{
    public class CoffeesController : Controller
    {
        private readonly ICoffeeRepository mCoffeeRepository;
        private readonly ICalculationService mCalculationService;


        public CoffeesController(ICoffeeRepository coffeeRepository, ICalculationService calculationService)
        {
            mCoffeeRepository = coffeeRepository;
            mCalculationService = calculationService;
        }


        // GET: Coffees
        public ActionResult Index()
        {
            var items = GetFilteredCoffees(null);
            var filter = new CoffeeFilterViewModel();
            filter.Load();

            return View(new ProductListViewModel
            {
                Filter = filter,
                Items = items
            });
        }


        // POST: Filter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(CoffeeFilterViewModel filter)
        {
            if (!Request.IsAjaxRequest())
            {
                return HttpNotFound();
            }
               
            var items = GetFilteredCoffees(filter);

            return PartialView("CoffeeList", items);
        }


        private IEnumerable<ProductListItemViewModel> GetFilteredCoffees(IRepositoryFilter filter)
        {
            var coffees = mCoffeeRepository.GetCoffees(filter);
           
            var items = coffees.Select(
                coffee => new ProductListItemViewModel(
                    coffee,
                    mCalculationService.CalculateListingPrice(coffee.SKU),
                    coffee.Product.PublicStatus?.PublicStatusDisplayName));
            return items;
        }
    }
}