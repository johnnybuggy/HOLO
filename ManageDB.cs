using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;


//using HOLO.Utilities;

namespace HOLO
{
    class ManageDB
    {
        private string Name;
        private string Path;
        private string SearchPath;
        public SQLiteConnection DBConnection;
        public List<string> FileList;

        public static List<string> CreateFileList(string searchpath, int maxfiles = 0)
        {
            string[] files = Directory.GetFiles(searchpath, "*.mp3", SearchOption.AllDirectories);
            var fll = files.ToList();
            fll = Utilities.ListRandomize(fll, fll.Count);
            if (maxfiles > 0)
                fll = fll.Take((int)maxfiles).ToList();
            //var fl = fll;

            //FileList = fll;
            return fll;
        }

        /*private bool SaveFileListToDB()
        {
            return true;
        }*/

        /*private bool SaveStatsToDB()
        {
            return true;
        }*/

        public bool ConnectDB(string name = "test.db", string path = "")
        {
            this.Name = name;
            this.Path = path;

            DBConnection = new SQLiteConnection();
            try
            {
                DBConnection.ConnectionString = "Data Source=" + this.Path + this.Name + ";";
                DBConnection.Open();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool CreateDB(string name = "test.db", string path = "")
        {
            this.Name = name;
            this.Path = path;
            this.SearchPath = path;

            ConnectDB(name, path);

            var cmd = DBConnection.CreateCommand();
            try
            {
                cmd.CommandText = "PRAGMA journal_mode=DELETE;"; cmd.ExecuteNonQuery();
                cmd.CommandText = "DROP TABLE IF EXISTS MAIN;"; cmd.ExecuteNonQuery();
                cmd.CommandText = "DROP TABLE IF EXISTS STATS;"; cmd.ExecuteNonQuery();
                cmd.CommandText = "VACUUM;"; cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE MAIN (id int, name string, path string);"; cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE STATS (id int, statname string, statparam1 int, statparam2 int, statvalue float);"; cmd.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
            }
            catch
            {
                new Exception("Something has gone wrong with the database");
                DBConnection.Close();
            }

            return true;
        }

        public bool PutNewRecord(string id, string filename, string filepath, Dictionary<string, double> values, string dbfile, string dbpath)
        {
            var dbConn = new SQLiteConnection();
            try
            {
                dbConn.ConnectionString = "Data Source=" + this.Path + this.Name + ";";
                dbConn.Open();
            }
            catch
            {
                return false;
            }

            //ConnectDB(dbfile, dbpath);
            var cmd = dbConn.CreateCommand();

            SQLiteParameter myparam1 = new SQLiteParameter();
            SQLiteParameter myparam2 = new SQLiteParameter();
            SQLiteParameter myparam3 = new SQLiteParameter();

            SQLiteParameter myparam2_1 = new SQLiteParameter();
            SQLiteParameter myparam2_2 = new SQLiteParameter();
            SQLiteParameter myparam2_3 = new SQLiteParameter();
            SQLiteParameter myparam2_4 = new SQLiteParameter();
            SQLiteParameter myparam2_5 = new SQLiteParameter();

            cmd.CommandText = "BEGIN TRANSACTION;";
            cmd.ExecuteNonQuery();

            foreach (var di in values)
            {
                cmd.CommandText = "INSERT INTO STATS VALUES (@id, @statname, @statparam1, @statparam2, @statvalue);";
                myparam2_1.ParameterName = "@id";
                myparam2_2.ParameterName = "@statname";
                myparam2_3.ParameterName = "@statparam1";
                myparam2_4.ParameterName = "@statparam2";
                myparam2_5.ParameterName = "@statvalue";

                myparam2_1.Value = id;
                myparam2_2.Value = di.Key;
                myparam2_3.Value = 0;
                myparam2_4.Value = 0;
                myparam2_5.Value = di.Value;

                cmd.Parameters.Add(myparam2_1); cmd.Parameters.Add(myparam2_2); cmd.Parameters.Add(myparam2_3);
                cmd.Parameters.Add(myparam2_4); cmd.Parameters.Add(myparam2_5);
                cmd.ExecuteNonQuery();

                //Application.DoEvents();
            }

            cmd.CommandText = "INSERT INTO MAIN VALUES (@id, @name, @path);";
            myparam1.ParameterName = "@id";
            myparam2.ParameterName = "@name";
            myparam3.ParameterName = "@path";

            myparam1.Value = id;
            myparam2.Value = filename;
            myparam3.Value = filepath;

            cmd.Parameters.Add(myparam1);
            cmd.Parameters.Add(myparam2);
            cmd.Parameters.Add(myparam3);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "COMMIT;";
            cmd.ExecuteNonQuery();

            dbConn.Close();
            
            return true;
        }

        public static bool PostprocessDB()
        {
            Utilities.ExecuteCommandSync("sqlite3.exe < postprocess.sql");
            return true;
        }

        public bool OpenDB()
        {
            if (DBConnection == null)
                DBConnection = new SQLiteConnection();

            if (DBConnection.State == System.Data.ConnectionState.Closed)
            {
                DBConnection.ConnectionString = "Data Source=" + this.Path + this.Name + ";";
                DBConnection.Open();
            }

            return true;
        }

        public bool CloseDB()
        {
            DBConnection.Close();
            return true;
        }

        public static int RequestInt(string path, string filename, string request)
        {
            var dbConn = new SQLiteConnection();
            try
            {
                dbConn.ConnectionString = "Data Source=" + path + filename + ";";
                dbConn.Open();
            }
            catch
            {
                return -1;
            }            
            var cmd = dbConn.CreateCommand();
            //var s = "select count(id) from main;";
            var s = request;
            cmd.CommandText = s;
            var data = cmd.ExecuteReader();

            if (data.HasRows)
            {
                data.Read();
                if (data.FieldCount > 1)
                    return -1;
                int r = int.Parse(data[0].ToString());
                dbConn.Close();
                return r;
            }

            dbConn.Close();

            return -1;
        }

        public static Dictionary<string, double> RequestSongDict(string path, string filename, string id)
        {
            var dbConn = new SQLiteConnection();
            try
            {
                dbConn.ConnectionString = "Data Source=" + path + filename + ";";
                dbConn.Open();
            }
            catch
            {
                return null;
            }
            var cmd = dbConn.CreateCommand();
            var s = "select statname, statvalue from stats where id = " + id + ";";
            
            cmd.CommandText = s;
            var data = cmd.ExecuteReader();
            var ans = new Dictionary<string, double>();

            while (data.HasRows)
            {
                data.Read();
                if (data.FieldCount != 2)
                    return null;
                string statname = data[0].ToString();
                if (statname == "")
                    continue;

                double statvalue = double.Parse(data[1].ToString());

                if (ans.ContainsKey(statname))
                    ans[statname] = statvalue;
                else
                    ans.Add(statname, statvalue);
            }

            dbConn.Close();

            return ans;
        }

        public static List<string> RequestStringList(string path, string filename, string request)
        {
            var ans = new List<string>();

            var dbConn = new SQLiteConnection();
            try
            {
                dbConn.ConnectionString = "Data Source=" + path + filename + ";";
                dbConn.Open();
            }
            catch
            {
                return null;
            }
            var cmd = dbConn.CreateCommand();
            //var s = "select count(id) from main;";
            var s = request;
            cmd.CommandText = s;
            var data = cmd.ExecuteReader();

            while (data.HasRows)
            {
                data.Read();
                if (data.FieldCount > 1)
                    return null;
                var r = data[0].ToString();

                ans.Add(r);
            }

            dbConn.Close();

            return ans;
        }

        public static List<KeyValuePair<int, KeyValuePair<string, double>>> LoadStatsFromDB(string path, string filename, int count, int varcount)
        {
            var ans = new List<KeyValuePair<int, KeyValuePair<string, double>>>();

            var dbConn = new SQLiteConnection();
            try
            {
                dbConn.ConnectionString = "Data Source=" + path + filename + ";";
                dbConn.Open();
            }
            catch
            {
                return null;
            }            
            var cmd = dbConn.CreateCommand();
            var s = "select id, statname, statvalue from stats limit " + (varcount * count).ToString() + "; ";
            cmd.CommandText = s;
            var data = cmd.ExecuteReader();
            
            while (data.HasRows)
            {
                data.Read();
                List<string> r = new List<string>();
                for (int i = 0; i < data.FieldCount; i++)
                    r.Add(data[i].ToString());
                if (r.Count > 3)
                    return null;

                if (r.Any(v => v == ""))
                    continue;
                var kv = new KeyValuePair<string, double>(r[1], double.Parse(r[2]));
                var kkv = new KeyValuePair<int, KeyValuePair<string, double>>(int.Parse(r[0]), kv);
                ans.Add(kkv);
            }

            dbConn.Close();
            dbConn.Dispose();

            return ans;
        }
    }
}
