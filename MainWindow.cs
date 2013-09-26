using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using NAudio.Wave;
using Meta.Numerics.SignalProcessing;
using Meta.Numerics;
using System.Data.SQLite;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

using fftwlib;

using System.Xml.Serialization;
using System.Collections.Concurrent;

using HOLO;

namespace MuzCatalogr
{
    public partial class HOLO : Form
    {
        public HOLO()
        {
            InitializeComponent();
        }

        public List<Harvesting._song_profile> splist;

        public static Dictionary<int, double> MWPreCalc;

        public struct triplet
        {
            public int a, b;
            public double c;

            public triplet(int _a, int _b, double _c)
            {
                a = _a;
                b = _b;
                c = _c;
            }
        }

        /*
        public struct _song_profile
        {
            public string name;
            public string path;
            public long total_samples;
            public int snap_size;
            public int snap_count;
            public double length;

            public List<List<double>> fft_snaps;
            public List<List<double>> fft_smoothed_snaps;
            public List<List<double>> fft_downscaled_snaps;
            public List<List<double>> fft_downscaled_stdevs;

            public List<List<double>> pcm_snaps_l;
            public List<List<double>> pcm_snaps_r;

            public Dictionary<string, double> dic;
            public Dictionary<string, List<double>> sdic;
            
            public _song_profile(string _name, string _path)
            {
                name = _name;
                path = _path;

                total_samples = 0;
                length = 0;
                snap_size = 0;
                snap_count = 0;

                fft_snaps = new List<List<double>>();
                fft_smoothed_snaps = new List<List<double>>();
                fft_downscaled_snaps = new List<List<double>>();
                fft_downscaled_stdevs = new List<List<double>>();

                pcm_snaps_l = new List<List<double>>();
                pcm_snaps_r = new List<List<double>>();

                dic = new Dictionary<string, double>();
                sdic = new Dictionary<string, List<double>>();
            }

            public bool MakeSnaps(int _snapsize, int _snapcount, string _path, int smooth_size)
            {
                using (Mp3FileReader pcm = new Mp3FileReader(_path))
                {
                    int samplesDesired = Math.Max(_snapsize, 65536);
                    
                    snap_size = _snapsize;
                    snap_count = _snapcount;
                    total_samples = pcm.Length;
                    length = pcm.TotalTime.Seconds;
                    path = _path;
                    long blockscount = (long)(_snapcount * ((float)total_samples / 50000000));
                    //long blockscount = _snapcount;

                    if ((pcm.WaveFormat.SampleRate != 44100) || (pcm.WaveFormat.BitsPerSample != 16))
                        return false;

                    pcm.Seek(0, SeekOrigin.Begin);

                    for (int i = 0; i < blockscount; i++)
                    {
                        byte[] buffer = new byte[samplesDesired * 4];
                        short[] left = new short[samplesDesired];
                        short[] right = new short[samplesDesired];
                        double[] leftd = new double[samplesDesired];
                        double[] rightd = new double[samplesDesired];

                        int bytesRead = 0;

                        //for (int j = 0; j < 1; j++) ///////////
                        int seek_counter = 0;
                    //seek: pcm.Seek((i + 1) * (i + 1) * (i + 1) * blockscount * samplesDesired % (total_samples - samplesDesired), SeekOrigin.Begin);
                        
                    seek:   pcm.Seek((long)(0.10 * total_samples) + (long)(0.80 * total_samples * i / blockscount), SeekOrigin.Begin);
                        seek_counter++;

                        try
                        {
                            bytesRead = pcm.Read(buffer, 0, 4 * samplesDesired);
                        }
                        catch
                        {
                            return false;
                        }

                        int index = 0;
                        for (int sample = 0; sample < bytesRead / 4; sample++)
                        {
                            left[sample] = BitConverter.ToInt16(buffer, index); index += 2;
                            right[sample] = BitConverter.ToInt16(buffer, index); index += 2;
                        }

                        if (left.Average(t => Math.Abs(t)) == 0)
                            if (seek_counter > 2)
                                return false;
                            else
                                goto seek;

                        //snap_log10_energy.Add(Math.Log10(left.Average(t => Math.Abs(t))));

                        leftd = Normalize(left, left.Length).Take(_snapsize).ToArray();
                        rightd = Normalize(right, right.Length).Take(_snapsize).ToArray();

                        //alglib.fhtr1d(ref leftd, _snapsize);// (leftd, _snapsize);

                        //pcm_snaps_l.Add(leftd.ToList());
                        //pcm_snaps_r.Add(rightd.ToList());

                        FourierTransformer ft = new FourierTransformer(_snapsize);
                        var xxa = new Complex[leftd.Length];
                        
                        
                        if (MWPreCalc.Count == leftd.Length)
                            for (int j = 0; j < leftd.Length; j++)
                                xxa[j] = new Complex(leftd[j] * MWPreCalc[j], leftd[j] * MWPreCalc[j]);
                        else
                            return false;

                        var ftt = ft.Transform(xxa).Take((int)(leftd.Length / 2.5)); // TAKING ONLY LEFT HALF OF FFT RESULT
                        
                        List<double> pow_re_im = new List<double>();

                        ftt.ToList().ForEach(t => pow_re_im.Add(Math.Log(ComplexMath.Abs(t))));
                        
                        fft_snaps.Add(pow_re_im);
                         
                        //fft_snaps.Add(leftd.ToList());
                    }
                    pcm.Close();
                }

                return true;
            }
        */

        public double Correl(List<double> vec1, List<double> vec2, double avg1, double avg2)
        {
            if (vec1.Count != vec2.Count)
                return 0;

            double s = 0, divisor1 = 0, divisor2 = 0;
            double vec1avg = vec1.Average(), vec2avg = vec2.Average();

            for (int i = 0; i < vec1.Count; i++)
            {
                double diff1 = vec1[i] - vec1avg;
                double diff2 = vec2[i] - vec2avg;
                s += (diff1) * (diff2);
                divisor1 += (diff1) * (diff1);
                divisor2 += (diff2) * (diff2);
            }

            return s / Math.Sqrt(divisor1 * divisor2);
        }

        //nu
        public List<double> Normalize(List<double> vec)
        {
            var ans = new List<double>();

            double min = vec.Min(),
                max = vec.Max();

            foreach (var t in vec)
                ans.Add(t - min);

            return ans;
        }
                
        public double StDev(List<double> vec)
        {
            double mean = vec.Average();
            return Math.Sqrt(vec.Average(t => Math.Pow(t - mean, 2)));
        }

        public double Skewness(List<double> vec)
        {
            double stdev = StDev(vec);
            double mean = vec.Average();
            return vec.Average(t => (Math.Pow((t - mean) / stdev, 3)));
        }

        public double Kurtosis(List<double> vec)
        {
            double stdev = StDev(vec);
            double mean = vec.Average();
            return vec.Average(t => (Math.Pow((t - mean) / stdev, 4)));
        }

        public double Median(List<double> vec, double quantile = 0.5)
        {
            var tmp = vec.OrderBy(t => t).ToList();
            if (tmp.Count == 1)
                return tmp[0];
            if (quantile == 1)
                return tmp[tmp.Count - 1];
            if (tmp.Count % 2 == 0)
                return tmp[(int)(tmp.Count * quantile)];
            else
                return tmp[(int)((tmp.Count - 1) * quantile) + 1];
        }

        //nu
        public int MedianVec(List<List<double>> vvec, string method)
        {
            var slice = new List<double>();

            for (int i = 0; i < vvec.Count(); i++)
            {
                switch (method)
                {
                    case "mean":
                        slice.Add(vvec[i].Average());
                        break;
                    case "stdev":
                        slice.Add(StDev(vvec[i]));
                        break;
                    case "median":
                        slice.Add(Median(vvec[i]));
                        break;
                    default:
                        return -1;
                }
            }

            var v = Median(slice);

            switch (method)
            {
                case "mean":
                    return vvec.FindIndex(t => t.Average() == v);
                case "stdev":
                    return vvec.FindIndex(t => StDev(t) == v);
                case "median":
                    return vvec.FindIndex(t => Median(t) == v);
                default:
                    return -1;
            }
        }

