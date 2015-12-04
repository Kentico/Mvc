using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

using CMS.SiteProvider;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Route constraint restricting culture parameter to cultures allowed by site.
    /// </summary>
    public class SiteCultureConstraint : IRouteConstraint
    {
        private readonly HashSet<string> allowedCultureNames;


        /// <summary>
        /// Creates a new instance of cultural route constraint for specific site.
        /// </summary>
        /// <param name="siteName">The code name of the site.</param>
        public SiteCultureConstraint(string siteName)
        {
            var siteCultureNames = CultureSiteInfoProvider.GetSiteCultureCodes(siteName);

            // Ignore case for culture comparison (both 'en-US' and 'en-us' are ok)
            allowedCultureNames = new HashSet<string>(siteCultureNames, StringComparer.InvariantCultureIgnoreCase);
        }


        /// <summary>
        /// Determines whether the URL parameter contains an allowed culture name for this constraint.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <param name="route">The object that this constraint belongs to.</param>
        /// <param name="parameterName">The name of the parameter that is being checked.</param>
        /// <param name="values">An object that contains the parameters for the URL.</param>
        /// <param name="routeDirection">An object that indicates whether the constraint check is being performed when an incoming request is being handled or when a URL is being generated.</param>
        /// <returns>True if the URL parameter contains an allowed culture name; otherwise, false.</returns>
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var cultureName = values[parameterName].ToString();

            return allowedCultureNames.Contains(cultureName);
        }
    }
}