using System.Collections.Generic;
using System.Web.Mvc;

using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides an adapter for the <see cref="CompareAttribute"/> attribute that localizes error messages in validation errors and client validation rules.
    /// </summary>
    internal class LocalizedCompareAttributeAdapter : DataAnnotationsModelValidator<CompareAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedCompareAttributeAdapter"/> class.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="context">The controller context.</param>
        /// <param name="attribute">The <see cref="CompareAttribute"/> attribute.</param>
        public LocalizedCompareAttributeAdapter(ModelMetadata metadata, ControllerContext context, CompareAttribute attribute)
            : base(metadata, context, attribute)
        {
        }
  

        /// <summary>
        /// Retrieves a collection of localized validation errors for the model and returns it.
        /// </summary>
        /// <param name="container">The container for the model.</param>
        /// <returns>A collection of localized validation errors for the model, or an empty collection if no errors have occurred.</returns>
        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            return LocalizationHelper.LocalizeValidationResults(base.Validate(container), Metadata.GetDisplayName(), Attribute.OtherPropertyDisplayName);
        }


        /// <summary>
        /// Retrieves a collection of localized client validation rules for the model and returns it.
        /// </summary>
        /// <returns>A collection of localized client validation rules for the model.</returns>
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var rules = new[]
            {
                new ModelClientValidationEqualToRule(ErrorMessage, FormatPropertyForClientValidation(Attribute.OtherProperty))
            };

            return LocalizationHelper.LocalizeValidationRules(rules, Metadata.GetDisplayName(), Attribute.OtherPropertyDisplayName);
        }


        /// <summary>
        /// Formats the property for client validation by prepending an asterisk (*) and a dot.
        /// </summary>
        /// <returns>The string "*." is prepended to the property.</returns>
        private static string FormatPropertyForClientValidation(string property)
        {
            return "*." + property;
        }
    }
}