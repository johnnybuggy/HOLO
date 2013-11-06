using System;
using System.Threading;
using System.Windows.Forms;
using Holo.Core;
using NLog;

namespace Holo.UI
{
    static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Mutex Mutex;

        private static bool InstanceExists()
        {
            bool CreatedNew;
            Mutex = new Mutex(false, "Holo.UI", out CreatedNew);
            return (!CreatedNew);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (InstanceExists())
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            // TODO: Resolve factory as a depenedency by means of some DI solution.
            DefaultFactory Factory = new DefaultFactory();

            try
            {
                HoloCore Core = new HoloCore(Factory);
                
                MainForm MainForm = new MainForm(Core)
                                        {
                                            Icon = Resource.HOLO
                                        };
                Core.SetView(MainForm);

                Application.Run(MainForm);

                Core.SaveDatabase();
            }
            catch (Exception E)
            {
                Logger.FatalException("Unhandled exception at the top level.", E);

                MessageBox.Show(E.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
