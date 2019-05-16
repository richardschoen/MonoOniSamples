using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using System.IO;
using MonoNancyDataService.Properties;

namespace MonoNancyDataService
{
    public class CustomRootPathProvider : IRootPathProvider
    {
        /// <summary>
        /// Get or set the desired root path for site content.
        /// Good for customizing default app content location directory
        /// </summary>
        /// <returns></returns>
        public string GetRootPath()
        {
            // Ex: Go up one level
            //return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\"));

            // Set root content path physical directory for static pages and views
            // based on SitePath setting in App Config if set
            if (Settings.Default.SitePath.Trim() != "")
            {
                // If sitepath directory setting is set and found, use it
                // Otherwise use application directory to serve content
                if (Directory.Exists(Settings.Default.SitePath)) {
                    if (Settings.Default.Debug)
                    {
                        Console.WriteLine("Using content path from SitePath setting: " + Settings.Default.SitePath);
                    }
                    return Path.GetFullPath(Settings.Default.SitePath);
                } else // SitePath directory is not found. Use default app location.
                {
                    if (Settings.Default.Debug)
                    {
                        Console.WriteLine("Warning: Content path directory for SitePath Not Found: " + Settings.Default.SitePath);
                        Console.WriteLine("Using content path default: " + Environment.CurrentDirectory);
                    }
                    return Path.GetFullPath(Environment.CurrentDirectory);
                }
            }
            else // SitePath setting is not set. Use default app location.
            {
                if (Settings.Default.Debug)
                {
                    Console.WriteLine("Warning: Content path directory for SitePath Not set.");
                    Console.WriteLine("Using content path default: " + Environment.CurrentDirectory);
                }
                return Path.GetFullPath(Environment.CurrentDirectory);
            }
        }
    }

}
