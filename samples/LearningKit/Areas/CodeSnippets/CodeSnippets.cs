using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

using CMS.Ecommerce;
using CMS.ContactManagement;
using CMS.Personas;

using Kentico.Ecommerce;
using Kentico.Activities;
using Kentico.ContactManagement;
using Kentico.Search;

namespace LearningKit.Areas.CodeSnippets
{
    /// <summary>
    /// This is a dummy class with code snippets used in the Kentico documentation.
    /// These code snippets do NOT take any part in the runnable LearningKit project.
    /// </summary>
    public class CodeSnippets : Controller
    {
        //DocSection:DifferentShippingAddress
        public bool ShippingAddressIsDifferent { get; set; }
        //EndDocSection:DifferentShippingAddress

        private object DummyEcommerceMethod()
        {
            IPricingService pricingService = null;
            ShoppingCart shoppingCart = null;
            SKUInfo productSku = null;
            bool applyDiscounts = false;
            bool applyTaxes = false;
            Variant variant = null;
            IVariantRepository mVariantRepository = null;
            SKUTreeNode product = null;
            SKUInfo sku = null;
            DummyViewModel model = null;
            Order order = null;
            PaymentResultInfo result = null;

            //DocSection:CalculatePriceOptions
            ProductPrice productPrice = pricingService.CalculatePrice(productSku, shoppingCart, applyDiscounts, applyTaxes);
            //EndDocSection:CalculatePriceOptions

            //DocSection:FormatPriceOptions
            decimal price = 5.50M;
            string formattedPrice = shoppingCart.Currency.FormatPrice(price);
            //EndDocSection:FormatPriceOptions
            
            //DocSection:VariantDisplayImg
            var response = new
            {
                // ...
                
                imagePath = Url.Content(variant.ImagePath)
            };
            //EndDocSection:VariantDisplayImg

            //DocSection:DisplayAttributeSelection
            // Gets the cheapest variant from the product
            List<Variant> variants = mVariantRepository.GetByProductId(product.NodeSKUID).OrderBy(v => v.VariantPrice).ToList();
            Variant cheapestVariant = variants.FirstOrDefault();
            
            // Gets the product's option categories.
            IEnumerable<ProductOptionCategory> categories = mVariantRepository.GetVariantOptionCategories(sku.SKUID);
            
            // Gets the cheapest variant's selected attributes
            IEnumerable<ProductOptionCategoryViewModel> variantCategories = cheapestVariant?.ProductAttributes.Select(
                option =>
                    new ProductOptionCategoryViewModel(option.SKUID,
                        categories.FirstOrDefault(c => c.ID == option.SKUOptionCategoryID)));
            //EndDocSection:DisplayAttributeSelection

            //DocSection:ShippingIsDifferent
            if (model.ShippingAddressIsDifferent)
            {
                // ...
            }
            //EndDocSection:ShippingIsDifferent

            //DocSection:DifferentPaymentMethods
            if (shoppingCart.PaymentMethod.PaymentOptionName.Equals("PaymentMethodCodeName"))
            {
                return RedirectToAction("ActionForPayment", "MyPaymentGateway");
            }
            //EndDocSection:DifferentPaymentMethods

            //DocSection:SetPaymentResult
            order.SetPaymentResult(result, true);
            //EndDocSection:SetPaymentResult

            //DocSection:RedirectForManualPayment
            return RedirectToAction("ThankYou", new { orderID = order.OrderID });
            //EndDocSection:RedirectForManualPayment
        }

        private object DummyEcommerceMethod2()
        {
            SKUInfo sku = null;
            IOrderRepository orderRepository = null;
            int orderId = 0;

            //DocSection:DisplayCatalogDiscounts
            // Initializes the needed services
            ShoppingService shoppingService = new ShoppingService();
            PricingService pricingService = new PricingService();
            
            // Gets the current shopping cart
            ShoppingCart shoppingCart = shoppingService.GetCurrentShoppingCart();
            
            // Calculates prices for the specified product
            ProductPrice price = pricingService.CalculatePrice(sku, shoppingCart, true, false);
            
            // Gets the catalog discount
            decimal catalogDiscount = price.Discount;
            //EndDocSection:DisplayCatalogDiscounts

            //DocSection:DisplayOrderList
            // Gets the current customer
            Customer currentCustomer = shoppingService.GetCurrentCustomer();
            
            // If the customer does not exist, returns error 404
            if (currentCustomer == null)
            {
                return HttpNotFound();
            }
            
