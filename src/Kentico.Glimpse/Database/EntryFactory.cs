using System;
using System.Data;
using System.Linq;

namespace Kentico.Glimpse.Database
{
    internal sealed class EntryFactory
    {
        private readonly IConnectionStringRegistry mConnectionStringRegistry;
        private readonly IEntryProvider[] mEntryProviders;


        public EntryFactory(IConnectionStringRegistry connectionStringRegistry)
        {
            mConnectionStringRegistry = connectionStringRegistry;
            mEntryProviders = new IEntryProvider[]
            {
                new InformationEntryProvider(),
                new CommandEntryProvider(connectionStringRegistry),
                new ConnectionEntryProvider(connectionStringRegistry)
            };
        }


        public object CreateEntry(DataRow row)
        {
            try
            {
                var entryProvider = mEntryProviders.FirstOrDefault(x => x.Matches(row));

                if (entryProvider == null)
                {
                    return null;
                }

                return entryProvider.GetEntry(row);
            }
            catch (Exception exception)
            {
                throw new ArgumentException("Cannot create debug log entry from the specified row", nameof(row), exception);
            }
        }
    }
}
