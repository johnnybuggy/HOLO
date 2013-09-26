using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;


namespace HOLO
{
    class Utilities
    {
        public static List<string> ListRandomize(List<string> vec, int depth)
        {
            var vv = new Random();
            int l = vec.Count();
            var ans = new List<string>();
            ans = vec;

            for (int i = 0; i < depth; i++)
            {
                int imin = vv.Next(0, l / 2);
                int imax = vv.Next(l / 2 + 1, l);
                ans.Reverse(imin, imax - imin);
            }
            return ans;
        }

        public static void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
            }
            catch //(Exception objException)
            {
                // Log the exception
            }
        }
        public static void ExecuteCommandAsync(string command)
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteCommandSync));
                //Make the thread as background thread.
                objThread.IsBackground = true;
                //Set the Priority of the thread.
                objThread.Priority = ThreadPriority.AboveNormal;
                //Start the thread.
                objThread.Start(command);
            }
            catch //(ThreadStartException objException)
            {
                // Log the exception
            }
            //catch (ThreadAbortException objException)
            {
                // Log the exception
            }
            //catch (Exception objException)
            {
                // Log the exception
            }
        }

        public static double[] Normalize(ref short[] vec, int length)
        {
            var ans = new double[length];
            double mean = vec.Average(t => (double)t);
            double stdev = 1; // Math.Sqrt(vec.Average(t => Math.Pow((double)t - mean, 2)));
            for (int i = 0; i < length; i++)
                ans[i] = (((double)vec[i] - mean) / stdev);
            return ans;
        }

        public static double[] NormalizeFull(ref short[] vec, int length)
        {
            var ans = new double[length];
            double mean = vec.Average(t => (double)t);
            double stdev = Math.Sqrt(vec.Average(t => Math.Pow((double)t - mean, 2)));
            for (int i = 0; i < length; i++)
                ans[i] = (((double)vec[i] - mean) / stdev);
            return ans;
        }

        public static double CalcCorrel(List<double> vec1, List<double> vec2, double avg1 = 0, double avg2 = 0)
        {
            if (vec1.Count != vec2.Count)
                return 0;

            double s = 0, divisor1 = 0, divisor2 = 0;

            for (int i = 0; i < vec1.Count; i++)
            {
                double diff1 = vec1[i] - avg1;
                double diff2 = vec2[i] - avg2;
                s += (diff1) * (diff2);
                divisor1 += (diff1) * (diff1);
                divisor2 += (diff2) * (diff2);
            }

            return s / Math.Sqrt(divisor1 * divisor2);
        }

        public static List<double> Smoothen(List<double> vec, int smooth_size)
        {
            List<double> ans = new List<double>();

            double v = 0;

            for (int i = 0; i < vec.Count; i++)
            {
                int first = Math.Max(i - (int)(smooth_size / 2), 0);
                int last = Math.Min(i + (int)(smooth_size / 2), vec.Count);
                v = 0;
                for (int j = first; j < last; j++)
                {
                    v += vec[j];
                }
                ans.Add(v / (last - first));
            }

            return ans;
        }

        public static double StDev(List<double> vec)
        {
            double avg = vec.Average();
            return Math.Sqrt(vec.Average(t => Math.Pow(t - avg, 2)));
        }

        public static List<double> NormalizeList(List<double> vec)
        {
            var ans = new List<double>();
            double mean = vec.Average();
            double stdev = Math.Sqrt(vec.Average(t => Math.Pow(t - mean, 2)));
            foreach (var t in vec)
                ans.Add((t - mean) / stdev);
            return ans;
        }

        public static double[] NormalizeArray(short[] vec, int length)
        {
            var ans = new double[length];
            double mean = vec.Average(t => (double)t);
            //double stdev = Math.Sqrt(vec.Average(t => Math.Pow((double)t - mean, 2)));
            for (int i = 0; i < length; i++)
                ans[i] = (((double)vec[i] - mean));// / stdev);
            return ans;
        }

        public static Dictionary<int, double> CalcMW(int sz)
        {
            var ans = new Dictionary<int, double>();
            
            ans.Clear();

            double a0 = 0.35875,
                a1 = 0.48829,
                a2 = 0.14128,
                a3 = 0.01168;

            for (int i = 0; i < sz; i++)
            {
                double val = a0
                    - a1 * Math.Cos(2 * i * Math.PI / (sz - 1))
                    + a2 * Math.Cos(4 * i * Math.PI / (sz - 1))
                    - a3 * Math.Cos(6 * i * Math.PI / (sz - 1));
                ans.Add(i, val);
            }
            return ans;
        }

        public static double VecDiff(List<double> vec1, List<double> vec2)
        {
            double ans = 0;
            for (int i = 0; i < vec1.Count; i++)
                ans += Math.Pow(vec1[i] - vec2[i], 2);

            return ans;
        }

        public static double VecDiffCorrel(List<double> vec1, List<double> vec2)
        {
            var vol_diff = Math.Pow(vec1.Average() - vec2.Average(), 2);
            var ans = vol_diff * (4 - CalcCorrel(vec1, vec2));
            return ans;
        }

        public static List<double> DownscaleList(List<double> input, float downscale_rate)
        {
            var ans = new List<double>();

            for (int i = 0; i < (int)(input.Count / downscale_rate); i++)
                ans.Add(input[(int)(i * downscale_rate)]);

            return ans;
        }

        public static bool GetTrueByProb(double prob)
        {
            var r = new Random();
            return r.NextDouble() <= prob;
        }

        public static void SaveToXML(object S, Type t, string filename)
        {
            var file = new System.IO.StreamWriter(filename);
            try
            {
                var serializer = new XmlSerializer(t);
                serializer.Serialize(file, S);

                file.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static object LoadFromXML(string filename, Type t)
        {
            var file = new System.IO.StreamReader(filename);

            var S = new object();

            try
            {
                var deserializer = new XmlSerializer(t);
                S = deserializer.Deserialize(file);
                file.Close();
                return S;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        public static List<List<double>> MakeTableFromKKV(ref List<KeyValuePair<int, KeyValuePair<string, double>>> lkkv, int varcount, int rowcount)
        {
            var ans = new List<List<double>>();

            for (int i = 0; i < rowcount; i++)
            {
                var row = new List<double>(varcount);
                for (int j = 0; j < varcount; j++)
                    row.Add(0);
                ans.Add(row);
            }

            List<string> varlist = lkkv.Select(v => v.Value.Key).Distinct().OrderBy(v => v).ToList();
            List<int> rowlist = lkkv.Select(v => v.Key).Distinct().OrderBy(v => v).ToList();

            foreach (var kkv in lkkv.AsParallel())
            {
                int varid = varlist.IndexOf(kkv.Value.Key);
                int rowid = rowlist.IndexOf(kkv.Key);
                ans[rowid][varid] = kkv.Value.Value;
            }

            return ans;
        }

        public static List<List<double>> ScaleTableByStdev(ref List<List<double>> input)
        {
            var ans = new List<List<double>>(input);

            for (int i = 0; i < input[0].Count; i++)
            {
                var k = new List<double>();
                for (int j = 0; j < input.Count; j++)
                    k.Add(input[j][i]);
                var kmax = k.Max();
                var kavg = k.Average();
                var kstd = Utilities.StDev(k);

                for (int j = 0; j < input.Count; j++)
                    ans[j][i] = (k[j] - kavg) / kstd;
            }

            return ans;
        }
    }
}
