using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Data;

/// <summary>
/// This helper class is used for running PASE/QSHELL commands from .Net apps.
/// Reference links:
/// IBM Shells and Utilities Site
/// https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_71/rzahg/rzahgshell.htm
/// </summary>

namespace MonoNancyDataService
{

    public class DbAccessPase
    {

        private string _lastError = "";
        private string _lastStdout = "";
        private int _lastPaseExitCode = 0;
        private int _lastResultCount = 0;

        /// <summary>
        /// Get error message after last command call
        /// </summary>
        /// <returns>Last error string value</returns>
        public string GetLastError()
        {
            return _lastError;
        }
        /// <summary>
        /// Get last PASE exit code
        /// </summary>
        /// <returns>Last pase exit int value</returns>
        public int GetLastPaseExitCode()
        {
            return _lastPaseExitCode;
        }
        /// <summary>
        /// Get STDOUT output after last command call.
        /// </summary>
        /// <returns>Last STDOUT string value</returns>
        public string GetLastStdout()
        {
            return _lastStdout;
        }
        /// <summary>
        /// Get last result count from select query
        /// </summary>
        /// <returns>Last result record count value</returns>
        public int GetLastResultCount()
        {
            return _lastResultCount;
        }

        /// <summary>
        /// Run pase command with optional arguments.
        /// Ex filename: ls  arguments: /tmp will list contents of /tmp folder
        /// </summary>
        /// <param name="filename">Filename or command. Ex: ls, system, mkdir, rm, etc</param>
        /// <param name="arguments">Parameter list for file or command.</param>
        /// <param name="logtoconsole">Log data to Console/Stdout. True=Log to console, False=Do not log. If false, must use GetStdout to get last stdout values.</param>
        /// <param name="logerrortoconsole">Log error to Console/Stdout. True=Log to console, False=Do not log. Must use GetLastError to get last error values.</param>
        /// <param name="appendstdouttolasterror">Append STDOUT message to last error message. True=Append STDOUT to last error message. False=Do not append STDOUT to last error. Default=true</param>
        /// <param name="throwerroronpaseerror">Throw an error is a non-zero PASE return code comes back. When a stored procedure
        ///  gets called and returns a resultset, the db2 command ends with error 2 even though the call 
        ///  returned records, so there my be times when you don't want an error thrown. Default=true - throw error on non-zero pase return.
        ///  </param>
        /// <returns>True=success, False=error</returns>
        public bool RunPaseCommand(string filename, string arguments, bool logtoconsole = false, bool logerrortoconsole = true, bool logpasecommand = true, bool appendstdouttolasterror = true,bool throwerroronpaserror=true)
        {

            try
            {
                _lastError = "";
                _lastPaseExitCode = 0;
                _lastStdout = "";

                // Place double quotes around pase command arguments using special value
                arguments = arguments.Replace("@sldbqt", "\\\"");
                arguments = arguments.Replace("@SLDBQT", "\\\"");
                arguments = arguments.Replace("@dbqt", @"""");
                arguments = arguments.Replace("@DBQT", @"""");
                arguments = arguments.Replace("dbqt", @"""");
                arguments = arguments.Replace("DBQT", @"""");
                arguments = arguments.Replace("@qt", @"'");
                arguments = arguments.Replace("@QT", @"'");

                // Output STDOUT to console if enabled
                if (logtoconsole && logpasecommand)
                {
                    // Log command line
                    Console.WriteLine("PaseCommand:" + filename + " " + arguments);
                }

                // Run command line
                StringBuilder outputBuilder;
                ProcessStartInfo processStartInfo;
                Process process;

                outputBuilder = new StringBuilder();

                processStartInfo = new ProcessStartInfo();
                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.Arguments = arguments;
                processStartInfo.FileName = filename;

                process = new Process();
                process.StartInfo = processStartInfo;
                // enable raising events because Process does not raise events by default
                process.EnableRaisingEvents = true;
                // attach the event handler for OutputDataReceived before starting the process
                process.OutputDataReceived += new DataReceivedEventHandler
                (
                    delegate (object sender, DataReceivedEventArgs e)
                    {
                        // append the new data to the data already read-in
                        outputBuilder.AppendLine(e.Data);
                    }
                );
                // start the process
                // then begin asynchronously reading the output
                // then wait for the process to exit
                // then cancel asynchronously reading the output
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
                process.CancelOutputRead();

                // Write output to console
                string output = outputBuilder.ToString();

                // Save last STDOUT value
                _lastStdout = output;

                // Save last exit code
                _lastPaseExitCode = process.ExitCode;

                // Output STDOUT to console
                if (logtoconsole)
                {
                    // Log STDOUT output
                    Console.Write(output);
                    Console.WriteLine("");
                }

                // If 0 return code then the PASE command ran successfully, but you 
                // may need to examine STDOUT
                if (_lastPaseExitCode == 0)
                {
                    return true;
                }
                else
                { // PASE command completed with errors

                    // See if we need to throw an error for a nonzero PASE return code
                    // If so, return false. If not, return true.
                    if (throwerroronpaserror)
                    {

                        _lastError = "Last PASE command exit code: " + _lastPaseExitCode;
                        // Append STDOUT buffer to last error message
                        if (appendstdouttolasterror)
                        {
                            _lastError = _lastError + " Stdout: " + _lastStdout;
                        }
                        return false;
                    } else
                    {
                        // Return true even though there was a nonzero PASE return
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                // Append STDOUT buffer to last error message
                if (appendstdouttolasterror)
                {
                    _lastError = _lastError + " Stdout: " + _lastStdout;
                }
                // If logging, write to console
                if (logtoconsole && logerrortoconsole)
                {
                    Console.Write(_lastError);
                    Console.WriteLine("");
                }
                return false;
            }

        }
        /// <summary>
        /// Run pase command with optional arguments.
        /// Ex filename: ls  arguments: /tmp will list contents of /tmp folder
        /// </summary>
        /// <param name="filename">Filename or command. Ex: ls, system, mkdir, rm, etc</param>
        /// <param name="arguments">Parameter list for file or command.</param>
        /// <param name="outputfilestdout">Output file for STDOUT.</param>
        /// <param name="logtoconsole">Log data to Console/Stdout. True=Log to console, False=Do not log. If false, must use GetStdout to get last stdout values.</param>
        /// <param name="logerrortoconsole">Log error to Console/Stdout. True=Log to console, False=Do not log. Must use GetLastError to get last error values.</param>
        /// <param name="appendstdouttolasterror">Append STDOUT message to last error message. True=Append STDOUT to last error message. False=Do not append STDOUT to last error. Default=true</param>
        /// <param name="throwerroronpaseerror">Throw an error is a non-zero PASE return code comes back. When a stored procedure
        ///  gets called and returns a resultset, the db2 command ends with error 2 even though the call 
        ///  returned records, so there my be times when you don't want an error thrown. Default=true - throw error on non-zero pase return.
        ///  </param>
        /// <returns>True=success, False=error</returns>
        //public bool RunPaseCommand(string filename, string arguments, string outputfilestdout, bool logtoconsole = false, bool logerrortoconsole = true, bool logpasecommand = true, bool appendstdouttolasterror = true, bool throwerroronpaserror = true)
        //{

        //    try
        //    {

        //        // Run Pase command
        //        var rtn = RunPaseCommand(filename, arguments, logtoconsole, logerrortoconsole, logpasecommand,appendstdouttolasterror,throwerroronpaserror);

        //        // Write STDOUT to file
        //        File.WriteAllText(outputfilestdout, GetLastStdout());

        //        return rtn;

        //    }
        //    catch (Exception ex)
        //    {
        //        _lastError = ex.Message;
        //        return false;
        //    }


        //}

        /// <summary>
        /// Run IBM i system CL command 
        /// </summary>
        /// <param name="clcommand">CL command to run</param>
        /// <param name="logtoconsole">Log data to Console/Stdout. True=Log to console, False=Do not log. If false, must use GetStdout to get last stdout values.</param>
        /// <returns>True=success, False=error</returns>
        public int ExecuteClCommand(string clcommand, bool logtoconsole = true)
        {

            try
            {
                _lastError = "";

                // Place double quotes around CL command
                clcommand = @"""" + clcommand.Trim() + @"""";

                // Run CL command line
                if (RunPaseCommand("system", clcommand, logtoconsole))
                {
                    return 0;
                }
                else
                {
                    throw new Exception(GetLastError());
                }

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return -2;
            }

        }
        /// <summary>
        /// Get directory list contents using PASE ls command
        /// </summary>
        /// <param name="arguments">Parameter list for file or command.</param>
        /// <param name="logtoconsole">Log data to Console/Stdout. True=Log to console, False=Do not log. If false, must use GetStdout to get last stdout values.</param>
        /// <returns>True=success, False=error</returns>
        public bool GetDirectoryList(string arguments, bool logtoconsole = true)
        {

            try
            {
                _lastError = "";

                // Run command
                return RunPaseCommand("ls", arguments, logtoconsole);

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }

        }

        /// <summary>
        /// Run Qshell db2 SQL select query
        /// https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_71/rzahz/rzahzdb2utility.htm
        /// </summary>
        /// <param name="sqld">SQL statement/command</param>
        /// <param name="db2arguments">Parameter list for db2 command to precede the SQL statement.</param>
        /// <param name="logtoconsole">Log data to Console/Stdout. True=Log to console, False=Do not log. If false, must use GetStdout to get last stdout values.</param>
        /// <param name="throwerroronpaseerror">Throw an error is a non-zero PASE return code comes back. When a stored procedure
        ///  gets called and returns a resultset, the db2 command ends with error 2 even though the call 
        ///  returned records, so there my be times when you don't want an error thrown. Default=true - throw error on non-zero pase return.
        ///  </param>

        /// <returns>True=success, False=error</returns>
        public bool RunDb2Query(string sql, string db2arguments = "", bool logtoconsole = true,bool throwerroronpaseerror=false)
        {

            try
            {
                _lastError = "";
                _lastResultCount = 0;

                // Run qshell DB2 command and escape correctly so that we can use statements that contain single quotes
                // https://kadler.github.io/2018/05/29/calling-qsh-utilities-from-pase.html
                // We will ignore PASE errors by default because db2 cli sproc call returs 466 error which 
                // in turn causes PASE error 2 which is incorrect because sproc call succeeded.
                // We will scan data for RECORD(S) SELECTED to determine if query was in fact successful.
                var rtnquery = RunPaseCommand("qsh", "-c " + @"""" + "db2 " + db2arguments + " \\\"" + sql + "\\\"" + @"""", logtoconsole,throwerroronpaserror:throwerroronpaseerror);

                // Get STDOUT and look for record result count element in each line
                string recordsSelected = "RECORD(S) SELECTED."; // Value to scan for

                // Check STDOUT buffer for RECORD(S) SELECTED value.
                // If not found, bail out. Query or procedure call failed.
                if (GetLastStdout().Contains(recordsSelected) == false)
                {
                    throw new Exception("db2 call does not contain value: " + recordsSelected + "-Most likely the db2 query failed. Check STDOUT.");
                }

                using (StringReader reader = new StringReader(GetLastStdout()))
                {
                    // Scan for record selected element and extract row count
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains(recordsSelected))
                        {
                            line = line.Replace(recordsSelected, ""); // Strip out record selected text
                            _lastResultCount = Convert.ToInt32(line); // Convert result count to integer
                        }

                    }
                }

                return rtnquery;

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }

        }
        /// <summary>
        /// Run Qshell db2 SQL select query and convert to DataTable
        /// https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_71/rzahz/rzahzdb2utility.htm
        /// </summary>
        /// <param name="sqld">SQL statement/command</param>
        /// <param name="db2arguments">Parameter list for db2 command to precede the SQL statement.</param>
        /// <param name="logtoconsole">Log data to Console/Stdout. True=Log to console, False=Do not log. If false, must use GetStdout to get last stdout values.</param>
        /// <returns>DataTable or null on errors</returns>
        public DataTable ExecuteQueryToDataTable(string sql, string db2arguments = "", bool logtoconsole = true)
        {

            try
            {
                _lastError = "";
                _lastResultCount = 0;

                // Run query 
                // If calling sproc, do not throw error on PASE error 2 that 
                // gets returned after a sproc call. If resultsets returned, db2 cli
                // throws a PASE 2 return code and db2 cli throw a 466 error even though
                // results were returned successfully.
                var rtnqry = false;
                var throwerroronpaseerror = true;

                // If sql starts with CALL, ignore pase errors
                if (sql.Trim().ToUpper().StartsWith("CALL "))
                    throwerroronpaseerror = false;
                
                // Run the query now
                rtnqry = RunDb2Query(sql, db2arguments, logtoconsole,throwerroronpaseerror);

                if (rtnqry)
                {

                    DataTable dt;
                    // If stored proc results, handle the parsing differently
                    // than a standard SQL query
                    if (GetLastStdout().Contains("NATIVE ERROR CODE: 466"))
                    {
                        // Stored procedure resultset return
                        dt = ConvertDb2SpResultsToDataTable(GetLastStdout());
                    }
                    else
                    {
                        // Standard query return
                        dt = ConvertDb2ResultsToDataTable(GetLastStdout());

                    }

                    return dt;
                }
                else
                {
                    throw new Exception(GetLastError());
                }

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return null;
            }

        }

        /// <summary>
        /// Run Qshell db2 SQL select query
        /// https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_71/rzahz/rzahzdb2utility.htm
        /// **Note: Right now the DB2 query and nonquery do the same thing, but that may change.
        /// </summary>
        /// <param name="sql">SQL statement/command</param>
        /// <param name="db2arguments">Parameter list for db2 command to precede the SQL statement.</param>
        /// <param name="logtoconsole">Log data to Console/Stdout. True=Log to console, False=Do not log. If false, must use GetStdout to get last stdout values.</param>
        /// <returns> Greater than 0=success, -2=Error</returns>
        public int ExecuteNonQuery(string sql, string db2arguments = "", bool logtoconsole = true)
        {

            try
            {
                _lastError = "";

                // Run qshell DB2 command and escape correctly so that we can use statements that contain single quotes
                // https://kadler.github.io/2018/05/29/calling-qsh-utilities-from-pase.html
                var rtn = RunPaseCommand("qsh", "-c " + @"""" + "db2 " + db2arguments + " \\\"" + sql + "\\\"" + @"""", logtoconsole);

                if (rtn)
                {
                    return 1;
                }
                else
                {
                    // LastError should already be set from PASE command, let's exit
                    throw new Exception(GetLastError());
                }

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return -2;
            }

        }

        /// <summary>
        /// Run Qshell commands via the PASE qsh command
        /// </summary>
        /// <param name="arguments">Qshell command and parameter arguments</param>
        /// <param name="addcommandswitch">True=Add -c switch after qsh command. Ex: qsh -c, False=Do not add command switch. Caller can add to arguments.</param>
        /// <param name="logtoconsole">Log data to Console/Stdout. True=Log to console, False=Do not log. If false, must use GetStdout to get last stdout values.</param>
        /// <param name="cmdadddblquotes">Place double quotes around qshell command.</param>
        /// <returns>True=success, False=error</returns>
        public bool RunQshCommand(string arguments = "", bool addcommandswitch = true, bool logtoconsole = true, bool cmdadddblquotes = true)
        {

            try
            {
                _lastError = "";

                // Modify arguments if command switch added
                if (addcommandswitch)
                {
                    // Add double quotes around command line if selected
                    if (cmdadddblquotes)
                    {
                        arguments = "-c " + @"""" + arguments + @"""";
                    }
                    else
                    {
                        arguments = "-c " + arguments;
                    }
                }

                // Run command
                return RunPaseCommand("qsh", arguments, logtoconsole);

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }

        }
        /// <summary>
        /// Create directory list table to hold IFS file crawler directory contents
        /// </summary>
        public bool CreateDirListTable(string library, string file, bool logtoconsole = true)
        {

            try
            {

                string sqlcreate = "CREATE TABLE @@LIBRARY.@@FILE(FILEDIRTYP varchar(20),FILEDIRNAM varchar(255),FILEDIRSIZ int)";

                _lastError = "";

                // Place file and library name into SQL statement
                sqlcreate = sqlcreate.Replace("@@LIBRARY", library.Trim().ToUpper());
                sqlcreate = sqlcreate.Replace("@@FILE", file.Trim().ToUpper());

                // Run DB2 command to create table
                var rtncreate = ExecuteNonQuery(sqlcreate, "", logtoconsole);

                // If true return code, scan STDOUT for potential errors. Currently
                // the Db2NonQuery/RunQshCommand does not return errors even if an 
                // error occurs. 
                // TODO - How to capture errors when running QSH command via PASE ??
                if (rtncreate >= 0)
                {
                    // If we find "Table" and "created", table was probably created successfully
                    if (GetLastStdout().Contains("Table") && GetLastStdout().Contains("created"))
                    {
                        return true;
                    }
                    else // Return false since create probably failed. 
                    {
                        return false;
                    }
                }
                else
                {
                    // A DB2 call error probably occurred. Check GetLastError message and GetLastStdout
                    return false;
                }

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }

        }

        /// <summary>
        /// Convert db2 cli query results data in selected STDOUT buffer to DataTable
        /// </summary>
        /// <param name="stdout">Pass in STDOUT result buffer from Qshell db2 cli query.</param>
        /// <param name="tableName">DataTable name. Default=Table1</param>
        /// <param name="logtoconsole">Log data to Console/Stdout. True=Log to console, False=Do not log. If false, must use GetStdout to get last stdout values.</param>
        /// <param name="debug">Write debug data to console. True=Log debug info to console, False=Do not log.</param>
        /// <param name="fullfieldtrim">Trim begin and end blank field data or just end. True=Trim(), False=TrimEnd(). Default=True</param>
        /// <returns>DataTable or null if no data or errors</returns>
        public DataTable ConvertDb2ResultsToDataTable(string stdout, string tableName = "Table1", bool logtoconsole = true, bool debug = false, bool fullfieldtrim = true)
        {

            DataTable dtTemp = null;
            string[] arrTemp;
            string strTemp = "";
            string[] arrFieldNames = null;
            string[] arrFieldWidths = null;
            int[] arrFieldWidthsInt = null;
            int[] arrFieldStartInt = null;

            try
            {
                _lastError = "";
                _lastResultCount = 0;

                // Get STDOUT and look for record result count element in each line
                if (logtoconsole && debug)
                {
                    Console.WriteLine("ConvertDb2ResultsToDataTable Start");
                }

                string line;
                string linefields = "";
                string linefieldwidths = "";
                int linecount = 1;
                // Extract field names and field width data lines from DB2 results in stdout
                using (StringReader reader = new StringReader(stdout))
                {
                    // Scan for record selected element and extract row count
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Log each data line
                        if (logtoconsole && debug)
                        {
                            Console.WriteLine(line);
                        }

                        // Ignore blank lines(s) at top of results
                        if (line.TrimEnd() != "")
                        {
                            // Handle extracting field width data from results and creating DataTable
                            if (line.Contains("RECORD(S) SELECTED."))
                            {
                                break;
                            }
                            // Handle extracting field names from results
                            else if (linecount == 1)
                            {
                                linefields = line;
                                // Split line on spaces to get each field element. 
                                // We will have some blank elements initially
                                arrTemp = linefields.Split(' ');
                                // Spin through and extract each field name element now
                                // Ignore blank elements
                                foreach (string val1 in arrTemp)
                                {
                                    if (val1.TrimEnd() != "")
                                    {
                                        strTemp = strTemp + val1 + ",";
                                    }
                                }
                                // Remove comma from last element
                                strTemp = strTemp.Remove(strTemp.Length - 1, 1);
                                // Extract field name array form string 
                                arrFieldNames = strTemp.Split(',');

                                // List out elements if debugging
                                if (logtoconsole && debug)
                                {
                                    foreach (string val1 in arrFieldNames)
                                    {
                                        Console.WriteLine("Field:" + val1);
                                    }
                                }

                            }
                            // Handle extracting field width data from results and creating DataTable
                            else if (linecount == 2)
                            {
                                linefieldwidths = line;
                                // Split field width on spaces to get each field width element
                                arrFieldWidths = linefieldwidths.Split(' ');

                                // Make sure field length and name array sizes match
                                if (arrFieldNames.Length != arrFieldWidths.Length)
                                {
                                    throw new Exception("It appears that the field name and field length arrays don't match. Something went wrong parsing query result data.");
                                }

                                // Calc field widths and set field width integer array
                                // Also set field start locations for data buffer parsing
                                arrFieldWidthsInt = new int[arrFieldWidths.Length];
                                arrFieldStartInt = new int[arrFieldWidths.Length];
                                int fieldStartPos = 0;
                                for (int ct = 0; ct < arrFieldWidths.Length; ct++)
                                {
                                    // Save current width
                                    arrFieldWidthsInt[ct] = arrFieldWidths[ct].Trim().Length;
                                    // Save current field start position
                                    arrFieldStartInt[ct] = fieldStartPos;
                                    // Increment field start pos by field length plus 1 
                                    // for space to next field in data buffer
                                    fieldStartPos = fieldStartPos + arrFieldWidths[ct].Trim().Length + 1;
                                }

                                // List out elements if debugging
                                if (logtoconsole && debug)
                                {
                                    for (int ct = 0; ct < arrFieldWidthsInt.Length; ct++)
                                    {
                                        Console.WriteLine("FieldWidth:" + arrFieldWidthsInt[ct] + "StartPos:" + arrFieldStartInt[ct]);
                                    }
                                }

                                // Create the new DataTable now
                                dtTemp = new DataTable();

                                // Set the table name and default to Table1 if blank
                                if (tableName.Trim() == "")
                                {
                                    tableName = "Table1";
                                }
                                dtTemp.TableName = tableName;

                                // Add columns to data table
                                for (int ct = 0; ct < arrFieldNames.Length; ct++)
                                {
                                    dtTemp.Columns.Add(arrFieldNames[ct].Trim());
                                }

                            }
                            // Add record data to data table now
                            else if (linecount > 2)
                            {

                                // Create new data row
                                DataRow dr = dtTemp.NewRow();

                                // Add each column of data for current record to the DataRow
                                for (int ct = 0; ct < arrFieldNames.Length; ct++)
                                {
                                    // Set value and trim ending whitespace from record
                                    if (fullfieldtrim)
                                    {
                                        dr[ct] = line.Substring(arrFieldStartInt[ct], arrFieldWidthsInt[ct]).ToString().Trim();
                                    }
                                    else
                                    {
                                        dr[ct] = line.Substring(arrFieldStartInt[ct], arrFieldWidthsInt[ct]).ToString().TrimEnd();
                                    }

                                    //Output field data for debug to check justification. Seems to trim just fine
                                    if (logtoconsole && debug)
                                    {
                                        Console.WriteLine(dr[ct]);
                                    }
                                }
                                // Add the row to the DataTable now.
                                dtTemp.Rows.Add(dr);

                            }
                            else // Let's 
                            {
                                break;
                            }
                            linecount++;
                        }

                    }

                }

                // Log data for debugging
                if (logtoconsole && debug)
                {
                    Console.WriteLine("DataTable Rows: " + dtTemp.Rows.Count);
                    Console.WriteLine("LineFields: " + linefields);
                    Console.WriteLine("LineFieldWidths: " + linefieldwidths);
                }

                return dtTemp;

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;

                // Log data for debugging
                if (logtoconsole && debug)
                {
                    Console.WriteLine("ConvertDb2ResultsToDataTable Error: " + ex.Message);
                }
                return null;
            }
            finally
            {
                if (logtoconsole && debug)
                {
                    Console.WriteLine("ConvertDb2ResultsToDataTable End");
                }
            }

        }

        /// <summary>
        /// Convert db2 cli stored proc call results data in selected STDOUT buffer to DataTable
        /// </summary>
        /// <param name="stdout">Pass in STDOUT result buffer from Qshell db2 cli query.</param>
        /// <param name="tableName">DataTable name. Default=Table1</param>
        /// <param name="logtoconsole">Log data to Console/Stdout. True=Log to console, False=Do not log. If false, must use GetStdout to get last stdout values.</param>
        /// <param name="debug">Write debug data to console. True=Log debug info to console, False=Do not log.</param>
        /// <param name="fullfieldtrim">Trim begin and end blank field data or just end. True=Trim(), False=TrimEnd(). Default=True</param>
        /// <returns>DataTable or null if no data or errors</returns>
        public DataTable ConvertDb2SpResultsToDataTable(string stdout, string tableName = "Table1", bool logtoconsole = true, bool debug = false, bool fullfieldtrim = true)
        {

            DataTable dtTemp = null;
            string[] arrTemp;
            string strTemp = "";
            string[] arrFieldNames = null;
            string[] arrFieldWidths = null;
            int[] arrFieldWidthsInt = null;
            int[] arrFieldStartInt = null;

            try
            {
                _lastError = "";
                _lastResultCount = 0;

                // Get STDOUT and look for record result count element in each line
                if (logtoconsole && debug)
                {
                    Console.WriteLine("ConvertDb2SpResultsToDataTable Start");
                }

                string line;
                string linefields = "";
                string linefieldwidths = "";
                int linecount = 1; 
                // Extract field names and field width data lines from DB2 results in stdout
                using (StringReader reader = new StringReader(stdout))
                {
                    // Log each data line from stdout
                    if (logtoconsole && debug)
                    {
                        Console.WriteLine("Logging STDOUT by line");
                    }

                    // Scan for record selected element and extract row count
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Log each data line
                        if (logtoconsole && debug)
                        {
                            Console.WriteLine(line);
                        }

                        // Ignore blank lines(s) at top of results
                        if (line.TrimEnd() != "")
                        {
                            // Handle extracting field width data from results and creating DataTable
                            if (line.Contains("RECORD(S) SELECTED."))
                            {
                                break;
                            }
                            // Handle extracting field names from proc results
                            else if (linecount == 7)
                            {
                                linefields = line;
                                // Split line on spaces to get each field element. 
                                // We will have some blank elements initially
                                arrTemp = linefields.Split(' ');
                                // Spin through and extract each field name element now
                                // Ignore blank elements
                                foreach (string val1 in arrTemp)
                                {
                                    if (val1.TrimEnd() != "")
                                    {
                                        strTemp = strTemp + val1 + ",";
                                    }
                                }
                                // Remove comma from last element
                                strTemp = strTemp.Remove(strTemp.Length - 1, 1);
                                // Extract field name array form string 
                                arrFieldNames = strTemp.Split(',');

                                // List out elements if debugging
                                if (logtoconsole && debug)
                                {
                                    foreach (string val1 in arrFieldNames)
                                    {
                                        Console.WriteLine("Field:" + val1);
                                    }
                                }

                            }
                            // Handle extracting field width data from results and creating DataTable
                            else if (linecount == 8)
                            {
                                linefieldwidths = line;
                                // Split field width on spaces to get each field width element
                                arrFieldWidths = linefieldwidths.Split(' ');

                                // Make sure field length and name array sizes match
                                if (arrFieldNames.Length != arrFieldWidths.Length)
                                {
                                    throw new Exception("It appears that the field name and field length arrays don't match. Something went wrong parsing query result data.");
                                }

                                // Calc field widths and set field width integer array
                                // Also set field start locations for data buffer parsing
                                arrFieldWidthsInt = new int[arrFieldWidths.Length];
                                arrFieldStartInt = new int[arrFieldWidths.Length];
                                int fieldStartPos = 0;
                                for (int ct = 0; ct < arrFieldWidths.Length; ct++)
                                {
                                    // Save current width
                                    arrFieldWidthsInt[ct] = arrFieldWidths[ct].Trim().Length;
                                    // Save current field start position
                                    arrFieldStartInt[ct] = fieldStartPos;
                                    // Increment field start pos by field length plus 1 
                                    // for space to next field in data buffer
                                    fieldStartPos = fieldStartPos + arrFieldWidths[ct].Trim().Length + 1;
                                }

                                // List out elements if debugging
                                if (logtoconsole && debug)
                                {
                                    for (int ct = 0; ct < arrFieldWidthsInt.Length; ct++)
                                    {
                                        Console.WriteLine("FieldWidth:" + arrFieldWidthsInt[ct] + "StartPos:" + arrFieldStartInt[ct]);
                                    }
                                }

                                // Create the new DataTable now
                                dtTemp = new DataTable();

                                // Set the table name and default to Table1 if blank
                                if (tableName.Trim() == "")
                                {
                                    tableName = "Table1";
                                }
                                dtTemp.TableName = tableName;

                                // Add columns to data table
                                for (int ct = 0; ct < arrFieldNames.Length; ct++)
                                {
                                    dtTemp.Columns.Add(arrFieldNames[ct].Trim());
                                }

                            }
                            // Add record data to data table now
                            else if (linecount > 8)
                            {

                                // Create new data row
                                DataRow dr = dtTemp.NewRow();

                                // Add each column of data for current record to the DataRow
                                for (int ct = 0; ct < arrFieldNames.Length; ct++)
                                {
                                    // Set value and trim ending whitespace from record
                                    if (fullfieldtrim)
                                    {
                                        dr[ct] = line.Substring(arrFieldStartInt[ct], arrFieldWidthsInt[ct]).ToString().Trim();
                                    }
                                    else
                                    {
                                        dr[ct] = line.Substring(arrFieldStartInt[ct], arrFieldWidthsInt[ct]).ToString().TrimEnd();
                                    }

                                    //Output field data for debug to check justification. Seems to trim just fine
                                    if (logtoconsole && debug)
                                    {
                                        Console.WriteLine(dr[ct]);
                                    }
                                }
                                // Add the row to the DataTable now.
                                dtTemp.Rows.Add(dr);

                            }
                            else // Let's keep going though rest of records
                            {
                                //Do nothing with this line
                            }
                            
                        }
                        
                        // Increment line count always - even if blank line 
                        linecount++;
                    }

                }

                // Log data for debugging
                if (logtoconsole && debug)
                {
                    Console.WriteLine("DataTable Rows: " + dtTemp.Rows.Count);
                    Console.WriteLine("LineFields: " + linefields);
                    Console.WriteLine("LineFieldWidths: " + linefieldwidths);
                }

                return dtTemp;

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;

                // Log data for debugging
                if (logtoconsole && debug)
                {
                    Console.WriteLine("ConvertDb2SpResultsToDataTable Error: " + ex.Message);
                }
                return null;
            }
            finally
            {
                if (logtoconsole && debug)
                {
                    Console.WriteLine("ConvertDb2SpResultsToDataTable End");
                }
            }

        }

        /// <summary>
        ///  This function converts DataTable to CSV and returns as a string
        ///  </summary>
        ///  <param name="dt">DataTable that contains data.</param>
        ///  <param name="sFieldSepchar">Field delimiter/separator. Default = Comma</param>
        ///  <param name="sFieldDataDelimChar">Field data delimiter character. Default = double quotes.</param>
        ///  <returns>CSV string from DataTable</returns>
        public string GetQueryResultsDataTableToCsvString(DataTable dt, string sFieldSepchar = ",", string sFieldDataDelimChar = "\"")
        {
            try
            {
                _lastError = "";

                //string sHeadings = "";
                //string sBody = "";
                StringBuilder sCsvData = new StringBuilder();

                // first write a line with the columns name
                string sep = "";
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                foreach (DataColumn col in dt.Columns)
                {
                    builder.Append(sep).Append(col.ColumnName);
                    sep = sFieldSepchar;
                }
                sCsvData.AppendLine(builder.ToString());

                // then write all the rows
                foreach (DataRow row in dt.Rows)
                {
                    sep = "";
                    builder = new System.Text.StringBuilder();

                    foreach (DataColumn col in dt.Columns)
                    {
                        builder.Append(sep);
                        builder.Append(sFieldDataDelimChar).Append(row[col.ColumnName]).Append(sFieldDataDelimChar);
                        sep = sFieldSepchar;
                    }
                    sCsvData.AppendLine(builder.ToString());
                }

                // Return CSV output
                return sCsvData.ToString();
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return "";
            }
        }

        /// <summary>
        ///  This function gets the DataTable of XML data loaded from the last query with LoadDataSetFromXMLFile and returns as a CSV file
        ///  </summary>
        ///  <param name="sOutputFile">Output CSV file</param>
        ///  <param name="sFieldSepchar">Field delimiter/separator. Default = Comma</param>
        ///  <param name="sFieldDataDelimChar">Field data delimiter character. Default = double quotes.</param>
        ///  <param name="replace">Replace output file True=Replace file,False=Do not replace</param>
        ///  <returns>True-CSV file written successfully, False-Failure writing CSV output file.</returns>
        public bool GetQueryResultsDataTableToCsvFile(DataTable dt, string sOutputFile, string sFieldSepchar = ",", string sFieldDataDelimChar = "\"", bool replace = false)
        {
            string sCsvWork;

            try
            {
                _lastError = "";

                // Delete existing file if replacing
                if (File.Exists(sOutputFile))
                {
                    if (replace)
                        File.Delete(sOutputFile);
                    else
                        throw new Exception("Output file " + sOutputFile + " already exists and replace not selected.");
                }

                // Get data and output
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(sOutputFile))
                {

                    // Get CSV string
                    sCsvWork = GetQueryResultsDataTableToCsvString(dt, sFieldSepchar, sFieldDataDelimChar);

                    // Write out CSV data
                    writer.Write(sCsvWork);

                    // Flush final output and close
                    writer.Flush();
                    writer.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        ///  This function gets the DataTable of data loaded from XML with LoadDataSetFromXMLFile and returns as a XML string
        ///  </summary>
        ///  <param name="sTableName">Table name. Default = "Table1"</param>
        ///  <param name="bWriteSchema">Write XML schema in return data</param>
        ///  <returns>XML string from data table</returns>
        public string GetQueryResultsDataTableToXmlString(DataTable dt, string sTableName = "Table1", bool bWriteSchema = false)
        {
            string sRtnXml = "";

            try
            {
                _lastError = "";

                // if table not set, default to Table1
                if (sTableName.Trim() == "")
                    sTableName = "Table1";

                // Export results to XML
                if (dt != null)
                {
                    StringBuilder SB = new StringBuilder();
                    System.IO.StringWriter SW = new System.IO.StringWriter(SB);
                    dt.TableName = sTableName;
                    // Write XMl with or without schema info
                    if (bWriteSchema)
                        dt.WriteXml(SW, System.Data.XmlWriteMode.WriteSchema);
                    else
                        dt.WriteXml(SW);
                    sRtnXml = SW.ToString();
                    SW.Close();
                    return sRtnXml;
                }
                else
                    throw new Exception("No data available. Error: " + GetLastError());
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return "";
            }
        }
        /// <summary>
        ///  This function gets the DataTable of XML data loaded from the last query with LoadDataSetFromXMLFile and returns as a CSV file
        ///  </summary>
        ///  <param name="sOutputFile">Output CSV file</param>
        ///  <param name="sTableName">Table name. Default = "Table1"</param>
        ///  <param name="bWriteSchema">Write XML schema in return data</param>
        ///  <param name="replace">Replace output file True=Replace file,False=Do not replace</param>
        ///  <returns>True-XML file written successfully, False-Failure writing XML output file.</returns>
        public bool GetQueryResultsDataTableToXmlFile(DataTable dt, string sOutputFile, string sTableName = "Table1", bool bWriteSchema = false, bool replace = false)
        {
            string sXmlWork;

            try
            {
                _lastError = "";

                // Delete existing file if replacing
                if (File.Exists(sOutputFile))
                {
                    if (replace)
                        File.Delete(sOutputFile);
                    else
                        throw new Exception("Output file " + sOutputFile + " already exists and replace not selected.");
                }

                // Get data and output 
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(sOutputFile))
                {

                    // Get XML string
                    sXmlWork = GetQueryResultsDataTableToXmlString(dt, sTableName, bWriteSchema);

                    // Write out CSV data
                    writer.Write(sXmlWork);

                    // Flush final output and close
                    writer.Flush();
                    writer.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }
        }
        /// <summary>
        ///  This function gets the DataTable of data loaded from XML with LoadDataSetFromXMLFile and returns as a JSON string
        ///  </summary>
        ///  <returns>CSV string from DataTable</returns>
        public string GetQueryResultsDataTableToJsonString(DataTable dt, bool debugInfo = false)
        {

            // TODO - Use Newtonsoft JSON to convert to JSON

            string sJsonData = "";
            JsonHelper oJsonHelper = new JsonHelper();

            try
            {
                _lastError = "";

                // If data table is blank, bail
                if (dt == null)
                    throw new Exception("Data table is Nothing. No data available.");

                // Convert DataTable to JSON
                sJsonData = oJsonHelper.DataTableToJsonWithStringBuilder(dt, debugInfo);

                // Return JSON output
                return sJsonData.ToString();
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return "";
            }
        }
        /// <summary>
        ///  This function gets the DataTable of XML data loaded from the last query with LoadDataSetFromXMLFile and returns as a JSON file
        ///  </summary>
        ///  <param name="sOutputFile">Output JSON file</param>
        ///  <param name="replace">Replace output file True=Replace file,False=Do not replace</param>
        ///  <returns>True-JSON file written successfully, False-Failure writing JSON output file.</returns>
        public bool GetQueryResultsDataTableToJsonFile(DataTable dt, string sOutputFile, bool replace = false)
        {
            string sJsonWork;

            try
            {
                _lastError = "";

                // Delete existing file if replacing
                if (File.Exists(sOutputFile))
                {
                    if (replace)
                        File.Delete(sOutputFile);
                    else
                        throw new Exception("Output file " + sOutputFile + " already exists and replace not selected.");
                }

                // Get data and output 
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(sOutputFile))
                {

                    // Get JSON string
                    sJsonWork = GetQueryResultsDataTableToJsonString(dt);

                    // Write out JSON data
                    writer.Write(sJsonWork);

                    // Flush final output and close
                    writer.Flush();
                    writer.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        ///  Open connection shim. Does nothing, but used for coding compatability with PC driver.
        ///  </summary>
        public bool OpenConnection(string connstring = "")
        {
            return true;
        }
        /// <summary>
        ///  Close connection shim. Does nothing, but used for coding compatability with PC driver.
        ///  </summary>
        public bool CloseConnection()
        {
            return true;
        }

        /// <summary>
        ///  Get last connection string shim. Returns nothing but used for coding compatability with PC driver.
        ///  </summary>
        public string GetLastConnectionString()
        {
            return "db2cli";
        }

    }
}