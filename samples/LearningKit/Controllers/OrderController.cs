using System;
using System.Web.Mvc;

using CMS.SiteProvider;

using Kentico.Ecommerce;

using LearningKit.Models.Checkout;

namespace LearningKit.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly IShoppingService shoppingService;

        /// <summary>
        /// Constructor.
        /// You can use a dependency injection container to initialize the services and repositories.
        /// </summary>
        //DocSection:Constructor
        public OrderController()
        {
            shoppingService = new ShoppingService();
            orderRepository = new KenticoOrderRepository();
        }
        //EndDocSection:Constructor

        /// <summary>
        /// Displays a page where order details can be listed.
        /// </summary>
        public ActionResult OrderDetail()
        {
            Order order = null;

            return View(order);
        }

        /// <summary>
        /// Displays details about an order specified with its ID.
        /// </summary>
        /// <param name="textBoxValue">Order ID as a string</param>
        [HttpPost]
        public ActionResult OrderDetail(string textBoxValue)
        {
            // Gets the order based on the entered order ID
            Order order = GetOrder(textBoxValue);

            return View(order);
        }

        /// <summary>
        /// Marks an order specified by an order ID as paid.
        /// </summary>
        /// <param name="textBoxValue">Order ID as a string</param>
        public ActionResult MarkOrderAsPaid(string textBoxValue)
        {
            // Gets the order based on the entered order ID
            Order order = GetOrder(textBoxValue);

            //DocSection:SetAsPaid
            // Sets the order as paid
            order.SetAsPaid();
            //EndDocSection:SetAsPaid

            return RedirectToAction("OrderDetail");
        }

        /// <summary>
        /// Returns the order based on the entered order ID.
        /// </summary>
        /// <param name="textOrderID">String value with the order ID</param>
        /// <returns>Order object of the order</returns>
        private Order GetOrder(string textOrderID)
        {
            int orderID = 0;

            Int32.TryParse(textOrderID, out orderID);

            // If the text value is not a number, returns null
            if (orderID == 0)
            {
                return null;
            }

            //DocSection:GetOrderDetails
            // Gets the order based on the order ID
            Order order = orderRepository.GetById(orderID);
            //EndDocSection:GetOrderDetails

            return order;
        }

        /// <summary>
        /// Displays a listing of the current user's orders.
        /// </summary>
        public ActionResult MyOrders()
        {
            //DocSection:MyOrders
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
            //EndDocSection:MyOrders

            return View(model);
        }

        /// <summary>
        /// Recreates shopping cart content based on a specified order.
        /// </summary>
        /// <param name="orderId">ID of an order to repurchase</param>
        [HttpPost]
        public ActionResult Reorder(int orderId)
        {
            //DocSection:Reorder
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
            //EndDocSection:Reorder

            // Displays the shopping cart
            return RedirectToAction("ShoppingCart", "Checkout");
        }
    }
}