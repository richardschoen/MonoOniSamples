using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Data;
using IBM.Data.DB2;

    /// <summary>
    /// This is a convenience DB2 data access class. 
    /// Uses IBM i Access DB2 Database Driver - IBM.Data.DB2.dll using Calvins Github Version of
    /// DB2 LUW ADO.NET provider, adapted to work with libdb400 under IBM i 
    /// https://github.com/MonoOni/db2i-ado.net
    /// ** This only runs on an IBMi that has Mono .Net installed.
    /// 
    /// Note: There are currently some deficiencies in the MetaData retreival for SELECT queries so 
    /// if you need field names in your data sets, you must provide them since all data 
    /// handling will need to be by field ordinal position.
    /// </summary>
    public class IbmDataDb2Access
    {

        private string _lastError = ""; // Last error messages
        private DB2Connection _conn = null; // Internal connection object
        private bool _connected = false; // Connected status
        private string _connstring = ""; // Stored full connection string value. Set on OpenConnection.
        private string _lastSql = ""; // Last SQL statement value

        /// <summary>
        /// Get last error
        /// </summary>
        /// <returns>Last error messages</returns>
        public string GetLastError()
        {
            return _lastError;
        }

        /// <summary>
        /// Get connection string
        /// </summary>
        /// <returns>Last full connection string</returns>
        public string GetLastConnectionString()
        {
            return _connstring;
        }

        /// <summary>
        /// Get last SQL statement executed
        /// </summary>
        /// <returns>Last full SQL executed</returns>
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
        /// Open DB2 connection
        /// </summary>
        /// <param name="connstring">Connection string</param>
        /// <param name="usecurrentuser">Use current logged in IBM i user. Seems to be required or DB2 driver will throw error.
        /// This option will append ;UID=Environment.UserName to connection string for current user.</param>
        /// <returns>True=connected, False=connection failure</returns>
        public bool OpenConnection(string connstring="DSN=*LOCAL;",bool usecurrentuser=true)
        {
            try { 

                _lastError = "";

                // if no connection string set base *LOCAL option
                if (connstring.Trim()=="")
                {
                    connstring = "DSN=*LOCAL;";
                }

                // if no UID info, add it to connection string
                if (usecurrentuser)
                {
                    if (connstring.ToLower().Contains("UID") == false) { 
                        connstring = connstring + ";UID=" + Environment.UserName.Trim().ToUpper() + ";";
                    }
                }
                
                //Save conenction string
                _connstring = connstring;

                // Create connection
                _conn = new DB2Connection(connstring);

                // Attempt to open the connection
                _conn.Open();

                _lastError = "Connection opened successfully.";
                _connected = true;

                return true;

            } catch (Exception ex)
            {
               _lastError = ex.Message;
                _connected = false;
                return false;
            }

        }
        /// <summary>
        /// Close DB2 connection
        /// </summary>
        /// <returns>True=disconnected, False=disconnection failure</returns>
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
    /// Execute SQL Query to Db2DataReader so we can iterate results efficiently
    /// </summary>
    /// <param name="connstring">Connection string</param>
    /// <param name="sql">SQL select</param>
    /// <param name="fetchrowmaximum">Append fetch first xx rows only to SQL select.</param>
    /// <returns>DB2DataReader or null on error.</returns>
    public DB2DataReader ExecuteQueryToDataReader(string sql,int fetchrowmaximum=0)
        {

            DB2DataReader _dataReader = null;

            string sqlWork = sql;

            try
            {

                _lastError = "";

                // Bail if not connected
                if (IsConnected()==false)
                {
                    throw new Exception("Not connected to database.");
                }

                // Add fetch first xx rows only
                if (fetchrowmaximum > 0)
                {
                   sqlWork = sqlWork + " " + "FETCH FIRST " + fetchrowmaximum + " ROWS ONLY"; 
                }

                // Create new query command
                DB2Command cmd = new DB2Command(sqlWork, _conn);

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
    public DataTable ExecuteQueryToDataTable(string sql, int fetchrowmaximum = 0, int startrecord=0,int maxrecords=999999, string tablename = "Table1")
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
                var adapter = new DB2DataAdapter(sqlWork, _conn);

                // Create the DataTable
                var dt1 = new DataTable();
            
                // Fill the data set with selected records
                adapter.Fill(startrecord,maxrecords,dt1);

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
    public DataSet ExecuteQueryToDataSet(string sql, int fetchrowmaximum = 0, int startrecord = 0,int maxrecords=999999,string tablename = "Table1")
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
                var adapter = new DB2DataAdapter(sqlWork, _conn);

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
        /// Execute SQL Query to INSERT/UPDATE/DELETE or other action query
        /// </summary>
        /// <param name="sql">SQL action query</param>
        /// <param name="appendwithnone">Append with no committment control isolation for 
        /// non-committed writes. See this link or google: "ERROR [55019]" isolation level
        /// True=No committment control, False=Use committment control.
        /// http://www-01.ibm.com/support/docview.wss?uid=swg21676715</param>
        /// <returns>Integer results with records affected or -2 for errors</returns>
        public int ExecuteNonQuery(string sql,bool appendnocommit = true)
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
                using (DB2Command cmd = new DB2Command(sqlWork, _conn))
                {

                    // Save last SQL 
                    _lastSql = sqlWork;

                    // Execute the action query
                    var irtnquery = cmd.ExecuteNonQuery();

                    cmd.Dispose();

                    _lastError="ExecuteNonQuery completed with return code: " + irtnquery;

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
        /// Execute CL Command
        /// </summary>
        /// <param name="clcommand">Ibm i CL Command</param>
        /// <param name="doubleupsinglequotes">Double up any passed single quotes. 
        /// Convenience so CL commands can be passed without doubling up quotes.</param>
        /// <returns>Integer results or -2 for errors</returns>
        public int ExecuteClCommand(string clcommand,bool doubleupsinglequotes=true)
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
            // TODO: Trigger program or external routine detected an error occurs
            //       if command fails, so no messages returned. Log for review.
            string sql = String.Format("CALL QSYS2.QCMDEXC('{0}')",clcommand);

                try
                {

                    _lastError = "";

                    // Query the database to a DataReader
                    using (var cmd = new DB2Command(sql, _conn)) {

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

    }
