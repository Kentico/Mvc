using System.Collections.Generic;
using System;

using CMS.Ecommerce;
using CMS.Globalization;
using CMS.SiteProvider;
using CMS.Membership;

namespace Kentico.Ecommerce.Tests
{
    public class EcommerceFakeFactory
    {
        public SiteInfo SiteInfo;
        public CurrencyInfo MainCurrency;
        public UserInfo DefaultUser;

        public CarrierInfo CarrierBasic;
        public ShippingOptionInfo ShippingOptionDefault;
        public PaymentOptionInfo PaymentMethodDefault,
            PaymentMethodDisabled;

        public OrderStatusInfo DefaultOrderStatus;

        public CountryInfo CountryUSA,
            CountryCZE;
        public StateInfo StateWashington;

        public CustomerInfo CustomerAnonymous;
        public AddressInfo CustomerAddressUSA,
            CustomerAddressCZE;

        public TaxClassInfo TaxClassDefault;
        public TaxClassCountryInfo TaxClassDefaultCountry;
        public TaxClassStateInfo TaxClassDefaultState;

        public SKUInfo SKUAvailable,
            SKUDisabled,
            SKULimited,
            SKUWithoutShipping,
            SKUWithTaxes;


        internal SiteInfo InitSite()
        {
            return SiteInfo = new SiteInfo
            {
                DisplayName = "Test site",
                SiteName = "TestSite",
                Status = SiteStatusEnum.Running,
                DomainName = "127.0.0.1"
            };
        }


        internal CurrencyInfo InitMainCurrency(int? siteId)
        {
            MainCurrency = new CurrencyInfo
            {
                CurrencyDisplayName = "Default US Dollar",
                CurrencyCode = "USD",
                CurrencyIsMain = true,
                CurrencyFormatString = "${0:F2}",
                CurrencyName = "Dollar",
                CurrencyEnabled = true,
                CurrencyRoundTo = 2,
            };

            if (siteId.HasValue)
            {
                MainCurrency.CurrencySiteID = siteId.Value;
            }

            return MainCurrency;
        }


        internal UserInfo InitUser()
        {
            var uniqueName = NewUniqueName();

            return DefaultUser = new UserInfo
            {
                UserEnabled = true,
                UserName = "DefaultName" + uniqueName,
                UserNickName = "NickName" + uniqueName,
                FirstName = "FirstName" + uniqueName,
                LastName = "LastName" + uniqueName,
                Email = "Email@Localhost.Local" + uniqueName
            };
        }


        internal IEnumerable<PaymentOptionInfo> InitPaymentMethods()
        {
            return new[]
            {
                PaymentMethodDefault = new PaymentOptionInfo
                {
                    PaymentOptionDisplayName = "Cash on delivery",
                    PaymentOptionName = "CashOnDelivery",
                    PaymentOptionEnabled = true,
                    PaymentOptionSiteID = SiteInfo.SiteID
                },

                PaymentMethodDisabled = new PaymentOptionInfo
                {
                    PaymentOptionDisplayName = "Money transfer",
                    PaymentOptionName = "MoneyTransfer",
                    PaymentOptionEnabled = false,
                    PaymentOptionSiteID = SiteInfo.SiteID
                }
            };
        }


        internal IEnumerable<CountryInfo> InitCountries()
        {
            return new[]
            {
                CountryUSA = new CountryInfo
                {
                    CountryName = "USA" +  Guid.NewGuid().ToString(),
                    CountryDisplayName = "USA",
                    CountryThreeLetterCode= "USA"
                },
                CountryCZE = new CountryInfo
                {
                    CountryName = "CZE" +  Guid.NewGuid().ToString(),
                    CountryDisplayName = "Czech republic",
                    CountryThreeLetterCode= "CZE"
                }
            };
        }


        internal IEnumerable<StateInfo> InitStates()
        {
            return new[]
            {
                StateWashington = new StateInfo
                {
                    CountryID = CountryUSA.CountryID,
                    StateName = "Washington" + Guid.NewGuid().ToString(),
                    StateDisplayName = "Washington",
                    StateCode = "RCW"
                }
            };
        }


        internal IEnumerable<OrderStatusInfo> InitOrderStatuses()
        {
            return new[]
            {
                DefaultOrderStatus = NewOrderStatus(1)
            };
        }


        internal IEnumerable<CustomerInfo> InitCustomers()
        {
            return new[]
            {
                CustomerAnonymous = new CustomerInfo
                {
                    CustomerFirstName = "John",
                    CustomerLastName = "Black",
                    CustomerEmail = "john.black@anonymous.local",
                    CustomerSiteID = SiteInfo.SiteID
                }
            };
        }


