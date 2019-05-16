//#define MonoIbmi // Define this when deploying to IBMi Natively with Mono

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MonoNancyDataService.Properties;

namespace MonoNancyDataService
{
    
    /// <summary>
    /// This class is used to perform Ibm i database access and program calls
    /// as REST web service calls
    /// </summary>
    public class IBMi : NancyModule
    {

        LogToFile logger = new LogToFile();
        bool debug = Settings.Default.Debug;
        private string _sessiontable = Settings.Default.SessionTable.Trim();
        private string _usercheckprogram = Settings.Default.UserCheckProgram.Trim();
        private string _requestauthorization = "";
        private string _requesthostip;

        // ------------------------------------------------------------
        // General status variables
        // ------------------------------------------------------------

        #if (MonoIbmi)
            //private MonoIbmiAdoNet.IbmDataDb2Access _db2 = new MonoIbmiAdoNet.IbmDataDb2Access();
            private DbAccessPase _db2 = new DbAccessPase();
        #else
            //private MonoIbmiAdoNet.IbmDataDb2iSeriesAccess _db2 = new MonoIbmiAdoNet.IbmDataDb2iSeriesAccess();
            private DbAccessOdbc _db2 = new DbAccessOdbc();
        #endif

        private DataTableConversion _convert = new DataTableConversion();

