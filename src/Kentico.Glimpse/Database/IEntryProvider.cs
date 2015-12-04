using System.Data;

namespace Kentico.Glimpse.Database
{
    internal interface IEntryProvider
    {
        bool Matches(DataRow row);
        object GetEntry(DataRow row);
    }
}
