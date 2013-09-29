using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HoloDB;
using HoloKernel;
using HoloKernel;
using HoloProcessors;

namespace HoloUI
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer tmProcessing;

        public MainForm()
        {
            InitializeComponent();

            tmProcessing = new System.Timers.Timer(1000);
            tmProcessing.Elapsed += new System.Timers.ElapsedEventHandler(tmProcessing_Elapsed);
            tmProcessing.Start();
        }

        void tmProcessing_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmProcessing.Stop();
            try
            {
                ProcessAudios();
            }catch(Exception ex)
            {
                //ignore
                Console.WriteLine(ex.Message);
            }
            tmProcessing.Start();
        }

        private void ProcessAudios()
        {
            //find unprocessed items
            var list = new List<Audio>();
            lock (RunManager.DB.Audios)
                foreach (var item in RunManager.DB.Audios)
                    if(item.State == AudioState.Unprocessed)
                        list.Add(item);

            var processor = RunManager.Factory.CreateAudioProcessor();
            processor.Progress += (o, e) => Invoke((MethodInvoker)(() =>
                                                                     {
                                                                         RunManager.DB.IsChanged = true;
                                                                         lbProgress.Visible = true;
                                                                         pbProgress.Visible = e.ProgressPercentage < 100;
                                                                         pbProgress.Value = e.ProgressPercentage;
                                                                         lbProgress.Text = "Processed: " +e.ProgressPercentage + "%";
                                                                         ssMain.Refresh();
                                                                         pnAudios.Refresh();
                                                                     }));
            processor.Process(list);
        }

        void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void MainForm_DragDrop(object sender, DragEventArgs e) 
        {
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            ScanInOtherThread(files);
        }

        private void btAddFolder_Click(object sender, EventArgs e)
        {
            var form = new SelectFolderForm();
            form.SelectedFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ScanInOtherThread(new string[] { form.SelectedFolder });
        }

        void ScanInOtherThread(IEnumerable<string> pathes)
        {
            var th = new Thread(()=>Scan(pathes));
            th.IsBackground = true;
            th.Start();
        }

        void Scan(IEnumerable<string> pathes)
        {
            try
            {
                int counter = 0;
                UpdateAddedCountLabel(counter);

                Dictionary<string, int> dict;
                lock (RunManager.DB.Audios)
                    dict = RunManager.DB.Audios.GetIndexesByFullPath();

                foreach (var path in AudioFileScanner.Scan(pathes))
                if(!dict.ContainsKey(path))
                {
                    var item = new Audio() {FullPath = path};
                    lock (RunManager.DB.Audios)
                        RunManager.DB.Audios.Add(item);
                    RunManager.DB.IsChanged = true;
                    counter++;
                    if (counter%7 == 0)
                        UpdateAddedCountLabel(counter);
                }

                UpdateAddedCountLabel(counter);

                Build();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        void ShowError(Exception ex)
        {
            if(InvokeRequired)
            {
                Invoke((MethodInvoker)(() => ShowError(ex)));
                return;
            }

            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateAddedCountLabel(int count)
        {
            if(InvokeRequired)
            {
                Invoke((MethodInvoker)(() => UpdateAddedCountLabel(count)));
                return;
            }

            lbAddedCount.Visible = count > 0;
            lbAddedCount.Text = "Added: " + count;
            lbAddedCount.Invalidate();
            ssMain.Refresh();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Build();
        }

        private void Build()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(Build));
                return;
            }

            showedItems = new Audios(RunManager.DB.Audios);//temp !!!!!
            pnAudios.Build(showedItems);
            lbItemCount.Text = "Items: " + showedItems.Count;
            Invalidate(true);
        }

        private void pbSettings_Click(object sender, EventArgs e)
        {
            cmSettings.Show(pbSettings, -200, pbSettings.Bounds.Height);
        }

        private void miRemoveShowedItems_Click(object sender, EventArgs e)
        {
            RemoveShowedItems();
        }

        private Audios showedItems;

        private void RemoveShowedItems()
        {
            try
            {
                lock (RunManager.DB.Audios)
                   RunManager.DB.Audios.RemoveRange(showedItems);
                RunManager.DB.IsChanged = true;

                Build();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fft = new FFTCalculator();
            var data = new float[] {1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0};
            Out(data);
            fft.RealFFT(data, true);
            data = fft.Norm(data);
            Out(data);
        }

        private static void Out(float[] data)
        {
            Console.WriteLine();
            foreach (var v in data) Console.Write(v.ToString("0.00") + "\t");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var factory = new DefaultFactory();
            var decoder = factory.CreateAudioDecoder();
            //decode audio source to samples and mp3 tags extracting
            AudioInfo info;
            Audio item = null;

            //item = new Audio() { FullPath = @"E:\Music\Classic music\Bah-Badinerie---Muzykal_naya-shutka-Syuita--2-si-minor-dlya-fleyty-s-orkestrom(muzofon.com).mp3" };
            //BuildTempogramm(decoder, item);
            //item = new Audio() { FullPath = @"E:\Music\Classic music\bach_-_orchestral_suite_no._2_in_b_minor_badinerie_(zaycev.net).mp3" };
            //BuildTempogramm(decoder, item);
            //item = new Audio() { FullPath = @"E:\Music\Classic music\17-Bach.SuiteNo2-Badinerie.mp3" };
            //BuildTempogramm(decoder, item);

            item = new Audio() { FullPath = @"E:\Music\Classic music\bach_-_toccata_(zaycev.net).mp3" };
            BuildTempogramm(decoder, item);

            

            //var item = new Audio() { FullPath = @"E:\Music\Ambient\Mushroomer_Surface.mp3" };
            //var item = new Audio() { FullPath = @"E:\Music\Nightwish\Nightwish - 05 The Phantom of the Opera (End of An Era) Live.mp3" };
            //var item = new Audio() { FullPath = @"E:\Music\Classic music\150 любимых мелодий\cd4\13_-_'K_Elize'_-Ljudvig_Vai_Bethoven.mp3" };
            //var item = new Audio() { FullPath = @"E:\Music\Classic music\150 любимых мелодий\cd4\06_-_'Polet_Shmelja'_Iz_Opery_'Skazka_O_Care_Saltane'_-Nikolaj_Rimskij-Korsakov.mp3" };
            //item = new Audio() { FullPath = @"E:\Music\Ambient\Mushroomer_Triton_Lair.mp3" };
            //BuildTempogramm(decoder, item);
            //item = new Audio() { FullPath = @"E:\Music\Ambient\Mushroomer_Ima_tower.mp3" };
            //BuildTempogramm(decoder, item);
            //item = new Audio() { FullPath = @"C:\Mushroomer_Ima_tower_1.wav"};
            //BuildTempogramm(decoder, item);
            //item = new Audio() { FullPath = @"c:\temp.wav" };
            //BuildTempogramm(decoder, item);
            //item = new Audio() { FullPath = @"c:\Solar_Wind_frag.wav" };
            //BuildTempogramm(decoder, item);
            //item = new Audio() { FullPath = @"E:\Music\Classic music\ChiMai.mp3" };
            //BuildTempogramm(decoder, item);
            //item = new Audio() { FullPath = @"E:\Music\Classic music\ElPadrino.mp3" };
            //BuildTempogramm(decoder, item);

            //item = new Audio() { FullPath = @"E:\Music\Classic music\CockeyesSong.mp3" };
            //BuildTempogramm(decoder, item);

            
            
            
            

            //item = new Audio() { FullPath = @"E:\Music\Oomph!\2001 - Ego\08 - Serotonin.mp3" };
            //BuildTempogramm(decoder, item);

            //item = new Audio() { FullPath = @"C:\1hz.wav"};
            //BuildTempogramm(decoder, item);

            //item = new Audio() { FullPath = @"C:\0.5hz.wav" };
            //BuildTempogramm(decoder, item);

            decoder.Dispose();
        }

        private static void BuildTempogramm(IAudioDecoder decoder, Audio item)
        {
            AudioInfo info;
            using (var stream = item.GetSourceStream())
                info = decoder.Decode(stream, 1000, item.GetSourceExtension());

            info.Samples.Normalize();

            var values = info.Samples.Values;

            

            //ToCSV(values);

            //build amplitude envelope
            var eb = new EnvelopeBuilder(RunManager.Factory);
            var s = eb.Build(info.Samples, 32);
            values = s.Values;


            //ToCSV(values);

            //diff
            var diff = new float[values.Length - 10];
            for (int i = 0; i < diff.Length; i++)
            {
                var v = values[i + 1] - values[i];
                if (v > 0)
                    diff[i] = v;
            }

            values = diff;
            ToCSV(values);

            //var s = new Samples() {Bitrate = 1};
            //var values = s.Values = new float[32] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0};
           // var values = s.Values = new float[32] { 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0 };
            
            var maxShift = (int) (16*s.Bitrate);
            var autoCorr = AutoCorr(values, maxShift, 4);
            //values = autoCorr;
            ToCSVWithX(autoCorr, 1 / s.Bitrate);

            //autoCorr = AutoCorr(autoCorr, maxShift / 3);
            //ToCSVWithX(autoCorr, 1 / s.Bitrate);

            /*
            var newSize = (int)Math.Pow(2, (int)Math.Log(values.Length, 2));
            var arr = new float[newSize];
            Array.Copy(values, arr, newSize);
            values = arr;

            new FFTCalculator().RealFFT(values, true);
            values = new FFTCalculator().Norm(values);
            //Array.Reverse(values);
            ToCSVWithX(values, 1/s.Bitrate);*/
        }

        private static float[] AutoCorr(float[] values, int maxShift, int pow = 2)
        {
            float[] autoCorr = new float[maxShift - 1];
            var l = values.Length;


            for (int shift = 1; shift < maxShift; shift++)
            {
                var sum = 0f;
                for (int i = 0; i < values.Length - (pow - 1)* shift; i++)
                {
                    var v = values[i];

                    for (int p = 1; p < pow; p++)
                    {
                        var ii = i + p * shift;
                        /*if(ii >= l) 
                        {
                            v = 0; 
                            break;
                        }*/
                        v *= values[ii];
                    }

                    sum += v;
                }
                autoCorr[shift - 1] = sum;
            }
            return autoCorr;
        }

        private static void ToCSV(float[] array)
        {
            var sb = new StringBuilder();
            foreach (var v in array)
                sb.AppendLine(v.ToString("0.000"));
            var temp = Path.GetTempFileName() + ".csv";
            File.WriteAllText(temp, sb.ToString());
            Process.Start(temp);
        }

        private static void ToCSVWithX(float[] array, float stepX = 1, float startX = 0)
        {
            var x = startX;
            var sb = new StringBuilder();
            foreach (var v in array)
            {
                sb.AppendLine(x.ToString("0.00") +";" + v.ToString("0.000"));
                x += stepX;
            }
            var temp = Path.GetTempFileName() + ".csv";
            File.WriteAllText(temp, sb.ToString());
            Process.Start(temp);
        }
    }
}
