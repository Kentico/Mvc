using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Routing;
using System.Web.SessionState;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Wraps an existing controller factory and modifies its behavior. When no controller is found, user is presented with a custom view.
    /// </summary>
    internal class ControllerFactoryWrapper : IControllerFactory
    {
        /// <summary>
        /// The wrapped controller factory.
        /// </summary>
        private readonly IControllerFactory mControllerFactory;


        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerFactoryWrapper"/> class that wraps the specified controller factory.
        /// </summary>
        /// <param name="controllerFactory">The controller factory to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="controllerFactory"/> is <c>null</c>.</exception>
        public ControllerFactoryWrapper(IControllerFactory controllerFactory)
        {
            if (controllerFactory == null)
            {
                throw new ArgumentNullException(nameof(controllerFactory));
            }

            this.mControllerFactory = controllerFactory;
        }


        /// <summary>
        /// Creates the specified controller by using the specified request context.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The controller specified by the request context.</returns>
        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            try
            {
                var controller = mControllerFactory.CreateController(requestContext, controllerName);
                WrapControllerActionInvoker(controller);
                
                return controller;
            }
            catch (HttpException exception)
            {
                if (exception.GetHttpCode() == 404)
                {
                    requestContext.RouteData.Values["controller"] = "HttpErrors";
                    requestContext.RouteData.Values["action"] = "NotFound";
                    return new HttpErrorsController();
                }

                throw;
            }
        }


        /// <summary>
        /// Gets the session behavior for a controller with the specified name.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The session behavior for a controller with the specified name.</returns>
        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return mControllerFactory.GetControllerSessionBehavior(requestContext, controllerName);
        }


        /// <summary>
        /// Releases the specified controller.
        /// </summary>
        /// <param name="controller">The controller to release.</param>
        public void ReleaseController(IController controller)
        {
            mControllerFactory.ReleaseController(controller);
        }


        private void WrapControllerActionInvoker(IController controller)
        {
            var controllerBase = controller as Controller;

            if (controllerBase == null)
            {
                return;
            }

            var asyncActionInvoker = controllerBase.ActionInvoker as IAsyncActionInvoker;

            if (asyncActionInvoker != null)
            {
                controllerBase.ActionInvoker = new AsyncActionInvokerWrapper(asyncActionInvoker);
            }
            else
            {
                controllerBase.ActionInvoker = new ActionInvokerWrapper(controllerBase.ActionInvoker);
            }
        }
    }
}