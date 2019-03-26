using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MonoAdoNetConnect
{
    /// <summary>
    /// Simple program to connect to IBM i DB2 ADO.Net Data Source and exercise the database actions.
    /// Uses IBM i Access DB2 Database Driver - IBM.Data.DB2.dll using Calvins Github Version of
    /// DB2 LUW ADO.NET provider, adapted to work with libdb400 under IBM i 
    /// https://github.com/MonoOni/db2i-ado.net
    /// ** This only runs on an IBMi that has Mono .Net installed.
    /// </summary>
    class Program
    {

        static Dictionary<String,int> dictFields = new Dictionary<String,int>();
        static IbmDataDb2Access db2 = new IbmDataDb2Access();

        static void Main(string[] args)
        {

            try
            {

                // Getting the current system timezone
                // TODO: Timezone is set at offset 00:00:00. Need to pick up UTC from setting somewhere. ?? See mono source perhaps. 
                Environment.SetEnvironmentVariable("TZ", "America/Chicago");

                // Must pass connection string
                if (args.Length < 3)
                {
                    Console.WriteLine("Required parms:");
                    Console.WriteLine("P1-[DB2 Connection string: DSN=*LOCAL;UID=CURRUSER;]");
                    Console.WriteLine("   [Blank connection string defaults to DSN=*LOCAL;UID=CURRUSER]");
                    Console.WriteLine("   [Use WRKRDBDIRE command to find *LOCAL and other remote accessible database names]");
                    Console.WriteLine("P2-[SQL SELECT query]");
                    Console.WriteLine("P3-[MaxRecords-0=All]");
                    Environment.Exit(99);
                }

                // Get parms into variables
                string pconn = args[0];
                string psql = args[1];
                int pmaxrecords = Convert.ToInt32(args[2]);

                // Make sure SQL query passed
                if (psql.Trim() == "")
                {
                    throw new Exception("SQL SELECT query is required.");
                }

                // Program start message
                Console.WriteLine("IBM.Data.DB2.dll Tester Connect Program Start " + DateTime.Now);

                // Attempt to open the connection
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Open New DB2 Connection " + pconn);
                var rtnconn = db2.OpenConnection(pconn);
                Console.WriteLine($"Open connection result: {rtnconn}");
                Console.WriteLine("Open connection last error: " + db2.GetLastError());

                // Run SQL select Query to data table
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Running query to DataTable now.");
                var dtResults = db2.ExecuteQueryToDataTable(psql,pmaxrecords);
                Console.WriteLine("Last SQL:" + db2.GetLastSql());

                if (dtResults != null)
                {
                    Console.WriteLine("Query returned DataTable successfully.");
                    Console.WriteLine(dtResults.Rows.Count  +  " rows returned in DataTable.");
                }
                else
                {
                    Console.WriteLine("Query error. Did NOT return DataTable successfully.");
                    Console.WriteLine("ExecuteQueryToDataTable error: " + db2.GetLastError());
                }

                // Output data table column names
                StringBuilder sb = new StringBuilder();
                sb.Append("DataTable column names: ");
                for (int fldct = 0; fldct < dtResults.Columns.Count; fldct++)
                {
                    // Append data table column name
                    sb.Append(dtResults.Columns[fldct].ColumnName + "|");
                }
                // Output data table column names
                Console.WriteLine(sb.ToString());
                // Reset string buffer
                sb.Clear();

                // If rows returned output the rows.
                Console.WriteLine("DataTable records: ");
                // Outpu column names
                for (int fldct = 0; fldct < dtResults.Columns.Count; fldct++)
                {
                    // Append field to current record line
                    sb.Append(dtResults.Columns[fldct].ColumnName + "|");
                }
                //Output record
                Console.WriteLine(sb.ToString());
                // Reset string buffer
                sb.Clear();

                foreach (DataRow drResults in dtResults.Rows)
                {

                    // Iterate all fields for current record
                    for (int fldct = 0; fldct < dtResults.Columns.Count; fldct++)
                    {
                        // Append field to current record line
                        sb.Append(drResults[fldct] + "|");
                    }
                    //Output record
                    Console.WriteLine(sb.ToString());
                    // Reset string buffer
                    sb.Clear();

                }

                // Run SQL select Query to data reader
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Running query to DataReader now.");
                var dr = db2.ExecuteQueryToDataReader(psql,pmaxrecords);
                Console.WriteLine("Last SQL:" + db2.GetLastSql());

                if (dr != null)
                {
                    Console.WriteLine("Query returned DB2DataReader successfully.");
                }
                else
                {
                    Console.WriteLine("Query error. Did NOT return DB2DataReader successfully.");
                    Console.WriteLine("ExecuteQueryToDataReader error: " + db2.GetLastError());
                }

                // If rows returned output the rows.
                if (dr.HasRows)
                {
                    // Getting field name metadata from DataReader fails.
                    // TODO: Need to have Calvin review Metadata handling. 
                    // Same issue with new native Client Access ODBC driver.
                    // DataTable seems to bring back field names though
                    //var tbl = dr.GetSchemaTable();

                    // Output each row of data
                    Console.WriteLine("DataReader records: ");
                    while (dr.Read())
                    {
                        // Iterate all fields for current record
                        for (int fldct = 0; fldct < dr.FieldCount; fldct++)
                        {
                            // Append field to current record line
                            sb.Append(dr.GetString(fldct) + "|");
                        }
                        //Output record
                        Console.WriteLine(sb.ToString());
                        // Reset string buffer
                        sb.Clear();
                    }

                }

                // Run CL command
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Send message via CL command.");
                var rtncmd = db2.ExecuteClCommand("SNDMSG MSG('ADONET TEST') TOUSR(QPGMR)",true);
                Console.WriteLine("CL Command return:" + rtncmd);
                Console.WriteLine("Last message:" + db2.GetLastError());

                // Delete records from QIWS.QCUSTCDT where CUSNUM=123456
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Delete any records from QIWS.QCUSTCDT with cusnum = 123456");
                var rtndelete = db2.ExecuteNonQuery("DELETE FROM QIWS.QCUSTCDT where CUSNUM=123456", true);
                Console.WriteLine("Delete cusnum=123456 return - records affected:" + rtndelete);
                Console.WriteLine("Last message:" + db2.GetLastError());
                Console.WriteLine("Last SQL:" + db2.GetLastSql());

                // INSERT record to QIWS.QCUSTCDT
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Insert a record in to QIWS.QCUSTCDT with cusnum = 123456");
                var rtninsert = db2.ExecuteNonQuery("INSERT INTO QIWS.QCUSTCDT (CUSNUM,LSTNAM) VALUES(123456,'AdoNet')",true);
                Console.WriteLine("Insert return - records affected:" + rtninsert);
                Console.WriteLine("Last message:" + db2.GetLastError());
                Console.WriteLine("Last SQL:" + db2.GetLastSql());

                // UPDATE record in QIWS.QCUSTCDT
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Update records in to QIWS.QCUSTCDT with cusnum = 123456");
                var rtnupdate = db2.ExecuteNonQuery("UPDATE QIWS.QCUSTCDT SET LSTNAM='AdoNet2' WHERE CUSNUM = 123456", true);
                Console.WriteLine("Update return - records affected:" + rtnupdate);
                Console.WriteLine("Last message:" + db2.GetLastError());
                Console.WriteLine("Last SQL:" + db2.GetLastSql());

                // Attempt to close the connection
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Close New DB2 Connection");
                var rtnclose = db2.CloseConnection();
                Console.WriteLine($"Close connection result: {rtnclose}");
                Console.WriteLine("Close connection last error: " + db2.GetLastError());
                Console.WriteLine("-------------------------------------------------------");

                // Normal exit code
                Environment.ExitCode = 0;;

            } catch (Exception ex)
            {
                //Error exit code
                Environment.ExitCode=99;
                Console.WriteLine("Message:" + ex.Message);
                Console.WriteLine("Stacktrace:" + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Innerexception:" + ex.InnerException.Message);
                }

            }
            finally
            {
                Console.WriteLine("ExitCode:" + Environment.ExitCode);
                Console.WriteLine("IBM.Data.DB2.dll Tester Connect Program End " + DateTime.Now);
                Environment.Exit(Environment.ExitCode);
            }

        }

        /// <summary>
        /// Get field ordinal position based on name
        /// </summary>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        static int GetFieldOrdinal(string fieldname)

        {
            int returnord = 0;
            if ( dictFields.TryGetValue(fieldname,out returnord))
            {
                return returnord;
            }
            else
            {
                return -99;
            }
        }


    }
}
