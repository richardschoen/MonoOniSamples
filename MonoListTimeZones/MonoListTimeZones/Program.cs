using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.ObjectModel;

namespace MonoListTimeZones
{
    /// <summary>
    /// This is a sample MonoListTimeZones
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            try {

            // Example: Set TZ environment variable for desired timezone
            // Note: You MUST set up TZ before calling any date/time functions. 
            Environment.SetEnvironmentVariable("TZ", "America/Chicago");
            Console.WriteLine("Sample timezone in TZ environment variable: America/Chicago");
            Console.WriteLine("Current time once TZ env variable set: " + DateTime.Now);

            // Display all timezone values. 
            ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();
            Console.WriteLine("The local system has the following {0} time zones", zones.Count);
            Console.WriteLine("Set the TZ environment variable in your program \r\nto the desired time zone value before using date/time functions.");
            Console.WriteLine("Time Zone List");
            foreach (TimeZoneInfo zone in zones)
              Console.WriteLine(zone.Id);

            Environment.ExitCode = 0;             

            }
         catch (Exception ex)
            {
                // Write error message to console/STDOUT log and set exit code 
                // to 99 to indicate an error to the Operating System. 
                Console.WriteLine("Error: " + ex.Message);
                Environment.ExitCode = 99;
            }
            finally
            {
                // Exit the program
                Console.WriteLine("ExitCode: " + Environment.ExitCode);
                Environment.Exit(Environment.ExitCode);
            }

        }
    }
}
