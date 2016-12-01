using System;

using CMS.Base;
using CMS.CMSImportExport;
using CMS.IO;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.DataEngine;


namespace SiteInstaller
{
    internal sealed class ImportHelper
    {
        bool importHandlersInitialized = false;
        bool importSiteFailed = false;
        string mWebSitePath = null;
        ImportResult mState = ImportResult.NOTSTARTED;
        ILogService mLogService;

        public enum ImportResult : int
        {
            NOTSTARTED = 2,
            SUCCEEDED = 0,
            FAILED = -1
        };


        private void ImportError_Execute(object sender, ImportErrorEventArgs e)
        {
            importSiteFailed = true;
            mLogService.Log(e.Exception.Message);
        }


        private void ImportProvider_OnProgressLog(string message)
        {
#if DEBUG
            mLogService.Log(message);
#endif
        }


        private void InitializeKenticoApplication()
        {
            ConnectionHelper.ConnectionString = WebConfigHelper.GetConnectionString(mWebSitePath);

            // Init CMS environment
            CMSApplication.Init();

            // Clear objects cached during previous (possibly failed) installation
            ModuleManager.ClearHashtables(false);
        }


        public ImportHelper(string webSitePath, ILogService logService)
        {
            if (string.IsNullOrEmpty(webSitePath))
            {
                throw new ArgumentNullException("webSitePath", "Website path has to be specified.");
            }

            mWebSitePath = webSitePath;
            mLogService = logService;
        }


        public ImportResult Status
        {
            get
            {
                return mState;
            }
        }


        public void ImportSitePackage(string packagePath, string siteDomainName, string siteName, string webSitePath)
        {
            mState = ImportResult.NOTSTARTED;

            try
            {
                InitializeKenticoApplication();

                string fullPackagePath = Path.GetFullPath(packagePath);

                // Disable smart search indexing (execute once at the end)
                using (new CMSActionContext { EnableSmartSearchIndexer = false })
                {
                    string siteDisplayName = siteName;

                    SiteInfo si = SiteInfoProvider.GetSiteInfo(siteName);
                    if (si == null)
                    {
                        if (!importHandlersInitialized)
                        {
                            // Register event handlers for import/export
                            ImportExportEvents.ImportError.Execute += ImportError_Execute;
                            ImportProvider.OnProgressLog += ImportProvider_OnProgressLog;
                            importHandlersInitialized = true;
                        }

                        UserInfo userInfo = UserInfoProvider.AdministratorUser;
                        var settings = new SiteImportSettings(userInfo);

                        settings.EnableSearchTasks = false;
                        settings.SiteDomain = siteDomainName;
                        settings.SiteName = siteName;
                        settings.SiteDisplayName = siteDisplayName;
                        settings.CreateVersion = false;

                        string zipName = Path.GetFileName(fullPackagePath);

                        fullPackagePath = fullPackagePath.Remove(fullPackagePath.Length - zipName.Length);
                        fullPackagePath += ZipStorageProvider.GetZipFileName(zipName);

                        settings.TemporaryFilesPath = fullPackagePath;
                        settings.SourceFilePath = fullPackagePath;
                        settings.TemporaryFilesCreated = true;

                        settings.WebsitePath = webSitePath;

                        // Import all, but only add new data
                        settings.ImportType = ImportTypeEnum.AllNonConflicting;
                        settings.ImportOnlyNewObjects = true;
                        settings.CopyFiles = false;

                        // Allow bulk inserts for faster import, web templates must be consistent enough to allow this without collisions
                        settings.AllowBulkInsert = true;

                        settings.SetSettings(ImportExportHelper.SETTINGS_DELETE_SITE, true);
                        settings.SetSettings(ImportExportHelper.SETTINGS_DELETE_TEMPORARY_FILES, false);
                        settings.SetSettings(ImportExportHelper.SETTINGS_RUN_SITE, false);

                        // Load default selection...
                        settings.LoadDefaultSelection();

                        // ...and import selected objects only
                        settings.DefaultProcessObjectType = ProcessObjectEnum.Selected;

                        ImportManager im = new ImportManager(settings);

                        importSiteFailed = false;
                        im.Import(null);

                        if (importSiteFailed)
                        {
                            SiteInfoProvider.StopSite(siteName);
                            mState = ImportResult.FAILED;
                            return;
                        }

                        SiteInfoProvider.RunSite(siteName);

                        mState = ImportResult.SUCCEEDED;
                    }
                    else
                    {
                        mLogService.Log(String.Format("Site with a given name ('{0}') already exists.", siteName));
                    }
                }
            }
            catch (Exception exception)
            {
                mState = ImportResult.FAILED;
                mLogService.Log(String.Format("Import failed due to following error: {0}", exception.Message));
            }
        }
    }
}
