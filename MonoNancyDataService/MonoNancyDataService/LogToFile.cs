using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MonoNancyDataService.Properties;

namespace MonoNancyDataService
{
    class LogToFile
    {

        /// <summary>
        /// Write entry to log file defined in App Settings
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool WriteLogFile(string message,string type)
        {
            try
            {
                string logfile = Settings.Default.LogFile.Trim().Replace("@@DATE",DateTime.Now.ToString("yyyyMMdd"));
                string msg = System.DateTime.Now + " - " + type + " - " + message + "\r\n";
                File.AppendAllText(logfile, msg);
                return true;
            } catch (Exception ex)
            {
                return false;
            }
        }


    }
}
