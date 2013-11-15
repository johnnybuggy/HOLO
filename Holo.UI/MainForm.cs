using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Holo.Core;
using Holo.Processing;
using Holo.Processing.Search;
using Holo.UI.Controls;
using HoloDB;
using NLog;

namespace Holo.UI
{
    public partial class MainForm : Form, IView
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly HoloCore Core;

        private Audios ShownItems;

        private System.Timers.Timer tmProcessing;

        public MainForm(HoloCore core)
        {
            if (core == null)
            {
                throw new ArgumentNullException("core");
            }

            Core = core;

            InitializeComponent();


            tmProcessing = new System.Timers.Timer(1000);
            tmProcessing.Elapsed += tmProcessing_Elapsed;
            tmProcessing.Start();
        }

        void tmProcessing_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmProcessing.Stop();

            try
            {
                Core.ProcessAudios(UpdateProcessingProgress);
            }
            catch (Exception E)
            {
                Logger.WarnException("Audio processing exception.", E);
            }

            tmProcessing.Start();
        }

        private void UpdateProcessingProgress(object sender, ProgressChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => UpdateProcessingProgress(sender, e)));
                return;
            }

            lbProgress.Visible = true;
            pbProgress.Visible = e.ProgressPercentage < 100;
            pbProgress.Value = e.ProgressPercentage;
            lbProgress.Text = "Processed: " + e.ProgressPercentage + "%";
            ssMain.Refresh();
            pnAudios.Refresh();
        }

        void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] Files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Core.ScanInBackground(Files);
        }

        private void btAddFolder_Click(object sender, EventArgs e)
        {
            var Form = new SelectFolderForm();

            Form.SelectedFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            if (Form.ShowDialog() == DialogResult.OK)
            {
                Core.ScanInBackground(new string[] { Form.SelectedFolder });
            }
        }



        public void ShowError(Exception ex)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => ShowError(ex)));
                return;
            }

            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void UpdateAddedCountLabel(int count)
        {
            if (InvokeRequired)
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
            DisplayItems();
        }

        public void DisplayItems()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(DisplayItems));
                return;
            }

            ShownItems = new Audios(Core.GetAudios());//temp !!!!!

            pnAudios.Build(ShownItems);
            lbItemCount.Text = "Items: " + ShownItems.Count;
            Invalidate(true);
        }

        private void pbSettings_Click(object sender, EventArgs e)
        {
            cmSettings.Show(pbSettings, -200, pbSettings.Bounds.Height);
        }

        private void miRemoveShowedItems_Click(object sender, EventArgs e)
        {
            RemoveShownItems();
        }



        private void RemoveShownItems()
        {
            Core.RemoveItems(ShownItems);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var Item in Core.GetAudios())
            {
                var Tempogram = Item.GetData<Tempogram>();

                if (Tempogram != null)
                {
                    TempogramProcessor.CalcTempo(Tempogram);
                }
            }

            Core.MarkDatabaseAsChanged();
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            var Pattern = tbSearch.Text;
            try
            {
                var Parts = Pattern.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (Parts.Length == 0)
                {
                    DisplayItems();
                    return;
                }

                ShownItems = new Audios();

                foreach (var Item in Core.GetAudios())
                {
                    bool IsFound = true;
                    var AudioName = Item.ShortName.ToLower();
                    foreach (var Part in Parts)
                    {
                        if (!AudioName.Contains(Part))
                        {
                            IsFound = false;
                            break;
                        }
                    }

                    if (IsFound)
                        ShownItems.Add(Item);
                }

                pnAudios.Build(ShownItems);
                lbItemCount.Text = "Items: " + ShownItems.Count;
                Invalidate(true);
            }
            catch (Exception E)
            {
                Logger.ErrorException("Files processing exception.", E);

                ShowError(E);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            AlgorithmEstimator Estimator = new AlgorithmEstimator(Core, "hash.csv", "scores.csv");

            List<EstimationResult> Results = new List<EstimationResult>();

            Results.Add(Estimator.EstimateAlgorithm<SearchByRandom>());
            Results.Add(Estimator.EstimateAlgorithm<SearchByTempoDistribution>());

            SimilarityOptionsVariator OptionsVariator = new SimilarityOptionsVariator();
            Results.AddRange(OptionsVariator.GetNextOptions().Select(Estimator.EstimateAlgorithm<SearchBySimilarity>));

            EstimationResultsForm ResultsForm = new EstimationResultsForm();
            ResultsForm.SetResults(Results);
            ResultsForm.Show();
        }
    }
}
