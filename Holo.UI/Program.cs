using System;
using System.Threading;
using System.Windows.Forms;
using Holo.UI.Exceptions;

namespace Holo.UI
{
    static class Program
    {
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

            #region To refactor
            // TODO: Legacy code. Needs refactoring.
            try
            {
                RunManager.OnStartApplication();
            }
            catch (ExitApplicationException ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            again:
            try
            {
                Application.Run(new MainForm
                                    {
                                        Icon = Resource.HOLO
                                    });
            }
            catch (ExitApplicationException ex)
            {
                //return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                CloseAppplicationEventArgs e = new CloseAppplicationEventArgs();
                RunManager.OnCloseApplication(e);
                if (e.RestartMainForm)
                    goto again;
            }
            catch (ExitApplicationException ex)
            {
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }
    }
}
