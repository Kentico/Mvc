using System.Linq;

using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.SiteProvider;
using DancingGoat.Infrastructure;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of products.
    /// </summary>
    public class KenticoProductRepository : IProductRepository
    {
        private readonly string mCultureName;
        private readonly bool mLatestVersionEnabled;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoProductRepository"/> class that returns products in the specified language.
        /// If the requested product doesn't exist in specified language then its default culture version is returned.
        /// </summary>
        /// <param name="cultureName">The name of a culture.</param>
        /// <param name="latestVersionEnabled">Indicates whether the repository will provide the most recent version of pages.</param>
        public KenticoProductRepository(string cultureName, bool latestVersionEnabled)
        {
            mCultureName = cultureName;
            mLatestVersionEnabled = latestVersionEnabled;
        }


        /// <summary>
        /// Returns the product with the specified identifier.
        /// </summary>
        /// <param name="nodeID">The product node identifier.</param>
        /// <returns>The product with the specified node identifier, if found; otherwise, null.</returns>
        [CacheDependency("ecommerce.sku|all")]
        [CacheDependency("nodeid|{0}")]
        public SKUTreeNode GetProduct(int nodeID)
        {
            var node = DocumentHelper.GetDocuments()
                .LatestVersion(mLatestVersionEnabled)
                .Published(!mLatestVersionEnabled)
                .OnSite(SiteContext.CurrentSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .WhereEquals("NodeID", nodeID)
                .FirstOrDefault();

            if ((node == null) || !node.IsProduct())
            {
                return null;
            }

            // Load product type specific fields from the database
            node.MakeComplete(true);

            return node as SKUTreeNode;
        }


        /// <summary>
        /// Returns the product with the specified SKU identifier.
        /// </summary>
        /// <param name="skuID">The product or variant SKU identifier.</param>
        /// <returns>The product with the specified SKU identifier, if found; otherwise, null.</returns>
        [CacheDependency("ecommerce.sku|all")]
        [CacheDependency("nodeid|{0}")]
        public SKUTreeNode GetProductForSKU(int skuID)
        {
            var sku = SKUInfoProvider.GetSKUInfo(skuID);
            if ((sku == null) || sku.IsProductOption)
            {
                return null;
            }

            if (sku.IsProductVariant)
            {
                skuID = sku.SKUParentSKUID;
            }

            var node = DocumentHelper.GetDocuments()
                .LatestVersion(mLatestVersionEnabled)
                .Published(!mLatestVersionEnabled)
                .OnSite(SiteContext.CurrentSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .WhereEquals("NodeSKUID", skuID)
                .FirstOrDefault();

            return node as SKUTreeNode;
        }
    }
}