using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using HoloKernel;

namespace HoloUI
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
            //magic
            typeof(Form).GetField("defaultIcon", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, Resource.HOLO_ico);

            //load database
            DB = DB.Load(DBPath);

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