        public IBMi()
        {

            //--------------------------------------------------------------------------
            // Shut down service
            //--------------------------------------------------------------------------
            Get["/api/ibmi/shutdown"] = parameters =>
            {

                if (debug)
                    logger.WriteLogFile("Shutdown of server selected", "SHUTDOWN");

                // Get auth info and check against session
                _requestauthorization = Request.Headers.Authorization;
                _requesthostip = Request.UserHostAddress;

                // Open connection to database
                var rtnconn = _db2.OpenConnection(Properties.Settings.Default.ConnectionString);

                // If connected, perform shutdown
                if (rtnconn)
                {

                    // Check auth info and against session table for valid session
                    if (IsSessionValid(_requestauthorization, _requesthostip) == false)
                    {
                        // Close the connection before returning API auth error
                        _db2.CloseConnection();

                        // Session is not valid
                        Response resp = @"{""message"":""ERROR:APIAUTH""}";
                        resp.ContentType = "application/json";
                        return resp;
                    }

                    // Close the connection after query
                    _db2.CloseConnection();

                    // End service application if authenticated
                    Environment.Exit(0);
                    return "";

                }
                else // No connection
                {
                    if (debug)
                        logger.WriteLogFile("ERROR:NOCONNECTION-" + _db2.GetLastError(), "ERROR");

                    Response resp = @"{""session"":""ERROR:NOCONNECTION""}";
                    resp.ContentType = "application/json";
                    return resp;
                }

            };

            //--------------------------------------------------------------------------
            // Run test query against QIWS.QCUSTCDT
            //--------------------------------------------------------------------------
            Get["/api/ibmi/querytest"] = parameters =>
            {
                if (debug)
                    logger.WriteLogFile("Executing /api/ibmi/querytest", "INFO");

                // Get auth info and check against session
                _requestauthorization = Request.Headers.Authorization;
                _requesthostip = Request.UserHostAddress;

                // Open connection to database
                var rtnconn = _db2.OpenConnection(Properties.Settings.Default.ConnectionString);

                if (debug)
                    logger.WriteLogFile("Conn: " +_db2.GetLastConnectionString(), "INFO");

                // If connected, perform data query
                if (rtnconn)
                {
                    // Check auth info and against session table for valid session
                    if (IsSessionValid(_requestauthorization, _requesthostip) == false)
                    {
                        // Close the connection before returning API auth error
                        _db2.CloseConnection();

                        // Session is not valid
                        Response resp = @"{""message"":""ERROR:APIAUTH""}";
                        resp.ContentType = "application/json";
                        return resp;
                    }

                    // Build query SQL
                    string query = "select * from qiws.qcustcdt where cusnum";

                    // Run action query log
                    if (debug)
                        logger.WriteLogFile("QueryTest SQL: " + query.ToString(), "INFO");

                    // Run query to data table
                    var dtResults = _db2.ExecuteQueryToDataTable(query);

                    // Return JSON response if query success
                    if (dtResults != null)
                    {

                        // Close the connection after query
                        _db2.CloseConnection();

                        Response resp = _convert.GetQueryResultsDataTableToJsonString(dtResults);
                        resp.ContentType = "application/json";
                        return resp;
                    }
                    else
                    {

                        // Get last error 
                        var msg = _db2.GetLastError();

                        // Close the connection after query
                        _db2.CloseConnection();

                        Response resp = @"{""message"":""ERROR:NODATA - " + msg + @"""}";
                        resp.ContentType = "application/json";
                        return resp;

                    }

                }
                else // No connection
                {
                    if (debug)
                        logger.WriteLogFile("ERROR:NOCONNECTION-" + _db2.GetLastError(), "ERROR");

                    Response resp = @"{""session"":""ERROR:NOCONNECTION""}";
                    resp.ContentType = "application/json";
                    return resp;
                }

            };

            //--------------------------------------------------------------------------
            // Run test query against QIWS.QCUSTCDT with selected cusnum (Use 938472 to test)
            //--------------------------------------------------------------------------
            Get["/api/ibmi/querytest2/{cusnum}"] = parameters =>
            {
                if (debug)
                    logger.WriteLogFile("Executing /ibmi/querytest2", "INFO");

                // Get auth info but don't use it for test
                _requestauthorization = Request.Headers.Authorization;
                _requesthostip = Request.UserHostAddress;

                // Open connection to database
                var rtnconn = _db2.OpenConnection(Properties.Settings.Default.ConnectionString);

                // If connected, perform data query
                if (rtnconn)
                {

                    // Check auth info and against session table for valid session
                    if (IsSessionValid(_requestauthorization, _requesthostip) == false)
                    {
                        // Close the connection before returning API auth error
                        _db2.CloseConnection();

                        // Session is not valid
                        Response resp = @"{""message"":""ERROR:APIAUTH""}";
                        resp.ContentType = "application/json";
                        return resp;
                    }

                    // Build query SQL
                    string query = "select * from qiws.qcustcdt where cusnum = " + parameters.cusnum;

                    // Run action query log
                    if (debug)
                        logger.WriteLogFile("QueryTest2 SQL: " + query.ToString(), "INFO");

                    // Run query to data table
                    var dtResults = _db2.ExecuteQueryToDataTable(query);

                    // Return JSON response if query success
                    if (dtResults != null)
                    {
                        // Close the connection after query
                        _db2.CloseConnection();

                        Response resp = _convert.GetQueryResultsDataTableToJsonString(dtResults);
                        resp.ContentType = "application/json";
                        return resp;

                    }
                    else
                    {

                        // Get last error 
                        var msg = _db2.GetLastError();

                        // Close the connection after query
                        _db2.CloseConnection();

                        Response resp = @"{""message"":""ERROR:NODATA - " + msg + @"""}";
                        resp.ContentType = "application/json";
                        return resp;

                    }

                }
                else // No connection
                {
                    if (debug)
                        logger.WriteLogFile("ERROR:NOCONNECTION-" + _db2.GetLastError(), "ERROR");

                    Response resp = @"{""session"":""ERROR:NOCONNECTION""}";
                    resp.ContentType = "application/json";
                    return resp;
                }

            };

