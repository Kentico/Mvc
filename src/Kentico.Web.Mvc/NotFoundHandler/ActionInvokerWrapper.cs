using System.Web;
using System.Web.Mvc;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Wraps an existing action invoker and modifies its behavior. When no action is found, user is presented with a custom view.
    /// </summary>
    internal class ActionInvokerWrapper : IActionInvoker
    {
        private readonly IActionInvoker mActionInvoker;


        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvokerWrapper"/> class that wraps the specified action invoker.
        /// </summary>
        /// <param name="actionInvoker">The action invoker to wrap.</param>
        public ActionInvokerWrapper(IActionInvoker actionInvoker)
        {
            this.mActionInvoker = actionInvoker;
        }


        public bool InvokeAction(ControllerContext controllerContext, string actionName)
        {
            if (InvokeActionWithNotFoundCatch(controllerContext, actionName))
            {
                return true;
            }

            controllerContext.RequestContext.RouteData.Values["controller"] = "HttpErrors";
            controllerContext.RequestContext.RouteData.Values["action"] = "NotFound";
            IController controller = new HttpErrorsController();
            controller.Execute(controllerContext.RequestContext);

            return true;
        }


        private bool InvokeActionWithNotFoundCatch(ControllerContext controllerContext, string actionName)
        {
            try
            {
                return mActionInvoker.InvokeAction(controllerContext, actionName);
            }
            catch (HttpException exception)
            {
                if (exception.GetHttpCode() == 404)
                {
                    return false;
                }
                throw;
            }
        }
    }
}