        public int CountVecBends(List<double> vec)
        {
            double a, b, c;
            int counter = 0;
            for (int i = 1; i < vec.Count - 1; i++)
            {
                a = vec[i - 1];
                b = vec[i];
                c = vec[i + 1];
                if (((b < a) && (b < c)) || ((b > a) && (b > c)))
                    counter++;
            }

            return counter;
        }

        public List<double> CalcVecDRange(List<double> vec, int wsize)
        {
            var dr = new List<double>();
            var ans = new List<double>();
            for (int i = 0; i < vec.Count - wsize; i += wsize) {
                var v = vec.Skip(i).Take(wsize);
                dr.Add(v.Max() - v.Min()); }
            ans.Add(dr.Average());
            ans.Add(StDev(dr));
            //ans.Add(Skewness(dr));
            return ans;
        }

        /*public double CalcVecDRangeStdev(List<double> vec, int wsize)
        {
            var dr = new List<double>();
            for (int i = 0; i < vec.Count - wsize; i += wsize / 2)
            {
                var v = vec.Skip(i).Take(wsize);
                dr.Add(v.Max() - v.Min());
            }
            return StDev(dr);
        }*/

        public List<string> MakeFilesList(string startpath)
        {
            string[] files = Directory.GetFiles(startpath, "*.mp3", SearchOption.AllDirectories);

            return files.ToList();
        }

        public string Appdx(long i, long max_i)
        {
            string ans = "";
            long mi = (long)Math.Pow(10, Math.Floor(Math.Log10(max_i)));
            int a = (int)Math.Log10(max_i);
            int b = (i == 0 ? 0 : (int)Math.Log10(i));
            for (long j = a; j > b; j--)
                ans += "0";

            return ans + i.ToString();
        }

        public Dictionary<string, double> CalcAllStats(Harvesting._song_profile itm, bool gatherinmem)
        {
            int resolution = (int)(1 * Math.Sqrt(itm.snap_size));

            var ans = new Dictionary<string, double>();

            //PostProcessSnaps(itm, 5 * (int)Math.Sqrt((double)nUD2.Value));
            
            var fs = new List<int>();

            for (int i = 8; i < itm.fft_snaps[0].Count; i += resolution )
            {
                fs.Add(i);
                resolution = (int)(resolution * 1.4);
            }

            var v = new List<double>();
            foreach (var f in fs)
            {
                v.Clear();
                for (int i = 0; i < itm.fft_snaps.Count; i++) {
                    v.Add(itm.fft_snaps[i][f]); }

                var counts = new int[10];

                for (int i = 0; i < 10; i++)
                    counts[i] = 0;

                for (int i = 0; i < v.Count; i++)
                {
                    int k = 0;
                    if      (v[i] < 5)       k = 0;
                    else if (v[i] < 7.0)     k = 1;
                    else if (v[i] < 8.8)     k = 2;
                    else if (v[i] < 10.5)    k = 3;
                    else if (v[i] < 11.3)    k = 4;
                    else if (v[i] < 12.2)    k = 5;
                    else if (v[i] < 12.7)    k = 6;
                    else if (v[i] < 13.2)    k = 7;
                    else if (v[i] < 14.0)    k = 8;
                    else if (v[i] >= 14.0)   k = 9;

                    counts[k]++;
                }

                for (int i = 0; i < 10; i++)
                    ans.Add("f" + f.ToString() + "v" + i.ToString(), (int)(100 * (double)counts[i] / (double)v.Count));
            }

            if (gatherinmem)
            {
            
            }            

            itm.fft_snaps.Clear();
            
            return ans;
        }

        public Dictionary<string, double> CalcAllStats2(Harvesting._song_profile itm, ref ClusterAnalysis.CentroidConfig CC, float downscale_rate)
        {
            var ans = new Dictionary<string, double>();

            //PostProcessSnaps(itm, 3 * (int)Math.Sqrt((double)nUD2.Value));

            for (int i = 0; i < CC.K; i++)
                ans.Add("f0v" + i.ToString(), 0);

            foreach(var snap in itm.fft_snaps)
            {
                int centroid_num = ClusterAnalysis.ChooseCentroid(ref CC, Utilities.DownscaleList(snap, downscale_rate), 3);
                ans["f0v" + centroid_num.ToString()] += 100 / (float)itm.fft_snaps.Count;
            }
            
            return ans;
        }

        public Dictionary<string, double> CalcAllStats2D(Harvesting._song_profile itm, ref ClusterAnalysis.CentroidConfig CC, float downscale_rate)
        {
            var ans = new Dictionary<string, double>();

            //PostProcessSnaps(itm, 2 * (int)Math.Sqrt((double)nUD2.Value));

            for (int i = 0; i < CC.K; i++)
                for (int j = 0; j < CC.K; j++)
                    ans.Add("f" + i.ToString() + "v" + j.ToString(), 0);

            var sequence = new List<int>();

            for (int k = 0; k < itm.fft_snaps.Count; k++)
                sequence.Add(ClusterAnalysis.ChooseCentroid(ref CC, Utilities.DownscaleList(itm.fft_snaps[k], downscale_rate), 1));

            if (sequence.Count > 1)
                for (int i = 1; i < sequence.Count; i++)
                    ans["f" + sequence[i - 1].ToString() + "v" + sequence[i].ToString()] += 100 / (float)itm.fft_snaps.Count;

            return ans;
        }

        public Dictionary<string, double> CalcAllStats3D(Harvesting._song_profile itm, ref ClusterAnalysis.CentroidConfig CC, float downscale_rate, double k_factor = 24)
        {
            var ans = new Dictionary<string, double>();

            for (int i = 0; i < CC.K; i++)
                for (int j = 0; j < CC.K; j++)
                    ans.Add("f" + i.ToString() + "v" + j.ToString(), 0);

            List<double> prev_dist = null;
            double K = (double)(CC.K * itm.fft_snaps.Count);

            for (int k = 0; k < itm.fft_snaps.Count; k++)
            {
                var distances = ClusterAnalysis.CentroidDistances(ref CC, Utilities.DownscaleList(itm.fft_snaps[k], downscale_rate));
                var distsum = distances.Sum();
                var step2 = distances.Select(v => Math.Pow(1 - v / distsum, k_factor)).ToList();
                var s2max = step2.Max();
                step2 = step2.Select(v => (v >= (0.65 * s2max) ? v / s2max : 0)).ToList();

                if (k > 0)
                {
                    for (int i = 0; i < prev_dist.Count; i++)
                        for (int j = 0; j < step2.Count; j++)
                            ans["f" + i.ToString() + "v" + j.ToString()] += prev_dist[i] * step2[j] / K;
                }
                prev_dist = step2;
            }            

            return ans;
        }

        public void PostProcessSnaps(Harvesting._song_profile sp, int mwsize)
        {
            double mw = 1.0 / mwsize;
            for (int j = 0; j < sp.fft_snaps.Count; j++) {
                var sn = new double[sp.fft_snaps[j].Count - mwsize];
                double t = 0;
                for (int i = 0; i < sp.fft_snaps[j].Count; i++) {
                    t += sp.fft_snaps[j][i] * mw;
                    if (i >= mwsize) {
                        sn[i - mwsize] = t;
                        t -= sp.fft_snaps[j][i - mwsize] * mw; 
                    } 
                }
                sp.fft_snaps[j] = sn.ToList();

            }                        
        }

        public delegate void UpdateConsole(string msg);

        public UpdateConsole myDelegate;

        public void AddConsoleMsg(string msg)
        {
            if (lbConsole.Items.Count % 36 == 0)
                lbConsole.Items.Clear();

            lbConsole.Items.Add(msg);
        }

        [Serializable]
        [XmlType(TypeName = "KeyValuePairStringDouble")]
        public struct KeyValuePair<K, V>
        {
            public K Key
            { get; set; }

            public V Value
            { get; set; }
        }

        public struct TRecord
        {
            public string path;
            public int id;
            public List<KeyValuePair<string, double>> dic;
        };

        public ConcurrentBag<TRecord> Records;

        public HOLOProperties.HarvestProps HP;
        CancellationTokenSource cts;
        CancellationToken ct;

