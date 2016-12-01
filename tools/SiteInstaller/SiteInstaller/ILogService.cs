
namespace SiteInstaller
{
    /// <summary>
    /// Logging service
    /// </summary>
    internal interface ILogService
    {
        /// <summary>
        /// Writes given meesage to log
        /// </summary>
        void Log(string message);
    }
}