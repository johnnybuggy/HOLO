using System.Drawing;
using Holo.Processing;

namespace Holo.UI.Controls
{
    class EnvelopeDrawer
    {
        public Color ForeColor = Color.Silver;

        public void Draw(Envelope envelope, Graphics gr, Rectangle bounds)
        {
            var kx = 1f * bounds.Width / envelope.Length;
            var ky = 1f * bounds.Height/2;
            var cy = bounds.Top + bounds.Height/2;
            var cx = bounds.Left;

            using(var pen = new Pen(ForeColor))
            for (int i = 0; i < envelope.Length; i++)
            {
                var x = i*kx;
                var y = envelope[i] * ky;
                gr.DrawLine(pen, cx + x, cy + y, cx + x, cy - y);
            }

            using (var pen = new Pen(ForeColor))
                gr.DrawLine(pen, cx, cy, cx + bounds.Width, cy);
        }
    }
}
