using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Holo.UI.Controls;
using HoloDB;
using HoloKernel;
using HoloProcessors;

namespace Holo.UI
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
            foreach (var item in RunManager.DB.Audios)
            {
                var t = item.GetData<Tempogram>();
                if(t != null)
                TempogramProcessor.CalcTempo(t);
            }
            RunManager.DB.IsChanged = true;
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            var pattern = tbSearch.Text;
            try
            {
                var parts = pattern.ToLower().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                {
                    Build();
                    return;
                }

                showedItems = new Audios();

                foreach(var item in RunManager.DB.Audios)
                {
                    bool found = true;
                    var name = item.ShortName.ToLower();
                    foreach(var part in parts)
                        if(!name.Contains(part))
                        {
                            found = false;
                            break;
                        }

                    if (found)
                        showedItems.Add(item);
                }

                pnAudios.Build(showedItems);
                lbItemCount.Text = "Items: " + showedItems.Count;
                Invalidate(true);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}
