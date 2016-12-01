using System.Collections.Generic;
using System.Linq;
using CMS.DataEngine;

namespace Kentico.Ecommerce.Tests
{
    internal static class TestExtensionMethods
    {
        internal static void InsertDB(this IEnumerable<BaseInfo> list)
        {
            list.ToList().ForEach(info => info.Insert());
        }
    }
}
