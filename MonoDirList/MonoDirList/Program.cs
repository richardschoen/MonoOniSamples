using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace MonoDirList
{
    /// <summary>
    /// This program is used to scan the selected top level folder and subfolders for file and directory info.
    /// Information is written out to an IFS file and also to a physical file. 
    /// 
    /// This program is used to write lines of data to a file
    /// See following link to search on multiple patterns:
    /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/b0c31115-f6f0-4de5-a62d-d766a855d4d1/directorygetfiles-with-searchpattern-to-get-all-dll-and-exe-files-in-one-call?forum=netfxbcl
    /// // TODO - Tweak to handle file filter filters. PASE/AIX paths are case sensitive and .pdf and .PDF don't get all files, only corresponding case.
    /// </summary>
    class Program
    {

        // Declare any work variables if needed
        static string parmdirectory = "";
        static string parmfilefilter = "";
        static bool parmalldirectories = false;
        static string parmoutputfile = "";
        static bool parmreplace = false;
        static int parmsleepms = 0;
        static string delim = ",";
        static long totalsize = 0;

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">command line parameters</param>
        static void Main(string[] args)
        {

            try
            {

                // Example: Set TZ environment variable for desired timezone
                // Note: You MUST set up TZ before calling any date/time functions. 
                Environment.SetEnvironmentVariable("TZ", Properties.Settings.Default.TimeZone.Trim());
                Console.WriteLine("TimeZone: " + Properties.Settings.Default.TimeZone.Trim());
                Console.WriteLine("Current time once TimeZone TZ env variable set: " + DateTime.Now);

                // Getting the current system timezone info. 
                TimeZoneInfo tz = TimeZoneInfo.Local;
                Console.WriteLine("TimeZoneInfo.Local Offset:" + tz.BaseUtcOffset);
                Console.WriteLine("DateTime.Now:" + DateTime.Now.ToString());
                // Removed below line. It started causing errors on Daylight savings time day. 3/10/2019 at 1am. See Notes.txt. Not sure why.
                // TODO - test this scenario at some point. See if it works the day after dylight savings. Might be a Mono data bug possibly ??
                // DateTime.Now by itself eems to work though as does using the same timezone def against files. Wierd.
                //Console.WriteLine("TimeonInfo.Local UTC Time:" + TimeZoneInfo.ConvertTime(DateTime.Now, tz).ToString());

                // Run DB2 command
                PaseCommandHelper pase = new PaseCommandHelper();

                // Validate passed in parms if any needed.
                if (args.Length < 6)
                {
                 throw new Exception("At least 6 parms required:\r\n[Input directory to list]\r\n[File filter. Ex: *.* or *.pdf]\r\n[includesubdirectories-true/false]\r\n[File name to write]\r\n[replace-true/false]\r\n[Sleep throttle in milliseconds-Ex:5000-5 seconds]");                  
                }

                // Move parms into more meaningful variables
                parmdirectory = args[0];
                parmfilefilter = args[1];
                parmalldirectories = Convert.ToBoolean(args[2]);
                parmoutputfile = args[3];
                parmreplace = Convert.ToBoolean(args[4]);
                parmsleepms = Convert.ToInt32(args[5]);

                // TODO: This errors out if checking a symbolic link. Best way to check symbolic link ?
                //Make sure input dir exists
                if (Directory.Exists(parmdirectory) == false)
                {
                    throw new Exception("Directory " + parmdirectory + " not found.");
                }

                // if output file exists, delete it if replace selected
                if (File.Exists(parmoutputfile))
                {
                    // replace selected
                    if (parmreplace)
                    {
                        File.Delete(parmoutputfile);
                    }
                }

                // List files now

                // Default filter to all if blanks
                if (parmfilefilter.Trim() == "")
                    parmfilefilter = "*.*";

                // See if selected main directory is a symbolic link rather than regular directory. If so, bail out.
                DirectoryInfo di = new DirectoryInfo(parmdirectory);

                FileAttributes fa = di.Attributes;
                // See if directory is a symbolic link
                if ((fa & FileAttributes.ReparsePoint) != 0)
                {
                    throw new Exception("Directory " + parmdirectory + " appears to be a symbolic link.");
                }

                string[] paths = {parmdirectory};
                RecursiveFileProcessor recurse1 = new RecursiveFileProcessor();

                // Set time zone value for file date/times
                recurse1.SetTimeZone(tz);
                // Set sleep throttle
                recurse1.SetSleepThrottle(parmsleepms);
                recurse1.IterateDirectories(paths,parmoutputfile);


                // TODO - Decide where these commands belong. Table should be dropped up front and 
                // and then after list processing, copy the results to DB2 table using CPYTOIMPF command.
                // Also the temp table in MONOTEMP could be soft coded perhaps so this process can be run 
                // by multiple simultaneous processes. Currently constrained to a s single OUTFILE name.

                // Drop temp table from MONOTEMP if already exists. 
                var rtnsql1 = pase.RunDb2NonQuery("DROP TABLE MONOTEMP.MONODIRLST");
                Console.WriteLine(String.Format("Drop table:" + rtnsql1));

                // Create temp table. If any errors, bail out.
                var rtnsql2 = pase.RunDb2NonQuery("CREATE TABLE MONOTEMP.MONODIRLST(DILEDIRTYP varchar(20),FILEDIRNAM varchar(255),DIRNAME varchar(255), FILENAME varchar(255), FILEEXT varchar(20), FILEDIRSIZ int, CRTTIME varchar(26),LSTWRTTIME varchar(26), LSTACCTIME varchar(26), CRTTIMEISO timestamp, LSTWRTISO timestamp, LSTACCISO timestamp)");
                Console.WriteLine(String.Format("Create table:" + rtnsql2));
                if (rtnsql2 == false)
                {
                    throw new Exception("Error occurred when trying to create file MONOTEMP/MONODIRLST");
                }

                // Copy list results to temp table. If any errors, bail out.
                var rtncmd2 = pase.RunClSystemCommand("CPYFRMIMPF FROMSTMF('" + parmoutputfile.Trim() +  "') TOFILE(MONOTEMP/MONODIRLST) MBROPT(*REPLACE) RCDDLM(*CRLF) FROMRCD(1) RMVCOLNAM(*YES)");
                Console.WriteLine(String.Format("Run CPYFRMIMPF command:" + rtncmd2));
                if (rtncmd2 == false)
                {
                    throw new Exception("Error occurred when trying to copy records to file MONOEMP/MONODIRLST");
                }

                // Set exit code to 0 for success
                Environment.ExitCode = 0;

                // STDOUT message denoting successful completion of work
                // What does this app do ?
                Console.WriteLine(String.Format("Listing directory: {0}" , parmdirectory));
                Console.WriteLine(String.Format("Output file: {0}", parmoutputfile));
                Console.WriteLine(String.Format("File filter: {0}",parmfilefilter));
                Console.WriteLine(String.Format("AllDirs: {0}",parmalldirectories));
                Console.WriteLine(String.Format("Replace output file: {0}",parmreplace));
                Console.WriteLine(String.Format("Sleep throttle ms: {0}",parmsleepms));
                Console.WriteLine(recurse1.GetFileCount() + " files located in " + parmdirectory + " " + DateTime.Now);
                Console.WriteLine(recurse1.GetDirCount() + " directories located in " + parmdirectory + " " + DateTime.Now);
                Console.WriteLine(recurse1.GetSymbolicLinkCount() + " symbolic links located in " + parmdirectory + " " + DateTime.Now);
                Console.WriteLine(recurse1.GetTotalByteCount()  + " bytes in " + parmdirectory + " " + DateTime.Now);
                Console.WriteLine(recurse1.GetInvalidCount() + " invalid links located in " + parmdirectory + " " + DateTime.Now);

            }
            catch (Exception ex)
            {
                Environment.ExitCode = 99;
                Console.WriteLine("Error:" + ex.Message + " Stack trace:" +  ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception:" + ex.InnerException.Message + " Stack trace:" + ex.InnerException.StackTrace);

                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine("Inner/inner exception:" + ex.InnerException.InnerException.Message + " Stack trace:" + ex.InnerException.InnerException.StackTrace);
                    }

                }

            }
            finally
            {
                Console.WriteLine("ExitCode:" + Environment.ExitCode);
                Environment.Exit(Environment.ExitCode);
            }

        }

    }
}
