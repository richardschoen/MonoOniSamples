using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace MonoRunPase 
{
    /// <summary>
    /// This program is used to run PASE or QSHELL commands from IBM i.
    /// All console data is written back to STDOUT so it can be processed in 
    /// a program that uses the PaseCommandHelper class
    /// 
    /// Command line arguments are:
    /// P1 - Action (db2-Run DB2 query, pase-Run PASE command, qsh-Run Qshell command, system-run IBMi CL command)
    /// P2 - Command - if db2-enter SQL query. 
    ///                if pase enter command file to run such as: ls, rm, etc.
    ///                if qshell, enter entire qshell command line to run: ex: ls /tmp 
    ///                if system, enter entire CL command to run.
    /// P3 - Arguments if db2, simply pass a single blank space.
    ///                if pase, pass any arguments that go with the command/program/script name being called.
    ///                if qshell simply pass a single blank space. All real parm data is passed in command line via P2.
    ///                if system, simply pass a single blank space. This parm is not used.
    /// </summary>
    class Program
    {

        // Declare any work variables if needed
        static string parmaction = "";
        static string parmcommand = "";
        static string parmarguments = "";
        static string parmdb2outputtype = "";
        static string parmdb2outputfile = "";

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">command line parameters</param>
        static void Main(string[] args)
        {

            try
            {
                // Set TZ environment variable for desired timezone
                Environment.SetEnvironmentVariable("TZ", "America/Chicago");

                Console.WriteLine("Start Run Command " + DateTime.Now);

                // Validate passed in parms if any needed.
                if (args.Length < 5)
                {
                 throw new Exception("At least 5 parms required: [Action-db2/system/pase/qsh] [Pase Command-ls,qsh,etc.] [Pase Command Parameters] [DB2OutputType-CSV/XML/JSON or blank if not using db2 action] [DB2OutputFile-IFS file for DB2 output or blank if not using DB2 action]");                  
                }

                // Move parms into more meaningful variable names
                parmaction = args[0];
                parmcommand = args[1];
                parmarguments = args[2];
                parmdb2outputtype = args[3];
                parmdb2outputfile = args[4];

                // Instantiate a Pase Command Helper instance
                PaseCommandHelper pase = new PaseCommandHelper();

                Console.WriteLine("P1=Action: " + args[0]);
                Console.WriteLine("P2=Command: " + args[1]);
                Console.WriteLine("P3=Arguments: " + args[2]);
                Console.WriteLine("P4=DB2OutputTye: " + args[3]);
                Console.WriteLine("P5=DB2OutputFile: " + args[4]);

                bool rtn = false;
                if (parmaction.Trim().ToLower()=="qsh") {
                    Console.WriteLine("Option:QshCommand");
                    Console.WriteLine("--BeginStdout--");
                    rtn = pase.RunQshCommand(parmcommand,true,true);
                    Console.WriteLine("--EndStdout--");
                    Console.WriteLine("ReturnVal:" + rtn);
                }
                else if (parmaction.Trim().ToLower() == "system")
                {
                    Console.WriteLine("Option:CLSystemCommand");
                    Console.WriteLine("--BeginStdout--");
                    rtn = pase.RunClSystemCommand(parmarguments,true);
                    Console.WriteLine("--EndStdout--");
                    Console.WriteLine("ReturnVal:" + rtn);
                }
                else if (parmaction.Trim().ToLower() == "db2")
                {
                    Console.WriteLine("Option:Db2Query");
                    Console.WriteLine("--BeginStdout--");
                    bool debugdb2 = false;
                    rtn = pase.RunDb2Query(parmcommand, parmarguments,true);
                    if (parmdb2outputtype.Trim().ToLower() == "xml")
                    {
                        var dt = pase.ConvertDb2ResultsToDataTable(pase.GetLastStdout(), "Table1", true, debugdb2, true);
                        var rtnconv = pase.GetQueryResultsDataTableToXmlFile(dt,parmdb2outputfile, replace: true);
                        Console.WriteLine("XML Convert success: " + rtnconv);
                        if (rtnconv == false)
                        {
                            Console.WriteLine(pase.GetLastError());
                            throw new Exception(pase.GetLastError());
                        }
                    }
                    else if (parmdb2outputtype.Trim().ToLower() == "json")
                    {
                        var dt = pase.ConvertDb2ResultsToDataTable(pase.GetLastStdout(), "Table1", true, debugdb2, true);
                        var rtnconv = pase.GetQueryResultsDataTableToJsonFile(dt, parmdb2outputfile, replace: true);
                        Console.WriteLine("JSON Convert success: " + rtnconv);
                        if (rtnconv == false)
                        {
                            Console.WriteLine(pase.GetLastError());
                            throw new Exception(pase.GetLastError());
                        }
                    }
                    else if (parmdb2outputtype.Trim().ToLower() == "csv")
                    {
                        var dt = pase.ConvertDb2ResultsToDataTable(pase.GetLastStdout(), "Table1", true, debugdb2, true);
                        var rtnconv = pase.GetQueryResultsDataTableToCsvFile(dt, parmdb2outputfile, replace: true);
                        Console.WriteLine("CSV Convert success: " + rtnconv);
                        if (rtnconv==false)
                        {
                            Console.WriteLine(pase.GetLastError());
                            throw new Exception(pase.GetLastError());
                        }
                    }
                    else 
                    {
                        Console.WriteLine("No data conversion performed. Only logged to STDOUT.");
                    }
                    Console.WriteLine("--EndStdout--");
                    Console.WriteLine("ReturnVal:" + rtn);
                    Console.WriteLine("RowsReturned:" + pase.GetLastResultCount());

                }
                else
                {
                    Console.WriteLine("Option:PaseCommand");
                    Console.WriteLine("--BeginStdout--");
                    rtn = pase.RunPaseCommand(parmcommand, parmarguments,true);
                    Console.WriteLine("--EndStdout--");
                    Console.WriteLine("ReturnVal:" + rtn);
                }
                Console.WriteLine("PaseExitCode:" + pase.GetLastPaseExitCode());

                // Set exit code based on pase exit
                Environment.ExitCode = pase.GetLastPaseExitCode();

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
                Console.WriteLine("End Run Command " + DateTime.Now);
                Environment.Exit(Environment.ExitCode);
            }

        }
    }
}
