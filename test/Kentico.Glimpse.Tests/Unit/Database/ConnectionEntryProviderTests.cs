using System;
using System.Data;

using Kentico.Glimpse.Database;

using NSubstitute;
using NUnit.Framework;
using StatePrinter;

namespace Kentico.Glimpse.Tests.Unit.Database
{
    [TestFixture]
    [Category("Kentico.Glimpse.Database")]
    public class ConnectionEntryProviderTests
    {
        [TestCase(null, false)]
        [TestCase("", true)]
        [TestCase("new SqlConnection()", true)]
        public void Matches_DetectsCompatibleRows(object connectionOperation, bool expected)
        {
            var provider = CreateProvider();
            var row = CreateDataRow(connectionOperation);

            var result = provider.Matches(row);

            Assert.AreEqual(expected, result);
        }


        [Test]
        public void GetEntry_ParsesEachColumn()
        {
            var provider = CreateProvider();
            var row = CreateDataRow();

            row["ConnectionString"] = "CustomConnectionString";
            row["ConnectionOp"] = "new SqlConnection()";
            row["Context"] = "StackTrace";

            var entry = provider.GetEntry(row);

            var expected = new ConnectionEntry
            {
                CustomConnectionStringName = "Custom",
                Text = "new SqlConnection()",
                StackTrace = "StackTrace"
            };

            var printer = new Stateprinter();
            Assert.AreEqual(printer.PrintObject(expected), printer.PrintObject(entry));
        }


        [Test]
        public void GetEntry_SupportsOptionalValues()
        {
            var provider = CreateProvider();
            var row = CreateDataRow();

            row["ConnectionString"] = DBNull.Value;

            var entry = (ConnectionEntry)provider.GetEntry(row);

            Assert.IsTrue(entry.CustomConnectionStringName == null);
        }


        [Test]
        public void GetEntry_DetectsCustomConnectionString()
        {
            var provider = CreateProvider();
            var row = CreateDataRow();

            row["ConnectionString"] = "CustomConnectionString";

            var entry = (ConnectionEntry)provider.GetEntry(row);

            Assert.AreEqual("Custom", entry.CustomConnectionStringName);
        }


        [Test]
        public void GetEntry_IgnoresConnectionStringThatIsNotCustom()
        {
            var provider = CreateProvider();
            var row = CreateDataRow();

            row["ConnectionString"] = "MainConnectionString";

            var entry = (ConnectionEntry)provider.GetEntry(row);

            Assert.IsNull(entry.CustomConnectionStringName);
        }


        private ConnectionEntryProvider CreateProvider()
        {
            return new ConnectionEntryProvider(TestUtils.ConnectionStringRegistry);
        }


        private DataRow CreateDataRow()
        {
            var row = TestUtils.CreateDataRow();

            row["ConnectionString"] = "CustomConnectionString";
            row["ConnectionOp"] = "new SqlConnection()";
            row["Context"] = "StackTrace";

            return row;
        }


        private DataRow CreateDataRow(object connectionOperation)
        {
            var row = CreateDataRow();

            row["ConnectionOp"] = connectionOperation ?? DBNull.Value;

            return row;
        }
    }
}
