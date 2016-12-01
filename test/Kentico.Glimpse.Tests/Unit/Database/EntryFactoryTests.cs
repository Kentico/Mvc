using System;
using System.Data;

using Kentico.Glimpse.Database;

using NSubstitute;
using NUnit.Framework;

namespace Kentico.Glimpse.Tests.Unit.Database
{
    [TestFixture]
    [Category("Unit")]
    public class EntryFactoryTests
    {
        [TestCase(null, null, "Operation", typeof(ConnectionEntry))]
        [TestCase(true, null, "Operation", typeof(ConnectionEntry))]
        [TestCase(false, null, "Operation", typeof(ConnectionEntry))]
        [TestCase(null, -1, "Operation", typeof(ConnectionEntry))]
        [TestCase(true, -1, "Operation", typeof(ConnectionEntry))]
        [TestCase(false, -1, "Operation", typeof(ConnectionEntry))]
        [TestCase(null, 0, "Operation", typeof(ConnectionEntry))]
        [TestCase(true, 0, null, typeof(InformationEntry))]
        [TestCase(true, 0, "Operation", typeof(InformationEntry))]
        [TestCase(false, 0, null, typeof(CommandEntry))]
        [TestCase(false, 0, "Operation", typeof(CommandEntry))]
        [TestCase(null, 1, "Operation", typeof(ConnectionEntry))]
        [TestCase(true, 1, null, typeof(InformationEntry))]
        [TestCase(true, 1, "Operation", typeof(InformationEntry))]
        [TestCase(false, 1, null, typeof(CommandEntry))]
        [TestCase(false, 1, "Operation", typeof(CommandEntry))]
        public void CreateEntry_ReturnsCorrectEntryType(object isInformation, object counter, object connectionOperation, Type expectedEntryType)
        {
            var factory = CreateFactory();
            var row = CreateDataRow(isInformation, counter, connectionOperation);

            var entry = factory.CreateEntry(row);

            Assert.AreEqual(expectedEntryType, entry.GetType());
        }


        [TestCase(null, null, null)]
        [TestCase(true, null, null)]
        [TestCase(false, null, null)]
        [TestCase(null, -1, null)]
        [TestCase(true, -1, null)]
        [TestCase(false, -1, null)]
        [TestCase(null, 0, null)]
        [TestCase(null, 1, null)]
        public void CreateEntry_ReturnsNullForAmbiguousRows(object isInformation, object counter, object connectionOperation)
        {
            var factory = CreateFactory();
            var row = CreateDataRow(isInformation, counter, connectionOperation);

            var entry = factory.CreateEntry(row);

            Assert.IsNull(entry);
        }


        [Test]
        public void CreateEntry_ThrowsArgumentExceptionForInvalidRows()
        {
            var factory = CreateFactory();
            var row = CreateDataRow();

            row["IsInformation"] = false;
            row["Counter"] = 0;
            row["QueryText"] = DBNull.Value;

            Assert.Throws<ArgumentException>(() => factory.CreateEntry(row));
        }


        private EntryFactory CreateFactory()
        {
            return new EntryFactory(TestUtils.ConnectionStringRegistry);
        }


        private DataRow CreateDataRow()
        {
            var row = TestUtils.CreateDataRow();

            row["IsInformation"] = false;
            row["Counter"] = 0;
            row["ConnectionOp"] = String.Empty;
            row["QueryName"] = String.Empty;
            row["QueryText"] = String.Empty;
            row["QueryParameters"] = String.Empty;
            row["ConnectionString"] = String.Empty;
            row["Context"] = String.Empty;
            row["QueryResults"] = String.Empty;
            row["QueryDuration"] = 0;
            row["QueryResultsSize"] = 0;
            row["QueryParametersSize"] = 0;

            return row;
        }


        private DataRow CreateDataRow(object isInformation, object counter, object connectionOperation)
        {
            var row = CreateDataRow();

            row["IsInformation"] = isInformation ?? DBNull.Value;
            row["Counter"] = counter ?? DBNull.Value;
            row["ConnectionOp"] = connectionOperation ?? DBNull.Value;

            return row;
        }
    }
}
