using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Wraps an existing controller factory and modifies its behavior. When no controller is found, user is presented with a custom view.
    /// </summary>
    internal class ControllerFactoryWrapper : IControllerFactory
    {
        private readonly IControllerFactory mControllerFactory;


        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerFactoryWrapper"/> class that wraps the specified controller factory.
        /// </summary>
        /// <param name="controllerFactory">The controller factory to wrap.</param>
        public ControllerFactoryWrapper(IControllerFactory controllerFactory)
        {
            this.mControllerFactory = controllerFactory;
        }


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


        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return mControllerFactory.GetControllerSessionBehavior(requestContext, controllerName);
        }


        public void ReleaseController(IController controller)
        {
            mControllerFactory.ReleaseController(controller);
        }


        private void WrapControllerActionInvoker(IController controller)
        {
            var controllerWithInvoker = controller as Controller;
            if (controllerWithInvoker != null)
            {
                controllerWithInvoker.ActionInvoker = new ActionInvokerWrapper(controllerWithInvoker.ActionInvoker);
            }
        }
    }
}