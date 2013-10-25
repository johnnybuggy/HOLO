using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Holo
{
    class Program
    {
        /// <summary>
        /// This is a launcher that starts main executable in the new shadow copy enabled AppDomain. 
        /// Shadow copy is configured with default settings.
        /// </summary>
        /// <param name="args"></param>
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        [STAThread]
        private static void Main(string[] args)
        {
            string StartupPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string ConfigFile = Path.Combine(StartupPath, "Holo.UI.exe.config");
            string Assembly = Path.Combine(StartupPath, "Holo.UI.exe");

            AppDomainSetup DomainSetup = new AppDomainSetup
                                             {
                                                 ApplicationName = "Holo.UI",
                                                 ShadowCopyFiles = "true",
                                                 ConfigurationFile = ConfigFile
                                             };

            AppDomain Domain = AppDomain.CreateDomain("Holo", AppDomain.CurrentDomain.Evidence, DomainSetup);

            Domain.ExecuteAssembly(Assembly);

            AppDomain.Unload(Domain);
        }
    }
}
