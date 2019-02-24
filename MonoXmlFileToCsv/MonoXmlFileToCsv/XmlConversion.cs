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
    class XmlConversion
    {

        private string _LastError = "";
        private int _iRecordCount = 0;

        /// <summary>
        /// Get last error value
        /// </summary>
        /// <returns>Last error message string</returns>
        public string GetLastError()
        {
            return _LastError;
        }
        /// <summary>
        /// Get last record count
        /// </summary>
        /// <returns>Last record cont from conversion</returns>
        public int GetLastRecordCount()
        {
            return _iRecordCount;
        }

        /// <summary>
        ///  This function loads an XML file and exports the data to a CSV file.
        ///  </summary>
        ///  <param name="xmlInputFile">XML File</param>
        ///  <param name="csvOutputFile">XML File</param>
        /// <param name="delimiter">Output file delimiter. Default= Comma (,)</param>
        /// <param name="dblquotes">Output double quotes arouns data. Y=output double quotes, N=no double quotes</param>
        /// <param name="usebuffering">Buffer records for less memory usage. true=use buffering, false=do not use buffering</param>
        /// <param name="iRecordFlushCount">How many records to buffer in memory before flushing to disk. Default=1000</param>
        ///  <returns>True-successfully converted XML file to CSV, False-failed to convert XML file</returns>
        public bool ConvertXmlFileToCSV(string xmlInputFile, string csvOutputFile, string delimiter = ",", string dblquotes = "N",bool useBuffering=false,int iRecordFlushCount=1000)

        {

            DataSet _dsXml = null;
            DataTable _dtXml = null;
            string dblqt = "";
            int iCurRecordCount = 1;

            try
            {
                _LastError = "";

                // Bail if no XML file
                if (System.IO.File.Exists(xmlInputFile) == false)
                {
                    throw new Exception("XML file " + xmlInputFile + " does not exist. Process cancelled.");
                }

                // Set dbl quote if passed in
                if (dblquotes.Trim().ToUpper() == "Y")
                {
                    dblqt = "\"";
                }
                else
                {
                    dblqt = "";
                }

                // Set delimiter if special keywords passed in
                if (delimiter.Trim().ToUpper()=="COMMA")
                {
                    delimiter = ",";
                }
                else if (delimiter.Trim().ToUpper() == "TAB")
                {
                    delimiter = "\t";
                }
                else if (delimiter.Trim().ToUpper() == "PIPE")
                {
                    delimiter = "|";
                }
                else if (delimiter.Trim().ToUpper() == "TILDE")
                {
                    delimiter = "~";
                }
                else if (delimiter.Trim().ToUpper() == "SEMICOLON")
                {
                    delimiter = ";";
                }
                else if (delimiter.Trim().ToUpper() == "SPACE")
                {
                    delimiter = " ";
                }

                // Load XML data into a dataset  
                _dsXml = new DataSet();
                _dsXml.ReadXml(xmlInputFile);

                // Start new DataTable for XML firt table. We're only expecitng a single table
                _LastError = _dsXml.Tables[0].Rows.Count + " rows were returned from XML file " + xmlInputFile;
                _dtXml = _dsXml.Tables[0];

                // Create string builder for work data
                StringBuilder sb = new StringBuilder();
                
                // Iterate the column names and output CSV header record
                foreach (DataColumn col in _dtXml.Columns)
                {
                    sb.Append(col.ColumnName + delimiter);
                }
                // Remove last data delimiter and add new line
                sb.Remove(sb.Length - 1, 1);
                sb.Append(Environment.NewLine);

                // Write headings to new file
                File.WriteAllText(csvOutputFile, sb.ToString());

                // Clear the string buffer before detail records
                sb.Clear();

                // Iterate the data rows and output data
                foreach (DataRow row in _dtXml.Rows)
                {
                    for (int i = 0; i < _dtXml.Columns.Count; i++)
                    {
                        sb.Append(dblqt + row[i].ToString() + dblqt + delimiter);
                    }

                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(Environment.NewLine);

                    iCurRecordCount++; // Increment Flush count
                    _iRecordCount++; // Increment total record count

                    // Output to file if flush count met, and 
                    // buffering is enabled
                    if (useBuffering)
                    {
                        if (iCurRecordCount == iRecordFlushCount)
                        {
                            // Append line to file
                            File.AppendAllText(csvOutputFile, sb.ToString());
                            sb.Clear();
                            iCurRecordCount = 0;
                        }
                    }

                }

                //Flush and append entire recordset or last chunk if buffering
                if (sb.Length > 0)
                {
                    File.AppendAllText(csvOutputFile, sb.ToString());
                }

                return true;
            }

            catch (Exception ex)
            {
                _LastError = ex.Message;
                return false;
            }

        }


    }
}
