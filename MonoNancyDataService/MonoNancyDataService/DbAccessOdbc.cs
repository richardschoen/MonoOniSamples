using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Data;
using System.Data.Odbc;

/// <summary>
/// This is a convenience ODBC data access class. 
/// </summary>
namespace MonoNancyDataService
{
    public class DbAccessOdbc
    {

        private string _lastError = "";
        private OdbcConnection _conn = null;
        private bool _connected = false;
        private string _connstring = "";
        private string _lastSql = "";

        /// <summary>
        /// Get last error
        /// </summary>
        /// <returns></returns>
        public string GetLastError()
        {
            return _lastError;
        }

        /// <summary>
        /// Get connection string
        /// </summary>
        /// <returns></returns>
        public string GetLastConnectionString()
        {
            return _connstring;
        }

        /// <summary>
        /// Get last SQL statement executed
        /// </summary>
        /// <returns></returns>
        public string GetLastSql()
        {
            return _lastSql;
        }

        /// <summary>
        /// Is connection open ?
        /// </summary>
        /// <returns>True=connection open, False=No connection open</returns>
        public bool IsConnected()
        {
            return _connected; ;
        }

        /// <summary>
        /// Open ODBC connection
        /// </summary>
        /// <param name="connstring">Connection string</param>
        /// <returns>True=connected, False=connection failure</returns>
        public bool OpenConnection(string connstring)
        {
            try
            {

                _lastError = "";

                //Save conenction string
                _connstring = connstring;

                // Create connection
                _conn = new OdbcConnection(connstring);

                // Attempt to open the connection
                _conn.Open();

                _lastError = "Connection opened successfully.";
                _connected = true;

                return true;

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                _connected = false;
                return false;
            }

        }
        /// <summary>
        /// Close ODBC connection
        /// </summary>
        /// <returns>True=connected, False=connection failure</returns>
        public bool CloseConnection()
        {
            try
            {

                _lastError = "";

                // Attempt to close the connection
                _conn.Close();

                _lastError = "Connection closed successfully.";
                _connected = false;

                return true;

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                _connected = false;
                return false;
            }

        }

        /// <summary>
        /// Execute SQL Query to OdbcDataReader so we can iterate results efficiently
        /// </summary>
        /// <param name="connstring">Connection string</param>
        /// <param name="sql">SQL select</param>
        /// <param name="fetchrowmaximum">Append fetch first xx rows only to SQL select for DB2.</param>
        /// <returns>OdbcDataReader or null on error.</returns>
        public OdbcDataReader ExecuteQueryToDataReader(string sql, int fetchrowmaximum = 0)
        {

            OdbcDataReader _dataReader = null;

            string sqlWork = sql;

            try
            {

                _lastError = "";

                // Bail if not connected
                if (IsConnected() == false)
                {
                    throw new Exception("Not connected to database.");
                }

                // Add fetch first xx rows only
                if (fetchrowmaximum > 0)
                {
                    sqlWork = sqlWork + " " + "FETCH FIRST " + fetchrowmaximum + " ROWS ONLY";
                }

                // Create new query command
                OdbcCommand cmd = new OdbcCommand(sqlWork, _conn);

                // Save last SQL 
                _lastSql = sqlWork;

                // Execute query to data reader
                _dataReader = cmd.ExecuteReader();

                _lastError = "Query to DataReader was successful.";

                // Return the reader to the called
                return _dataReader;

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return null;
            }

        }

        /// <summary>
        /// Execute SQL Query to DataTable
        /// </summary>
        /// <param name="sql">SQL select</param>
        /// <param name="fetchrowmaximum">Append fetch first xx rows only to SQL select.</param>
        /// <param name="startrecord">Starting record. Default=0</param>
        /// <param name="maxrecords">Max records to read. Default=999999</param>
        /// <param name="tablename">DataTable name. Default=Table1</param>
        /// <returns>DataTable or null on error;</returns>
        public DataTable ExecuteQueryToDataTable(string sql, int fetchrowmaximum = 0, int startrecord = 0, int maxrecords = 999999, string tablename = "Table1")
        {

            string sqlWork = sql;

            try
            {

                _lastError = "";

                // Bail if not connected
                if (IsConnected() == false)
                {
                    throw new Exception("Not connected to database.");
                }

                // Add fetch first xx rows only
                if (fetchrowmaximum > 0)
                {
                    sqlWork = sqlWork + " " + "FETCH FIRST " + fetchrowmaximum + " ROWS ONLY";
                }

                // Query the database to a DataSet

                // Save last SQL 
                _lastSql = sqlWork;

                // New DataAdapter
                var adapter = new OdbcDataAdapter(sqlWork, _conn);

                // Create the DataTable
                var dt1 = new DataTable();

                // Fill the data set with selected records
                adapter.Fill(startrecord, maxrecords, dt1);

                _lastError = "Query to DataTable was successful.";

                return dt1;

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return null;
            }

        }

