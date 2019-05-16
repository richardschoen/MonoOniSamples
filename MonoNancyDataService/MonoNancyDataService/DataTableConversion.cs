using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MonoNancyDataService
{
    public class DataTableConversion
    {

        private string _LastError = "";

        /// <summary>
        ///  Returns last error message string
        ///  </summary>
        ///  <returns>Last error message string</returns>
        ///  <remarks></remarks>
        public string GetLastError()
        {
            try
            {
                return _LastError;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        ///  This function gets the DataTable of data loaded from XML with LoadDataSetFromXMLFile and returns as a CSV string
        ///  </summary>
        ///  <param name="dtResults">DataTable to convert</param>
        ///  <param name="sFieldSepchar">Field delimiter/separator. Default = Comma</param>
        ///  <param name="sFieldDataDelimChar">Field data delimiter character. Default = double quotes.</param>
        ///  <returns>CSV string from DataTable</returns>
        public string GetQueryResultsDataTableToCsvString(DataTable dtResults, string sFieldSepchar = ",", string sFieldDataDelimChar = "\"")
        {
            try
            {
                _LastError = "";

                //string sHeadings = "";
                //string sBody = "";
                StringBuilder sCsvData = new StringBuilder();

                // first write a line with the columns name
                string sep = "";
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                foreach (DataColumn col in dtResults.Columns)
                {
                    builder.Append(sep).Append(col.ColumnName);
                    sep = sFieldSepchar;
                }
                sCsvData.AppendLine(builder.ToString());

                // then write all the rows
                foreach (DataRow row in dtResults.Rows)
                {
                    sep = "";
                    builder = new System.Text.StringBuilder();

                    foreach (DataColumn col in dtResults.Columns)
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
                _LastError = ex.Message;
                return "";
            }
        }
        /// <summary>
        ///  This function gets the DataTable of XML data loaded from the last query with LoadDataSetFromXMLFile and returns as a CSV file
        ///  </summary>
        ///  <param name="dtResults">DataTable to convert</param>
        ///  <param name="sOutputFile">Output CSV file</param>
        ///  <param name="sFieldSepchar">Field delimiter/separator. Default = Comma</param>
        ///  <param name="sFieldDataDelimChar">Field data delimiter character. Default = double quotes.</param>
        ///  <param name="replace">Replace output file True=Replace file,False=Do not replace</param>
        ///  <returns>True-CSV file written successfully, False-Failure writing CSV output file.</returns>
        public bool GetQueryResultsDataTableToCsvFile(DataTable dtResults,string sOutputFile, string sFieldSepchar = ",", string sFieldDataDelimChar = "\"", bool replace = false)
        {
            string sCsvWork;

            try
            {
                _LastError = "";

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
                    sCsvWork = GetQueryResultsDataTableToCsvString(dtResults,sFieldSepchar, sFieldDataDelimChar);

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
                _LastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        ///  This function gets the DataTable of data loaded from XML with LoadDataSetFromXMLFile and returns as a XML string
        ///  </summary>
        ///  <param name="dtResults">DataTable to convert</param>
        ///  <param name="sTableName">Table name. Default = "Table1"</param>
        ///  <param name="bWriteSchema">Write XML schema in return data</param>
        ///  <returns>XML string from data table</returns>
        public string GetQueryResultsDataTableToXmlString(DataTable dtResults,string sTableName = "Table1", bool bWriteSchema = false)
        {
            string sRtnXml = "";

            try
            {
                _LastError = "";

                // if table not set, default to Table1
                if (sTableName.Trim() == "")
                    sTableName = "Table1";

                // Export results to XML
                if (dtResults != null)
                {
                    StringBuilder SB = new StringBuilder();
                    System.IO.StringWriter SW = new System.IO.StringWriter(SB);
                    dtResults.TableName = sTableName;
                    // Write XMl with or without schema info
                    if (bWriteSchema)
                        dtResults.WriteXml(SW, System.Data.XmlWriteMode.WriteSchema);
                    else
                        dtResults.WriteXml(SW);
                    sRtnXml = SW.ToString();
                    SW.Close();
                    return sRtnXml;
                }
                else
                    throw new Exception("No data available. Error: " + GetLastError());
            }
            catch (Exception ex)
            {
                _LastError = ex.Message;
                return "";
            }
        }
        /// <summary>
        ///  This function gets the DataTable of XML data loaded from the last query with LoadDataSetFromXMLFile and returns as a CSV file
        ///  </summary>
        ///  <param name="dtResults">DataTable to convert</param>
        ///  <param name="sOutputFile">Output CSV file</param>
        ///  <param name="sTableName">Table name. Default = "Table1"</param>
        ///  <param name="bWriteSchema">Write XML schema in return data</param>
        ///  <param name="replace">Replace output file True=Replace file,False=Do not replace</param>
        ///  <returns>True-XML file written successfully, False-Failure writing XML output file.</returns>
        public bool GetQueryResultsDataTableToXmlFile(DataTable dtResults,string sOutputFile, string sTableName = "Table1", bool bWriteSchema = false, bool replace = false)
        {
            string sXmlWork;

            try
            {
                _LastError = "";

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
                    sXmlWork = GetQueryResultsDataTableToXmlString(dtResults, sTableName, bWriteSchema);

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
                _LastError = ex.Message;
                return false;
            }
        }
        /// <summary>
        ///  This function gets the DataTable of data loaded from XML with LoadDataSetFromXMLFile and returns as a JSON string
        ///  </summary>
        ///  <returns>CSV string from DataTable</returns>
        public string GetQueryResultsDataTableToJsonString(DataTable dtResults,bool debugInfo = false)
        {

            // TODO - Use Newtonsoft JSON to convert to JSON

            string sJsonData = "";
            JsonHelper oJsonHelper = new JsonHelper();

            try
            {
                _LastError = "";

                // If data table is blank, bail
                if (dtResults == null)
                    throw new Exception("Data table is Nothing. No data available.");

                // Convert DataTable to JSON
                sJsonData = oJsonHelper.DataTableToJsonWithStringBuilder(dtResults, debugInfo);

                // Return JSON output
                return sJsonData.ToString();
            }
            catch (Exception ex)
            {
                _LastError = ex.Message;
                return "";
            }
        }
        /// <summary>
        ///  This function gets the DataTable of XML data loaded from the last query with LoadDataSetFromXMLFile and returns as a JSON file
        ///  </summary>
        ///  <param name="dtResults">DataTable to convert</param>
        ///  <param name="sOutputFile">Output JSON file</param>
        ///  <param name="replace">Replace output file True=Replace file,False=Do not replace</param>
        ///  <returns>True-JSON file written successfully, False-Failure writing JSON output file.</returns>
        public bool GetQueryResultsDataTableToJsonFile(DataTable dtResults,string sOutputFile, bool replace = false)
        {
            string sJsonWork;

            try
            {
                _LastError = "";

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
                    sJsonWork = GetQueryResultsDataTableToJsonString(dtResults);

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
                _LastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        ///  Convert DataTable Row List to Generic List and optionally include column names.
        ///  </summary>
        ///  <param name="dtTemp">DataTable Object</param>
        ///  <param name="firstRowColumnNames">Optional - Return first row as column names. False=No column names, True=Return column names. Default=False</param>
        ///  <returns>List object</returns>
        public List<List<object>> ConvertDataTableToList(DataTable dtTemp, bool firstRowColumnNames = false)
        {
            List<List<object>> result = new List<List<object>>();
            List<object> values = new List<object>();

            try
            {
                _LastError = "";

                // Include first row as columns
                if (firstRowColumnNames)
                {
                    foreach (DataColumn column in dtTemp.Columns)
                        values.Add(column.ColumnName);
                    result.Add(values);
                }

                // Output all the data now
                foreach (DataRow row in dtTemp.Rows)
                {
                    values = new List<object>();
                    foreach (DataColumn column in dtTemp.Columns)
                    {
                        if (row.IsNull(column))
                            values.Add(null);
                        else
                            values.Add(row[column]);
                    }
                    result.Add(values);
                }
                return result;
            }
            catch (Exception ex)
            {
                _LastError = ex.Message;
                return null;
            }
        }

        /// <summary>
        ///  Convert JSON to DataTable by deserializing JSON data
        ///  Note: The assumption is a simple JSON return array for this to work.
        ///  Complex JSON may not convert correctly
        ///  </summary>
        ///  <param name="jsonData">JSON data. </param>
        ///  <returns>DataTable object or null on error</returns>
        public DataTable ConvertJsonToDataTable(string jsonData, string dataTableName = "Table1")
        {

            try
            {
                _LastError = "";

                // Deserialize JSON to a DataTable
                var table = JsonConvert.DeserializeObject<DataTable>(jsonData);
                table.TableName = dataTableName;
                return table;

            }
            catch (Exception ex)
            {
                _LastError = ex.Message;
                return null;
            }
        }

        /// <summary>
        ///  Convert JSON to List by deserializing JSON data
        ///  Note: The assumption is a simple JSON return array for this to work.
        ///  Complex JSON may not convert correctly
        ///  </summary>
        ///  <param name="jsonData">JSON data. </param>
        ///  <param name="firstRowColumnNames">Optional - Return first row as column names. False=No column names, True=Return column names. Default=False</param>
        ///  <returns>List object</returns>
        public List<List<object>> ConvertJsonToList(string jsonData, bool firstRowColumnNames = false)
        {

            try
            {
                _LastError = "";

                // Deserialize JSON to a DataTable
                var table = JsonConvert.DeserializeObject<DataTable>(jsonData);
                table.TableName = "Table1"; // Set tablename. Doesn't really matter for list return
                return ConvertDataTableToList(table, firstRowColumnNames);

            }
            catch (Exception ex)
            {
                _LastError = ex.Message;
                return null;
            }
        }

    }
}
