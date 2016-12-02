using System;
using System.Linq;

using CMS.Ecommerce;
using CMS.SiteProvider;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class KenticoCustomerAddressRepositoryTests : UnitTests
    {
        private KenticoCustomerAddressRepository mRepository;

        private const int SITE_ID1 = 1;

        private const int CUSTOMER_WITHADDRESS_ID = 1;
        private const int CUSTOMER_WITHOUTADDRESS_ID = 2;

        private const int ADDRESS_ID1 = 1;
        private const int ADDRESS_ID2 = 2;
        private const int NONEXISTENT_ADDRESS_ID = 3;


        [SetUp]
        public void SetUp()
        {
            SetUpSite();
            SetUpCustomerAddresses();

            mRepository = new KenticoCustomerAddressRepository();
        }


        [Test]
        public void GetAddress_ExistingAddress_ReturnsAddress()
        {
            var address = mRepository.GetById(ADDRESS_ID1);

            Assert.AreEqual(ADDRESS_ID1, address.ID);
        }


        [Test]
        public void GetAddress_NonExistentAddress_ReturnsNull()
        {
            var address = mRepository.GetById(NONEXISTENT_ADDRESS_ID);

            Assert.IsNull(address);
        }


        [Test]
        public void GetCustomerAddresses_WithAddresses_ReturnsAddresses()
        {
            var addresses = mRepository.GetByCustomerId(CUSTOMER_WITHADDRESS_ID);

            CMSAssert.All(
                () => Assert.IsNotNull(addresses, "Returned null instead of a collection of addresses."),
                () => Assert.AreEqual(2, addresses.Count(), "Wrong number of addresses returned.")
            );
        }


        [Test]
        public void GetCustomerAddresses_WithoutAddresses_ReturnsEmpty()
        {
            var addresses = mRepository.GetByCustomerId(CUSTOMER_WITHOUTADDRESS_ID);

            Assert.IsEmpty(addresses);
        }


        private void SetUpSite()
        {
            Fake<SiteInfo, SiteInfoProvider>().WithData(
                new SiteInfo
                {
                    SiteName = "testSite",
                    SiteID = SITE_ID1
                });
        }


        private void SetUpCustomerAddresses()
        {
            Fake<CustomerInfo, CustomerInfoProvider>().WithData(
                new CustomerInfo
                {
                    CustomerID = CUSTOMER_WITHADDRESS_ID,
                    CustomerSiteID = SITE_ID1,
                    CustomerCreated = DateTime.Now
                },
                new CustomerInfo
                {
                    CustomerID = CUSTOMER_WITHOUTADDRESS_ID,
                    CustomerSiteID = SITE_ID1,
                    CustomerCreated = DateTime.Now
                });

            Fake<AddressInfo, AddressInfoProvider>().WithData(
                new AddressInfo
                {
                    AddressID = ADDRESS_ID1,
                    AddressCustomerID = CUSTOMER_WITHADDRESS_ID
                },
                new AddressInfo
                {
                    AddressID = ADDRESS_ID2,
                    AddressCustomerID = CUSTOMER_WITHADDRESS_ID
                });
         }
    }
}