        internal IEnumerable<TaxClassInfo> InitTaxClasses()
        {
            return new[]
            {
                TaxClassDefault = new TaxClassInfo
                {
                    TaxClassDisplayName = "DefaultTaxClass",
                    TaxClassName = "DefaultTaxClass",
                    TaxClassSiteID = SiteInfo.SiteID
                }
            };
        }


        internal IEnumerable<TaxClassCountryInfo> InitTaxClassCountries()
        {
            return new[]
            {
                TaxClassDefaultCountry = new TaxClassCountryInfo
                {
                    CountryID = CountryUSA.CountryID,
                    TaxClassID = TaxClassDefault.TaxClassID,
                    TaxValue = 10
                }
            };
        }


        internal IEnumerable<TaxClassStateInfo> InitTaxClassStates()
        {
            return new[]
            {
                TaxClassDefaultState = new TaxClassStateInfo
                {
                    StateID = StateWashington.StateID,
                    TaxClassID = TaxClassDefault.TaxClassID,
                    TaxValue = 20
                }
            };
        }


        internal IEnumerable<CarrierInfo> InitCarriers()
        {
            return new[]
            {
                CarrierBasic = new CarrierInfo
                {
                    CarrierAssemblyName = "CMS.Ecommerce",
                    CarrierClassName = "CMS.Ecommerce.DefaultCarrierProvider",
                    CarrierName = "CarrierBasic",
                    CarrierDisplayName = "CarrierBasic",
                    CarrierSiteID = SiteInfo.SiteID
                }
            };
        }


        internal IEnumerable<AddressInfo> InitCustomerAddresses()
        {
            return new[]
            {
                CustomerAddressUSA = new AddressInfo
                {
                    AddressCity = "USACity",
                    AddressLine1 = "USALine1",
                    AddressLine2 = "USALine2",
                    AddressZip = "USAZip",
                    AddressCountryID = CountryUSA.CountryID,
                    AddressStateID = StateWashington.StateID,
                    AddressPersonalName = "USAPersonalName",
                    AddressName = "USAName",
                    AddressCustomerID = CustomerAnonymous.CustomerID
                },
                CustomerAddressCZE = new AddressInfo
                {
                    AddressCity = "CZECity",
                    AddressLine1 = "CZELine1",
                    AddressLine2 = "CZELine2",
                    AddressZip = "CZEZip",
                    AddressCountryID = CountryUSA.CountryID,
                    AddressStateID = 0,
                    AddressPersonalName = "CZEPersonalName",
                    AddressName = "CZEName",
                    AddressCustomerID = CustomerAnonymous.CustomerID
                }
            };
        }


        internal IEnumerable<ShippingOptionInfo> InitShippingOptions()
        {
            return new[]
            {
                ShippingOptionDefault = new ShippingOptionInfo
                {
                    ShippingOptionSiteID = SiteInfo.SiteID,
                    ShippingOptionName = "Default",
                    ShippingOptionDisplayName = "Default",
                    ShippingOptionEnabled = true,
                    ShippingOptionCarrierID = CarrierBasic.CarrierID
                }
            };
        }


        internal IEnumerable<ShippingCostInfo> InitShippingCosts()
        {
            return new ShippingCostInfo[]
            {
                new ShippingCostInfo
                {
                    ShippingCostMinWeight = 0,
                    ShippingCostShippingOptionID = ShippingOptionDefault.ShippingOptionID,
                    ShippingCostValue = 10
                }
            };
        }


        internal IEnumerable<SKUInfo> InitSKUs()
        {
            return new[]
            {
                SKUAvailable = new SKUInfo { SKUSiteID = SiteInfo.SiteID, SKUPrice = 10, SKUName = "SKU_AVAILABLE", SKUEnabled = true },
                SKUDisabled = new SKUInfo { SKUSiteID = SiteInfo.SiteID, SKUPrice = 10, SKUName = "SKU_DISABLED", SKUEnabled = false },
                SKULimited = new SKUInfo { SKUSiteID = SiteInfo.SiteID, SKUPrice = 10, SKUName = "SKU_LIMITED", SKUEnabled = true, SKUAvailableItems = 1, SKUSellOnlyAvailable = true },
                SKUWithoutShipping = new SKUInfo { SKUSiteID = SiteInfo.SiteID, SKUPrice = 10, SKUName = "SKU_WITHOUT_SHIPPING", SKUEnabled = true, SKUNeedsShipping = false },
                SKUWithTaxes = new SKUInfo { SKUSiteID = SiteInfo.SiteID, SKUPrice = 10, SKUName = "SKU_WITH_TAXES", SKUEnabled = true }
            };
        }


