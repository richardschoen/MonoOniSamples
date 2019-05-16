using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using MonoNancyDataService.Properties;

namespace MonoNancyDataService
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions conventions)
        {

            base.ConfigureConventions(conventions);

            // TODO - How to override defaults paths

            // Set root site default content folder location. For static HTML files, images, etc.
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/", Settings.Default.ContentLoc.Trim()));

            // Set home page. Handled in IndexModule.cs
            //conventions.StaticContentsConventions.AddFile("/", Settings.Default.HomePage);

        }

        /// <summary>
        /// Use custom root path provider for site content. 
        /// Picks up path from SitePath app config setting
        /// </summary>
        protected override IRootPathProvider RootPathProvider
        {
            get { return new CustomRootPathProvider(); }
        }
    
    }

}
