using System;
using System.Web.Mvc;

using CMS.Ecommerce;
using CMS.SiteProvider;

using Kentico.Ecommerce;

using LearningKit.Models.Checkout;

namespace LearningKit.Controllers
{
    /// <remarks>
    /// This class tries only to demonstrate behavior of a payment controller.
    /// However, the class is not suitable for any real usage and cannot work on its own.
    /// </remarks>
    public class PaymentController : Controller
    {
        private readonly IOrderRepository orderRepository;

        /// <summary>
        /// Constructor.
        /// You can use a dependency injection container to initialize the repositories.
        /// </summary>
        public PaymentController()
        {
            orderRepository = new KenticoOrderRepository();
        }


        /// <summary>
        /// Fictitious method for creating a payment response information and paying the order.
        /// </summary>
        /// <param name="orderID">ID of the paid order.</param>
        public ActionResult Index(int orderID)
        {
            // Gets the order
            Order order = orderRepository.GetById(orderID);

            // Creates a fictitious response
            ResponseModel response = new ResponseModel()
            {
                InvoiceNo = order.OrderID,
                Message = "Successfully paid",
                Completed = true,
                TransactionID = new Random().Next(100000, 200000).ToString(),
                Amount = order.TotalPrice,
                ResponseCode = "",
                Approved = true
            };

            // Validates the response and pays the order
            Validate(response);

            // Redirects to the thank-you page
            return RedirectToAction("ThankYou", "Checkout");
        }


        /// <summary>
        /// Pays the specified order with a fictitious response.
        /// </summary>
        /// <param name="response">Fictitious response about payment.</param>
        private void Validate(ResponseModel response)
        {
            //DocSection:PaymentValidation
            if (response != null)
            {
                // Gets the order based on the invoice number from the response
                Order order = orderRepository.GetById(response.InvoiceNo);
                
                // Checks whether the paid amount of money matches the order price
                // and whether the payment was approved
                if (response.Amount == order.TotalPrice && response.Approved)
                {
                    // Creates a payment result object that will be viewable in Kentico
                    PaymentResultInfo result = new PaymentResultInfo
                    {
                        PaymentDate = DateTime.Now,
                        PaymentDescription = response.Message,
                        PaymentIsCompleted = response.Completed,
                        PaymentTransactionID = response.TransactionID,
                        PaymentStatusValue = response.ResponseCode,
                        PaymentMethodName = "PaymentName"
                    };
                    
                    // Saves the payment result to the database
                    order.SetPaymentResult(result);
                }
            }
            //EndDocSection:PaymentValidation
        }
    }
}