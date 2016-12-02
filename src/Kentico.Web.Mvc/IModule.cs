using System.Web;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Represents a contract for request pipeline modules that provide Kentico ASP.NET MVC integration features.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="application">The ASP.NET application that defines common methods, properties, and events.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="application"/> is null.</exception>
        void Initialize(HttpApplication application);
    }
}
