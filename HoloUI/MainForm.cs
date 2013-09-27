using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HoloKernel;
using HoloKernel;

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
                ProcessAudioSources();
            }catch(Exception ex)
            {
                //ignore
                Console.WriteLine(ex.Message);
            }
            tmProcessing.Start();
        }

        private void ProcessAudioSources()
        {
            //find unprocessed items
            var list = new List<AudioSource>();
            lock (RunManager.DB.AudioSources)
                foreach (var item in RunManager.DB.AudioSources)
                    if(item.State == AudioSourceState.Unprocessed)
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
                                                                         pnAudioSources.Refresh();
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
                lock (RunManager.DB.AudioSources)
                    dict = RunManager.DB.AudioSources.GetIndexesByFullPath();

                foreach (var path in AudioFileScanner.Scan(pathes))
                if(!dict.ContainsKey(path))
                {
                    var item = new AudioSource() {FullPath = path};
                    lock (RunManager.DB.AudioSources)
                        RunManager.DB.AudioSources.Add(item);
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

            showedItems = new AudioSources(RunManager.DB.AudioSources);//temp !!!!!
            pnAudioSources.Build(showedItems);
            lbItemCount.Text = "Items: " + showedItems.Count;
        }

        private void pbSettings_Click(object sender, EventArgs e)
        {
            cmSettings.Show(pbSettings, -200, pbSettings.Bounds.Height);
        }

        private void miRemoveShowedItems_Click(object sender, EventArgs e)
        {
            RemoveShowedItems();
        }

        private AudioSources showedItems;

        private void RemoveShowedItems()
        {
            try
            {
                lock (RunManager.DB.AudioSources)
                   RunManager.DB.AudioSources.RemoveRange(showedItems);
                RunManager.DB.IsChanged = true;

                Build();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}