        List<string> FilesList;
        bool OptionsChanged;

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure want to (re)create the database?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            if (cb_CollectSnaps.Checked)
            {
                SaveHOLOptions("holoptions_tmp.xml", false);
                HP = LoadHOLOptions("holoptions_tmp.xml", false);
            }
            else
            {
                if (OptionsChanged)
                    HP = LoadHOLOptions("holoptions_custom.xml", false);
                else
                    HP = LoadHOLOptions("holoptions.xml", false);
            }

            lbConsole.Items.Clear();
            timer1.Enabled = true;

            var doConsecutive = cb_Consecutive.Checked;
            var doCollectSnaps = cb_CollectSnaps.Checked;

            var filescount = 0;
            if (cbLimList.Checked)
                filescount = (int)nUDListSize.Value;

            var MDB = new ManageDB();
            List<string> fl;

            if ((FilesList != null) || (FilesList.Count == (int)nUDListSize.Value))
                fl = FilesList;
            else
            {
                RefreshFileList(textBox1.Text, filescount, tv1, '\\');
                fl = FilesList;
            }
            
            lbConsole.Items.Add("Found files: " + fl.Count);

            MDB.CreateDB("test.db");

            splist = new List<Harvesting._song_profile>();
            splist.Clear();

            int line_counter = 0;

            Harvesting.Init(HP.SnapSize);

            var SnapsCollection = new List<List<double>>();
            var ClusterConfig = ClusterAnalysis.LoadCentroids(HP.CentroidConfigFile);
            //var VMatrix = (List<List<double>>)Utilities.LoadFromXML("vmatrix.xml", typeof(List<List<double>>));
            
            var lasttick = DateTime.Now;
            var firsttick = DateTime.Now;

            int n_threads = (int)nUDThreads.Value;

            lbConsole.Items.Clear();

            myDelegate = new UpdateConsole(AddConsoleMsg);

            var options = new ParallelOptions { MaxDegreeOfParallelism = n_threads };
            cts = new CancellationTokenSource();
            options.CancellationToken = cts.Token;
            ct = cts.Token;

            float DownscaleRate = HP.DownScaleRate;
            int ClustersCount = HP.ClusteringProperties.ClustersCount;
                        
            Records = new ConcurrentBag<TRecord>();
            
            Task.Factory.StartNew(() =>
                {                    
                    try
                    {
                        Parallel.ForEach(fl, options, f =>
                        {
                            options.CancellationToken.ThrowIfCancellationRequested();

                            //s_count = fl.IndexOf(f);

                            var itm = new Harvesting._song_profile(Path.GetFileName(f), f);

                            string proceed;
                            //proceed = Harvesting.MakeSnaps3(ref itm, HP.SnapSize, HP.SnapCount, f, HP.OverlapRate, HP.FreqCutoffRate);
                            proceed = Harvesting.MakeSnaps2(ref itm, HP.SnapSize, HP.SnapCount, f, HP.OverlapRate, HP.FreqCutoffRate, HP.DoFullNormalize == 1);
                            PostProcessSnaps(itm, 1 * (int)Math.Sqrt(HP.SnapSize));
                            
                            var d = new Dictionary<string, double>();

                            if (proceed == "OK")
                            {
                                if (doCollectSnaps)
                                {                                    
                                    itm.fft_snaps.ForEach(snap => SnapsCollection.Add(Utilities.DownscaleList(snap, DownscaleRate)));
                                    //itm.fft_snaps.ForEach(snap => SnapsCollection.Add(snap));
                                    d.Add("f0v0", 0);
                                }
                                else
                                {
                                    //Harvesting.MakePCASnaps(ref itm, ref VMatrix);

                                    if (HP.StatsCalcMethod == "Classic 2D")
                                        d = CalcAllStats(itm, false);
                                    else if (HP.StatsCalcMethod == "Clustered 1D")
                                        d = CalcAllStats2(itm, ref ClusterConfig, DownscaleRate);
                                    else if (HP.StatsCalcMethod == "Clustered 2D")
                                        d = CalcAllStats3D(itm, ref ClusterConfig, DownscaleRate);

                                    //MDB.OpenDB();
                                    MDB.PutNewRecord(fl.IndexOf(f).ToString(), itm.name, itm.path, d, "test.db", "");
                                    
                                    /*var r = new TRecord();
                                    var kv = new KeyValuePair<string, double>();
                                    r.dic = new List<KeyValuePair<string, double>>();
                                    d.ToList().Where(v => v.Value > 0).ToList().ForEach(v => { kv.Key = v.Key; kv.Value = v.Value; r.dic.Add(kv); });
                                    r.id = s_count;
                                    r.path = itm.path;
                                    Records.Add(r);*/
                                }
                            }
                            else
                            {
                                this.Invoke(myDelegate, new Object[] { "ERROR: file \"" + itm.name + "\" says \"" + proceed + "\"" });
                            }

                            line_counter++;

                            var s_per_hour = (int)(line_counter / (DateTime.Now - firsttick).TotalHours);
                            this.Invoke(myDelegate, new Object[] { "[" + DateTime.Now.ToLongTimeString() + " : " + line_counter + " of " + fl.Count + ": " + s_per_hour.ToString() + "sph]: " + f });
                        });
                    }
                    catch (Exception E)
                    {                        
                        MDB.CloseDB();
                        ManageDB.PostprocessDB();
                        this.Invoke(myDelegate, new Object[] { "Database postprocessed." });
                        this.Invoke(myDelegate, new Object[] { "HARVESTING CANCELLED. Final message is: \"" + E.Message + "\"" });
                        timer1.Enabled = false;
                        
                        return;
                    }

                    this.Invoke(myDelegate, new Object[] { "All items done." });

                    if (doCollectSnaps)
                    {
                        this.Invoke(myDelegate, new Object[] { "Collection contains " + SnapsCollection.Count.ToString() + " snaps." });

                        //SnapsCollection = ClusterAnalysis.PCAnalysis(ref SnapsCollection, ClustersCount);

                        if (HP.ClusteringProperties.CleanupOutliers == 1)
                        {
                            double avg_d = ClusterAnalysis.AverageDistance(SnapsCollection);
                            this.Invoke(myDelegate, new Object[] { "Average distance is: " + avg_d });

                            var eps = HP.ClusteringProperties.NeigborhoodDistance * avg_d / 100;
                            this.Invoke(myDelegate, new Object[] { "Total snaps removed: " + ClusterAnalysis.CleanupByEpsilon(SnapsCollection, eps, HP.ClusteringProperties.MinNeighborhoodPerc / 1000) });
                            this.Invoke(myDelegate, new Object[] { "Collection contains " + SnapsCollection.Count.ToString() + " snaps." });
                        }
                                                
                        this.Invoke(myDelegate, new Object[] { "Performing cluster analysis" });
                        
                        if (HP.ClusteringProperties.ClustersMethod == "K-Means")
                            ClusterAnalysis.KMeansAnalysis(ref SnapsCollection, ClustersCount, 5);
                        else if (HP.ClusteringProperties.ClustersMethod == "Greedy")
                            ClusterAnalysis.GreedyAnalysis(ref SnapsCollection, ClustersCount, 5, HP.ClusteringProperties.CentroidSearchMethod);
                        this.Invoke(myDelegate, new Object[] { "Cluster analysis done." });
                    }
                    else
                    {
                        MDB.CloseDB();
                        ManageDB.PostprocessDB();
                        Utilities.ExecuteCommandAsync("copy test.db \"" + tbDBFile.Text + "\"");
                        //Utilities.SaveToXML(Records.ToList(), typeof(List<TRecord>), "db2.xml");
                    }

                    timer1.Enabled = false;
                    
                    //button3.Enabled = true;
                    //pbBusy.Visible = false;
                });
        }

        private void FulfillDBList(ComboBox C, ListBox LB = null)
        {
            string[] files = Directory.GetFiles(".\\", "*.db", SearchOption.TopDirectoryOnly);

            if (C != null)
                C.Items.Clear();
            if (LB != null)
                LB.Items.Clear();

            foreach (var f in files)
                if ((string)f != ".\\test_winners.db")
                {
                    if (C != null)
                        C.Items.Add(f);
                    if (LB != null)
                        LB.Items.Add(f);
                }
            if (C != null)
                C.SelectedItem = ".\\test.db";
        }