            //--------------------------------------------------------------------------
            // Run SQL query action
            //--------------------------------------------------------------------------
            Post["/api/ibmi/execquery"] = parameters =>
            {

                try
                {
                    if (debug)
                        logger.WriteLogFile("Executing /api/ibmi/execquery", "INFO");

                    // Get auth info and check against session
                    _requestauthorization = Request.Headers.Authorization;
                    _requesthostip = Request.UserHostAddress;

                    // Get posted data 
                    PostedData receivedData = this.Bind<PostedData>();

                    // Get JSON data from POST
                    var jsonObject = JObject.Parse(receivedData.JsonData);

                    // Extract values from json data
                    var action = jsonObject.GetValue("action");
                    var query = jsonObject.GetValue("query");
                    if (query == null)
                        query = "";

                    // Open connection to database
                    var rtnconn = _db2.OpenConnection(Properties.Settings.Default.ConnectionString);

                    // If connected, perform data query
                    if (rtnconn)
                    {

                        // Check auth info and against session table for valid session
                        if (IsSessionValid(_requestauthorization, _requesthostip) == false)
                        {
                            // Close the connection before returning API auth error
                            _db2.CloseConnection();

                            // Session is not valid
                            Response resp = @"{""message"":""ERROR:APIAUTH""}";
                            resp.ContentType = "application/json";
                            return resp;
                        }

                        // Run action query log
                        if (debug)
                            logger.WriteLogFile("ExecuteQuery SQL: " + query.ToString(), "INFO");

                        // Run query to data table
                        var dtResults = _db2.ExecuteQueryToDataTable(query.ToString());

                        // Return JSON response if query success
                        if (dtResults != null)
                        {
                            // Close the connection after query
                            _db2.CloseConnection();

                            Response resp = _convert.GetQueryResultsDataTableToJsonString(dtResults);
                            resp.ContentType = "application/json";
                            return resp;

                        }
                        else
                        {

                            // Get last error 
                            var msg = _db2.GetLastError();

                            // Close the connection after query
                            _db2.CloseConnection();

                            Response resp = @"{""message"":""ERROR:NODATA - " + msg + @"""}";
                            resp.ContentType = "application/json";
                            return resp;

                        }
                    }
                    else // No connection
                    {
                        if (debug)
                            logger.WriteLogFile("ERROR:NOCONNECTION-" + _db2.GetLastError(), "ERROR");

                        Response resp = @"{""session"":""ERROR:NOCONNECTION""}";
                        resp.ContentType = "application/json";
                        return resp;

                    }

                } catch (Exception ex)
                {
                    // Handle any misc errors
                    if (debug)
                        logger.WriteLogFile("ERROR:UNHANDLED-" + ex.Message, "INFO");
                    Response resp = @"{""message"":""ERROR:UNHANDLED""}";
                    resp.ContentType = "application/json";
                    return resp;
                }

            };

            //--------------------------------------------------------------------------
            // Run SQL non-query action for insert/update/delete or other action query
            //--------------------------------------------------------------------------
            Post["/api/ibmi/execnonquery"] = parameters =>
            {
                try
                {

                    if (debug)
                    logger.WriteLogFile("Executing /api/ibmi/execnonquery", "INFO");

                    // Get auth info
                    _requestauthorization = Request.Headers.Authorization;
                    _requesthostip = Request.UserHostAddress;

                    // Get posted data 
                    PostedData receivedData = this.Bind<PostedData>();

                    // Get JSON data from POST
                    var jsonObject = JObject.Parse(receivedData.JsonData);

                    // Extract values from json data
                    var action = jsonObject.GetValue("action");
                    var query = jsonObject.GetValue("query");
                    if (query == null)
                        query = "";

                    // Open connection to database
                    var rtnconn = _db2.OpenConnection(Properties.Settings.Default.ConnectionString);

                    // If connected, perform data query
                    if (rtnconn)
                    {
                        // Check auth info and against session table for valid session
                        if (IsSessionValid(_requestauthorization, _requesthostip) == false)
                        {
                            // Close the connection before returning API auth error
                            _db2.CloseConnection();

                            // Session is not valid
                            Response resp = @"{""message"":""ERROR:APIAUTH""}";
                            resp.ContentType = "application/json";
                            return resp;
                        }

                        // Run action query log
                        if (debug)
                            logger.WriteLogFile("ExecuteNonQuery SQL: " + query.ToString(), "INFO");

                        // Execute action query
                        var rtnnonquery = _db2.ExecuteNonQuery(query.ToString());

                        if (debug)
                        {
                            logger.WriteLogFile("ExecuteNonQuery lasterror: " + _db2.GetLastError(), "INFO");
                            // The following 2 lines only work with PaseCommandHelper
                            //logger.WriteLogFile("ExecuteNonQuery lastpaseexitcode: " + _db2.GetLastPaseExitCode(), "INFO");
                            //logger.WriteLogFile("ExecuteNonQuery laststdout:\r\n" + _db2.GetLastStdout (), "INFO");
                        }

                        // Return JSON response if query success
                        if (rtnnonquery != -2)
                        {
                            // Close the connection after query
                            _db2.CloseConnection();

                            Response resp = @"{""message"":""OK:Query completed successfully. Return: " + rtnnonquery.ToString() + @"""}";
                            resp.ContentType = "application/json";
                            return resp;

                        }
                        else // Error on query
                        {
                            // Get last error 
                            var msg = _db2.GetLastError();

                            // Close the connection after query
                            _db2.CloseConnection();

                            Response resp = @"{""message"":""ERROR: - " + msg + @"""}";
                            resp.ContentType = "application/json";
                            return resp;
                        }
                    }
                    else // No connection
                    {
                        if (debug)
                            logger.WriteLogFile("ERROR:NOCONNECTION-" + _db2.GetLastError(), "ERROR");

                        Response resp = @"{""session"":""ERROR:NOCONNECTION""}";
                        resp.ContentType = "application/json";
                        return resp;
                    }
                }
                catch (Exception ex)
                {
                    // Handle any misc errors
                    if (debug)
                        logger.WriteLogFile("ERROR:UNHANDLED-" + ex.Message, "INFO");
                    Response resp = @"{""message"":""ERROR:UNHANDLED""}";
                    resp.ContentType = "application/json";
                    return resp;
                }

            };

            //--------------------------------------------------------------------------
            // Run IBMi CL system command. 
            //--------------------------------------------------------------------------
            Post["/api/ibmi/execclcommand"] = parameters =>
            {
                try
                {

                    if (debug)
                        logger.WriteLogFile("Executing /api/ibmi/execclcommand", "INFO");

                    // Get auth info
                    _requestauthorization = Request.Headers.Authorization;
                    _requesthostip = Request.UserHostAddress;

                    // Get posted data 
                    PostedData receivedData = this.Bind<PostedData>();

                    // Get JSON data from POST
                    var jsonObject = JObject.Parse(receivedData.JsonData);

                    // Extract values from json data
                    var action = jsonObject.GetValue("action");
                    var command = jsonObject.GetValue("command");
                    if (command == null)
                        command = "";

                    // Open connection to database
                    var rtnconn = _db2.OpenConnection(Properties.Settings.Default.ConnectionString);

                    // If connected, perform CL command
                    if (rtnconn)
                    {
                        // Check auth info and against session table for valid session
                        if (IsSessionValid(_requestauthorization, _requesthostip) == false)
                        {
                            // Close the connection before returning API auth error
                            _db2.CloseConnection();

                            // Session is not valid
                            Response resp = @"{""message"":""ERROR:APIAUTH""}";
                            resp.ContentType = "application/json";
                            return resp;
                        }

                        // Run command log
                        if (debug)
                            logger.WriteLogFile("ExecuteClCommand CMD: " + command.ToString(), "INFO");

                        // Run CL command
                        var rtncmd = _db2.ExecuteClCommand(command.ToString());

                        // Return JSON response if query success
                        if (rtncmd != -2)
                        {

                            // Close the connection after query
                            _db2.CloseConnection();

                            Response resp = @"{""message"":""OK:CL command complete. Return code: " + rtncmd.ToString() + @"""}";
                            resp.ContentType = "application/json";
                            return resp;
                        }
                        else // CL command error
                        {
                            // Get last error 
                            var msg = _db2.GetLastError();

                            // Close the connection after query
                            _db2.CloseConnection();

                            if (debug)
                                logger.WriteLogFile("ERROR: CL command error. Return code: " + rtncmd.ToString() + " - " + msg , "INFO");

                            Response resp = @"{""message"":""ERROR:Return code " + rtncmd.ToString() + @"""}";
                            resp.ContentType = "application/json";
                            return resp;

                        }
                    }
                    else // No connection
                    {
                        if (debug)
                            logger.WriteLogFile("ERROR:NOCONNECTION-" + _db2.GetLastError(), "INFO");

                        Response resp = @"{""message"":""ERROR:NOCONNECTION""}";
                        resp.ContentType = "application/json";
                        return resp;
                    }
                }
                catch (Exception ex)
                {
                    // Handle any misc errors
                    if (debug)
                        logger.WriteLogFile("ERROR:UNHANDLED-" + ex.Message, "INFO");
                    Response resp = @"{""message"":""ERROR:UNHANDLED""}";
                    resp.ContentType = "application/json";
                    return resp;
                }

            };

            //--------------------------------------------------------------------------
            // Run web session login process. Generates a new session record 
            //--------------------------------------------------------------------------
            Get["/api/ibmi/login/{user}/{pass}"] = parameters =>
            {
                if (debug)
                    logger.WriteLogFile("Executing /api/ibmi/login", "INFO");

                // Extract parm values from json data
                String puser = parameters.user;
                if (puser == null)
                    puser = "";
                // Pad field to 10 for pass to login proc
                if (puser.Length<10)
                {
                    puser = puser.PadRight(10);
                }

                String ppass = parameters.pass;
                if (ppass == null)
                    ppass = "";
                // Pad field to 50 for pass to login proc
                if (ppass.Length < 50)
                {   
                    ppass = ppass.PadRight(50);
                }
                
                // Open connection to database
                var rtnconn = _db2.OpenConnection(Properties.Settings.Default.ConnectionString);

                // If connected, perform data query
                if (rtnconn)
                {

                    // Run RPG proc to check IBM i user/password credentials
                    var dtResults = _db2.ExecuteQueryToDataTable(String.Format("CALL {2}('{0}','{1}')",puser,ppass,_usercheckprogram));

                    // Return JSON response if query success
                    if (dtResults != null)
                    {
                        var newsessid = "";
                        // Check login return code
                        if (dtResults.Rows[0][0].ToString() == "0")  
                        {
                            // Delete all user session records before inserting new one
                            // if allow multiple sessions not enabled.
                            // This insures a single session for each user ID
                            if (Settings.Default.AllowMultipleLogins == false)
                            {
                                var rtndelete = _db2.ExecuteNonQuery(String.Format("DELETE FROM {1} WHERE SESSUSER='{0}'", puser.Trim().ToUpper(), _sessiontable));
                            }

                            // Write session record
                            var newguid = System.Guid.NewGuid().ToString();
                            var newsessip = Request.UserHostAddress.ToString();
                            var rtninsert = _db2.ExecuteNonQuery(String.Format("INSERT INTO {3} (SESSID,SESSUSER,SESSIP) VALUES('{0}','{1}','{2}')", newguid, puser.Trim().ToUpper(), newsessip,_sessiontable));

                            // Only 1 record expected on insert :-)
                            if (rtninsert == 1)
                            {
                                newsessid = newguid;
                            }
                            else
                            {
                                newsessid = "ERROR:NONE";
                            }

                        }
                        else {
                            newsessid = "ERROR:NONE";
                        }

                        // Close the connection after query
                        _db2.CloseConnection();

                        Response resp = @"{""session"":""" + newsessid + @"""}";
                        resp.ContentType = "application/json";
                        return resp;

                    }
                    else
                    {

                        // Get last error 
                        var msg = _db2.GetLastError();

                        // Close the connection after query
                        _db2.CloseConnection();

                        if (debug)
                            logger.WriteLogFile("ERROR:NODATA - " + msg , "ERROR");
                        //Response resp = @"{""session"":""ERROR:NODATA - " + msg + @"""}";
                        Response resp = @"{""session"":""ERROR:NODATA""}";
                        resp.ContentType = "application/json";
                        return resp;

                    }
                }
                else // No connection
                {
                    if (debug)
                        logger.WriteLogFile("ERROR:NOCONNECTION-" + _db2.GetLastError(), "ERROR");

                    Response resp = @"{""session"":""ERROR:NOCONNECTION""}";
                    resp.ContentType = "application/json";
                    return resp;
                }

            };

            //--------------------------------------------------------------------------
            // Run web session logout process. Removes existing session record 
            //--------------------------------------------------------------------------
            Get["/api/ibmi/logout"] = parameters =>
            {

                if (debug)
                    logger.WriteLogFile("Executing /api/ibmi/logout", "INFO");

                // Get auth info and check against session
                _requestauthorization = Request.Headers.Authorization;
                _requesthostip = Request.UserHostAddress;

                // Open connection to database
                var rtnconn = _db2.OpenConnection(Properties.Settings.Default.ConnectionString);

                // If connected, perform shutdown
                if (rtnconn)
                {

                    // Check auth info and against session table for valid session
                    if (IsSessionValid(_requestauthorization, _requesthostip) == false)
                    {
                        // Close the connection before returning API auth error
                        _db2.CloseConnection();

                        // Session is not valid
                        Response resp = @"{""message"":""ERROR:APIAUTH""}";
                        resp.ContentType = "application/json";
                        return resp;
                    }

                    // Delete selected session record
                    var rtndelete = _db2.ExecuteNonQuery(String.Format("DELETE FROM {1} WHERE SESSID='{0}'", _requestauthorization,_sessiontable));

                    // Close the connection after query
                    _db2.CloseConnection();

                    if (rtndelete > 0)
                    {
                        // Logged out
                        Response resp = @"{""message"":""OK:Logged Out""}";
                        resp.ContentType = "application/json";
                        return resp;

                    }
                    {
                        // Logged out
                        Response resp = @"{""message"":""ERROR:Not Logged Out""}";
                        resp.ContentType = "application/json";
                        return resp;
                    }

                }
                else // No connection
                {
                    if (debug)
                        logger.WriteLogFile("ERROR:NOCONNECTION-" + _db2.GetLastError(), "ERROR");

                    Response resp = @"{""session"":""ERROR:NOCONNECTION""}";
                    resp.ContentType = "application/json";
                    return resp;
                }

            };

        }

        /// <summary>
        /// Get selected header first value. Normally header should just have 1 value.
        /// </summary>
        /// <param name="req">Request object</param>
        /// <param name="headername">Header name</param>
        /// <returns>Return header first value</returns>
        private string GetAuthorizationHeader(Request req,string headername)
        {

            try
            {
                // Get authorization header if this is a request
                var hdrs = req.Headers;

                string auth = hdrs.Authorization;
                // Iterate headers and grab authorization2 value
                foreach (var entry in hdrs)
                {
                    if (entry.Key.ToLower() == headername.ToLower())
                    {
                        //https://stackoverflow.com/questions/22352780/how-to-get-ienumerable-list-values
                        return entry.Value.First();
                    }
                }

                // Nothing selected
                return "";

            }
            catch (Exception ex)
            {
                return "";
            }

        }

        /// <summary>
        /// Check session info to see if session is valid.
        /// We check both the unique session ID and request IP address.
        /// We should only get 1 unique sesion record
        /// **Note: Database connection must be opened already so we can use existing one.
        /// </summary>
        /// <returns>True=Valid session in session table. False=Invalid or no session</returns>
        private bool IsSessionValid(string sessionid, string sessionhostip)
        {

            try
            {
                // If Auth is not enabled, always return true
                if (Settings.Default.AuthEnabled == false)
                {
                    // Session always true if auth not enabled
                    return true;
                }


                // Query for session by session ID and current IP address. Must match
                // TODO - Also add expiration check later based on last used time
                string sql = String.Format("SELECT * FROM {2} WHERE SESSID='{0}' and SESSIP='{1}'", sessionid.Trim(),sessionhostip.Trim(),_sessiontable);

                // Query Session table 
                var dtResults = _db2.ExecuteQueryToDataTable(sql);

                // If at 1 session row, we have a valid session
                if (dtResults != null)
                {
                    // Session should only be 1 row based on unique id/ip combo
                    if (dtResults.Rows.Count == 1)
                    {
                        return true;
                    }
                    else // Returned rows <> 0
                    {
                        return false;
                    }
                } else // No results
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }

}