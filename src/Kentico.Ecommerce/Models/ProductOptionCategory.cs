using System.Collections.Generic;

using CMS.Ecommerce;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents an option category with selectable options.
    /// </summary>
    public class ProductOptionCategory
    {
        internal readonly OptionCategoryInfo OriginalCategory;

        /// <summary>
        /// Gets the ID of the option category.
        /// </summary>
        public int ID => OriginalCategory.CategoryID;


        /// <summary>
        /// Gets the category's display name.
        /// </summary>
        public string DisplayName => string.IsNullOrEmpty(OriginalCategory.CategoryLiveSiteDisplayName) ?
            OriginalCategory.CategoryDisplayName :
            OriginalCategory.CategoryLiveSiteDisplayName;


        /// <summary>
        /// Gets the category's default selection text.
        /// </summary>
        public string DefaultText => OriginalCategory.CategoryDefaultRecord;


        /// <summary>
        /// Gets the category's selection type. See <see cref="OptionCategorySelectionTypeEnum"/> for detailed information.
        /// </summary>
        public OptionCategorySelectionTypeEnum SelectionType => OriginalCategory.CategorySelectionType;


        /// <summary>
        /// Category's product options. See <see cref="SKUInfo"/> for detailed information.
        /// </summary>
        public IEnumerable<SKUInfo> CategoryOptions
        {
            get;
            set;
        }


        /// <summary>
        /// Creates a new instance of the <see cref="ProductOptionCategory"/> class.
        /// </summary>
        /// <param name="category"><see cref="OptionCategoryInfo"/> object representing an original Kentico option category info object from which the model is created.</param>
        /// <param name="options">Collection of selectable product options.</param>
        public ProductOptionCategory(OptionCategoryInfo category, IEnumerable<SKUInfo> options)
        {
            OriginalCategory = category;
            CategoryOptions = options;
        }

    }
}
