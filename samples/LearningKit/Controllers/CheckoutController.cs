using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

using CMS.Ecommerce;
using CMS.Globalization;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;

using Kentico.Ecommerce;

using LearningKit.Models.Checkout;

namespace LearningKit.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly string siteName = SiteContext.CurrentSiteName;

        //DocSection:Constructor
        private readonly IShoppingService shoppingService;
        private readonly IPricingService pricingService;
        private readonly IPaymentMethodRepository paymentRepository;
        private readonly ICustomerAddressRepository addressRepository;
        private readonly IShippingOptionRepository shippingOptionRepository;
        
        /// <summary>
        /// Constructor.
        /// You can use a dependency injection container to initialize the services and repositories.
        /// </summary>
        public CheckoutController()
        {
            shoppingService = new ShoppingService();
            pricingService = new PricingService();
            paymentRepository = new KenticoPaymentMethodRepository();
            addressRepository = new KenticoCustomerAddressRepository();
            shippingOptionRepository = new KenticoShippingOptionRepository();
        }
        //EndDocSection:Constructor
        
        //DocSection:DisplayCart
        /// <summary>
        /// Displays the current site's shopping cart.
        /// </summary>
        public ActionResult ShoppingCart()
        {
            // Initializes the shopping cart model
            ShoppingCartViewModel model = new ShoppingCartViewModel();
            
            // Gets the current user's shopping cart
            model.Cart = shoppingService.GetCurrentShoppingCart();
            model.RemainingAmountForFreeShipping = pricingService.CalculateRemainingAmountForFreeShipping(model.Cart);
            
            // Displays the shopping cart
            return View(model);
        }
        //EndDocSection:DisplayCart

        //DocSection:AddItem
        /// <summary>
        /// Adds products to the current site's shopping cart.
        /// </summary>
        /// <param name="itemSkuId">ID of the added product (its SKU object).</param>
        /// <param name="itemUnits">Number of added units.</param>
        [HttpPost]
        public ActionResult AddItem(int itemSkuId, int itemUnits)
        {
            // Gets the current user's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // Adds a specified number of units of a specified product
            cart.AddItem(itemSkuId, itemUnits);
            
            // Displays the shopping cart
            return RedirectToAction("ShoppingCart");
        }
        //EndDocSection:AddItem
        
        //DocSection:UpdateItem
        /// <summary>
        /// Updates number of units of shopping cart items in the current site's shopping cart.
        /// </summary>
        /// <param name="itemID">ID of the updated shopping cart item.</param>
        /// <param name="itemUnits">Result number of units of the shopping cart item.</param>
        [HttpPost]
        public ActionResult UpdateItem(int itemID, int itemUnits)
        {
            // Gets the current user's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // Updates a specified product with a specified number of units
            cart.UpdateQuantity(itemID, itemUnits);
            
            // Displays the shopping cart
            return RedirectToAction("ShoppingCart");
        }
        //EndDocSection:UpdateItem

        //DocSection:RemoveItem
        /// <summary>
        /// Removes a shopping cart item from the current site's shopping cart.
        /// </summary>
        /// <param name="itemID">ID of the removed shopping cart item.</param>
        [HttpPost]
        public ActionResult RemoveItem(int itemID)
        {
            // Gets the current user's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // Removes a specified product from the shopping cart
            cart.RemoveItem(itemID);
            
            // Displays the shopping cart
            return RedirectToAction("ShoppingCart");
        }
        //EndDocSection:RemoveItem

        //DocSection:DetailUrl
        /// <summary>
        /// Redirects to a product detail page based on the ID of a product's SKU object.
        /// </summary>
        /// <param name="skuID">ID of the product's SKU object.</param>
        public ActionResult ItemDetail(int skuID)
        {
            // Gets the SKU object
            SKUInfo sku = SKUInfoProvider.GetSKUInfo(skuID);
            
            // If the SKU does not exist or it is a product option, returns error 404
            if (sku == null || sku.IsProductOption)
            {
                return HttpNotFound();
            }
            
            // If the SKU is a product variant, uses its parent product's ID
            if (sku.IsProductVariant)
            {
                skuID = sku.SKUParentSKUID;
            }
            
            // Gets the product's page
            TreeNode node = DocumentHelper.GetDocuments()
                .LatestVersion(false)
                .Published(true)
                .OnSite(siteName)
                .Culture("en-us")
                .CombineWithDefaultCulture()
                .WhereEquals("NodeSKUID", skuID)
                .FirstOrDefault();
            
            // If no page for the product exists, returns error 404
            if (node == null)
            {
                return HttpNotFound();
            }
            
            // Redirects to product detail page action method with the product information
            return RedirectToAction("Detail", "Product", new
                                                         {
                                                             id = node.NodeID,
                                                             productAlias = node.NodeAlias            
                                                         });
        }
        //EndDocSection:DetailUrl

        //DocSection:Checkout
        /// <summary>
        /// Validates the shopping cart and proceeds to the next checkout step with customer details.
        /// </summary>
        [HttpPost]
        [ActionName("ShoppingCart")]
        public ActionResult ShoppingCartCheckout()
        {
            // Gets the current user's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // Validates the shopping cart
            ShoppingCartCheckResult checkResult = cart.ValidateContent();
            
            // If the validation is successful, redirects to the next step of the checkout process
            if (!checkResult.CheckFailed)
            {
                return RedirectToAction("DeliveryDetails");
            }
            
            // If the validation fails, redirects back to shopping cart
            return RedirectToAction("ShoppingCart");
        }
        //EndDocSection:Checkout
        
        //DocSection:CouponCode
        /// <summary>
        /// Applies the specified coupon code to the specified shopping cart.
        /// </summary>
        /// <param name="cart">Shopping cart to which the coupon code is applied.</param>
        /// <param name="couponCode">Coupon code to be applied.</param>
        [HttpPost]
        private bool ApplyCouponCode(ShoppingCart cart, string couponCode)
        {
            // Assigns the coupon code to the shopping cart
            cart.CouponCode = couponCode;
            cart.Save();
            
            // Returns whether the code is valid (i.e. whether the coupon code applies any discount or is empty)
            return cart.HasUsableCoupon;
        }
        //EndDocSection:CouponCode
        
        //DocSection:DisplayDelivery
        /// <summary>
        /// Displays the customer detail checkout process step without any additional functionality for registered customers.
        /// </summary>
        public ActionResult DeliveryDetails()
        {
            // Gets the current user's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // If the shopping cart is empty, displays the shopping cart
            if (cart.IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }
            
            // Gets all countries for the country selector
            SelectList countries = new SelectList(CountryInfoProvider.GetAllCountries(), "CountryID", "CountryDisplayName");
            
            // Gets all enabled shipping options for the shipping option selector
            SelectList shippingOptions = new SelectList(shippingOptionRepository.GetAllEnabled(), "ShippingOptionID", "ShippingOptionDisplayName");
            
            // Loads the customer details
            DeliveryDetailsViewModel model = new DeliveryDetailsViewModel
            {
                Customer = new CustomerModel(cart.Customer),
                BillingAddress = new BillingAddressModel(cart.BillingAddress, countries, null),
                ShippingOption = new ShippingOptionModel(cart.ShippingOption, shippingOptions)
            };
            
            // Displays the customer details step
            return View(model);
        }
        //EndDocSection:DisplayDelivery
        
        //DocSection:DisplayDeliveryAddressSelector
        /// <summary>
        /// Displays the customer detail checkout process step with an address selector for registered customers.
        /// </summary>
        public ActionResult DeliveryDetailsAddressSelector()
        {
            // Gets the current user's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // If the shopping cart is empty, displays the shopping cart
            if (cart.IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }
            
            // Gets all countries for the country selector
            SelectList countries = new SelectList(CountryInfoProvider.GetAllCountries(), "CountryID", "CountryDisplayName");
            
            // Gets the current customer
            Customer customer = cart.Customer;
            
            // Gets all customer billing addresses for the address selector
            IEnumerable<CustomerAddress> customerAddresses = Enumerable.Empty<CustomerAddress>();
            if (customer != null)
            {
                customerAddresses = addressRepository.GetByCustomerId(customer.ID);
            }
            
            // Prepares address selector options
            SelectList addresses = new SelectList(customerAddresses, "ID", "Name");
            
            // Gets all enabled shipping options for the shipping option selector
            SelectList shippingOptions = new SelectList(shippingOptionRepository.GetAllEnabled(), "ShippingOptionID", "ShippingOptionDisplayName");
            
            // Loads the customer details
            DeliveryDetailsViewModel model = new DeliveryDetailsViewModel
            {
                Customer = new CustomerModel(cart.Customer),
                BillingAddress = new BillingAddressModel(cart.BillingAddress, countries, addresses),
                ShippingOption = new ShippingOptionModel(cart.ShippingOption, shippingOptions)
            };
            
            // Displays the customer details step
            return View(model);
        }
        //EndDocSection:DisplayDeliveryAddressSelector
        
        //DocSection:PostDelivery
        /// <summary>
        /// Validates the entered customer details and proceeds to the next checkout process step with the preview of the order.
        /// </summary>
        /// <param name="model">View model with the customer details.</param>
        [HttpPost]
        public ActionResult DeliveryDetails(DeliveryDetailsViewModel model)
        {
            // Gets the current user's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // Gets all enabled shipping options for the shipping option selector
            SelectList shippingOptions = new SelectList(shippingOptionRepository.GetAllEnabled(), "ShippingOptionID", "ShippingOptionDisplayName");
            
            // If the ModelState is not valid, assembles the country list and the shipping option list and displays the step again
            if (!ModelState.IsValid)
            {
                SelectList countries = new SelectList(CountryInfoProvider.GetAllCountries(), "CountryID", "CountryDisplayName");
                model.BillingAddress.Countries = countries;
                model.ShippingOption.ShippingOptions = new ShippingOptionModel(cart.ShippingOption, shippingOptions).ShippingOptions;
                return View(model);
            }
            
            // Gets the shopping cart's customer and applies the customer details from the checkout process step
            if (cart.Customer == null)
            {
                cart.Customer = new Customer();
            }
            model.Customer.ApplyToCustomer(cart.Customer);
            
            // Gets the shopping cart's billing address and applies the billing address from the checkout process step
            cart.BillingAddress = addressRepository.GetById(model.BillingAddress.AddressID) ?? new CustomerAddress();
            model.BillingAddress.ApplyTo(cart.BillingAddress);
            
            // Sets the address personal name and saves the shopping cart
            cart.BillingAddress.PersonalName = $"{cart.Customer.FirstName} {cart.Customer.LastName}";
            cart.Save();
            
            // Redirects to the next step of the checkout process
            return RedirectToAction("PreviewAndPay");
        }
        //EndDocSection:PostDelivery
        
        //DocSection:LoadingStates
        /// <summary>
        /// Loads states of the specified country.
        /// </summary>
        /// <param name="countryId">ID of the selected country.</param>
        /// <returns>Serialized display names of the loaded states.</returns>
        [HttpPost]
        public JsonResult CountryStates(int countryId)
        {
            // Gets the display names of the country's states
            var responseModel = StateInfoProvider.GetCountryStates(countryId)
                .Select(s => new
                {
                    id = s.StateID,
                    name = HTMLHelper.HTMLEncode(s.StateDisplayName)
                });
            
            // Returns serialized display names of the states
            return Json(responseModel);
        }
        //EndDocSection:LoadingStates

        
        //DocSection:LoadingAddress
        /// <summary>
        /// Loads information of an address specified by its ID.
        /// </summary>
        /// <param name="addressID">ID of the address.</param>
        /// <returns>Serialized information of the loaded address.</returns>
        [HttpPost]
        public JsonResult CustomerAddress(int addressID)
        {
            // Gets the address with its ID
            CustomerAddress address = addressRepository.GetById(addressID);
            
            // Checks whether the address was retrieved
            if (address == null)
            {
                return null;
            }
            
            // Creates a response with all address information
            var responseModel = new
            {
                Line1 = address.Line1,
                Line2 = address.Line2,
                City = address.City,
                PostalCode = address.PostalCode,
                CountryID = address.CountryID,
                StateID = address.StateID,
                PersonalName = address.PersonalName
            };
            
            // Returns serialized information of the address
            return Json(responseModel);
        }
        //EndDocSection:LoadingAddress
        
        //DocSection:PreparePayment
        /// <summary>
        /// Decides whether the specified payment method is valid on the current site.
        /// </summary>
        /// <param name="paymentMethodID">ID of the applied payment method.</param>
        /// <returns>True if the payment method is valid.</returns>
        private bool IsPaymentMethodValid(int paymentMethodID)
        {
            // Gets the current user'S shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // Gets a list of all applicable payment methods to the current user's shopping cart
            List<PaymentOptionInfo> paymentMethods = GetApplicablePaymentMethods(cart).ToList();
            
            // Returns whether an applicable payment method exists with the entered payment method's ID
            return paymentMethods.Exists(p => p.PaymentOptionID == paymentMethodID);
        }
        
        /// <summary>
        /// Gets all applicable payment methods on the current site.
        /// </summary>
        /// <param name="cart">Shopping cart of the site</param>
        /// <returns>Collection of applicable payment methods</returns>
        private IEnumerable<PaymentOptionInfo> GetApplicablePaymentMethods(ShoppingCart cart)
        {
            // Gets all enabled payment methods from Kentico
            IEnumerable<PaymentOptionInfo> enabledPaymentMethods = paymentRepository.GetAll();
            
            // Returns all applicable payment methods
            return enabledPaymentMethods.Where(cart.IsPaymentMethodApplicable);
        }
        //EndDocSection:PreparePayment
        
        //DocSection:PreparePreview
        /// <summary>
        /// Prepares a view model of the preview checkout process step including the shopping cart,
        /// the customer details, and the payment method.
        /// </summary>
        /// <returns>View model with information about the future order.</returns>
        private PreviewAndPayViewModel PreparePreviewViewModel()
        {
            // Gets the current user's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // Prepares the customer details
            DeliveryDetailsViewModel deliveryDetailsModel = new DeliveryDetailsViewModel
            {
                Customer = new CustomerModel(cart.Customer),
                BillingAddress = new BillingAddressModel(cart.BillingAddress, null, null)
            };
            
            // Prepares the payment method
            PaymentMethodViewModel paymentViewModel = new PaymentMethodViewModel
            {
                PaymentMethods = new SelectList(GetApplicablePaymentMethods(cart), "PaymentOptionID", "PaymentOptionDisplayName")
            };
            
            // Gets the selected payment method if any
            PaymentOptionInfo paymentMethod = cart.PaymentMethod;
            if (paymentMethod != null)
            {
                paymentViewModel.PaymentMethodID = paymentMethod.PaymentOptionID;
            }
            
            // Prepares a model from the preview step
            PreviewAndPayViewModel model = new PreviewAndPayViewModel
            {
                DeliveryDetails = deliveryDetailsModel,
                Cart = cart,
                PaymentMethod = paymentViewModel
            };
            
            return model;
        }
        //EndDocSection:PreparePreview
        
        //DocSection:DisplayPreview
        /// <summary>
        /// Display the preview checkout process step.
        /// </summary>
        public ActionResult PreviewAndPay()
        {
            // Gets the current user's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // If the cart is empty, returns to the shopping cart
            if (cart.IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }
            
            // Prepares a model from the preview step
            PreviewAndPayViewModel model = PreparePreviewViewModel();
            
            // Displays the preview step
            return View(model);
        }
        //EndDocSection:DisplayPreview
        
        //DocSection:PostPreview
        /// <summary>
        /// Validates that all information is correct to create an order, creates an order,
        /// and redirects the customer to payment.
        /// </summary>
        /// <param name="model">View model with information about the future order.</param>
        [HttpPost]
        public ActionResult PreviewAndPay(PreviewAndPayViewModel model)
        {
            // Gets the current user's shopping cart
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            
            // Validates the shopping cart
            ShoppingCartCheckResult checkResult = cart.ValidateContent();
            
            // Gets the selected payment method and assigns it to the shopping cart
            cart.PaymentMethod = paymentRepository.GetById(model.PaymentMethod.PaymentMethodID);
            
            // If the validation was not successful, displays the preview step again
            if (checkResult.CheckFailed || !IsPaymentMethodValid(model.PaymentMethod.PaymentMethodID))
            {
                // Prepares a model from the preview step
                PreviewAndPayViewModel viewModel = PreparePreviewViewModel();
                
                // Displays the preview step again
                return View("PreviewAndPay", viewModel);
            }
            
            // Creates an order from the shopping cart
            Order order = shoppingService.CreateOrder(cart);
            
            // Deletes the shopping cart from the database
            shoppingService.DeleteShoppingCart(cart);
            
            // Redirects to the payment gateway
            return RedirectToAction("Index", "Payment", new { orderID = order.OrderID });
        }
        //EndDocSection:PostPreview
        
        /// <summary>
        /// Displays a thank-you page, where user is redirected after creating an order.
        /// </summary>
        /// <param name="orderID">ID of the created order.</param>
        public ActionResult ThankYou(int orderID = 0)
        {
            ViewBag.OrderID = orderID;

            return View();
        }
    }
}
