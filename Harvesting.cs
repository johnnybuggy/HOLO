using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Meta.Numerics.SignalProcessing;
using Meta.Numerics;

using NAudio.Wave;

namespace HOLO
{
    public class Harvesting
    {
        public struct _song_profile
        {
            public string name;
            public string path;
            public long total_samples;
            public int snap_size;
            public int snap_count;
            public double length;

            public List<List<double>> fft_snaps;
            //public List<List<double>> fft_smoothed_snaps;
            //public List<List<double>> fft_downscaled_snaps;
            //public List<List<double>> fft_downscaled_stdevs;

            public List<List<double>> pca_snaps;

            public List<List<double>> pcm_snaps_l;
            public List<List<double>> pcm_snaps_r;

            //public Dictionary<string, double> dic;
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
                //fft_smoothed_snaps = new List<List<double>>();
                //fft_downscaled_snaps = new List<List<double>>();
                //fft_downscaled_stdevs = new List<List<double>>();

                pca_snaps = new List<List<double>>();

                pcm_snaps_l = new List<List<double>>();
                pcm_snaps_r = new List<List<double>>();

                //dic = new Dictionary<string, double>();
                sdic = new Dictionary<string, List<double>>();
            }
        }

        public static Dictionary<int, double> MWPreCalc;

        public static void Init(int sz)
        {
            MWPreCalc = new Dictionary<int,double>(Utilities.CalcMW(sz));
        }

        public static string MakeSnaps(ref _song_profile sp, int _snapsize, int _snapcount, string _path, bool doConsecutive)
        {
            var SR = new SoundReader();
            SR.OpenFile(_path);

            int samplesDesired = _snapsize; //Math.Max(_snapsize, 65536);

            sp.snap_size = _snapsize;
            sp.snap_count = _snapcount;
            sp.total_samples = SR.FR.Length;
            sp.length = SR.FR.TotalTime.Seconds;
            sp.path = _path;
            long blockscount = (long)(_snapcount * ((float)sp.total_samples / 50000000));

            if (!SR.CheckFile())
            {
                SR.CloseFile();
                return "Wrong format";
            }

            SR.CloseFile();

            using (SR.FR = new Mp3FileReader(_path))
            {
                for (int i = 0; i < blockscount; i++)
                {
                    var data = SR.GetNextChunk(samplesDesired, SR.FR);
                    double[] leftd = data[0];
                    double[] rightd = data[1];

                    //for (int j = 0; j < 1; j++) ///////////
                    int seek_counter = 0;
                //seek: pcm.Seek((i + 1) * (i + 1) * (i + 1) * blockscount * samplesDesired % (total_samples - samplesDesired), SeekOrigin.Begin);

                seek:
                    if (!doConsecutive)
                        SR.SetPosition((long)(0.20 * sp.total_samples) + (long)(0.70 * sp.total_samples * i / blockscount));
                    else
                        SR.SetPosition((long)(0.20 * sp.total_samples) + i * _snapsize);
                    seek_counter++;

                    if ((leftd.Max(t => Math.Abs(t)) == 0) && (rightd.Max(t => Math.Abs(t)) == 0))
                        if (seek_counter > 4)
                            return "Too much silence";
                        else
                        {
                            blockscount++;
                            i++;
                            goto seek;                            
                        }

                    var pow_re_im = SoundProcessing.AmplitudeFromLR(_snapsize, leftd, rightd, 4.0, MWPreCalc);

                    sp.fft_snaps.Add(pow_re_im);
                }
                SR.CloseFile();
            }

            return "OK";
        }

        public static string MakePCASnaps(ref _song_profile sp, ref List<List<double>> vmatrix)
        {
            sp.pca_snaps = ClusterAnalysis.PCAGetNewSpace(ref sp.fft_snaps, ref vmatrix, vmatrix.Count);
            sp.fft_snaps = sp.pca_snaps;
            return "OK";
        }
        
