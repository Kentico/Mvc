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
using DancingGoat.Models.Brewers;
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
    public class BrewersControllerTests : UnitTests
    {
        private const string BREWER_TITLE1 = "Brewer1";
        private const string BREWER_TITLE2 = "Brewer2";

        private BrewersController mController;
        private BrewerFilterViewModel mFilter;
        private BrewerFilterViewModel mFilter2;
        private BrewerFilterViewModel mFilter3;


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
            mController = new BrewersController(repository, calculationService);
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
                .ShouldRenderPartialView("BrewersList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models =>
                    models.Any(item => item.Name == BREWER_TITLE1)
                    && models.Any(y => y.Name == BREWER_TITLE2)
                );
        }


        [Test]
        public void Filter_ApplyRestrictiveFilter_RendersDefaultViewWithFilteredData()
        {
            MockHttpContext(true);

            mController.WithCallTo(c => c.Filter(mFilter2))
                .ShouldRenderPartialView("BrewersList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models =>
                    models.Any(item => item.Name == BREWER_TITLE2)
                    && models.Count() == 1
                );
        }


        [Test]
        public void Filter_ApplyFullRestrictiveFilter_RendersDefaultViewWithoutData()
        {
            MockHttpContext(true);

            mController.WithCallTo(c => c.Filter(mFilter3))
                .ShouldRenderPartialView("BrewersList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models => !models.Any());
        }


        private IBrewerRepository MockDataSource(SKUInfo skuInfo)
        {
            Fake().DocumentType<Brewer>(Brewer.CLASS_NAME);

            var brewer1 = TreeNode.New<Brewer>().With(x =>
            {
                x.DocumentName = BREWER_TITLE1;
                x.SKU = skuInfo;
            });

            var brewer2 = TreeNode.New<Brewer>().With(x =>
            {
                x.DocumentName = BREWER_TITLE2;
                x.SKU = skuInfo;
            });

            var repository = Substitute.For<IBrewerRepository>();

            mFilter = Substitute.For<BrewerFilterViewModel>();
            repository.GetBrewers(mFilter).Returns(new List<Brewer> { brewer1, brewer2 });

            mFilter2 = Substitute.For<BrewerFilterViewModel>();
            repository.GetBrewers(mFilter2).Returns(new List<Brewer> { brewer2 });

            mFilter3 = Substitute.For<BrewerFilterViewModel>();
            repository.GetBrewers(mFilter3).Returns(new List<Brewer>());

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
