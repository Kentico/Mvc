using System;
using System.Collections.Generic;
using System.Data;

namespace Kentico.Glimpse.Database
{
    internal class CommandEntryProvider : IEntryProvider
    {
        private readonly IConnectionStringRegistry mConnectionStringRegistry;
        private readonly HashSet<string> mEntryHashes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


        public CommandEntryProvider(IConnectionStringRegistry connectionStringRegistry)
        {
            mConnectionStringRegistry = connectionStringRegistry;
        }


        public bool Matches(DataRow row)
        {
            var isInformation = row["IsInformation"];
            var counter = row["Counter"];

            if (isInformation == DBNull.Value || counter == DBNull.Value)
            {
                return false;
            }

            return !(bool)isInformation && (int)counter >= 0;
        }


        public object GetEntry(DataRow row)
        {
            var entry = new CommandEntry
            {
                Name = GetName(row),
                Text = GetText(row),
                Parameters = GetParameters(row),
                CustomConnectionStringName = GetCustomConnectionStringName(row),
                StackTrace = GetStackTrace(row),
                Duration = GetDuration(row),
                BytesReceived = GetBytesReceived(row),
                BytesSent = GetBytesSent(row),
                Result = GetResult(row),
                IsDuplicate = false
            };

            // Detect duplicate queries using a hash that is a concatenation of query name, text and the text representation of query parameters and result
            string hash = GetEntryHash(entry);
            if (mEntryHashes.Contains(hash))
            {
                entry.IsDuplicate = true;
            }
            else
            {
                mEntryHashes.Add(hash);
            }

            return entry;
        }

        
        private string GetName(DataRow row)
        {
            var value = row["QueryName"];

            if (value == DBNull.Value)
            {
                return null;
            }

            return (string)value;
        }

        
        private string GetText(DataRow row)
        {
            return (string)row["QueryText"];
        }

        
        private string GetParameters(DataRow row)
        {
            return (string)row["QueryParameters"];
        }

        
        private string GetCustomConnectionStringName(DataRow row)
        {
            var value = row["ConnectionString"];

            if (value == DBNull.Value)
            {
                return null;
            }

            var connectionString = (string)value;

            return mConnectionStringRegistry.GetCustomConnectionStringName(connectionString);
        }

        
        private string GetStackTrace(DataRow row)
        {
            return (string)row["Context"];
        }

        
        private string GetResult(DataRow row)
        {
            return (string)row["QueryResults"];
        }

        
        private TimeSpan GetDuration(DataRow row)
        {
            var value = (double)row["QueryDuration"];

            // We cannot use the FromSeconds method because of possible loss of precision
            return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerSecond));
        }

        
        private long GetBytesReceived(DataRow row)
        {
            return (int)row["QueryResultsSize"];
        }

        
        private long GetBytesSent(DataRow row)
        {
            return (int)row["QueryParametersSize"];
        }


        private string GetEntryHash(CommandEntry entry)
        {
            return String.Join("|", entry.Name, entry.Text, entry.Parameters, entry.Result);
        }
    }
}
