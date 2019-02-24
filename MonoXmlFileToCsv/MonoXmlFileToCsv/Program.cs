using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using System.IO;

namespace MonoXmlFileToCsv
{
    /// <summary>
    /// This is a sample XML file extract to CSV format using 
    /// built-in XML formatting. Assumes as single record style XML file.
    /// 
    /// Couple other article links if you want other techniques.
    /// Google: XML to CSV C# or VB
    /// https://gist.github.com/riyadparvez/4467668
    /// DataStreams framework - Paid$$
    /// https://github.com/mrstebo/DataStreams
    /// NuGet for existing DataStreams library https://www.csvreader.com/
    /// </summary>
    class Program
    {


        static void Main(string[] args)
        {

            try
            {

                // Set TZ environment variable for desired timezone
                // TODO - Set your desired timezone
                Environment.SetEnvironmentVariable("TZ", "America/Chicago");

                // Get command line parms and bail out if less than expected number of parms passed. 
                if (args.Length < 4)
                {
                    throw new Exception("4 required parms: [Input XML File Name] [Output CSV File Name] [delimiter-,|~] [doublequotes-Y/N]");
                }

                // Extract parms from command line
                string xmlinputfile = args[0];
                string csvoutputfile = args[1];
                string delimiter = args[2]; 
                string doublequotes = args[3]; 

                // Output parm data
                Console.WriteLine("Start of XML to CSV processing " + DateTime.Now);
                Console.WriteLine("Parameters");
                Console.WriteLine("XML input file: " + xmlinputfile);
                Console.WriteLine("CSV output file: " + csvoutputfile);
                Console.WriteLine("Delimiter: " + delimiter);

                // Do XML to CSV conversion
                XmlConversion conv = new XmlConversion();
                var rtnconv = conv.ConvertXmlFileToCSV(xmlinputfile,csvoutputfile,delimiter,doublequotes);

                // Successful query. Exit program with a success message and 0 error.
                if (rtnconv)
                {
                    Console.WriteLine(String.Format("XML file {0} exported successfully to CSV file {1}", xmlinputfile, csvoutputfile));
                    Console.WriteLine(String.Format("{0} records were converted", conv.GetLastRecordCount()));
                    Environment.ExitCode = 0;
                } else // unsuccessful conversion. Throw error
                {
                    Console.WriteLine(String.Format("Error exporting XML file {0} to CSV file {1}", xmlinputfile, csvoutputfile));
                    throw new Exception(conv.GetLastError());
                }

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
                Console.WriteLine("End of XML to CSV processing " + DateTime.Now);
                Environment.Exit(Environment.ExitCode);
            }

        }
    }
}