        private void FulfillCombo(ComboBox C, string path, string template, string selected = "")
        {
            string[] files = Directory.GetFiles(path, template, SearchOption.TopDirectoryOnly);

            C.Items.Clear();

            foreach (var f in files)
                C.Items.Add(f);

            if (selected != "")
                C.SelectedItem = selected;
        }

        private void FulfillTreeView(TreeView tv, List<string> file_list, char pathSeparator)
        {
            file_list = file_list.OrderByDescending(v => v.Count(w => w == pathSeparator)).ThenBy(v => v).ToList();

            tv.Nodes.Clear();
            tv.CheckBoxes = true;

            TreeNode lastNode = null;
            string subPathAgg;
            foreach (var path in file_list)
            {
                subPathAgg = string.Empty;
                foreach (string subPath in path.Split(pathSeparator))
                {
                    subPathAgg += subPath + pathSeparator;
                    TreeNode[] nodes = tv.Nodes.Find(subPathAgg, true);
                    if (nodes.Length == 0)
                        if (lastNode == null)
                            lastNode = tv.Nodes.Add(subPathAgg, subPath);
                        else
                            lastNode = lastNode.Nodes.Add(subPathAgg, subPath);
                    else
                        lastNode = nodes[0];
                }
            }
        }

        private void RefreshFileList(string path, int filescount, TreeView tv, char delimiter)
        {
            FilesList = ManageDB.CreateFileList(path, filescount);
            lOptionsInfo.Text = FilesList.Count + " files to be processed.";
            //FulfillTreeView(tv, FilesList, delimiter);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (fbrDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBox1.Text = fbrDlg.SelectedPath;
            else
                return;

            pbBusy.Visible = true;
            RefreshFileList(fbrDlg.SelectedPath, cbLimList.Checked ? (int)nUDListSize.Value : 0, tv1, '\\');
            pbBusy.Visible = false;
        }

        private void nUD2_ValueChanged(object sender, EventArgs e)
        {
            lbl_sec_snap.Text = ((float)((int)(100 * nUD2.Value / 44100)) / 100).ToString();
            lbl_sec_tot.Text = ((float)((int)(100 * nUD.Value * nUD2.Value / 44100)) / 100).ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            nUD2_ValueChanged(sender, e);

            FulfillDBList(comboBox1, lbDBlist);
            FulfillDBList(null, lbDBDisco);
            FulfillCombo(ho_cbCentroidFile, ".\\", "centroid*.xml", ".\\centroids.xml");

            lCPUs.Text = "Current CPU count = " + System.Environment.ProcessorCount;

            if (webBrowser1.IsOffline)
            {
                webBrowser1.Visible = false;
                pbHelp.Visible = true;
            }
        }

        private void nUD_ValueChanged(object sender, EventArgs e)
        {
            nUD2_ValueChanged(sender, e);
        }

        public void OpenSnapDB(string dbpath, string searchfilter, bool openwhole, bool keeptgt = false, DataGridView dGV = null, DataGridView dGV_tgt = null, DataGridView dGV_pls = null)
        {
            var MDB = new ManageDB();

            string sf;

            if (!openwhole)
            {
                if (searchfilter.Length > 0)
                    sf = " where path like '%" + searchfilter.Replace(";", "%").Replace(" ", "%") + "%'";
                else sf = "";
            }
            else
                sf = " limit 100";

            MDB.ConnectDB(dbpath);

            var cmd = MDB.DBConnection.CreateCommand();
            cmd.CommandText = "select * from main" + sf;
            var data = cmd.ExecuteReader();

            dGV.Columns.Clear();
            if (!keeptgt)
                dGV_tgt.Columns.Clear();
            dGV_pls.Columns.Clear();

            foreach (var c in data.GetValues())
            {
                dGV.Columns.Add("dg_" + (string)c, (string)c);
                if (!keeptgt)
                    dGV_tgt.Columns.Add("dgt_" + (string)c, (string)c);
                dGV_pls.Columns.Add("dgp_" + (string)c, (string)c);

                if ((string)c == "path")
                {
                    dGV.Columns["dg_path"].Width = 500;
                    if (!keeptgt)
                        dGV_tgt.Columns["dgt_path"].Width = 500;
                    dGV_pls.Columns["dgp_path"].Width = 500;
                }
            }

            dGV_pls.Columns.Add("dgp_dist", "Distance");
            dGV_pls.Columns["dgp_dist"].ValueType = typeof(double);

            while (data.HasRows)
            {
                data.Read();

                List<string> r = new List<string>();

                for (int i = 0; i < data.FieldCount; i++)
                    r.Add(data[i].ToString());

                dGV.Rows.Add(r.ToArray());
            }

            MDB.CloseDB();
        }

        private void dGV_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var c = new DataGridViewCellCollection(dGV.Rows[e.RowIndex]);
            c = dGV.Rows[e.RowIndex].Cells;

            var cs = new List<string>();

            for (int i = 0; i < c.Count; i++)
                cs.Add(c[i].Value.ToString());

