using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Extensions
{
    /// <summary>
    /// Provides a set of static methods for testing controllers.
    /// </summary>
    public static class ViewResultTestExtensions
    {
        private static readonly FieldInfo mViewResultField = typeof(ViewResultTest).GetField("_viewResult", BindingFlags.Instance | BindingFlags.NonPublic);

        
        /// <summary>
        /// Checks whether a view result contains a model that matches the specified condition.
        /// </summary>
        /// <typeparam name="TModel">The type of view result model.</typeparam>
        /// <param name="instance">The view result test.</param>
        /// <param name="predicate">The delegate that defines the model condition.</param>
        /// <returns>The model test.</returns>
        /// <exception cref="TestStack.FluentMVCTesting.ViewResultModelAssertionException">The view result model does not match the specified condition.</exception>
        /// <remarks>
        /// This method substitutes the WithModel method with condition parameter as it serializes the model into JSON.
        /// Kentico TreeNode class is not serializable when the database is not available and as the result the WithModel method fails.
        /// </remarks>
        public static ModelTest<TModel> WithModelMatchingCondition<TModel>(this ViewResultTest instance, Expression<Func<TModel, bool>> predicate) where TModel : class
        {
            var test = instance.WithModel<TModel>();
            var viewResult = mViewResultField.GetValue(instance) as ViewResultBase;
            var model = viewResult.Model as TModel;

            var compiledPredicate = predicate.Compile();

            if (!compiledPredicate(model))
            {
                throw new ViewResultModelAssertionException("Expected view model to pass the given condition, but it failed.");
            }

            return test;
        }
    }
}