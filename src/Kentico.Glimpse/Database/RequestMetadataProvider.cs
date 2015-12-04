using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using CMS.Base;

namespace Kentico.Glimpse.Database
{
    internal sealed class RequestMetadataProvider
    {
        private readonly EntryFactory mEntryFactory;


        public RequestMetadataProvider(EntryFactory entryFactory)
        {
            mEntryFactory = entryFactory;
        }


        public RequestMetadata GetRequestMetadata(RequestLog source)
        {
            var entries = new List<object>(source.LogTable.Rows.Count);

            foreach (DataRow row in source.LogTable.Rows)
            {
                try
                {
                    object entry = mEntryFactory.CreateEntry(row);
                    if (entry != null)
                    {
                        entries.Add(entry);
                    }
                }
                catch (ArgumentException exception)
                {
                    Trace.TraceWarning(exception.Message);
                }
            }

            var commandEntries = entries.OfType<CommandEntry>();

            return new RequestMetadata
            {
                Entries = entries,
                Statistics = new RequestMetadataStatistics
                {
                    TotalCommands = commandEntries.LongCount(),
                    TotalDuration = commandEntries.Aggregate(TimeSpan.Zero, (value, entry) => value + entry.Duration),
                    TotalBytesReceived = commandEntries.Sum(x => x.BytesReceived),
                    TotalBytesSent = commandEntries.Sum(x => x.BytesSent),
                    TotalDuplicateCommands = commandEntries.LongCount(x => x.IsDuplicate)
                }
            };
        }
    }
}