            // Creates a view model representing a collection of the customer's orders
            OrdersViewModel model = new OrdersViewModel()
            {
                Orders = orderRepository.GetByCustomerId(currentCustomer.ID)
            };
            //EndDocSection:DisplayOrderList

            //DocSection:ReorderExistingOrder
            // Gets the order based on its ID
            Order order = orderRepository.GetById(orderId);
            
            // Gets the current visitor's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // Loops through the items in the order and adds them to the shopping cart
            foreach (OrderItem item in order.OrderItems)
            {
                cart.AddItem(item.SKUID, item.Units);
            }
            
            // Saves the shopping cart
            cart.Save();
            //EndDocSection:ReorderExistingOrder

            return null;
        }

        private void DummyEcommerceMethod3()
        {
            //DocSection:DisplayFreeShippingOffers
            // Initializes the needed services
            ShoppingService shoppingService = new ShoppingService();
            PricingService pricingService = new PricingService();
            
            // Gets the current shopping cart
            ShoppingCart shoppingCart = shoppingService.GetCurrentShoppingCart();
            
            // Gets the remaining amount for free shipping
            decimal remainingFreeShipping = pricingService.CalculateRemainingAmountForFreeShipping(shoppingCart);
            //EndDocSection:DisplayFreeShippingOffers
        }

        private void DummyPersonalizationMethod()
        {
            //DocSection:PersonaPersonalization
            // Gets the current contact
            IContactTrackingService contactService = new ContactTrackingService();
            ContactInfo currentContact = contactService.GetCurrentContactAsync(User.Identity.Name).Result;
            
            // Gets the code name of the current contact's persona
            string currentPersonaName = currentContact.GetPersona()?.PersonaName;
            
            // Checks whether the current contact is assigned to the "EarlyAdopter" persona
            if (String.Equals(currentPersonaName, "EarlyAdopter", StringComparison.InvariantCultureIgnoreCase))
            {
                // Serve personalized content for the "EarlyAdopter" persona
            }
            //EndDocSection:PersonaPersonalization
        }

        private void DummySearchMethod()
        {
            IEnumerable<string> searchIndexes = null;
            string searchText = null;
            int PAGE_SIZE = 0;
            
            //DocSection:InitializeSearch
            ISearchService searchService = new SearchService();
            
            SearchResult searchResult = searchService.Search(new SearchOptions(searchText, searchIndexes)
            {
                CultureName = "en-us",
                CombineWithDefaultCulture = true,
                PageSize = PAGE_SIZE
            });
            //EndDocSection:InitializeSearch
        }

        private void DummyContactMethod()
        {
            //DocSection:GetCurrentContact
            // Gets the current contact
            IContactTrackingService contactService = new ContactTrackingService();
            ContactInfo currentContact = contactService.GetCurrentContactAsync(User.Identity.Name).Result;
            //EndDocSection:GetCurrentContact
        }
    }

    internal class ProductOptionCategoryViewModel
    {
        public ProductOptionCategoryViewModel(int skuId, ProductOptionCategory category)
        {
        }
    }

    internal class OrdersViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
    }

    internal class DummyViewModel
    {
        public bool ShippingAddressIsDifferent { get; set; }
    }

    //DocSection:ContactTrackingServiceInit
    public class MembershipController : Controller
    {
        private readonly IContactTrackingService contactTrackingService;
        
        public MembershipController()
        {
            contactTrackingService = new ContactTrackingService();
            
            // ...
        }
        
        // ...
    //EndDocSection:ContactTrackingServiceInit

        private string userName = "PlaceHolder";

        private async void DummyContactMergeMethod()
        {
            //DocSection:MergeContact
            await contactTrackingService.MergeUserContactsAsync(userName);
            //EndDocSection:MergeContact
        }
    }
}

namespace LearningKit.Areas.CodeSnippetDuplicates
{
    //DocSection:MembershipActivityLoggerInit
    public class MembershipController : Controller
    {
        private readonly IMembershipActivitiesLogger membershipActivityLogger;
        
        public MembershipController()
        {
            membershipActivityLogger = new MembershipActivitiesLogger();
            
            // ...
        }
        
        // ...
        //EndDocSection:MembershipActivityLoggerInit

        private string userName = "PlaceHolder";

        private void DummyLogActivityMethod()
        {
            //DocSection:LogRegisterActivity
            membershipActivityLogger.LogRegisterActivity(userName);
            //EndDocSection:LogRegisterActivity

            //DocSection:LogSignInActivity
            membershipActivityLogger.LogLoginActivity(userName);
            //EndDocSection:LogSignInActivity
        }
    }
}