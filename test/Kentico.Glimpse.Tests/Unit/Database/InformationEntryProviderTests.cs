using System;
using System.Data;

using Kentico.Glimpse.Database;

using NUnit.Framework;
using StatePrinter;

namespace Kentico.Glimpse.Tests.Unit.Database
{
    [TestFixture]
    [Category("Kentico.Glimpse.Database")]
    public class InformationEntryProviderTests
    {
        [TestCase(null, null, false)]
        [TestCase(true, null, false)]
        [TestCase(false, null, false)]
        [TestCase(null, -1, false)]
        [TestCase(null, 0, false)]
        [TestCase(null, 1, false)]
        [TestCase(true, -1, false)]
        [TestCase(true, 0, true)]
        [TestCase(true, 1, true)]
        [TestCase(false, -1, false)]
        [TestCase(false, 0, false)]
        [TestCase(false, 1, false)]
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

            row["QueryName"] = "Title";
            row["QueryText"] = "Text";

            var entry = provider.GetEntry(row);

            var expected = new InformationEntry
            {
                Title = "Title",
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

            var entry = (InformationEntry)provider.GetEntry(row);

            Assert.IsTrue(entry.Title == null);
        }

        
        private InformationEntryProvider CreateProvider()
        {
            return new InformationEntryProvider();
        }


        private DataRow CreateDataRow()
        {
            var row = TestUtils.CreateDataRow();

            row["IsInformation"] = true;
            row["Counter"] = 0;
            row["QueryName"] = "Title";
            row["QueryText"] = "Text";

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
