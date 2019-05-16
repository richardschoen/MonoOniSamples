using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNancyDataService
{

    /// <summary>
    /// String helper functions
    /// </summary>
    class StringHelper
    {

        /// <summary>
        /// Pair up single quotes in data field before 
        /// inserting into database with SQL
        /// </summary>
        /// <param name="originalvalue">Original string value</param>
        /// <returns>Modified string value or exception on errors</returns>
        public string StrQt(string originalvalue)
        {
                // Set work field
                string strWork = originalvalue;
                strWork = strWork.Replace("'", "''");
                return strWork;
        }

        /// <summary>
        /// Replace single quotes with double quote or other value before 
        /// inserting into database with SQL
        /// </summary>
        /// <param name="originalvalue">Original string value</param>
        /// <param name="replacementvalue">Replacement string value</param>
        /// <returns>Modified string value or exception on errors</returns>
        public string StrRepl(string originalvalue,string valuetoreplace="'",string replacementvalue="\"")
        {
                // Set work field
                string strWork = originalvalue;
                // Replace values
                strWork = strWork.Replace(valuetoreplace,replacementvalue);
                return strWork;
        }

        /// <summary>
        /// Get new guid
        /// </summary>
        /// <returns>New guid or exception on error</returns>
        public string GetNewGuid()
        {

            return System.Guid.NewGuid().ToString();
        }


    }
}
