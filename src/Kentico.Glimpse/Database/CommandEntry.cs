using System;

namespace Kentico.Glimpse.Database
{
    internal sealed class CommandEntry
    {
        public string Name;
        public string Text;
        public string Parameters;
        public string Result;
        public string CustomConnectionStringName;
        public TimeSpan Duration;
        public long BytesReceived;
        public long BytesSent;
        public string StackTrace;
        public bool IsDuplicate;
    }
}
