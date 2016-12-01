using CMS.Ecommerce;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of products.
    /// </summary>
    public interface IProductRepository : IRepository
    {
        /// <summary>
        /// Returns the product with the specified identifier.
        /// </summary>
        /// <param name="nodeID">The product identifier.</param>
        /// <returns>The product with the specified node identifier, if found; otherwise, null.</returns>
        SKUTreeNode GetProduct(int nodeID);


        /// <summary>
        /// Returns the product with the specified SKU identifier.
        /// </summary>
        /// <param name="skuID">The product or variant SKU identifier.</param>
        /// <returns>The product with the specified SKU identifier, if found; otherwise, null.</returns>
        SKUTreeNode GetProductForSKU(int skuID);
    }
}