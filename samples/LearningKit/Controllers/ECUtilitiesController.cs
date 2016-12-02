using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMS.Ecommerce;
using CMS.SiteProvider;
using Kentico.Ecommerce;

namespace LearningKit.Controllers
{
    /// <summary>
    /// Controller providing EC utilities to ease the setup and use of Learning Kit EC implementations.
    /// </summary>
    /// <remarks>
    /// This is a temporary solution and this controller will be deleted before making the kit public.
    /// </remarks>
    public class ECUtilitiesController : Controller
    {
        private readonly IShoppingService mService;

        public ECUtilitiesController()
        {
            mService = new ShoppingService();
        }


        /// <summary>
        /// Randomly fills a cart with relevant products from COM_SKU. 
        /// </summary>
        /// <remarks>
        /// To modify which products are relevant, edit <see cref="GetRelevantSKUIDs"/>.
        /// </remarks>
        public ActionResult FillShoppingCart()
        {
            var cart = mService.GetCurrentShoppingCart();

            var SKUIDs = GetRelevantSKUIDs();

            if (SKUIDs.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    int chosenSKUID = new Random().Next(0, SKUIDs.Count - 1);
                    int units = new Random().Next(1, 6);

                    cart.AddItem(SKUIDs[chosenSKUID], units);
                }
            }

            cart.Save();

            return RedirectToAction("ShoppingCart", "Checkout");
        }


        /// <summary>
        /// If COM_SKU is empty or has less than 3 records (SKUs), creates up to 3 sample SKUs.
        /// </summary>
        public ActionResult CreateSampleSKUs()
        {
            var SKUIDs = GetRelevantSKUIDs();

            if (SKUIDs.Count < 3)
            {
                for (int i = 0; i < (3 - SKUIDs.Count); i++)
                {
                    SKUInfoProvider.SetSKUInfo(new SKUInfo()
                    {
                        SKUName = "SampleProduct No. " + (i + 1),
                        SKUDescription = "This is a sample product for MVC Learning Kit.",
                        SKUShortDescription = "LearningKit_SampleData",
                        SKUPrice = 15.99 + new Random().Next(1, 25),
                        SKUSiteID = 1,
                        SKUEnabled = true,
                        SKUTrackInventory = TrackInventoryTypeEnum.ByProduct,
                        SKUAvailableItems = 100
                    });
                }
            }

            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Deletes all sample SKUs created by <see cref="CreateSampleSKUs"/>.
        /// </summary>
        public ActionResult DeleteSampleSKUs()
        {
            var sampleSKUs = SKUInfoProvider.GetSKUs(1).WhereEquals("SKUShortDescription", "LearningKit_SampleData");

            foreach (var SKU in sampleSKUs)
            {
                SKUInfoProvider.DeleteSKUInfo(SKU);
            }

            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Removes all items from the current shopping cart.
        /// </summary>
        public ActionResult RemoveAllItemsFromShoppingCart()
        {
            var cart = mService.GetCurrentShoppingCart();

            cart.RemoveAllItems();

            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Gets relevant SKUs from the database based on the query.
        /// </summary>
        /// <returns>List of IDs of relevant SKUs.</returns>
        private List<int> GetRelevantSKUIDs()
        {
            return SKUInfoProvider.GetSKUs(SiteContext.CurrentSiteID)
                .WhereTrue("SKUEnabled")
                .WhereNull("SKUOptionCategoryID")
                .WhereEquals("SKUProductType", "PRODUCT")
                .WhereEquals("SKUTrackInventory", "ByProduct")
                .OrderBy("SKUID")
                .Select(sku => sku.SKUID)
                .ToList();
        }
    }
}