        /// <summary>
        /// Execute SQL Query to DataSet
        /// </summary>
        /// <param name="sql">SQL select</param>
        /// <param name="fetchrowmaximum">Append fetch first xx rows only to SQL select.</param>
        /// <param name="startrecord">Starting record. Default=0</param>
        /// <param name="maxrecords">Max records to read. Default=999999</param>
        /// <param name="tablename">DataTable name. Default=Table1</param>
        /// <returns>DataSet or null on error;</returns>
        public DataSet ExecuteQueryToDataSet(string sql, int fetchrowmaximum = 0, int startrecord = 0, int maxrecords = 999999, string tablename = "Table1")
        {

            string sqlWork = sql;

            try
            {

                _lastError = "";

                // Bail if not connected
                if (IsConnected() == false)
                {
                    throw new Exception("Not connected to database.");
                }

                // Add fetch first xx rows only
                if (fetchrowmaximum > 0)
                {
                    sqlWork = sqlWork + " " + "FETCH FIRST " + fetchrowmaximum + " ROWS ONLY";
                }

                // Query the database to a DataSet

                // Save last SQL 
                _lastSql = sqlWork;

                // New DataAdapter
                var adapter = new OdbcDataAdapter(sqlWork, _conn);

                // Create the DataSet
                var ds1 = new DataSet();

                // Create the DataTable
                var dt1 = new DataTable();

                // Fill the data table with selected records
                adapter.Fill(startrecord, maxrecords, dt1);

                // Add DataTable to DataSet
                ds1.Tables.Add(dt1);

                _lastError = "Query to DataSet was successful.";

                return ds1;

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return null;
            }

        }

        /// <summary>
        /// Execute SQL Query to INSERT/UPDATE/DELETE or other action
        /// </summary>
        /// <param name="connstring">Connection string</param>
        /// <param name="sql">SQL action query</param>
        /// <param name="appendwithnone">Append with no committment control isolation for 
        /// non-committed writes. See this link or google: "ERROR [55019]" isolation level
        /// True=No committment control, False=Use committment control.
        /// http://www-01.ibm.com/support/docview.wss?uid=swg21676715</param>
        /// <returns>Integer results or -2 for errors</returns>
        public int ExecuteNonQuery(string sql, bool appendnocommit = true)
        {


            string sqlWork = sql;

            try
            {

                _lastError = "";

                // Bail if not connected
                if (IsConnected() == false)
                {
                    throw new Exception("Not connected to database.");
                }

                // Append No commit to SQL statement if passed
                if (appendnocommit)
                {
                    sqlWork = sqlWork + " WITH NONE"; // Use WITH NONE or WITH NC for no commit
                }

                // Create new query command
                using (OdbcCommand cmd = new OdbcCommand(sqlWork, _conn))
                {

                    // Save last SQL 
                    _lastSql = sqlWork;

                    // Execute the action query
                    var irtnquery = cmd.ExecuteNonQuery();

                    cmd.Dispose();

                    _lastError = "ExecuteNonQuery completed with return code: " + irtnquery;

                    return irtnquery;

                }

            }
            catch (Exception ex)
            {
                _lastError = "ExecuteNonQuery error: " + ex.Message;
                return -2;
            }

        }

