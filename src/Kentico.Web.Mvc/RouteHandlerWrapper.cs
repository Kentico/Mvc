using System.Web;
using System.Web.Routing;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Enable classes implementing <see cref="IHttpHandler"/> interface to be used as <see cref="IRouteHandler"/>.
    /// </summary>
    /// <typeparam name="THttpHandler">Class implementing <see cref="IHttpHandler"/> interface.</typeparam>
    public class RouteHandlerWrapper<THttpHandler> : IRouteHandler 
        where THttpHandler : IHttpHandler, new()
    {
        /// <summary>
        /// Retrieves an instance of class implementing <see cref="IHttpHandler"/> interface,
        /// based on the type specified in the constructor.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        /// <returns>Returns class that can process a request.</returns>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new THttpHandler();
        }
    }
}
