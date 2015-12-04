using System;
using System.Data;

using NSubstitute;

namespace Kentico.Glimpse.Tests.Unit.Database
{
    internal static class TestUtils
    {
        private static readonly DataTable mDataTable = CreateDataTable();
        private static readonly IConnectionStringRegistry mConnectionStringRegistry = CreateConnectionStringRegistry();


        public static IConnectionStringRegistry ConnectionStringRegistry
        {
            get
            {
                return mConnectionStringRegistry;
            }
        }


        public static DataRow CreateDataRow()
        {
            return mDataTable.NewRow();
        }


        private static DataTable CreateDataTable()
        {
            var table = new DataTable();

            table.Columns.Add("IsInformation", typeof(Boolean));
            table.Columns.Add("Counter", typeof(Int32));
            table.Columns.Add("ConnectionOp", typeof(String));
            table.Columns.Add("QueryName", typeof(String));
            table.Columns.Add("QueryText", typeof(String));
            table.Columns.Add("QueryParameters", typeof(String));
            table.Columns.Add("ConnectionString", typeof(String));
            table.Columns.Add("Context", typeof(String));
            table.Columns.Add("QueryResults", typeof(String));
            table.Columns.Add("QueryDuration", typeof(Double));
            table.Columns.Add("QueryResultsSize", typeof(Int32));
            table.Columns.Add("QueryParametersSize", typeof(Int32));

            return table;
        }


        private static IConnectionStringRegistry CreateConnectionStringRegistry()
        {
            var connectionStringRegistry = Substitute.For<IConnectionStringRegistry>();
            connectionStringRegistry.GetCustomConnectionStringName(Arg.Any<String>()).ReturnsForAnyArgs(x => x.Arg<string>() == "CustomConnectionString" ? "Custom" : null);

            return connectionStringRegistry;
        }
    }
}
