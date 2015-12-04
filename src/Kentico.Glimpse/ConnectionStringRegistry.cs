using System;

using CMS.Base;
using CMS.DataEngine;

namespace Kentico.Glimpse
{
    /// <summary>
    /// Represents a collection of connection strings and their names.
    /// </summary>
    internal sealed class ConnectionStringRegistry : IConnectionStringRegistry
    {
        /// <summary>
        /// Returns a name of the specified connection string that is not default.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A name of the specified connection string, if found and is not default; otherwise, <code>null</code>.</returns>
        public string GetCustomConnectionStringName(string connectionString)
        {
            var name = SettingsHelper.ConnectionStrings.GetConnectionStringName(connectionString);

            if (String.IsNullOrEmpty(name) || name == ConnectionHelper.DEFAULT_CONNECTIONSTRING_NAME)
            {
                return null;
            }

            return name;
        }
    }
}
