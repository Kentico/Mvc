using Kentico.Core.DependencyInjection;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Interface for a service providing methods for managing shopping carts.
    /// </summary>
    public interface IShoppingService : IService
    {
        /// <summary>
        /// Returns the current shopping cart.
        /// </summary>
        /// <returns><see cref="ShoppingCart"/> object representing the shopping cart on the current site.</returns>
        ShoppingCart GetCurrentShoppingCart();


        /// <summary>
        /// Creates a new order from the shopping cart. Saves the shopping cart before the order creation.
        /// </summary>
        /// <param name="cart">Validated shopping cart (<see cref="ShoppingCart"/>) from which an order is created.</param>
        /// <returns><see cref="Order"/> object representing the created order.</returns>
        Order CreateOrder(ShoppingCart cart);


        /// <summary>
        /// Deletes a shopping cart from the database.
        /// </summary>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) that is deleted.</param>
        void DeleteShoppingCart(ShoppingCart cart);


        /// <summary>
        /// Gets the current customer.
        /// </summary>
        /// <returns><see cref="Customer"/> object representing the current customer. Returns <c>null</c> if there is not any current customer.</returns>
        Customer GetCurrentCustomer();
    }
}