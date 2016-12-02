using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace DancingGoat.Tests.Extensions
{
    /// <summary>
    /// Provides a set of static methods for testing controllers.
    /// </summary>
    public static class ControllerTestsExtensions
    {
        /// <summary>
        /// Validates specified model and sets ModelState errors if the model is invalid.
        /// </summary>
        /// <typeparam name="TController">The type of controller whose view utilizes model to be validated.</typeparam>
        /// <typeparam name="TViewModel">The type of view model to be validated.</typeparam>
        /// <param name="controller">The view result test.</param>
        /// <param name="viewModelToValidate">View model to be validated.</param>
        /// <remarks>
        /// This method validates specified model with respect to member data annotations and sets ModelState errors if the model is invalid.
        /// </remarks>
        public static void ValidateViewModel<TViewModel, TController>(this TController controller, TViewModel viewModelToValidate) where TController : Controller
        {
            var validationContext = new ValidationContext(viewModelToValidate, null, null);
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(viewModelToValidate, validationContext, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(validationResult.MemberNames.FirstOrDefault() ?? string.Empty, validationResult.ErrorMessage);
            }
        }
    }
}
