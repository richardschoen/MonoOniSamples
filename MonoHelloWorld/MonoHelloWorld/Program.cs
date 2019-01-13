using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace MonoHelloWorld
{
    /// <summary>
    /// This is a sample MonoHelloWorld app setting up a basic 
    /// template for your console apps running on IBMi with Mono.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            // Expected number of command line parms
            int expectedParms = 3;

            // Set TZ environment variable for desired timezone
            // TODO: Timezone is set at offset 00:00:00. Need to pick up UTC from setting somewhere. ?? See mono source perhaps. 
            // TODO: Still need to figure best way to do timezone. Can we make TZ env variable work ????
            // TODO: List all possible timezone choices to a file
            Environment.SetEnvironmentVariable("TZ", "America/Chicago");

            // Getting the current system timezone info. 
            TimeZoneInfo tz = TimeZoneInfo.Local;
            Console.WriteLine("TimeZoneInfo.Local Offset:" + tz.BaseUtcOffset);
            Console.WriteLine("TimeonInfo.Local UTC Time:" + TimeZoneInfo.ConvertTime(DateTime.Now, tz).ToString());

            // Let's get some of the common system attributes we might want to use

            // Get assembly path
            string assemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            // Get assembly full path
            string assemblyFullPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            // Get assembly exe
            string assemblyExe = System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            // Get current program name
            string currentprogramName = System.AppDomain.CurrentDomain.FriendlyName;           
            // Get current program base directory
            string currentprogramDir = System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            // Get main module full path
            string mainModule = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            // Get current directory
            string currentDirectory = Environment.CurrentDirectory;
            // Get system directory
            string systemDirectory = Environment.SystemDirectory;
            // Get user name
            string userName = Environment.UserName;
            // Get machine name
            string machineName = Environment.MachineName;
            // Get current time
            var currDateTime = System.DateTime.Now.ToString();

            try
            {

                // Get command line parms and bail out if less than expected number of parms passed. 
                // Parms should be passed with double quotes around values and single space between parms.
                // Example Windows command line call: pgm1.exe "parm1" "parm2" "parm3"
                // Example IBM i PASE or Qshell command line call: mono pgm1.exe "parm1" "parm2" "parm3"
                if (args.Length < expectedParms)
                {
                    throw new Exception(expectedParms + " required parms: [Parm 1] [Parm 2] [Parm 3=true|false]");
                }

                // Extract parms from command line
                string parm1 = args[0];
                string parm2 = args[1];
                bool parm3 = Convert.ToBoolean(args[2]);

                Console.WriteLine("UTC Time:" + TimeZoneInfo.ConvertTime(DateTime.Now, tz).ToString());
                Console.WriteLine("UTC Offset:" + tz.GetUtcOffset(DateTime.Now).ToString());

                // Output any log info to console. This info will get returned in STDOUT which 
                // also gets pipelined back to IBMi job if you use the MONO command to call this from 
                // your IBMi system jobs. 
                Console.WriteLine("Starting " + assemblyExe + " " + System.DateTime.Now.ToString());
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("Parameters:");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("Parm 1:" + parm1);
                Console.WriteLine("Parm 2:" + parm2);
                Console.WriteLine("Parm 3:" + parm3);

                // Do some work now. (query database, transfer file, etc....)
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("Date/time, various paths, etc:");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("Current date/time: " + currDateTime);
                Console.WriteLine("Assembly full path: " + assemblyFullPath);
                Console.WriteLine("Assembly path: " + assemblyPath);
                Console.WriteLine("Assembly Exe: " + assemblyExe);
                Console.WriteLine("Current program: " + currentprogramName);
                Console.WriteLine("Current program directory: " + currentprogramDir);
                Console.WriteLine("Current directory: " + currentDirectory);
                Console.WriteLine("Main module: " + mainModule);
                Console.WriteLine("System directory: " + systemDirectory);
                Console.WriteLine("Machine name: " + machineName);
                Console.WriteLine("User name: " + userName);

                // Successful query. Exit program with a success message and 0 error.
                Console.WriteLine(assemblyExe +" was successful");
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
                Console.WriteLine("Ending " + assemblyExe + " " + System.DateTime.Now.ToString());
                Environment.Exit(Environment.ExitCode);
            }

        }
    }
}
