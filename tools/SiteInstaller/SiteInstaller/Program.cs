using System;
using System.Collections.Generic;
using System.Linq;

using CMS.IO;

namespace SiteInstaller
{
    internal class Program
    {
        private const string DOMAIN = "domain";
        private const string PACKAGE = "package";
        private const string SITE_NAME = "sitename";
        private const string WEB_SITE_PATH = "websitepath";

        private const char ARGUMENT_NAME_VALUE_DELIMITER = '=';
        private static readonly string[] ARGUMENT_HELP_VARIANTS = { "--help", "help", "-h", "/h", "/?" };
        private static readonly string[] REQUIRED_ARGUMENTS = { WEB_SITE_PATH, DOMAIN, PACKAGE, SITE_NAME };
        private const int EXIT_SUCCESS = 0;
        private const int EXIT_FAILURE = -1;

        static int Main(string[] args)
        {
            if (args.Any(key => ARGUMENT_HELP_VARIANTS.Contains(key, StringComparer.OrdinalIgnoreCase)))
            {
                PrintHelp();
                return EXIT_SUCCESS;
            }

            var logService = new ConsoleLogService();
            var arguments = ParseArguments(args);

            if (!ValidateArguments(arguments, logService))
            {
                return EXIT_FAILURE;
            }

            var ih = new ImportHelper(arguments[WEB_SITE_PATH], logService);
            ih.ImportSitePackage(arguments[PACKAGE], arguments[DOMAIN], arguments[SITE_NAME], arguments[WEB_SITE_PATH]);

            return (int)ih.Status;
        }


        private static IDictionary<string, string> ParseArguments(string[] args)
        {
            return args.Select(arg => arg.Split(ARGUMENT_NAME_VALUE_DELIMITER)).ToDictionary(s => s[0].ToLower(), s => s[1]);
        }


        private static bool ValidateArguments(IDictionary<string, string> arguments, ILogService logService)
        {
            if (arguments == null || arguments.Keys.Intersect(REQUIRED_ARGUMENTS).Count() != REQUIRED_ARGUMENTS.Length)
            {
                PrintHelp();
                return false;
            }

            if (!File.Exists(arguments[PACKAGE]))
            {
                logService.Log("Argument 'package' must be path to .zip file that contains exported site.");
                return false;
            }

            if (!Directory.Exists(arguments[WEB_SITE_PATH]))
            {
                logService.Log("Argument 'websitepath' is not valid -- the directory doesn't exist.");
                return false;
            }

            if (String.IsNullOrEmpty(arguments[DOMAIN]))
            {
                logService.Log("Argument 'domain' must be specified.");
                return false;
            }

            if (String.IsNullOrEmpty(arguments[SITE_NAME]))
            {
                logService.Log("Argument 'sitename' must be specified.");
                return false;
            }

            return true;      
        }


        private static void PrintHelp()
        {
            Console.WriteLine("Usage: SiteInstaller siteName=<site name> domain=<site domain> package=<path to package> webSitePath=<physical path to web site root>");
        }
    }
}
