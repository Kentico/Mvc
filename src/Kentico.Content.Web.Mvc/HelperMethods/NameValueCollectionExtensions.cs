using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Kentico.Content.Web.Mvc
{
    /// <summary>
    /// Provides extension methods for the <see cref="NameValueCollection"/> class.
    /// </summary>
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Adds an entry with the specified name and value to the <see cref="NameValueCollection"/>.
        /// </summary>
        /// <param name="collection">The name value collection.</param>
        /// <param name="name">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add</param>
        public static void Add(this NameValueCollection collection, string name, object value)
        {
            collection.Add(name, Convert.ToString(value));
        }


        /// <summary>
        /// Adds size constraint's values to the collection.
        /// </summary>
        /// <param name="collection">The name value collection.</param>
        /// <param name="constraint">Size constraint.</param>
        public static void AddSizeConstraint(this NameValueCollection collection, SizeConstraint constraint)
        {
            if (constraint.WidthComponent > 0)
            {
                collection.Add("width", constraint.WidthComponent);
            }

            if (constraint.HeightComponent > 0)
            {
                collection.Add("height", constraint.HeightComponent);
            }

            if (constraint.MaxWidthOrHeightComponent > 0)
            {
                collection.Add("maxsidesize", constraint.MaxWidthOrHeightComponent);
            }

            // Prevent Kentico from using site settings
            if (!constraint.IsEmpty)
            {
                collection.Add("resizemode", "force");
            }
        }


        /// <summary>
        /// Returns a query string that represents the collection.
        /// Uses the format: ?name1=value1&amp;name2=value2...
        /// </summary>
        /// <param name="collection">The name value collection.</param>
        public static string ToQueryString(this NameValueCollection collection)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string key in collection)
            {
                if (!String.IsNullOrWhiteSpace(key))
                {
                    foreach (string val in collection[key].Split(','))
                    {
                        builder.Append(builder.Length == 0 ? "?" : "&")
                            .Append(HttpUtility.UrlEncode(key))
                            .Append("=")
                            .Append(HttpUtility.UrlEncode(val));
                    }
                }
            }

            return builder.ToString();
        }
    }
}
