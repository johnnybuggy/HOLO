using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Holo.Core;
using Holo.Processing;
using Holo.Processing.Search;
using HoloDB;

namespace Holo.UI.Controls
{
    public partial class AudiosPanel : UserControl
    {
        private Font SmallFont;
        private Font SmallestFont;
        private FindSimilarPropertiesForm findSimilarPropertiesForm = new FindSimilarPropertiesForm();

        public AudiosPanel()
        {
            InitializeComponent();

            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SmallFont = new Font("SmallFont", Font.Size - 1);
            SmallestFont = new Font("SmallFont", Font.Size - 2);
        }

        private const int itemWidth = 250;
        private const int itemHeight = 100;
        private const int itemPadding = 5;

        private int itemsPerRow;
        private int rowsCount;
        private int selectedItemIndex;

        private Audios items;

        private EnvelopeDrawer envelopeDrawer = new EnvelopeDrawer();
        private SamplesDrawer SamplesDrawer = new SamplesDrawer();
        private CDFDrawer cdfDrawer = new CDFDrawer();

        public void Build(Audios items)
        {
            this.items = items;
            itemsPerRow = Math.Max(1, ClientSize.Width/itemWidth);
            rowsCount = 1 + items.Count/itemsPerRow;
            selectedItemIndex = Math.Min(items.Count, selectedItemIndex);
            AutoScrollMinSize = new Size(itemWidth, rowsCount * itemHeight);
        }

        public void Rebuild()
        {
            if (items!=null)
                Build(items);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Rebuild();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var startX = HorizontalScroll.Value;
            var startY = VerticalScroll.Value;

            var startIndex = startY / itemHeight * itemsPerRow;
            var drawItemCount = itemsPerRow*(2 + ClientSize.Height/itemHeight);

            for(int i=0; i<drawItemCount;i++)
            {
                var itemIndex = startIndex + i;
                var itemY = (itemIndex / itemsPerRow) * itemHeight - startY;
                var itemX = (itemIndex % itemsPerRow) * itemWidth - startX;
                var itemRect = new Rectangle(itemX, itemY, itemWidth, itemHeight);
                itemRect.Inflate(-itemPadding, -itemPadding);
                DrawItem(e.Graphics, itemIndex, itemRect);
            }
        }

        Rectangle GetIconRect(Rectangle itemBounds)
        {
            return new Rectangle(itemBounds.Left + 3, itemBounds.Top + 3, 22, 22);
        }

        Rectangle GetShortNameRect(Rectangle itemBounds)
        {
            return new Rectangle(itemBounds.Left + 22, itemBounds.Top + 3, itemBounds.Width - 22, 15);
        }

        Rectangle GetEnvelopeRect(Rectangle itemBounds)
        {
            return new Rectangle(itemBounds.Left + 22, itemBounds.Top + 18, 64, 32);
        }

        Rectangle GetVolumeDescRect(Rectangle itemBounds)
        {
            return new Rectangle(itemBounds.Left + 22, itemBounds.Top + 18 + 35, 32, 32);
        }

        Rectangle GetTempogramRect(Rectangle itemBounds)
        {
            return new Rectangle(itemBounds.Left + 64 + 25, itemBounds.Top + 18, 63, 32);
        }

        Rectangle GetShortTempogramRect(Rectangle itemBounds)
        {
            return new Rectangle(itemBounds.Left + 64 + 25, itemBounds.Top + 18 + 35, 63, 32);
        }

        Rectangle GetTempoRect(Rectangle itemBounds)
        {
            return new Rectangle(itemBounds.Left + 64 + 25 + 65, itemBounds.Top + 18, 50, 35);
        }

        private void DrawItem(Graphics graphics, int itemIndex,  Rectangle bounds)
        {
            if (itemIndex < 0 || itemIndex >= items.Count)
                return;
            var item = items[itemIndex];
            var color1 = Color.White;
            var color2 = Color.FromArgb(210, 210, 210);

            if (itemIndex == selectedItemIndex)
                color2 = Color.Orange;

            using (var brush = new LinearGradientBrush(bounds, color1, color2, LinearGradientMode.Vertical))
                graphics.FillRectangle(brush, bounds);

            using(var pen = new Pen(color2))
                graphics.DrawRectangle(pen, bounds);

            using(var brush = new SolidBrush(ForeColor))
            {
                var sf = new StringFormat(){Trimming = StringTrimming.EllipsisCharacter};
                graphics.DrawString(item.ShortName, Font, brush, GetShortNameRect(bounds), sf);
            }
            //draw icon
            graphics.DrawImage(Resource.audio_volume_medium, GetIconRect(bounds));

            var envelope = item.GetData<Envelope>();
            if (envelope != null)
                envelopeDrawer.Draw(envelope, graphics, GetEnvelopeRect(bounds));

            var tempogram = item.GetData<Tempogram>();
            if (tempogram != null)
            {
                cdfDrawer.Draw(tempogram.LongTempogram, graphics, GetTempogramRect(bounds));
                cdfDrawer.Draw(tempogram.ShortTempogram, graphics, GetShortTempogramRect(bounds));

                using (var brush = new SolidBrush(ForeColor))
                {
                    var sf = new StringFormat() {Trimming = StringTrimming.None};
                    graphics.DrawString(tempogram.Description, SmallFont, brush, GetTempoRect(bounds), sf);
                }
            }

            var volumeDescriptor = item.GetData<VolumeDescriptor>();
            if (volumeDescriptor != null)
                cdfDrawer.Draw(volumeDescriptor, graphics, GetVolumeDescRect(bounds));

        }