        public static string MakeSnaps2(ref _song_profile sp, int _snapsize, int _snapcount, string _path, int overlap = 2, double lowpass = 4.0, bool fullnormalize = false)
        {
            var SR = new SoundReader();
            var result = SR.OpenFile(_path);
            if (result != "OK")
                return result;

            int samplesDesired = _snapsize;

            sp.snap_size = _snapsize;
            sp.snap_count = _snapcount;
            sp.total_samples = SR.FR.Length;
            sp.length = SR.FR.TotalTime.Seconds;
            sp.path = _path;
            
            int blockscount = (int)(_snapcount * ((float)sp.total_samples / (4 * 5 * 60 * 44100))); // 5 minutes song equivalent
            blockscount = Math.Min(blockscount, 5 * 60 * 44100 / _snapsize); // haircut to 5 minutes long (maximum)
            blockscount = Math.Min(blockscount, (int)(0.8 * _snapcount * ((float)sp.total_samples / (4 * 5 * 60 * 44100)))); // haircut to no more than 80% of song

            if (!SR.CheckFile())
            {
                SR.CloseFile();
                return "Wrong format";
            }

            SR.CloseFile();

            using (SR.FR = new Mp3FileReader(_path))
            {
                SR.SetPosition((long)(0.20 * sp.total_samples));

                double[][] alldata;

                try
                {
                    alldata = SR.GetNextChunk(samplesDesired * blockscount, SR.FR, fullnormalize);
                }
                catch (Exception E)
                {
                    return E.Message;
                }

                for (int i = 0; i < (blockscount - 1) * overlap; i++)
                {
                    double[] leftd = alldata[0];
                    double[] rightd = leftd;
                    var pow_re_im = SoundProcessing.AmplitudeFromLR2(_snapsize, leftd, Math.Max((int)(i * samplesDesired / overlap)-1, 0), samplesDesired, lowpass, MWPreCalc);

                    sp.fft_snaps.Add(pow_re_im);
                }
                
                SR.CloseFile();
            }

            return "OK";
        }

        public static string MakeSnaps3(ref _song_profile sp, int _snapsize, int _snapcount, string _path, int overlap = 2, double lowpass = 4.0)
        {
            var SR = new SoundReader();
            var result = SR.OpenFile(_path);
            if (result != "OK")
                return result;

            int samplesDesired = _snapsize;

            sp.snap_size = _snapsize;
            sp.snap_count = _snapcount;
            sp.total_samples = SR.FR.Length;
            sp.length = SR.FR.TotalTime.Seconds;
            sp.path = _path;

            int blockscount = (int)(_snapcount * ((float)sp.total_samples / (4 * 5 * 60 * 44100))); // 5 minutes song equivalent
            blockscount = Math.Min(blockscount, 5 * 60 * 44100 / _snapsize); // haircut to 5 minutes long (maximum)
            blockscount = Math.Min(blockscount, (int)(0.8 * _snapcount * ((float)sp.total_samples / (4 * 5 * 60 * 44100)))); // haircut to no more than 80% of song

            if (!SR.CheckFile())
            {
                SR.CloseFile();
                return "Wrong format";
            }

            SR.CloseFile();

            using (SR.FR = new Mp3FileReader(_path))
            {
                SR.SetPosition((long)(0.20 * sp.total_samples));

                double[][] alldata;

                try
                {
                    alldata = SR.GetNextChunk(samplesDesired * blockscount, SR.FR);
                }
                catch (Exception E)
                {
                    return E.Message;
                }

                List<double> f_snap = null;

                for (int i = 0; i < (blockscount - 1) * overlap - 1; i++)
                {
                    double[] leftd = alldata[0];
                    double[] rightd = leftd;
                    var pow_re_im = SoundProcessing.AmplitudeFromLR2(_snapsize, leftd, Math.Max((int)(i * samplesDesired / overlap) - 1, 0), samplesDesired, lowpass, MWPreCalc);

                    if (i > 0)
                        sp.fft_snaps.Add(pow_re_im.Zip(f_snap, (a, b) => a - b).ToList());                        

                    f_snap = pow_re_im;
                }

                SR.CloseFile();
            }

            return "OK";
        }
    }
}
