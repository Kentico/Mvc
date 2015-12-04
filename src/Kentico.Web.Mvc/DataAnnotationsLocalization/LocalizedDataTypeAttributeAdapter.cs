using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides an adapter for attributes derived from the <see cref="DataTypeAttribute"/> attribute, e.g. Phone or EmailAddress, that localizes error messages in validation errors and client validation rules.
    /// </summary>
    internal class LocalizedDataTypeAttributeAdapter : DataAnnotationsModelValidator
    {
        private readonly string mRuleName;


        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedDataTypeAttributeAdapter"/> class.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="context">The controller context.</param>
        /// <param name="attribute">The <see cref="DataTypeAttribute"/> attribute.</param>
        /// <param name="ruleName">The name of the client validation rule.</param>
        public LocalizedDataTypeAttributeAdapter(ModelMetadata metadata, ControllerContext context, DataTypeAttribute attribute, string ruleName) : base(metadata, context, attribute)
        {
            mRuleName = ruleName;
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
            var rules = new ModelClientValidationRule[] {
                new ModelClientValidationRule
                {
                    ValidationType = mRuleName,
                    ErrorMessage = ErrorMessage
                }
            };

            return LocalizationHelper.LocalizeValidationRules(rules, Metadata.GetDisplayName());
        }
    }
}