        public int PointToItemIndex(Point p)
        {
            var j = (p.X + HorizontalScroll.Value)/itemWidth;
            if (j >= itemsPerRow) return - 1;

            return (p.Y + VerticalScroll.Value)/itemHeight * itemsPerRow  + j;
        }

        private Point clickMousePoint;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            clickMousePoint = e.Location;

            var index = PointToItemIndex(e.Location);
            if (index != selectedItemIndex)
            {
                selectedItemIndex = index;
                OnSelectedIndexChanged(index);
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                cmMain.Show(PointToScreen(e.Location));
        }

        private void OnSelectedIndexChanged(int index)
        {
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if(Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
            if(Math.Abs(clickMousePoint.X - e.Location.X) + Math.Abs(clickMousePoint.Y - e.Location.Y) > 3)
            {
                if (selectedItemIndex < 0 || selectedItemIndex >= items.Count)
                    return;
                var item = items[selectedItemIndex];
                var dataObject = new DataObject(DataFormats.FileDrop, new string[]{item.FullPath});
                DoDragDrop(dataObject, DragDropEffects.All);
            }
        }

        private void miPlay_Click(object sender, EventArgs e)
        {
            try
            {          
                if (selectedItemIndex < 0 || selectedItemIndex >= items.Count)
                    return;
                var item = items[selectedItemIndex];
                Process.Start(item.FullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void miVolumeDistr_Click(object sender, EventArgs e)
        {
            try
            {
                if(selectedItemIndex < 0 || selectedItemIndex >= items.Count)
                    return;
                var item = items[selectedItemIndex];
                var desc = item.GetData<VolumeDescriptor>();
                if(desc != null)
                foreach (var i in items)
                {
                    var d = i.GetData<VolumeDescriptor>();
                    if(d == null)
                    {
                        i.Tag = 10;
                        continue;
                    }
                    i.Tag = d.Distance(desc);
                }

                items.Sort((a1, a2) => a1.Tag.CompareTo(a2.Tag));

                ScrollToTopLeft();
                selectedItemIndex = 0;
                Invalidate();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tempoDistributionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItemIndex < 0 || selectedItemIndex >= items.Count)
                return;
            try
            {
                items.SearchBy<SearchByTempoDistribution>(selectedItemIndex);

                ScrollToTopLeft();
                selectedItemIndex = 0;
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void ScrollToTopLeft()
        {
            HorizontalScroll.Value = 0;
            VerticalScroll.Value = 0;
            AutoScrollMinSize += new Size(1, 1);
            AutoScrollMinSize -= new Size(1, 1);   
        }

        private void sortByTempoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                items.Sort((a1, a2) =>
                {
                    var t1 = a1.GetData<Tempogram>();
                    var t2 = a2.GetData<Tempogram>();

                    if (t1 != null && t2 != null)
                    {
                        return t1.Intensity.CompareTo(t2.Intensity);
                    }
                    else
                    {
                        return a1.ShortName.CompareTo(a2.ShortName);
                    }
                });

                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void miDebug_Click(object sender, EventArgs e)
        {
            if (selectedItemIndex < 0 || selectedItemIndex >= items.Count)
                return;
            try
            {
                var item = items[selectedItemIndex];

                //new Tester(RunManager.Factory).Process(item);

                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sortByRhythmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                items.Sort(
                        (a1, a2) =>
                        {
                            var t1 = a1.GetData<Tempogram>();
                            var t2 = a2.GetData<Tempogram>();
                            if (t1 != null && t2 != null && t1.LongRhythmIsValid && t2.LongRhythmIsValid)
                                return t1.LongRhythm.CompareTo(t2.LongRhythm);
                            if (t1 == null && t2 != null) return 1;
                            if (t2 == null && t1 != null) return -1;
                            if (t1 == t2) return 0;
                            if (t1.LongRhythmIsValid && !t2.LongRhythmIsValid) return -1;
                            if (t2.LongRhythmIsValid && !t1.LongRhythmIsValid) return 1;

                            return 0;
                        }
                    );
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void findSimilarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItemIndex < 0 || selectedItemIndex >= items.Count)
                return;
            try
            {
                if (findSimilarPropertiesForm.ShowDialog() != DialogResult.OK)
                    return;

                SimilarityOptions Options = new SimilarityOptions()
                    {
                        AmpEnvelope = findSimilarPropertiesForm.cbAmpEnvelope.Checked,
                        Intensity = findSimilarPropertiesForm.cbIntensity.Checked,
                        LongRhythm = findSimilarPropertiesForm.cbLongRhythm.Checked,
                        ShortRhythm = findSimilarPropertiesForm.cbShortRhythm.Checked,
                        VolumeDistr = findSimilarPropertiesForm.cbVolumeDistr.Checked
                    };

                items.SearchBy<SearchBySimilarity>(selectedItemIndex, Options);
                
                ScrollToTopLeft();
                selectedItemIndex = 0;
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sortByNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                items.Sort(
                        (a1, a2) =>
                            {
                                return a1.ShortName.CompareTo(a2.ShortName);
                        }
                    );
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
