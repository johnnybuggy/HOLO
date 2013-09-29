using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HoloUI;

namespace HOLO2
{
    static class Program
    {
        static Mutex mutex;

        static bool InstanceExists()
        {
            bool createdNew;
            mutex = new Mutex(false, "Holo2", out createdNew);
            return (!createdNew);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (StartShadowCopy()) return;

            //запуск выполнен в отдельном методе, для того что бы не подгружались длл
            Work();
        }

        static void Work()
        {
            //не допускаем двух копий программы
            if (InstanceExists())
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
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
                Application.Run(new MainForm());
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
                    goto again;//перезапускаем главную форму
            }
            catch (ExitApplicationException ex)
            {
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region StartShadowCopy Routines

        private static readonly string DomainName = "MarvinDomain";
        private static readonly string CmdParamName = "OriginalExePath";

        /// <summary>
        /// Копирует этот exe и зависимые dll во временную папку, и запускает приложение оттуда.
        /// Свойства AppDomain.CurrentDomain.BaseDirectory, Application.ExecutablePath и Application.StartupPath будет указывать на исходную папку.
        /// Аргументы командной строки также корректно передаются.
        /// Настоящий путь к копии exe можно узнать по Environment.GetCommandLineArgs()[0]
        /// </summary>
        /// <returns>true если запущена копия, false если мы и есть копия (либо мы запущены из VS)</returns>
        private static bool StartShadowCopy()
        {
            if (Environment.GetCommandLineArgs()[0].ToLower().EndsWith(".vshost.exe"))
                return false;//мы запущены из VisualStudio и не будем копироваться

            string originalExePath = null;
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                var parts = arg.Split(new char[] { '=' }, 2);
                if (parts.Length == 2)
                    if (parts[0] == CmdParamName)
                        originalExePath = parts[1].Trim('\"');
            }

            if (AppDomain.CurrentDomain.FriendlyName != DomainName)
            {
                string exeFilePath = Assembly.GetExecutingAssembly().Location;

                //Debugger.Launch(); // You can uncomment to automatically launch VS for debugging
                if (originalExePath == null)
                {
                    string copyDirectoryPath = Path.GetTempPath() + Path.GetFileNameWithoutExtension(exeFilePath);

                    if (!Directory.Exists(copyDirectoryPath))
                        Directory.CreateDirectory(copyDirectoryPath);
                    else
                        CleanDirectory(new DirectoryInfo(copyDirectoryPath));

                    string copyExePath = copyDirectoryPath + "\\" + Path.GetFileNameWithoutExtension(exeFilePath) + ".exe";
                    try
                    {
                        File.Copy(exeFilePath, copyExePath, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }


                    //build commandline args
                    var sb = new StringBuilder();
                    var args = Environment.GetCommandLineArgs();
                    for (int i = 1; i < args.Length; i++)
                        sb.Append(args[i] + " ");
                    sb.Append(CmdParamName + "=\"" + exeFilePath + "\"");
                    //start exe
                    Process.Start(copyExePath, sb.ToString());
                    return true;
                }

                Thread mainThread = new Thread(() =>
                {
                    //Debugger.Launch();    // You can uncomment to automatically launch VS for debugging
                    AppDomainSetup appDomainShodowCopySetup = AppDomain.CurrentDomain.SetupInformation;
                    appDomainShodowCopySetup.ShadowCopyFiles = true.ToString();
                    appDomainShodowCopySetup.ApplicationBase = Path.GetDirectoryName(originalExePath);
                    appDomainShodowCopySetup.CachePath = Path.GetDirectoryName(exeFilePath) + "\\CachePath";

                    AppDomain marvinDomain = AppDomain.CreateDomain(DomainName, null, appDomainShodowCopySetup);
                    marvinDomain.ExecuteAssembly(exeFilePath);//start my domain in ShadowCopy mode
                    try
                    {
                        AppDomain.Unload(marvinDomain);
                    }
                    catch { }
                });

                mainThread.Start();
                return true;
            }

            //подменяем Application.ExecutablePath и Application.StartupPath
            if (originalExePath != null)
            {
                typeof(Application).GetField("executablePath", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).SetValue(typeof(Application), originalExePath);
                typeof(Application).GetField("startupPath", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).SetValue(typeof(Application), Path.GetDirectoryName(originalExePath));
            }

            return false;
        }

        private static void CleanDirectory(DirectoryInfo di)
        {
            if (di == null)
                return;

            foreach (FileSystemInfo fsEntry in di.GetFileSystemInfos())
            {
                CleanDirectory(fsEntry as DirectoryInfo);

                if (fsEntry is FileInfo)
                    if ((fsEntry as FileInfo).IsReadOnly) (fsEntry as FileInfo).IsReadOnly = false;

                try { fsEntry.Delete(); }
                catch { ; }
            }

            WaitForDirectoryToBecomeEmpty(di);
        }

        private static void WaitForDirectoryToBecomeEmpty(DirectoryInfo di)
        {
            for (int i = 0; i < 5; i++)
            {
                if (di.GetFileSystemInfos().Length == 0)
                    return;
                Console.WriteLine(di.FullName + i);
                Thread.Sleep(50 * i);
            }
        }

        #endregion
    }
}
