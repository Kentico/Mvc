using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides an adapter for the <see cref="RequiredAttribute"/> attribute that localizes error messages in validation errors and client validation rules.
    /// </summary>
    internal class LocalizedRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedRequiredAttributeAdapter"/> class.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="context">The controller context.</param>
        /// <param name="attribute">The <see cref="RequiredAttribute"/> attribute.</param>
        public LocalizedRequiredAttributeAdapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute) : base(metadata, context, attribute)
        {

        }


        /// <summary>
        /// Retrieves a collection of localized validation errors for the model and returns it.
        /// </summary>
        /// <param name="container">The container for the model.</param>
        /// <returns>A collection of localized validation errors for the model, or an empty collection if no errors have occurred.</returns>
        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            return LocalizationHelper.LocalizeValidationResults(base.Validate(container), Metadata.GetDisplayName());
        }


        /// <summary>
        /// Retrieves a collection of localized client validation rules for the model and returns it.
        /// </summary>
        /// <returns>A collection of localized client validation rules for the model.</returns>
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return LocalizationHelper.LocalizeValidationRules(base.GetClientValidationRules(), Metadata.GetDisplayName());
        }
    }
}