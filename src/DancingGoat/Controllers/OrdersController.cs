using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using CMS.Ecommerce;

using Kentico.Ecommerce;
using Kentico.Membership;

using DancingGoat.Models.Orders;


namespace DancingGoat.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository mOrderRepository;
        private readonly IShoppingService mShoppingService;


        private UserManager UserManager => HttpContext.GetOwinContext().Get<UserManager>();


        public OrdersController(IOrderRepository orderRepository, IShoppingService shoppingService)
        {
            mOrderRepository = orderRepository;
            mShoppingService = shoppingService;
        }


        // GET: Orders
        [Authorize]
        public ActionResult Index()
        {
            var currentCustomer = mShoppingService.GetCurrentCustomer();
            var orders = currentCustomer != null
                ? mOrderRepository.GetByCustomerId(currentCustomer.ID).Select(order => new OrdersListViewModel(order))
                : Enumerable.Empty<OrdersListViewModel>();

            return View(orders);
        }


        // GET: Orders/OrderDetail
        [Authorize]
        public ActionResult OrderDetail(int? orderID)
        {
            if (orderID == null)
            {
                return RedirectToAction("Index");
            }

            var order = mOrderRepository.GetById(orderID.Value);
            var currentUser = UserManager.FindByName(User.Identity.Name);

            if ((order == null) || !order.IsCreatedByUser(currentUser.Id))
            {
                return RedirectToAction("NotFound", "HttpErrors");
            }

            return View(new OrderDetailViewModel
            {
                InvoiceNumber = order.OrderInvoiceNumber,
                TotalPrice = order.TotalPrice,
                StatusName = order.StatusName,
                OrderAddress = new OrderAddressViewModel(order.BillingAddress),
                OrderItems = order.OrderItems,
                OrderCurrency = order.Currency
            });
        }
    }
}