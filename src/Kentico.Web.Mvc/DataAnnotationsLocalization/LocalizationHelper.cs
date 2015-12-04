using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.Helpers;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides methods for localization of model validation errors and client validation rules.
    /// </summary>
    internal static class LocalizationHelper
    {
        /// <summary>
        /// Localizes the specified collection of validation errors and formats error messages using the string representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="results">A collection of validation errors.</param>
        /// <param name="arguments">An object array that contains zero or more objects to format.</param>
        /// <returns>A collection of localized validation errors.</returns>
        public static IEnumerable<ModelValidationResult> LocalizeValidationResults(IEnumerable<ModelValidationResult> results, params object[] arguments)
        {
            if (results == null)
            {
                return null;
            }

            return results.Select(result => LocalizeValidationResult(result, arguments));
        }


        /// <summary>
        /// Localizes the specified collection of client validation rules and formats error messages using the string representation of a corresponding object in a specified array.
        /// </summary>
        /// <param name="rules">A collection of client validation rules.</param>
        /// <param name="arguments">An object array that contains zero or more objects to format.</param>
        /// <returns>A collection of localized client validation rules.</returns>
        public static IEnumerable<ModelClientValidationRule> LocalizeValidationRules(IEnumerable<ModelClientValidationRule> rules, params object[] arguments)
        {
            if (rules == null)
            {
                return null;
            }

            return rules.Select(rule => LocalizeValidationRule(rule, arguments));
        }


        private static ModelValidationResult LocalizeValidationResult(ModelValidationResult result, object[] arguments)
        {
            result.Message = ResHelper.GetStringFormat(result.Message, arguments);

            return result;
        }


        private static ModelClientValidationRule LocalizeValidationRule(ModelClientValidationRule rule, object[] arguments)
        {
            rule.ErrorMessage = ResHelper.GetStringFormat(rule.ErrorMessage, arguments);

            return rule;
        }
    }
}