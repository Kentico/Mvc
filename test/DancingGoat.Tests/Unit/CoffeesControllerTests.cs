using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Ecommerce;
using CMS.Tests;

using DancingGoat.Controllers;
using DancingGoat.Models.Coffees;
using DancingGoat.Models.Products;
using DancingGoat.Repositories;
using DancingGoat.Services;
using DancingGoat.Tests.Extensions;
using Kentico.Ecommerce;

using NSubstitute;
using NUnit.Framework;

using Tests.DocumentEngine;
using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class CoffeesControllerTests : UnitTests
    {
        private const string COFFEE_TITLE1 = "Coffee1";
        private const string COFFEE_TITLE2 = "Coffee2";

        private CoffeesController mController;
        private CoffeeFilterViewModel mFilter;
        private CoffeeFilterViewModel mFilter2;
        private CoffeeFilterViewModel mFilter3;


        [SetUp]
        public void SetUp()
        {
            var price = new ProductPrice();
            var shoppingService = Substitute.For<ShoppingService>();
            var pricingService = Substitute.For<PricingService>();
            var calculationService = Substitute.For<CalculationService>(shoppingService, pricingService);
            var skuInfo = Substitute.For<SKUInfo>();
            
            calculationService.CalculateListingPrice(skuInfo).Returns(price);

            var repository = MockDataSource(skuInfo);
            mController = new CoffeesController(repository, calculationService);
        }


        [Test]
        public void Index_RendersDefaultView()
        {
            mController.WithCallTo(c => c.Index())
                .ShouldRenderDefaultView();
        }


        [Test]
        public void Filter_NonAjaxCall_ReturnsHttpNotFoundResult()
        {
            MockHttpContext(false);

            mController.WithCallTo(c => c.Filter(mFilter))
                .ShouldGiveHttpStatus(HttpStatusCode.NotFound);
               
        }

        [Test]
        public void Filter_ApplyNonRestrictiveFilter_RendersDefaultViewWithAllData()
        {
            MockHttpContext(true);

            mController.WithCallTo(c => c.Filter(mFilter))
                .ShouldRenderPartialView("CoffeeList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models => 
                    models.Any(item => item.Name == COFFEE_TITLE1)
                    && models.Any(y => y.Name == COFFEE_TITLE2)
                );
        }


        [Test]
        public void Filter_ApplyRestrictiveFilter_RendersDefaultViewWithFilteredData()
        {
            MockHttpContext(true);

            mController.WithCallTo(c => c.Filter(mFilter2))
                .ShouldRenderPartialView("CoffeeList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models => 
                    models.Any(item => item.Name == COFFEE_TITLE2) 
                    && models.Count() == 1
                );
        }


        [Test]
        public void Filter_ApplyFullRestrictiveFilter_RendersDefaultViewWithoutData()
        {
            MockHttpContext(true);

            mController.WithCallTo(c => c.Filter(mFilter3))
                .ShouldRenderPartialView("CoffeeList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models => !models.Any());
        }


        private ICoffeeRepository MockDataSource(SKUInfo skuInfo)
        {
            Fake().DocumentType<Coffee>(Coffee.CLASS_NAME);

            var coffee1 = TreeNode.New<Coffee>().With(x =>
            {
                x.Fields.IsDecaf = false;
                x.DocumentName = COFFEE_TITLE1;
                x.SKU = skuInfo;
            });

            var coffee2 = TreeNode.New<Coffee>().With(x =>
            {
                x.Fields.IsDecaf = true;
                x.DocumentName = COFFEE_TITLE2;
                x.SKU = skuInfo;
            });

            var repository = Substitute.For<ICoffeeRepository>();

            // Filter without restriction
            mFilter = Substitute.For<CoffeeFilterViewModel>();
            repository.GetCoffees(mFilter).Returns(new List<Coffee> { coffee1, coffee2 });

            // Filter for decafed coffees
            mFilter2 = Substitute.For<CoffeeFilterViewModel>();
            repository.GetCoffees(mFilter2).Returns(new List<Coffee> { coffee2 });

            // There is no coffee for this filter
            mFilter3 = Substitute.For<CoffeeFilterViewModel>();
            repository.GetCoffees(mFilter3).Returns(new List<Coffee>());

            return repository;
        }


        private void MockHttpContext(bool isAjaxPostback)
        {
            var httpContextSub = Substitute.For<HttpContextBase>();
            var requestSub = Substitute.For<HttpRequestBase>();
            httpContextSub.Request.Returns(requestSub);

            if (isAjaxPostback)
            {
                requestSub.Headers.Returns(new System.Collections.Specialized.NameValueCollection { { "X-Requested-With", "XMLHttpRequest" } });
            }
            
            mController.ControllerContext = new ControllerContext(httpContextSub, new RouteData(), mController);
        }
    }
}
