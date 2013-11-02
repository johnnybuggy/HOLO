using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using HoloKernel;
using HoloDB;

namespace Holo.UI
{
    public static class RunManager
    {
        /// <summary>
        /// Path to the database file
        /// </summary>
        public static string DBPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "holo.db"); }
        }

        public static DB DB { get; set; }

        public static Factory Factory { get; set; }

        public static void OnStartApplication()
        {
            //load database
            try
            {
                DB = DB.Load(DBPath, DefaultFactory.GetWellKnownTypes());
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DB = new DB();
            }

            //init factory
            Factory = new DefaultFactory();
        }

        public static void OnCloseApplication(CloseAppplicationEventArgs e)
        {
            //save database
            if(DB.IsChanged)
                DB.Save(DBPath);
        }
    }

    public class CloseAppplicationEventArgs : EventArgs
    {
        public bool RestartMainForm { get; set; }
    }
}
