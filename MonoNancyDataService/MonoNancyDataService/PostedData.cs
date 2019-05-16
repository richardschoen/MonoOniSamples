using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoNancyDataService.Properties;

namespace MonoNancyDataService
{
    /// <summary>
    /// This class is used to hold deserialized data that was posted. We're keeping it simple to expect a json packet
    /// </summary>
    public class PostedData
    {
        LogToFile logger = new LogToFile();
        bool debug = Settings.Default.Debug;

        // Set up properties with default values
        public string JsonData { get; set; } = "";
    }

}
