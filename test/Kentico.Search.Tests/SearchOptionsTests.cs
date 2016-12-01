using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Search.Tests
{
    public class SearchOptionsTests
    {
        [TestFixture]
        [Category.Unit]
        public class Ctor
        {
            [TestCase(null)]
            [TestCase("")]
            [TestCase("      ")]
            [Description("Search query cannot be null or whitespace string.")]
            public void SearchOptions_NoQueryText_ThrowsArgumentException(string query)
            {
                Assert.Throws<ArgumentException>(() => new SearchOptions(query, new[] {"index"}));
            }


            [Test]
            public void SearchOptions_NoIndex_ThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SearchOptions("search query", null));
            }
        }
    }
}
