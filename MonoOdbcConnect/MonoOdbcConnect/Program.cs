using System;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Collections.Generic;

namespace MonoOdbcConnect
{

    //----------------------------------------------------------------------------------
    // This sample assumes the new Native PASE IBM i Access ODBC Driver is installed as is UnixODBC
    // UnixODBC can be installed via the Open Source Package Management menu in ACS client. (Tools/Open Source Package Management)
    //
    // This is meant to be an SQL query exerciser for the IBM i Access ODBC driver
    //
    // UnixODBC config files: 
    //
    // UnixODBC ODBC DSN def file
    // /qopensys/etc/odbc.ini
    //
    // UnixODBC Driver Install info
    // /qopensys/etc/odbcinst.ini
    //----------------------------------------------------------------------------------

    public class MonoOdbcConnect
    {
        public static void Main(string[] args)
        {

            string delim = "|";
            string parmsql = "";
            string parmconnstring="";

            try
            {

                // Set TZ environment variable for desired timezone
                // TODO: Timezone is set at offset 00:00:00. Need to pick up UTC from setting somewhere. ?? See mono source perhaps. 
                // TODO: Still need to figure best way to do timezone. Can we make TZ env variable work ????
                // TODO: List all possible timezone choices to a file
                Environment.SetEnvironmentVariable("TZ", "America/Chicago");

                // Getting the current system timezone info. 
                TimeZoneInfo tz = TimeZoneInfo.Local;
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("MonoOdbcConnect Program Start " + System.DateTime.Now.ToString());
                Console.WriteLine("TimeZoneInfo.Local Offset:" + tz.BaseUtcOffset);
                Console.WriteLine("TimeonInfo.Local UTC Time:" + TimeZoneInfo.ConvertTime(DateTime.Now, tz).ToString());

                // Must pass connection string
                if (args.Length < 2)
                {

                    throw new Exception("Parms required: [IBM i Access ODBC Connection Ex ODBC Connect DSN: DSN= mydsn;UID=USER1;PWD=PASS1;] [sql query]");
                }

                // Get parm values
                parmconnstring = args[0];
                parmsql = args[1];

                // Create a connection
                Console.WriteLine("Connection String:" + parmconnstring);
                Console.WriteLine("SQL query: " + parmsql);

                // Connection object
                IDbConnection dbConn;

                // Attempt to connect with connection string
                dbConn = new OdbcConnection(parmconnstring);
                dbConn.Open();

                // Output ODBC version info
                Console.WriteLine("Connected to Database");
                Console.WriteLine("ODBC Version {0}", ((OdbcConnection)dbConn).ServerVersion);
                Console.WriteLine("--------------------------------------------------------");

                // Create command run query
                IDbCommand dbCmd = dbConn.CreateCommand();

                // Set the command query
                dbCmd.CommandText = parmsql;

                // Execute the query and get a data reader
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("Your DataReader Query Is Running");
                Console.WriteLine("--------------------------------------------------------");
                IDataReader dbReader = dbCmd.ExecuteReader();

                // Output column name meta data. (Was failing before latest MONO update)
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("Your DataReader Columns Are Served");
                Console.WriteLine("--------------------------------------------------------");
                StringBuilder sbRow = new StringBuilder();
                for (int i = 0; i < dbReader.FieldCount; i++)
                {
                    sbRow.Append(dbReader.GetName(i) + delim);
                }
                Console.WriteLine(sbRow.ToString());
                sbRow.Clear();

                // Output the fields
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("Your DataReader Records Are Now Served");
                Console.WriteLine("--------------------------------------------------------");
                while (dbReader.Read())
                {
                    for (int i = 0; i < dbReader.FieldCount; i++)
                    {
                        sbRow.Append(dbReader.GetString(i) + delim);
                    }
                    Console.WriteLine(sbRow.ToString());
                    sbRow.Clear();
                }

                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("Ending Successfully");
                Console.WriteLine("--------------------------------------------------------");

                // clean up
                dbReader.Close();
                dbReader = null;
                dbCmd.Dispose();
                dbCmd = null;
                dbConn.Close();
                dbConn = null;

            }
            catch (Exception ex)
            {
                Environment.ExitCode = 99;
                Console.WriteLine("Message:" + ex.Message);
                Console.WriteLine("Stacktrace:" + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Innerexception:" + ex.InnerException.Message);
                }

            }
            finally
            {
                Console.WriteLine("MonoOdbcConnect Program End " + System.DateTime.Now.ToString());
                Environment.Exit(Environment.ExitCode);
            }


        }
    }

}
