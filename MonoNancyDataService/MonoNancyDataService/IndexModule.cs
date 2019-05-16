using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using MonoNancyDataService.Properties;

namespace MonoNancyDataService
{
        /// <summary>
        /// Handle serving of our application home page
        /// </summary>
        public class IndexModule : NancyModule
        {
            // Pull in settings 
            LogToFile logger = new LogToFile();
            bool debug = Settings.Default.Debug;

        public IndexModule()
        {

            // Serve up the home page using relative path from app settings
            Get[@"/"] = parameters =>
            {
                if (debug)
                    logger.WriteLogFile("Serving home page: " + Settings.Default.HomePage, "INFO");

                // Re-direct to default/home page
                return Response.AsRedirect(Settings.Default.HomePage);

            };
        }
    }
}
