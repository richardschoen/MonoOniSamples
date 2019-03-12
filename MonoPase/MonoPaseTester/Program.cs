using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoPase;

namespace MonoPaseTester
{
 /// <summary>
 /// 
 /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            try
            {

                    string sql = "";
                    string tofile = "/tmp/monopasetester.txt";

                    PaseCommandHelper pase = new PaseCommandHelper();

                    // Set SQL if passed or use default
                    if (args.Length >= 1)
                    {
                        sql = args[0];
                    } else
                    {
                        sql = "select * from qiws.qcustcdt";
                    }

                    Console.WriteLine("SQL:" + sql);
                    Console.WriteLine("ToFile:" + tofile);

                    // Run query using db2 cli. Log query to stdout. For large query may not want to.
                    var rtnqry = pase.RunDb2Query(sql, "", true);
                    if (rtnqry == false)
                    {
                        throw new Exception("RunDb2Query failed. Error:" + pase.GetLastError());
                    }

                    // Get stdout from db2 cli call and extract into DataTable object
                    var dt = pase.ConvertDb2ResultsToDataTable(pase.GetLastStdout());
                    if (dt == null)
                    {
                    throw new Exception("Error converting DB2 results to DataTable.");
                    }

                    // Output result to CSV file
                    var rtn=pase.GetQueryResultsDataTableToCsvFile(dt, tofile,replace:true);
                    if (rtn == false)
                    {
                    throw new Exception("Error converting DataTable to CSV. Error:" + pase.GetLastError());
                    }

                Console.WriteLine("Done");

            } catch(Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
            }

        }
    }
}
