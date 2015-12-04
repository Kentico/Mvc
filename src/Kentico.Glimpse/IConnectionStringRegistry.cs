namespace Kentico.Glimpse
{
    /// <summary>
    /// Represents a contract for a collection of connection strings and their names.
    /// </summary>
    internal interface IConnectionStringRegistry
    {
        /// <summary>
        /// Returns a name of the specified connection string that is not default.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A name of the specified connection string, if found and is not default; otherwise, <code>null</code>.</returns>
        string GetCustomConnectionStringName(string connectionString);
    }
}
