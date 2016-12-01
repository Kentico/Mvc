using System;
using System.Web.Configuration;

namespace SiteInstaller
{
    internal static class WebConfigHelper
    {
        private const string WEB_CONFIG = "web.config";
        private const string CONNECTION_STRING = "CMSConnectionString";


        /// <summary>
        /// Returns connection string from website on given path and given connection string name
        /// </summary>
        internal static string GetConnectionString(string webSitePath, string connectionStringName = CONNECTION_STRING)
        {
            VirtualDirectoryMapping virtualDirectoryMapping = new VirtualDirectoryMapping(webSitePath, true, WEB_CONFIG);
            WebConfigurationFileMap webConfigurationFileMap = new WebConfigurationFileMap();
            webConfigurationFileMap.VirtualDirectories.Add(string.Empty, virtualDirectoryMapping);

            var manager = WebConfigurationManager.OpenMappedWebConfiguration(webConfigurationFileMap, String.Empty);
            var connectionString = manager.ConnectionStrings.ConnectionStrings[connectionStringName];
            return connectionString != null ? connectionString.ConnectionString : string.Empty;
        }
    }
}
