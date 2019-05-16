using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Nancy.Hosting.Self;
using MonoNancyDataService.Properties;

namespace MonoNancyDataService
{
    class Program
    {
        // Start main program
        static void Main(string[] args)
        {

            // Instantiate Logger
            LogToFile logger = new LogToFile();
            bool debug = Settings.Default.Debug;

            // Create web server config
            HostConfiguration hostConf = new HostConfiguration();

            // Setting rewritelocalhost=true and binding to localhost is supposed to listen on 0.0.0.0 all addresses
            hostConf.RewriteLocalhost = Properties.Settings.Default.RewriteLocalHost;

            // Start the site listener
            using (var host = new NancyHost(hostConf,new Uri(Properties.Settings.Default.Uri)))
            {

                try
                {

                    if (debug)
                        logger.WriteLogFile("Starting service on URL: " + Properties.Settings.Default.Uri,"INFOSTARTUP");

                    // Make sure /tmp folder exists
                    if (Directory.Exists(Settings.Default.TempDirectory)==false)
                        Directory.CreateDirectory(Settings.Default.TempDirectory);

                    // Start web site 
                    host.Start();
                
                    // Write out console messages for info
                    Console.WriteLine(Properties.Settings.Default.AppDescription);
                    Console.WriteLine("Listening on: " + Properties.Settings.Default.Uri);
                    Console.WriteLine(Settings.Default.AppDescription + " is active.");

                    // Console.readline waits for key press to exit. Changed to indefinite wait
                    //Console.WriteLine("Press enter to exit the application");
                    //Console.ReadLine();

                    // Wait indefinitely (This option insures the web app will not end. Need to kill process or add API exit option)
                    System.Threading.Thread.Sleep(-1);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("MonoNancyDataService startup error: " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    // Exit normally
                    Environment.Exit(0);
                }


            }
        }
    }
}
