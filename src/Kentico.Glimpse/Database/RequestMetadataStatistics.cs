using System;

namespace Kentico.Glimpse.Database
{
    internal sealed class RequestMetadataStatistics
    {
        public long TotalCommands;
        public TimeSpan TotalDuration;
        public long TotalBytesReceived;
        public long TotalBytesSent;
        public long TotalDuplicateCommands;
    }
}
