using System.Web.Mvc;
using System.Linq;

using CMS.Ecommerce;
using CMS.Helpers;

using Kentico.Ecommerce;

using DancingGoat.Models.Checkout;
using DancingGoat.ActionSelectors;
using DancingGoat.Repositories;
using DancingGoat.Services;

namespace DancingGoat.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IShoppingService mShoppingService;
        private readonly ICheckoutService mCheckoutService;
        private readonly IContactRepository mContactRepository;
        private readonly IProductRepository mProductRepository;


        public CheckoutController(IShoppingService shoppingService, IContactRepository contactRepository, IProductRepository productRepository, ICheckoutService checkoutService)
        {
            mShoppingService = shoppingService;
            mContactRepository = contactRepository;
            mCheckoutService = checkoutService;
            mProductRepository = productRepository;
        }


        public ActionResult ShoppingCart(CartViewModel cartViewModel)
        {
            var viewModel = mCheckoutService.PrepareCartViewModel();

            return View(viewModel);
        }


        [HttpPost]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult ShoppingCartCheckout(CartViewModel model)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            var checkResult = cart.ValidateContent();
                        
            cart.Evaluate();

            if (!checkResult.CheckFailed)
            {
                cart.Save();
                return RedirectToAction("DeliveryDetails");
            }
            
            // Fill model state with errors from the check result
            ProcessCheckResult(checkResult);

            var viewModel = mCheckoutService.PrepareCartViewModel(cart.AppliedCouponCodes);

            return View("ShoppingCart", viewModel);
        }


        [HttpPost]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult AddCouponCode(CouponCodesUpdateModel model)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();

            var couponCode = model.NewCouponCode;
            if (string.IsNullOrEmpty(couponCode))
            {
                return View("ShoppingCart", mCheckoutService.PrepareCartViewModel(cart.AppliedCouponCodes));
            }

            couponCode = couponCode.Trim();
            if (!cart.AddCouponCode(couponCode))
            {
                ModelState.AddModelError("NewCouponCode", ResHelper.GetString("DancingGoatMvc.Checkout.CouponCodeInvalid"));
            }            
            
            var cartViewModel = mCheckoutService.PrepareCartViewModel(cart.AppliedCouponCodes);
            return View("ShoppingCart", cartViewModel);
        }


        [HttpPost]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult RemoveCouponCode(CouponCodesUpdateModel model)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();

            cart.RemoveCouponCode(model.RemoveCouponCode);

            var cartViewModel = mCheckoutService.PrepareCartViewModel(cart.AppliedCouponCodes);
            return View("ShoppingCart", cartViewModel);
        }


        public ActionResult DeliveryDetails()
        {
            var cart = mShoppingService.GetCurrentShoppingCart();

            if (cart.IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }

            var viewModel = mCheckoutService.PrepareDeliveryDetailsViewModel();

            return View(viewModel);
        }


        public ActionResult PreviewAndPay()
        {
            var cart = mShoppingService.GetCurrentShoppingCart();

            if (cart.IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }

            var viewModel = mCheckoutService.PreparePreviewViewModel();

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PreviewAndPay(PreviewViewModel model)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();

            if (!mCheckoutService.IsPaymentMethodValid(model.PaymentMethod.PaymentMethodID))
            {
                ModelState.AddModelError("PaymentMethod.PaymentMethodID", ResHelper.GetString("DancingGoatMvc.Payment.PaymentMethodRequired"));
            }

            var checkResult = cart.ValidateContent();

            if (checkResult.CheckFailed)
            {
                ProcessCheckResult(checkResult);
            }

            if (cart.IsEmpty)
            {
                ModelState.AddModelError("cart.empty", ResHelper.GetString("DancingGoatMvc.Checkout.EmptyCartError"));
            }

            if (!ModelState.IsValid)
            {
                var viewModel = mCheckoutService.PreparePreviewViewModel(model.PaymentMethod);
                return View("PreviewAndPay", viewModel);
            }

            cart.PaymentMethod = mCheckoutService.GetPaymentMethod(model.PaymentMethod.PaymentMethodID);
            cart.Evaluate();

            mShoppingService.CreateOrder(cart);
            mShoppingService.DeleteShoppingCart(cart);

            return RedirectToAction("ThankYou");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult DeliveryDetails(DeliveryDetailsViewModel model)
        {
            // Check the selected shipping option
            if (!mCheckoutService.IsShippingOptionValid(model.ShippingOption.ShippingOptionID))
            {
                ModelState.AddModelError("ShippingOption.ShippingOptionID", ResHelper.GetString("DancingGoatMvc.Shipping.ShippingOptionRequired"));
            }

            // Check if the billing address's country and state are valid
            var countryStateViewModel = model.BillingAddress.BillingAddressCountryStateSelector;
            if (!mCheckoutService.IsCountryValid(countryStateViewModel.CountryID))
            {
                countryStateViewModel.CountryID = 0;
                ModelState.AddModelError("BillingAddress.BillingAddressCountryStateSelector.CountryID", ResHelper.GetString("DancingGoatMvc.Address.CountryIsRequired"));
            }
            else if (!mCheckoutService.IsStateValid(countryStateViewModel.CountryID, countryStateViewModel.StateID))
            {
                countryStateViewModel.StateID = 0;
                ModelState.AddModelError("BillingAddress.BillingAddressCountryStateSelector.StateID", ResHelper.GetString("DancingGoatMvc.Address.StateIsRequired"));
            }

            if (!ModelState.IsValid)
            {
                var viewModel = mCheckoutService.PrepareDeliveryDetailsViewModel(model.Customer, model.BillingAddress, model.ShippingOption);
                return View(viewModel);
            }
            
            var cart = mShoppingService.GetCurrentShoppingCart();
            var customer = cart.Customer ?? new Customer();

            bool emailCanBeChanged = !User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(customer.Email);
            model.Customer.ApplyToCustomer(customer, emailCanBeChanged);
            cart.Customer = customer;

            var modelAddressID = model.BillingAddress.BillingAddressSelector?.AddressID ?? 0;
            var billingAddress =  mCheckoutService.GetAddress(modelAddressID) ?? new CustomerAddress();
            
            model.BillingAddress.ApplyTo(billingAddress);
            billingAddress.PersonalName = $"{customer.FirstName} {customer.LastName}";

            cart.BillingAddress = billingAddress;
            cart.ShippingOption = mCheckoutService.GetShippingOption(model.ShippingOption.ShippingOptionID);

            cart.Evaluate();
            cart.Save();

            return RedirectToAction("PreviewAndPay");
        }


        public ActionResult ThankYou()
        {
            var companyContact = mContactRepository.GetCompanyContact();

            var viewModel = new ThankYouViewModel
            {
                Phone = companyContact.Phone
            };

            return View(viewModel);
        }


        [HttpPost]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult AddItem(CartItemUpdateModel item)
        {
            if (ModelState.IsValid)
            {
                var cart = mShoppingService.GetCurrentShoppingCart();
                cart.AddItem(item.SKUID, item.Units);
                cart.Evaluate();
            }

            return RedirectToAction("ShoppingCart");
        }


        [HttpPost]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult UpdateItem(CartItemUpdateModel item)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();

            if (ModelState.IsValid)
            {
                cart.UpdateQuantity(item.ID, item.Units);
                cart.Evaluate();
            }
            else
            {
                // Add an item error and save ViewData so that the ShoppingCart action can show validation errors
                var key = item.ID.ToString();
                ModelState.Add(key, ModelState["Units"]);
            }

            var cartViewModel = mCheckoutService.PrepareCartViewModel(cart.AppliedCouponCodes);
            return View("ShoppingCart", cartViewModel);
        }


        [HttpPost]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult RemoveItem(CartItemUpdateModel item)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            cart.RemoveItem(item.ID);
            cart.Evaluate();

            var cartViewModel = mCheckoutService.PrepareCartViewModel(cart.AppliedCouponCodes);
            return View("ShoppingCart", cartViewModel);
        }


        [HttpPost]
        public JsonResult CustomerAddress(int addressID)
        {
            var address = mCheckoutService.GetCustomerAddress(addressID);

            if (address == null)
            {
                return null;
            }

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

            return Json(responseModel);
        }


        public ActionResult ItemDetail(int skuId)
        {
            var product = mProductRepository.GetProductForSKU(skuId);

            if (product == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Detail", "Product", new
                                                         {
                                                             id = product.NodeID,
                                                             productAlias = product.NodeAlias
                                                         });
        }


        [HttpPost]
        public JsonResult CountryStates(int countryId)
        {
            var responseModel = mCheckoutService.GetCountryStates(countryId)
                .Select(s => new
                {
                    id = s.StateID,
                    name = HTMLHelper.HTMLEncode(s.StateDisplayName)
                });

            return Json(responseModel);
        }


        [HttpPost]
        public ActionResult ShippingChanged(int? paymentId)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            cart.PaymentMethod = mCheckoutService.GetPaymentMethod(paymentId ?? 0);
            cart.Evaluate();

            return PartialView("_ShoppingCartTotals", cart);
        }


        private void ProcessCheckResult(ShoppingCartCheckResult checkResult)
        {
            // Process the check result for each item
            foreach (var itemResult in checkResult.ItemResults)
            {
                // Get the item errors separated by a space and add them into the model state
                var error = itemResult.GetMessage(" ");

                // Use the item ID as an error unique identifier
                var key = itemResult.CartItem.CartItemID.ToString();
                ModelState.AddModelError(key, error);
            }

            if (checkResult.ShippingOptionNotAvailable)
            {
                ModelState.AddModelError("shipping", ResHelper.GetString("DancingGoatMvc.Shipping.NotApplicable"));
            }
        }
    }
}