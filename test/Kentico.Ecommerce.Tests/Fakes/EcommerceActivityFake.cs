using CMS.Activities;

namespace Kentico.Ecommerce.Tests.Fakes
{
    internal class EcommerceActivityFake
    {
        public string LoggedActivity { get; set; }


        public int Quantity { get; set; }


        public int SkuId { get; set; }


        public string SkuName { get; set; }


        public int OrderId { get; set; }


        public double TotalPrice { get; set; }


        public string TotalPriceAsString { get; set; }


        public VisitorData VisitorTitleData { get; set; }
    }
}
