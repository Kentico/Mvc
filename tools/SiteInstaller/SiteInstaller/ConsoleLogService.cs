using System;

namespace SiteInstaller
{
    /// <summary>
    /// Logging service with console output
    /// </summary>
    internal sealed class ConsoleLogService : ILogService
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
