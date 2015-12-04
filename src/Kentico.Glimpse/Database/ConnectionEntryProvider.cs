using System;
using System.Data;

namespace Kentico.Glimpse.Database
{
    internal sealed class ConnectionEntryProvider : IEntryProvider
    {
        private readonly IConnectionStringRegistry mConnectionStringRegistry;


        public ConnectionEntryProvider(IConnectionStringRegistry connectionStringRegistry)
        {
            mConnectionStringRegistry = connectionStringRegistry;
        }


        public bool Matches(DataRow row)
        {
            var operation = row["ConnectionOp"];

            if (operation == DBNull.Value)
            {
                return false;
            }

            return true;
        }


        public object GetEntry(DataRow row)
        {
            return new ConnectionEntry
            {
                CustomConnectionStringName = GetCustomConnectionStringName(row),
                Text = GetText(row),
                StackTrace = GetStackTrace(row)
            };
        }


        private string GetCustomConnectionStringName(DataRow row)
        {
            var value = row["ConnectionString"];

            if (value == DBNull.Value)
            {
                return null;
            }

            var connectionString = (string)value;

            return mConnectionStringRegistry.GetCustomConnectionStringName(connectionString);
        }


        private string GetText(DataRow row)
        {
            return (string)row["ConnectionOp"];
        }


        private string GetStackTrace(DataRow row)
        {
            return (string)row["Context"];
        }
    }
}