        public OrderStatusInfo NewOrderStatus(int order, bool orderIsPaid = false)
        {
            var stringGuid = NewUniqueName();

            return new OrderStatusInfo
            {
                StatusName = stringGuid,
                StatusDisplayName = stringGuid,
                StatusEnabled = true,
                StatusColor = "#00FF00",
                StatusOrder = order,
                StatusOrderIsPaid = orderIsPaid,
                StatusSendNotification = true,
                StatusSiteID = SiteInfo.SiteID
            };
        }


        public PaymentOptionInfo NewPaymentOption()
        {
            var name = NewUniqueName();

            return new PaymentOptionInfo
            {
                PaymentOptionAllowIfNoShipping = true,
                PaymentOptionDisplayName = name,
                PaymentOptionName = name,
                PaymentOptionEnabled = true
            };
        }


        public OptionCategoryInfo NewOptionCategory(string name, OptionCategoryTypeEnum type = OptionCategoryTypeEnum.Attribute, OptionCategorySelectionTypeEnum selectionType = OptionCategorySelectionTypeEnum.Dropdownlist)
        {
            return new OptionCategoryInfo
            {
                CategoryName = NewUniqueName(),
                CategoryEnabled = true,
                CategorySelectionType = selectionType,
                CategoryDisplayName = name,
                CategoryType = type,
                CategorySiteID = SiteInfo.SiteID
            };
        }


        public SKUInfo NewSKU(string name = null, decimal price = 10)
        {
            return new SKUInfo
            {
                SKUSiteID = SiteInfo.SiteID,
                SKUPrice = price,
                SKUName = name ?? NewUniqueName(),
                SKUEnabled = true,
                SKUAvailableItems = 1,
                SKUSellOnlyAvailable = true,
                SKUNeedsShipping = false
            };
        }


        public DiscountInfo NewCatalogDiscount(decimal value = 10, bool isFlat = false, int order = 1)
        {
            return new DiscountInfo
            {
                DiscountName = NewUniqueName(),
                DiscountDisplayName = "Catalog Discount",
                DiscountSiteID = SiteInfo.SiteID,
                DiscountOrder = order,
                DiscountEnabled = true,
                DiscountUsesCoupons = false,
                DiscountApplyTo = DiscountApplicationEnum.Products,
                DiscountCustomerRestriction = DiscountCustomerEnum.All,
                DiscountApplyFurtherDiscounts = true,
                DiscountIsFlat = isFlat,
                DiscountValue = value
            };
        }


        public DiscountInfo NewOrderDiscount(decimal value = 10, bool isFlat = false, int order = 1)
        {
            return new DiscountInfo
            {
                DiscountName = NewUniqueName(),
                DiscountDisplayName = "Order Discount",
                DiscountSiteID = SiteInfo.SiteID,
                DiscountOrder = order,
                DiscountEnabled = true,
                DiscountUsesCoupons = false,
                DiscountApplyTo = DiscountApplicationEnum.Order,
                DiscountCustomerRestriction = DiscountCustomerEnum.All,
                DiscountApplyFurtherDiscounts = true,
                DiscountIsFlat = isFlat,
                DiscountValue = value
            };
        }


        public GiftCardInfo NewGiftCard(decimal value = 10m)
        {
            return new GiftCardInfo
            {
                GiftCardName = NewUniqueName(),
                GiftCardDisplayName = "Gift card",
                GiftCardSiteID = SiteInfo.SiteID,
                GiftCardEnabled = true,
                GiftCardValue = value
            };
        }


        public DiscountInfo NewShippingDiscount(decimal value, bool isFlat, decimal minDiscountOrderAmount)
        {
            return new DiscountInfo
            {
                DiscountName = NewUniqueName(),
                DiscountDisplayName = "Shipping Discount",
                DiscountSiteID = SiteInfo.SiteID,
                DiscountOrder = 1,
                DiscountEnabled = true,
                DiscountUsesCoupons = false,
                DiscountApplyTo = DiscountApplicationEnum.Shipping,
                DiscountCustomerRestriction = DiscountCustomerEnum.All,
                DiscountApplyFurtherDiscounts = true,
                DiscountIsFlat = isFlat,
                DiscountValue = value,
                DiscountOrderAmount = minDiscountOrderAmount
            };
        }


        public String NewUniqueName()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
