using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Wraps an existing asynchronous action invoker and modifies its behavior. When no action is found, user is presented with a custom view.
    /// </summary>
    internal class AsyncActionInvokerWrapper : ActionInvokerWrapper, IAsyncActionInvoker
    {
        /// <summary>
        /// The wrapped asynchronous action invoker.
        /// </summary>
        private readonly IAsyncActionInvoker mActionInvoker;


        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncActionInvokerWrapper"/> class that wraps the specified asynchronous action invoker.
        /// </summary>
        /// <param name="actionInvoker">The asynchronous action invoker to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionInvoker"/> is <c>null</c>.</exception>
        public AsyncActionInvokerWrapper(IAsyncActionInvoker actionInvoker) : base(actionInvoker)
        {
            if (actionInvoker == null)
            {
                throw new ArgumentNullException(nameof(actionInvoker));
            }

            this.mActionInvoker = actionInvoker;
        }


        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionName">The name of the asynchronous action.</param>
        /// <param name="callback">The callback method.</param>
        /// <param name="state">The state.</param>
        /// <returns>The status of the asynchronous result.</returns>
        public IAsyncResult BeginInvokeAction(ControllerContext controllerContext, string actionName, AsyncCallback callback, object state)
        {
            var innerAsyncResult = mActionInvoker.BeginInvokeAction(controllerContext, actionName, callback, state);

            return new AsyncResultWrapper(innerAsyncResult, controllerContext);
        }


        /// <summary>
        /// Waits for the pending asynchronous action to complete.
        /// </summary>
        /// <param name="asyncResult">The reference to the pending asynchronous action to wait for.</param>
        /// <returns><c>true</c> if the action was found; otherwise, <c>false</c>.</returns>
        public bool EndInvokeAction(IAsyncResult asyncResult)
        {
            var asyncResultWrapper = (AsyncResultWrapper)asyncResult;

            if (EndInvokeActionWithNotFoundCatch(asyncResultWrapper.InnerAsyncResult))
            {
                return true;
            }

            InvokeNotFoundAction(asyncResultWrapper.ControllerContext);

            return true;
        }
        

        private bool EndInvokeActionWithNotFoundCatch(IAsyncResult asyncResult)
        {
            try
            {
                return mActionInvoker.EndInvokeAction(asyncResult);
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