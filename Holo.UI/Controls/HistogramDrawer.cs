using System.Drawing;
using HoloProcessors;

namespace Holo.UI.Controls
{
    public class CDFDrawer
    {
        public Color ForeColor = Color.Silver;

        public void Draw(Histogram hist, Graphics gr, Rectangle bounds)
        {
            var w = bounds.Width;
            var kx = 1f / bounds.Width;
            var ky = 1f * bounds.Height;
            var cy = bounds.Top + bounds.Height;
            var cx = bounds.Left;

            var ddd = hist[1f];

            var max = 0f;

            for (float i = 0; i <= 1; i += 1f / w)
            {
                var v = hist[i];
                if (v > max)
                    max = v;
            }

            ky = bounds.Height / max;

            using (var pen = new Pen(ForeColor))
                for (float i = 0; i <= 1; i += 1f/w)
                {
                    var x = i * w;
                    var y = hist[i] * ky;
                    gr.DrawLine(pen, cx + x, cy, cx + x, cy - y);
                }

            using (var pen = new Pen(ForeColor))
                gr.DrawLine(pen, cx, cy, cx + w, cy);
        }
    }
}
