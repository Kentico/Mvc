using System;
using System.Web.Mvc;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// When action returns <see cref="HttpNotFoundResult"/> result, user is presented with a custom view.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    internal class HandleNotFoundErrorAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!(filterContext.Result is HttpNotFoundResult))
            {
                return;
            }

            filterContext.Result = new ViewResult
            {
                ViewName = "NotFound"
            };
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 404;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }


        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            return;
        }
    }
}