using System;
using System.Data;

using Kentico.Glimpse.Database;

using NSubstitute;
using NUnit.Framework;
using StatePrinter;

namespace Kentico.Glimpse.Tests.Unit.Database
{
    [TestFixture]
    [Category("Unit")]
    public class CommandEntryProviderTests
    {
        [TestCase(null, null, false)]
        [TestCase(true, null, false)]
        [TestCase(false, null, false)]
        [TestCase(null, -1, false)]
        [TestCase(null, 0, false)]
        [TestCase(null, 1, false)]
        [TestCase(true, -1, false)]
        [TestCase(true, 0, false)]
        [TestCase(true, 1, false)]
        [TestCase(false, -1, false)]
        [TestCase(false, 0, true)]
        [TestCase(false, 1, true)]
        public void Matches_DetectsCompatibleRows(object isInformation, object counter, bool expected)
        {
            var provider = CreateProvider();
            var row = CreateDataRow(isInformation, counter);

            var result = provider.Matches(row);

            Assert.AreEqual(expected, result);
        }


        [Test]
        public void GetEntry_ParsesEachColumn()
        {
            var provider = CreateProvider();
            var row = CreateDataRow();

            row["QueryName"] = "Name";
            row["QueryText"] = "Text";
            row["QueryParameters"] = "Parameters";
            row["ConnectionString"] = "CustomConnectionString";
            row["Context"] = "StackTrace";
            row["QueryResults"] = "Result";
            row["QueryDuration"] = TimeSpan.FromSeconds(1).TotalSeconds;
            row["QueryResultsSize"] = 10;
            row["QueryParametersSize"] = 20;

            var entry = provider.GetEntry(row);

            var expected = new CommandEntry
            {
                BytesReceived = 10,
                BytesSent = 20,
                CustomConnectionStringName = "Custom",
                Duration = TimeSpan.FromSeconds(1),
                IsDuplicate = false,
                Name = "Name",
                Parameters = "Parameters",
                Result = "Result",
                StackTrace = "StackTrace",
                Text = "Text"
            };

            var printer = new Stateprinter();
            Assert.AreEqual(printer.PrintObject(expected), printer.PrintObject(entry));
        }


        [Test]
        public void GetEntry_SupportsOptionalValues()
        {
            var provider = CreateProvider();
            var row = CreateDataRow();

            row["QueryName"] = DBNull.Value;
            row["ConnectionString"] = DBNull.Value;

            var entry = (CommandEntry)provider.GetEntry(row);

            Assert.IsTrue(entry.CustomConnectionStringName == null && entry.Name == null);
        }

        [TestCase("A", "B", "C", "D", true)]
        [TestCase("X", "B", "C", "D", false)]
        [TestCase("A", "X", "C", "D", false)]
        [TestCase("A", "B", "X", "D", false)]
        [TestCase("A", "B", "C", "X", false)]
        public void GetEntry_MarksDuplicateEntries(object name, object text, object parameters, object result, bool isDuplicate)
        {
            var provider = CreateProvider();

            var first = CreateDataRow();
            first["QueryName"] = "A";
            first["QueryText"] = "B";
            first["QueryParameters"] = "C";
            first["QueryResults"] = "D";

            var second = CreateDataRow();
            second["QueryName"] = name;
            second["QueryText"] = text;
            second["QueryParameters"] = parameters;
            second["QueryResults"] = result;

            var firstEntry = (CommandEntry)provider.GetEntry(first);
            var secondEntry = (CommandEntry)provider.GetEntry(second);

            Assert.IsTrue(!firstEntry.IsDuplicate && secondEntry.IsDuplicate == isDuplicate);
        }


        [Test]
        public void GetEntry_DetectsCustomConnectionString()
        {
            var provider = CreateProvider();
            var row = CreateDataRow();

            row["ConnectionString"] = "CustomConnectionString";

            var entry = (CommandEntry)provider.GetEntry(row);

            Assert.AreEqual("Custom", entry.CustomConnectionStringName);
        }


        [Test]
        public void GetEntry_IgnoresConnectionStringThatIsNotCustom()
        {
            var provider = CreateProvider();
            var row = CreateDataRow();

            row["ConnectionString"] = "MainConnectionString";

            var entry = (CommandEntry)provider.GetEntry(row);

            Assert.IsNull(entry.CustomConnectionStringName);
        }


        private CommandEntryProvider CreateProvider()
        {
            return new CommandEntryProvider(TestUtils.ConnectionStringRegistry);
        }


        private DataRow CreateDataRow()
        {
            var row = TestUtils.CreateDataRow();

            row["IsInformation"] = false;
            row["Counter"] = 0;
            row["QueryName"] = "Name";
            row["QueryText"] = "Text";
            row["QueryParameters"] = "Parameters";
            row["ConnectionString"] = "CustomConnectionString";
            row["Context"] = "StackTrace";
            row["QueryResults"] = "Result";
            row["QueryDuration"] = 0;
            row["QueryResultsSize"] = 0;
            row["QueryParametersSize"] = 0;

            return row;
        }


        private DataRow CreateDataRow(object isInformation, object counter)
        {
            var row = CreateDataRow();

            row["IsInformation"] = isInformation ?? DBNull.Value;
            row["Counter"] = counter ?? DBNull.Value;

            return row;
        }
    }
}
