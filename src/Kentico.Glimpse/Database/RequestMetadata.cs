using System.Collections.Generic;

namespace Kentico.Glimpse.Database
{
    internal sealed class RequestMetadata
    {
        public RequestMetadataStatistics Statistics = new RequestMetadataStatistics();
        public List<object> Entries = new List<object>();
    }
}