            dGV_tgt.Rows.Add(cs.ToArray());
        }

        private List<List<string>> PlaylistRequest(List<string> idList, string database)
        {
            var db = new SQLiteConnection();
            db.ConnectionString = "Data Source=" + database + ";";
            db.Open();

            var cmd = db.CreateCommand();

            string maxthr = (nUD_MaxThr.Value).ToString();
            string tgtid = String.Join(", ", idList.ToArray());

            string s = "select distinct a.id, a.name, a.path, b.distance from main a, (";
            s += "select b.id, sum((coalesce(a.statvalue,0) - coalesce(b.statvalue,0))*(coalesce(a.statvalue,0) - coalesce(b.statvalue,0))/c2.value) as distance ";
            s += ", 10000*avg((coalesce(a.statvalue,0) - coalesce(b.statvalue,0))*(coalesce(a.statvalue,0) - coalesce(b.statvalue,0))/(c2.value*c2.value)) as meandiff ";
            s += "from stats a, stats b, ";
            s += "feature c2 ";
            s += "where ";
            s += "a.id in (" + tgtid + ") ";
            s += "and coalesce(a.statname,'n') = c2.statname ";
            //s += "and coalesce(a.statname,'n') = s1.statname ";
            s += "and c2.feature = 'max_min' ";
            s += "and coalesce(a.statname,'n') = coalesce(b.statname,'n') ";
            s += "and coalesce(a.statvalue,0) <= coalesce(b.statvalue,0) + " + maxthr + "*c2.value/100 and coalesce(a.statvalue,0) >= coalesce(b.statvalue,0) - " + maxthr + "*c2.value/100 ";

            s += "group by b.id ";
            s += "having ";
            s += "meandiff <= " + ((int)(nUD_MaxMean.Value * nUD_MaxMean.Value)).ToString() + " ";

            if (cbOrderBy.Checked)
                s += "order by distance ";
            s += "limit " + ((int)nUD_tracks.Value).ToString();
            s += ") b where a.id = b.id;";// );";

            cmd.CommandText = s;
            var data = cmd.ExecuteReader();
            var result = new List<List<string>>();

            while (data.HasRows)
            {
                data.Read();
                List<string> r = new List<string>();
                for (int i = 0; i < data.FieldCount; i++)
                    r.Add(data[i].ToString());
                result.Add(r);                
            }

            return result;
        }

        private void RenderRequest(DataGridView dgv, List<List<string>> result)
        {
            dgv.Invoke(new Action(() =>
            {
                foreach (var row in result)
                {
                    //tb1.Invoke(new Action(() => tb1.Text = theResultOfYourServiceCall));
                    dgv.Rows.Add(row.ToArray());
                    //dgv.Rows.Add(row.ToArray());
                }
            }));
        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (dGV_tgt.RowCount < 1)
            {
                MessageBox.Show("No target tracks selected!");
                return;
            }

            Application.DoEvents();
            var idList = new List<string>();
            for (int i = 0; i < dGV_tgt.RowCount; i++)
            {
                idList.Add(dGV_tgt.Rows[i].Cells["dgt_id"].Value.ToString());
            }

            if (MessageBox.Show("Question", "Do you wish to proceed?", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            pbWait.Visible = true;
            button26.Enabled = false;
            var result = new List<List<string>>();
            string database = (string)comboBox1.SelectedItem;
            var task = Task.Factory.StartNew(() =>
              {
                result = PlaylistRequest(idList, database);
              });

            task.ContinueWith(task2 => {
                dGV_pls.Invoke(new Action(() => { dGV_pls.Rows.Clear(); }));
                RenderRequest(dGV_pls, result);
                pbWait.Invoke(new Action(() => { pbWait.Visible = false; }));
                button26.Invoke(new Action(() => { button26.Enabled = true; }));
            });
        }        

        private void button6_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            string database = (string)comboBox1.SelectedItem;            
            OpenSnapDB(database, "", false, false, dGV, dGV_tgt, dGV_pls);            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            dGV_tgt.Rows.Clear();
        }

        private void dGV_tgt_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string filename = dGV_tgt.Rows[e.RowIndex].Cells["dgt_path"].Value.ToString();
            Utilities.ExecuteCommandAsync("explorer \"" + filename + "\"");
        }

        private void dGV_pls_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string filename = dGV_pls.Rows[e.RowIndex].Cells["dgp_path"].Value.ToString();
            Utilities.ExecuteCommandAsync("explorer \"" + filename + "\"");
        }

        private void cbEnableFilter_CheckedChanged(object sender, EventArgs e)
        {
            bool keeptgt = false;
            if (dGV_tgt.Columns.Count > 1)
                keeptgt = true;

            if (cbEnableFilter.Checked)
                OpenSnapDB((string)comboBox1.SelectedItem, tbFilterTracks.Text, false, keeptgt, dGV, dGV_tgt, dGV_pls);
            else
                OpenSnapDB((string)comboBox1.SelectedItem, "", false, keeptgt, dGV, dGV_tgt, dGV_pls);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            cbEnableFilter_CheckedChanged(sender, e);
        }

        private void SaveAndOpenPlaylist(List<string> playlist)
        {
            var s = new StreamWriter(@"tmp_playlist.m3u8", false);
            foreach (var p in playlist)
            {
                s.WriteLine(p);
            }
            s.Close();
            Utilities.ExecuteCommandAsync("explorer \"tmp_playlist.m3u8\"");
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            List<string> playlist = new List<string>();
            
            for (int i = 0; i < dGV_pls.Rows.Count; i++)
            {
                playlist.Add(dGV_pls.Rows[i].Cells["dgp_path"].Value.ToString());
            }

            SaveAndOpenPlaylist(playlist);
        }

        private void cbOverStats_CheckedChanged(object sender, EventArgs e)
        {            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
            if (lbDBlist.SelectedIndex >= 0)
            {
                OpenSnapDB((string)lbDBlist.SelectedItem, "", true, false, dGV, dGV_tgt, dGV_pls);
                comboBox1.SelectedItem = lbDBlist.SelectedItem;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(2);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Utilities.ExecuteCommandAsync("explorer \"tmp_playlist.m3u8\"");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ManageDB.PostprocessDB();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string tgt_id = dGV_pls.SelectedRows[0].Cells["dgp_id"].Value.ToString();
            string win_id = dGV_tgt.Rows[0].Cells["dgt_id"].Value.ToString();
            label12.Text = tgt_id + "->" + win_id + ": GRM";

            PutScoreToDB(tgt_id, win_id, "GRM");
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string tgt_id = dGV_pls.SelectedRows[0].Cells["dgp_id"].Value.ToString();
            string win_id = dGV_tgt.Rows[0].Cells["dgt_id"].Value.ToString();
            label12.Text = tgt_id + "->" + win_id + ": GTM";

            PutScoreToDB(tgt_id, win_id, "GTM");
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string tgt_id = dGV_pls.SelectedRows[0].Cells["dgp_id"].Value.ToString();
            string win_id = dGV_tgt.Rows[0].Cells["dgt_id"].Value.ToString();
            label12.Text = tgt_id + "->" + win_id + ": GM";

            PutScoreToDB(tgt_id, win_id, "GM");
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            string tgt_id = dGV_pls.SelectedRows[0].Cells["dgp_id"].Value.ToString();
            string win_id = dGV_tgt.Rows[0].Cells["dgt_id"].Value.ToString();
            label12.Text = tgt_id + "->" + win_id + ": TM";

            PutScoreToDB(tgt_id, win_id, "TM");
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            string tgt_id = dGV_pls.SelectedRows[0].Cells["dgp_id"].Value.ToString();
            string win_id = dGV_tgt.Rows[0].Cells["dgt_id"].Value.ToString();
            label12.Text = tgt_id + "->" + win_id + ": WTF";

            PutScoreToDB(tgt_id, win_id, "WTF");
        }

        public void PutScoreToDB(string id_tgt, string id_winner, string win_type)
        {
            var db = new SQLiteConnection();
            db.ConnectionString = "Data Source=test_winners.db;";
            db.Open();

            var cmd = db.CreateCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS winners (id_tgt string, id_winner string, win_type string);";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO winners VALUES ('" + id_tgt + "', '" + id_winner + "', '" + win_type + "');";
            cmd.ExecuteNonQuery();

            db.Close();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Utilities.ExecuteCommandSync("sqlite3.exe < select_winners.sql");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            lbSplist.Items.Clear();

            if (splist.Count == 0)
                return;

            foreach (var s in splist)
            {
                lbSplist.Items.Add(s.path);
            }

            clbData.Items.Clear();

            foreach (var sn in splist[0].sdic.Keys)
            {
                clbData.Items.Add(sn);
            }
        }

        public List<double> MWMedian(List<double> vec, int wsize, double quant)
        {
            var ans = new List<double>();

            int sz = vec.Count;

            for (int i = 0; i < sz; i++)
            {
                //ans.Add(Median(vec.GetRange(Math.Max(i - (int)(wsize / 2), 0), Math.Min(wsize, sz - i)), quant));
                ans.Add(vec.GetRange(Math.Max(i - (int)(wsize / 2), 0), Math.Min(wsize, sz - i)).Average());
            }

            return ans;
        }

        public void UpdateChart()
        {
            chrt.Series.Clear();

            foreach (var si in lbSplist.SelectedItems)
            {
                var itm = splist.Where(t => t.path == (string)si).FirstOrDefault();

                foreach (var sn in clbData.CheckedItems)
                {
                    var srs = chrt.Series.Add(itm.name + " " + (string)sn);
                    srs.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;

                    if (checkBox1.Checked)
                    {
                        chrt.ChartAreas[0].AxisX.IsLogarithmic = true;
                        chrt.ChartAreas[0].AxisX.Minimum = 10;
                    }
                    else
                    {
                        chrt.ChartAreas[0].AxisX.IsLogarithmic = false;
                        chrt.ChartAreas[0].AxisX.Minimum = 0;
                    }

                    var vv = new List<double>();

                    if (cbMedian.Checked)
                        vv = MWMedian(itm.sdic[(string)sn], (int)nudMedian.Value, 0.5);
                    else
                        vv = itm.sdic[(string)sn];

                    for (int i = 0; i < vv.Count; i++)
                        //if (Math.Sqrt(i) % 1 == 0)
                            srs.Points.AddXY(i + 1, vv[i]);// - vv[0]);
                }
            }
        }

        private void lbSplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void clbData_MouseClick(object sender, MouseEventArgs e)
        {
            UpdateChart();
        }

        private void cbMedian_CheckedChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void nudMedian_ValueChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateChart();
        }

        public double linterp(double p, double _min, double _max)
        {
            return (1 - p) * _min + p * _max;
        }

        public Color cinterp(double p, Color cmin, Color cmax, int alpha = 255)
        {
            var c = new Color();
            c = Color.FromArgb(alpha,
                               (int)(linterp(p, cmin.R, cmax.R)),
                               (int)(linterp(p, cmin.G, cmax.G)),
                               (int)(linterp(p, cmin.B, cmax.B)));
            return c;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (dGV_tgt.RowCount < 1)
            {
                MessageBox.Show("No target tracks selected!");
                return;
            }

            var idSurf = dGV_tgt.Rows[0].Cells["dgt_id"].Value.ToString();

            var db = new SQLiteConnection();
            db.ConnectionString = "Data Source=" + (string)comboBox1.SelectedItem + ";";
            db.Open();

            var cmd = db.CreateCommand();

            var s = "select * from stats where id = " + idSurf + "; ";

            cmd.CommandText = s;

            var data = cmd.ExecuteReader();

            var rs = new List<List<string>>();

            while (data.HasRows)
            {
                data.Read();
                List<string> r = new List<string>();
                for (int i = 0; i < data.FieldCount; i++)
                    r.Add(data[i].ToString());
                rs.Add(r);
            }

            var tr = new List<triplet>();
            
            foreach (var t in rs)
            if (t[0] != "")
            {
                double stat = Double.Parse(t[4]);
                var freq = int.Parse(t[1].Split('v')[0].Substring(1));
                var vol = int.Parse(t[1].Split('v')[1]);

                tr.Add(new triplet(freq, vol, stat));
            }

            chrt.Series.Clear();
            //chrt.ChartAreas[0].AxisY.Maximum = 25;
            chrt.ChartAreas[0].AxisY.Minimum = 0;
            chrt.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chrt.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            
            int fr = 1;
            int cfr = 0;
            foreach (var tt in tr.OrderBy(y => y.a))
            {
                if (tt.c == 0)
                    continue;

                if (tt.a != cfr)
                {
                    fr++;
                    cfr = tt.a;
                }
                var srs = chrt.Series.Add("f" + tt.a + "v" + tt.b);
                srs.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
                srs.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Square;
                srs.MarkerSize = 24;                

                double ttc = tt.c / tr.Max(r => r.c);
                double mid = 0.3;
                if (ttc <= mid) {
                    srs.MarkerColor = cinterp(ttc / mid,                Color.LightGreen,   Color.Yellow,   127); }
                else {
                    srs.MarkerColor = cinterp((ttc - mid) / (1 - mid),  Color.Yellow,       Color.Red,      127); }
                srs.Points.AddXY(fr, tt.b);
            }

            tabControl1.SelectTab(4);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            FulfillDBList(comboBox1);
        }

        private void cb_CollectSnaps_CheckedChanged(object sender, EventArgs e)
        {
            groupBox4.Visible = cb_CollectSnaps.Checked;
        }

        private void SaveHOLOptions(string file, bool defaultfile)
        {
            var HO = new HOLOProperties.HarvestProps();

            HO.AmplitudeFunc = (string)ho_lbAmplFunc.SelectedItem;
            HO.CentroidConfigFile = (string)ho_cbCentroidFile.SelectedItem;
            HO.ClustersFuzz = (int)ho_nUDChoiceFuzz.Value;
            HO.DoFullNormalize = ho_cbFullNormalize.Checked ? 1 : 0;
            HO.DownScaleRate = (int)nUD_Downscale.Value;
            HO.FreqCutoffRate = (float)ho_nUDFreqCutoff.Value;
            HO.OverlapRate = (int)ho_nUDOverlapping.Value;
            HO.SnapCount = (int)nUD.Value;
            HO.SnapSize = (int)nUD2.Value;
            HO.StatsCalcMethod = (string)ho_lbStatsCalc.SelectedItem;

            HO.ClusteringProperties = new HOLOProperties.ClusterProps();

            HO.ClusteringProperties.CentroidSearchMethod = (string)ho_cbCentroidSearch.SelectedItem;
            HO.ClusteringProperties.CleanupOutliers = ho_cbCleanOutliers.Checked ? 1 : 0;
            HO.ClusteringProperties.ClustersCount = (int)nUD_Clusters.Value;
            HO.ClusteringProperties.ClustersMethod = (string)ho_cbClusterMethod.SelectedItem;
            HO.ClusteringProperties.MinNeighborhoodPerc = (float)ho_nUDMinNeighPerc.Value;
            HO.ClusteringProperties.NeigborhoodDistance = (float)ho_nUDNeighDist.Value;

            if (defaultfile)
                Utilities.SaveToXML(HO, typeof(HOLOProperties.HarvestProps), "holoptions.xml");
            else
                Utilities.SaveToXML(HO, typeof(HOLOProperties.HarvestProps), file);
        }

        private HOLOProperties.HarvestProps LoadHOLOptions(string file, bool defaultfile)
        {
            HOLOProperties.HarvestProps HO;
            if (defaultfile)
                HO = (HOLOProperties.HarvestProps)Utilities.LoadFromXML("holoptions.xml", typeof(HOLOProperties.HarvestProps));
            else
                HO = (HOLOProperties.HarvestProps)Utilities.LoadFromXML(file, typeof(HOLOProperties.HarvestProps));

            ho_lbAmplFunc.SelectedItem = HO.AmplitudeFunc;
            ho_cbCentroidFile.SelectedItem = HO.CentroidConfigFile;
            ho_nUDChoiceFuzz.Value = HO.ClustersFuzz;
            ho_cbFullNormalize.Checked = HO.DoFullNormalize == 1;
            nUD_Downscale.Value = HO.DownScaleRate;
            ho_nUDFreqCutoff.Value = (decimal)HO.FreqCutoffRate;
            ho_nUDOverlapping.Value = HO.OverlapRate;
            nUD.Value = HO.SnapCount;
            nUD2.Value = HO.SnapSize;
            ho_lbStatsCalc.SelectedItem = HO.StatsCalcMethod;

            ho_cbCentroidSearch.SelectedItem = HO.ClusteringProperties.CentroidSearchMethod;
            ho_cbCleanOutliers.Checked = HO.ClusteringProperties.CleanupOutliers == 1;
            nUD_Clusters.Value = HO.ClusteringProperties.ClustersCount;
            ho_cbClusterMethod.SelectedItem = HO.ClusteringProperties.ClustersMethod;
            ho_nUDMinNeighPerc.Value = (decimal)HO.ClusteringProperties.MinNeighborhoodPerc;
            ho_nUDNeighDist.Value = (decimal)HO.ClusteringProperties.NeigborhoodDistance;

            return HO;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure want to save all options to the holoptions_custom.xml?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            SaveHOLOptions("holoptions_custom.xml", false);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure want to load all options from the holoptions_custom.xml?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            HP = LoadHOLOptions("holoptions_custom.xml", false);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure want to load default options?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            HP = LoadHOLOptions("holoptions.xml", true);
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            Utilities.ExecuteCommandSync("sqlite3.exe < postprocess.sql");
        }

        private void ho_cbCleanOutliers_CheckedChanged(object sender, EventArgs e)
        {
            groupBox6.Visible = ho_cbCleanOutliers.Checked;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure want to interrupt harvesting?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                this.Invoke(myDelegate, new Object[] { "CANCEL request has been made. Please wait for safe cancelling to be finished." });
                cts.Cancel();
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            pbBusy.Visible = true;
            RefreshFileList(textBox1.Text, cbLimList.Checked ? (int)nUDListSize.Value : 0, tv1, '\\');
            pbBusy.Visible = false;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Text == "Options")
            {
                OptionsChanged = true;
                lOptionsInfo.Text = "Custom options were defined";
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure want to save all options to the holoptions_custom.xml?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            SaveHOLOptions("holoptions.xml", true);
        }

        private bool AppExit(FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure want to interrupt harvesting?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                this.Invoke(myDelegate, new Object[] { "CANCEL request has been made. Please wait for safe cancelling to be finished." });
                cts.Cancel();
                return false;
            }
            else
            {
                e.Cancel = false;
                return true;
            }
        }

        public struct pts
        {
            public int id;
            public List<double> vars;
        };

        public List<pts> pts_list;
        public List<int> Tids;
        public List<List<double>> PCA;
        public int var1, var2, var3, var4, var5;
        public double xfrom, xto, yfrom, yto;
        public double vxmin, vxmax, vymin, vymax;
        public bool show_bbox;
        public int bb_xfrom, bb_xto, bb_yfrom, bb_yto;
        Bitmap B;
        Graphics G;

        struct brsh
        {
            public PathGradientBrush pgb;
            public GraphicsPath gp;
        };

        brsh MakeRadialBrush(float x, float y, float r, Color c, float scale)
        {
            brsh b = new brsh();
            b.gp = new GraphicsPath();
            b.gp.AddEllipse(scale * (x - r), scale * (y - r), scale * (2 * r), scale * (2 * r));
            b.pgb = new PathGradientBrush(b.gp);
            b.pgb.CenterColor = c;
            b.pgb.CenterPoint = new PointF(scale * x, scale * y);
            //var sc = new Color[] { Color.FromArgb(c.A / 5, c.R, c.G, c.B) };
            var sc = new Color[] { Color.FromArgb(0, c.R, c.G, c.B) };
            b.pgb.SurroundColors = sc;
            return b;
        }

        private void ShowDiscoMap(Bitmap B, Graphics G, int var1, int var2, double xfrom, double xto, double yfrom, double yto, int alpha = 30)
        {
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            G.Clear(Color.Black);

            var br = new SolidBrush(Color.Navy);
            br.Color = Color.FromArgb(alpha, br.Color);

            int w = pbDisco.Width;
            int h = pbDisco.Height;

            if (((yfrom - yto) == 0) || ((xfrom - xto) == 0))
            {
                pbDisco.Image = B;
                return;
            }

            G.DrawLine(new Pen(Color.DarkGray), 0, (int)((h * yfrom) / (yfrom - yto)), w, (int)((h * yfrom) / (yfrom - yto)));
            G.DrawLine(new Pen(Color.DarkGray), (int)((w * xfrom) / (xfrom - xto)), 0, (int)((w * xfrom) / (xfrom - xto)), h);

            double v3min = PCA.Select(v => v[var3]).Min();
            double v3max = PCA.Select(v => v[var3]).Max();
            double v3diff = v3max - v3min;

            double v4min = PCA.Select(v => v[var4]).Min();
            double v4max = PCA.Select(v => v[var4]).Max();
            double v4diff = v4max - v4min;

            double v5min = PCA.Select(v => v[var5]).Min();
            double v5max = PCA.Select(v => v[var5]).Max();
            double v5diff = v5max - v5min;

            int th = (int)nUDThickness.Value;

            var idTgtList = new List<string>();
            var idPlsList = new List<string>();

            if (cbDiscoHighlight.Checked)
            {
                for (int i = 0; i < dGV_tgt.RowCount; i++)
                    idTgtList.Add(dGV_tgt.Rows[i].Cells["dgt_id"].Value.ToString());
                for (int i = 0; i < dGV_pls.RowCount; i++)
                    idPlsList.Add(dGV_pls.Rows[i].Cells["dgp_id"].Value.ToString());
            }

            for (int i = 0; i < PCA.Count; i++)
            {
                int r, g, b;
                if (checkBox3.Checked)
                    r = (int)nUDVar3Alt.Value;
                else
                    r = (int)(255 * (PCA[i][var3] - v3min) / v3diff);

                if (checkBox4.Checked)
                    g = (int)nUDVar4Alt.Value;
                else
                    g = (int)(255 * (PCA[i][var4] - v4min) / v4diff);

                if (checkBox5.Checked)
                    b = (int)nUDVar5Alt.Value;
                else
                    b = (int)(255 * (PCA[i][var5] - v5min) / v5diff);

                br.Color = Color.FromArgb(alpha, r, g, b);

                int x = (int)(w * (PCA[i][var1] - xfrom) / (xto - xfrom));
                int y = (int)(h * (PCA[i][var2] - yfrom) / (yto - yfrom));
                int radius = th;

                if (idTgtList.Contains(Tids[i].ToString()))
                {
                    br.Color = Color.LightGreen;
                    if (((x - radius) >= 0) &&
                        ((y - radius) >= 0) &&
                        ((x + radius) <= w) &&
                        ((y + radius) <= h))    
                        G.FillEllipse(br, x - radius, y - radius, 2 * radius, 2 * radius);
                }
                else if (idPlsList.Contains(Tids[i].ToString()))
                {
                    br.Color = Color.LightYellow;
                    if (((x - radius) >= 0) &&
                        ((y - radius) >= 0) &&
                        ((x + radius) <= w) &&
                        ((y + radius) <= h))
                        G.FillEllipse(br, x - radius, y - radius, 2 * radius, 2 * radius);
                }
                else
                {
                    var rbr = MakeRadialBrush(x, y, radius, br.Color, 1);
                    G.FillPath(rbr.pgb, rbr.gp);
                }                
            }

            if (show_bbox)
            {
                G.DrawRectangle(new Pen(Color.DarkGray), bb_xfrom, bb_yfrom, bb_xto - bb_xfrom, bb_yto - bb_yfrom);
            }

            pbDisco.Image = B;
        }

        private void ShowDiscoMap2(Bitmap B, Graphics G, int alpha = 30)
        {
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            G.Clear(Color.Black);

            PCA.OrderBy(v => v.Average());

            var br = new SolidBrush(Color.Navy);
            br.Color = Color.FromArgb(alpha, br.Color);

            int w = pbDisco.Width;
            int h = pbDisco.Height;

            double xoffs = 0.0, yoffs = 0.0;

            var colors = new List<Color>();
            colors.Add(Color.Red);
            colors.Add(Color.Orange);
            colors.Add(Color.Yellow);
            colors.Add(Color.Green);
            colors.Add(Color.Cyan);
            colors.Add(Color.Blue);
            colors.Add(Color.MediumPurple);

            var orderby = new List<int>();
            var orderwhat = new List<double>();
            var avglist = new List<double>();
            var stdlist = new List<double>();

            string hairsort = (string)lbHairSort.SelectedItem;

            orderby = Enumerable.Range(0, PCA[0].Count).ToList();
            var ordersample = Enumerable.Range(0, PCA.Count).Select(v => (double)v).ToList();

            for (int i = 0; i < PCA[0].Count; i++)
            {
                var tmpwhat = new List<double>();
                for (int j = 0; j < PCA.Count; j++)
                    tmpwhat.Add(PCA[j][i]);
                avglist.Add(tmpwhat.Average());
                stdlist.Add(Utilities.StDev(tmpwhat));
                tmpwhat.Sort();

                if (hairsort == "By mean")
                    orderwhat.Add(tmpwhat.Average());
                if (hairsort == "By stdev")
                    orderwhat.Add(Utilities.StDev(tmpwhat));
                if (hairsort == "By mixed")
                    orderwhat.Add(Utilities.CalcCorrel(ordersample, tmpwhat));
                //orderwhat.Add(Utilities.StDev(tmpwhat) * (1.1 * (tmpwhat.Max() - tmpwhat.Min()) + tmpwhat.Average()));                
            }
            if (hairsort != "None")
                orderby = orderby.OrderBy(v => orderwhat[v]).ToList();

            float th = (float)nUDThickness.Value;
            
            for (int p = 0; p < PCA.Count; p++)
            {
                var pts = new Point[PCA[0].Count];
                for (int i = 0; i < PCA[0].Count; i++)
                {
                    int ii = orderby[i];
                    pts[i].X = (int)(w * (xoffs + (((double)i + 0.5) / (double)PCA[p].Count)));
                    pts[i].Y = (int)(h * (yoffs + 0.5 + 0.1 * (PCA[p][ii] - avglist[ii]) / (stdlist.Average())));
                }

                //G.DrawLines(new Pen(Color.FromArgb(alpha, Color.Salmon), 2), pts);
                G.DrawCurve(new Pen(Color.FromArgb(alpha, Color.White), th), pts);
            }
            pbDisco.Image = B;
        }

        private void button23_Click(object sender, EventArgs e)
        {          
            int id_count = ManageDB.RequestInt("", (string)lbDBDisco.SelectedItem, "select count(id) from main;");
            int statname_count = ManageDB.RequestInt("", (string)lbDBDisco.SelectedItem, "select count(distinct statname) from stats;");

            nUDDiscoNum.Maximum = id_count;
            Application.DoEvents();

            var T = ManageDB.LoadStatsFromDB("", (string)lbDBDisco.SelectedItem, (int)nUDDiscoNum.Value, statname_count);

            T = T.OrderBy(v => v.Key).ToList();
            Tids = T.Select(v => v.Key).Distinct().OrderBy(v => v).ToList();

            var TT = Utilities.MakeTableFromKKV(ref T, statname_count, (int)Tids.Count);

            if ((string)lbViewArray.SelectedItem == "Compressed")
            {
                //var TTstd = Utilities.ScaleTableByStdev(ref TT);
                PCA = ClusterAnalysis.PCAnalysis(ref TT, 0, 0.0001);
            }
            else
                PCA = TT;

            pts_list = new List<pts>();
            for (int i = 0; i < PCA.Count - 1; i++)
            {
                var p = new pts();
                p.id = Tids[i];
                p.vars = PCA[i];
                pts_list.Add(p);
            }
            
            lDiscoInfo.Text = "Vars chosen: " + PCA[0].Count.ToString();

            B = new Bitmap(pbDisco.Width, pbDisco.Height);
            G = Graphics.FromImage(B);

            nUDvar1.Maximum = PCA[0].Count - 1;
            nUDvar2.Maximum = PCA[0].Count - 1;
            nUDvar3.Maximum = PCA[0].Count - 1;
            nUDvar4.Maximum = PCA[0].Count - 1;
            nUDvar5.Maximum = PCA[0].Count - 1;

            Application.DoEvents();

            var1 = (int)nUDvar1.Value;
            var2 = (int)nUDvar2.Value;
            var3 = (int)nUDvar3.Value;
            var4 = (int)nUDvar4.Value;
            var5 = (int)nUDvar5.Value;

            int alpha = (int)nUDOpacity.Value;
            show_bbox = false;

            double KK = 3;
            xfrom = vxmin = -KK * StDev(PCA.Select(v => v[var1]).ToList());
            xto   = vxmax =  KK * StDev(PCA.Select(v => v[var1]).ToList());
            yfrom = vymin = -KK * StDev(PCA.Select(v => v[var2]).ToList());
            yto   = vymax =  KK * StDev(PCA.Select(v => v[var2]).ToList());

            if ((string)lbViewType.SelectedItem == "Scatterplot")
                ShowDiscoMap(B, G, var1, var2, vxmin, vxmax, vymin, vymax, alpha);
            if ((string)lbViewType.SelectedItem == "Hairplot")
                ShowDiscoMap2(B, G, alpha);
        }

        public List<string> discoPlaylist;

        private void pbDisco_MouseClick(object sender, MouseEventArgs e)
        {
            if ((string)lbViewType.SelectedItem != "Scatterplot")
                return;

            double xrgn = (double)e.X / pbDisco.Width;
            double yrgn = (double)e.Y / pbDisco.Height;            
            double xscale = (vxmax - vxmin);
            double yscale = (vymax - vymin);
            double xarea = 0.025 * xscale;
            double yarea = 0.025 * yscale;

            double var1v, var2v;
            GetPointFromViewport(e.X, e.Y, pbDisco.Width, pbDisco.Height, out var1v, out var2v);

            var ans = pts_list
                .Where(v =>
                    (Math.Abs(vxmin + (xrgn * xscale - v.vars[var1])) <= xarea) &&
                    (Math.Abs(vymin + (yrgn * yscale - v.vars[var2])) <= yarea)
                    )
                .Select(v => v.id)
                .ToList();

            if (ans.Count() == 0)
                lList.Text = ans.Count().ToString();
            if (ans.Count() > 0)
            {
                string s = "";
                ans.Take(50).ToList().ForEach(r => s += r.ToString() + ", ");
                var sql = "select path from main where id in (" + s.Substring(0, s.Length - 2) + ");";
                var pth = ManageDB.RequestStringList("", (string)lbDBDisco.SelectedItem, sql);

                discoPlaylist = pth;
                lbDiscoPlaylist.Items.Clear();
                pth.ForEach(v => lbDiscoPlaylist.Items.Add(v));
                //lList.Text = string.Join("\n", pth.ToArray());                
            }

            show_bbox = true;
            bb_xfrom = e.X - (int)(xarea * pbDisco.Width / xscale);
            bb_xto   = e.X + (int)(xarea * pbDisco.Width / xscale);
            bb_yfrom = e.Y - (int)(yarea * pbDisco.Height / yscale);
            bb_yto   = e.Y + (int)(yarea * pbDisco.Height / yscale);

            if ((string)lbViewType.SelectedItem == "Scatterplot")
                ShowDiscoMap(B, G, var1, var2, vxmin, vxmax, vymin, vymax, (int)nUDOpacity.Value);
            if ((string)lbViewType.SelectedItem == "Hairplot")
                ShowDiscoMap2(B, G, (int)nUDOpacity.Value);            
        }

        private void button24_Click(object sender, EventArgs e)
        {
            B = new Bitmap(pbDisco.Width, pbDisco.Height);
            G = Graphics.FromImage(B);

            var1 = (int)nUDvar1.Value;
            var2 = (int)nUDvar2.Value;
            var3 = (int)nUDvar3.Value;
            var4 = (int)nUDvar4.Value;
            var5 = (int)nUDvar5.Value;

            int alpha = (int)nUDOpacity.Value;
            if ((string)lbViewType.SelectedItem == "Scatterplot")
                ShowDiscoMap(B, G, var1, var2, vxmin, vxmax, vymin, vymax, (int)nUDOpacity.Value);
            if ((string)lbViewType.SelectedItem == "Hairplot")
                ShowDiscoMap2(B, G, (int)nUDOpacity.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
        }

        private void GetPointFromViewport(int x, int y, int w, int h, out double var1v, out double var2v)
        {
            var1v = (vxmax - vxmin) * (double)x / (double)w;
            var2v = (vymax - vymin) * (double)y / (double)h;
        }

        private void SetViewport(double dx, double dy, double zoomx = 0, double zoomy = 0)
        {
            if ((string)lbViewType.SelectedItem != "Scatterplot")
                return;

            vxmin += (zoomx + dx) * (vxmax - vxmin);
            vxmax -= (zoomx - dx) * (vxmax - vxmin);
            vymin += (zoomy + dy) * (vymax - vxmin);
            vymax -= (zoomy - dy) * (vymax - vymin);
            ShowDiscoMap(B, G, var1, var2, vxmin, vxmax, vymin, vymax, (int)nUDOpacity.Value);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            SetViewport(-0.05, 0, 0);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            SetViewport(0.05, 0, 0);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            SetViewport(0, -0.05, 0);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            SetViewport(0, 0.05, 0);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            SetViewport(0, 0, 0.05, 0.05);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            SetViewport(0, 0, -0.05, -0.05);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            var KK = 3;
            xfrom = vxmin = -KK * StDev(PCA.Select(v => v[var1]).ToList());
            xto   = vxmax =  KK * StDev(PCA.Select(v => v[var1]).ToList());
            yfrom = vymin = -KK * StDev(PCA.Select(v => v[var2]).ToList());
            yto   = vymax =  KK * StDev(PCA.Select(v => v[var2]).ToList());

            if ((string)lbViewType.SelectedItem == "Scatterplot")
                ShowDiscoMap(B, G, var1, var2, vxmin, vxmax, vymin, vymax, (int)nUDOpacity.Value);
            if ((string)lbViewType.SelectedItem == "Hairplot")
                ShowDiscoMap2(B, G, (int)nUDOpacity.Value);
        }

        private void pbDisco_Click(object sender, EventArgs e)
        {

        }

        private void button33_Click(object sender, EventArgs e)
        {
            var d = ManageDB.RequestSongDict("", (string)lbDBDisco.SelectedItem, "1");

            int cnt = 2000;
            var gen = new List<int>();
            gen.Add(0);
            var r = new Random();

            for (int i = 0; i < cnt - 1; i++)
            {
                var nxt = d.Where(v => int.Parse(v.Key.Split('v')[1]) == gen[i]).ToList();
                var maxprob = nxt.Max(v => v.Value);
                if (nxt.Max(v => v.Value) == 0)
                {
                    gen[i] = r.Next(nxt.Count());
                    i--;
                    continue;
                }

                int p = -1;
                while (p == -1)
                {
                    var n = r.Next(nxt.Count());
                    if (Utilities.GetTrueByProb(nxt[n].Value / maxprob))
                        p = n;
                }
                gen.Add(p);
            }

            MessageBox.Show(String.Join("-", gen.ToArray()));
        }

        private void button34_Click(object sender, EventArgs e)
        {
            if (discoPlaylist.Count > 0)
                SaveAndOpenPlaylist(discoPlaylist);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            nUDVar3Alt.Enabled = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            nUDVar4Alt.Enabled = checkBox4.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            nUDVar5Alt.Enabled = checkBox5.Checked;
        }
    }
}
