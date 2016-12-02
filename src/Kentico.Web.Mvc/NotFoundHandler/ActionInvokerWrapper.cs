using System;
using System.Web;
using System.Web.Mvc;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Wraps an existing synchronous action invoker and modifies its behavior. When no action is found, user is presented with a custom view.
    /// </summary>
    internal class ActionInvokerWrapper : IActionInvoker
    {
        /// <summary>
        /// The wrapped synchronous action invoker.
        /// </summary>
        private readonly IActionInvoker mActionInvoker;


        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvokerWrapper"/> class that wraps the specified synchronous action invoker.
        /// </summary>
        /// <param name="actionInvoker">The synchronous action invoker to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionInvoker"/> is <c>null</c>.</exception>
        public ActionInvokerWrapper(IActionInvoker actionInvoker)
        {
            if (actionInvoker == null)
            {
                throw new ArgumentNullException(nameof(actionInvoker));
            }

            this.mActionInvoker = actionInvoker;
        }


        /// <summary>
        /// Invokes the specified action by using the specified controller context.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <returns><c>true</c> if the action was found; otherwise, <c>false</c>.</returns>
        public bool InvokeAction(ControllerContext controllerContext, string actionName)
        {
            if (InvokeActionWithNotFoundCatch(controllerContext, actionName))
            {
                return true;
            }

            InvokeNotFoundAction(controllerContext);

            return true;
        }


        /// <summary>
        /// Invokes an action that displays a custom view.
        /// The convention specifies that there is a shared view with a name NotFound.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        protected void InvokeNotFoundAction(ControllerContext controllerContext)
        {
            controllerContext.RequestContext.RouteData.Values["controller"] = "HttpErrors";
            controllerContext.RequestContext.RouteData.Values["action"] = "NotFound";
            IController controller = new HttpErrorsController();
            controller.Execute(controllerContext.RequestContext);
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