        /// <summary>
        /// Execute CL Command on IBM i System. Only works with Client Access/400 and IBM i.
        /// </summary>
        /// <param name="connstring">Connection string</param>
        /// <param name="clcommand">Ibm i CL Command</param>
        /// <param name="doubleupsinglequotes">Double up any passed single quotes. 
        /// Convenience so CL commands can be passed without doubling up quotes.</param>
        /// <returns>Integer results or -2 for errors</returns>
        public int ExecuteClCommand(string clcommand, bool doubleupsinglequotes = true)
        {

            int irtnquery = 0;

            // Bail if not connected
            if (IsConnected() == false)
            {
                throw new Exception("Not connected to database.");
            }

            // Double up any single quotes
            if (doubleupsinglequotes)
            {
                clcommand = clcommand.Replace("'", "''");
            }

            // Format CL command
            string sql = String.Format("CALL QSYS2.QCMDEXC('{0}')", clcommand);

            try
            {

                _lastError = "";

                // Query the database to a DataReader
                using (var cmd = new OdbcCommand(sql, _conn))
                {

                    // Execute the actionquery
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    irtnquery = cmd.ExecuteNonQuery();

                    cmd.Dispose();

                    _lastError = "CL command:" + clcommand + " completed with return code " + irtnquery;

                    return irtnquery;

                }

            }
            catch (Exception ex)
            {
                _lastError = ex.Message + ex.StackTrace;
                return -2;
            }

        }

        /// <summary>
        /// Check iSeries User Permissions sample stored procedure call.
        /// Requires USERCHECK command to exist in library RJSDOTNET and also the stored procedure.
        /// </summary>
        /// <param name="sUser"></param>
        /// <param name="sPassword"></param>
        /// <param name="sLibrary"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CallUSERCHECK(string connstring, string sUser, string sPassword, string sLibrary)
        {

            int lctraceid = 0;

            try
            {
                // Connect to iSeries and run SQL command
                using (OdbcConnection conn = new OdbcConnection(connstring))
                {
                    OdbcCommand cmd;
                    lctraceid = 100;

                    // Set command object and connection info 
                    // for the command object to current 
                    // IBMDB2 connection
                    lctraceid = 200;
                    conn.Open();
                    cmd = new OdbcCommand("{CALL RJSDOTNET.USERCHECK(?,?,?,?)}", conn);
                    //cmd.Connection = conn;

                    // Set up user as CHAR(10)
                    lctraceid = 300;
                    var p1 = cmd.Parameters.Add("@IUSER", OdbcType.Char, 10);
                    // Always set parms as input/output for program call
                    p1.Direction = ParameterDirection.Input;
                    // Set user parm
                    p1.Value = sUser;

                    // Set up password as CHAR(50)
                    lctraceid = 400;
                    var p2 = cmd.Parameters.Add("@IPASS", OdbcType.Char, 50);
                    // Always set parms as input/output for program call
                    p2.Direction = ParameterDirection.Input;
                    // Set pass parm
                    p2.Value = sPassword;

                    // Set up library as CHAR(10)
                    lctraceid = 500;
                    var p3 = cmd.Parameters.Add("@ILIBRARY", OdbcType.Char, 10);
                    // Always set parms as input/output for program call
                    p3.Direction = ParameterDirection.Input;
                    // Set library parm
                    p3.Value = sLibrary;

                    // Set up an Integer return value
                    lctraceid = 600;
                    var rtnp1 = cmd.Parameters.Add("@RTNVALID", OdbcType.Int);
                    // Always set parms as input/output for program call
                    rtnp1.Direction = ParameterDirection.Output;
                    // This amount will be incremented in the CL
                    rtnp1.Value = 0;

                    // Set up program call command line
                    // The question marks are parameter markers for the call
                    lctraceid = 700;
                    cmd.CommandType = CommandType.Text;
                    //cmd.CommandText = "{CALL @@LIBRARY.@@SPROCNAME(?,?,?,?)}";
                    //cmd.CommandText = "{CALL @@LIBRARY.@@SPROCNAME}";
                    // Replace parms
                    //cmd.CommandText = cmd.CommandText.Replace("@@LIBRARY", sLibrary);
                    //cmd.CommandText = cmd.CommandText.Replace("@@SPROCNAME", "USERCHECK");

                    Console.WriteLine(cmd.CommandText);

                    // Execute iSeries non-stored procedure program call
                    lctraceid = 800;
                    cmd.ExecuteNonQuery();

                    // If invalid user login, bail out
                    lctraceid = 900;
                    if (Convert.ToInt32(rtnp1.Value) == 0)
                        throw new Exception("Error - Invalid login for user " + sUser);

                    // Close connection 
                    lctraceid = 1000;
                    conn.Close();

                    _lastError = "User " + sUser + " logged in successfully.";

                    // Return true if successful
                    return true;
                }
            }
            catch (Exception ex)
            {
                _lastError = "User login error: " + ex.Message + " TraceID:" + lctraceid;
                return false;
            }
        }

    }
}