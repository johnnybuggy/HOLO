using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HoloKernel;
using HoloKernel;
using HoloUI.Controls;

namespace HoloUI
{
    public partial class AudioSourcesPanel : UserControl
    {
        public AudioSourcesPanel()
        {
            InitializeComponent();

            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        private const int itemWidth = 250;
        private const int itemHeight = 100;
        private const int itemPadding = 5;

        private int itemsPerRow;
        private int rowsCount;
        private int selectedItemIndex;

        private AudioSources items;

        private EnvelopeDrawer envelopeDrawer = new EnvelopeDrawer();
        private SamplesDrawer SamplesDrawer = new SamplesDrawer();

        public void Build(AudioSources items)
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
            return new Rectangle(itemBounds.Left + 22, itemBounds.Top + 18, 100, 32);
        }

        Rectangle GetTempogramRect(Rectangle itemBounds)
        {
            return new Rectangle(itemBounds.Left + 130, itemBounds.Top + 18, 100, 32);
        }

        private void DrawItem(Graphics graphics, int itemIndex,  Rectangle bounds)
        {
            if (itemIndex < 0 || itemIndex >= items.Count)
                return;
            var item = items[itemIndex];
            var color1 = Color.White;
            var color2 = Color.Silver;

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

            if (item.Envelope != null)
                envelopeDrawer.Draw(item.Envelope, graphics, GetEnvelopeRect(bounds));

            if (item.Tempogram != null)
                SamplesDrawer.DrawOneSide(item.Tempogram, graphics, GetTempogramRect(bounds), true);
        }

        public int PointToItemIndex(Point p)
        {
            var j = (p.X + HorizontalScroll.Value)/itemWidth;
            if (j >= itemsPerRow) return - 1;

            return (p.Y + VerticalScroll.Value)/itemHeight * itemsPerRow  + j;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

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
    }
}
