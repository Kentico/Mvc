using System;
using System.Threading;
using System.Web.Mvc;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Wraps the status of an asynchronous action and its context.
    /// </summary>
    internal sealed class AsyncResultWrapper : IAsyncResult
    {
        /// <summary>
        /// The wrapped status of an asynchronous action.
        /// </summary>
        private readonly IAsyncResult mInnerAsyncResult;
        
        
        /// <summary>
        /// The controller context.
        /// </summary>
        private readonly ControllerContext mControllerContext;


        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResultWrapper"/> class that wraps the specified status and context.
        /// </summary>
        /// <param name="innerAsyncResult">The status of an asynchronous action to wrap.</param>
        /// <param name="controllerContext">The controller context.</param>
        /// <exception cref="ArgumentNullException"><paramref name="innerAsyncResult"/> or <paramref name="controllerContext"/> is <c>null</c>.</exception>
        public AsyncResultWrapper(IAsyncResult innerAsyncResult, ControllerContext controllerContext)
        {
            if (innerAsyncResult == null)
            {
                throw new ArgumentNullException(nameof(innerAsyncResult));
            }

            if (controllerContext == null)
            {
                throw new ArgumentNullException(nameof(controllerContext));
            }

            mInnerAsyncResult = innerAsyncResult;
            mControllerContext = controllerContext;
        }


        /// <summary>
        /// Gets the wrapped status of an asynchronous action.
        /// </summary>
        public IAsyncResult InnerAsyncResult
        {
            get
            {
                return mInnerAsyncResult;
            }
        }


        /// <summary>
        /// Gets the controller context.
        /// </summary>
        public ControllerContext ControllerContext
        {
            get
            {
                return mControllerContext;
            }
        }


        /// <summary>
        /// Gets a user-defined object that contains information about an asynchronous action.
        /// </summary>
        public object AsyncState
        {
            get
            {
                return mInnerAsyncResult.AsyncState;
            }
        }


        /// <summary>
        /// Gets a <see cref="System.Threading.WaitHandle"/> that is used to wait for an asynchronous action to complete.
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return mInnerAsyncResult.AsyncWaitHandle;
            }
        }


        /// <summary>
        /// Gets a value that indicates whether the asynchronous action completed synchronously.
        /// </summary>
        public bool CompletedSynchronously
        {
            get
            {
                return mInnerAsyncResult.CompletedSynchronously;
            }
        }


        /// <summary>
        /// Gets a value that indicates whether the asynchronous action has completed.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return mInnerAsyncResult.IsCompleted;
            }
        }
    }
}
