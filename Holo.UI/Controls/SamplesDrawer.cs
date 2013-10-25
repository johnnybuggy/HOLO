using System.Drawing;
using HoloKernel;

namespace Holo.UI.Controls
{
    class SamplesDrawer
    {
        public Color ForeColor = Color.Silver;

        public void Draw(Samples samples, Graphics gr, Rectangle bounds, bool normalize = false)
        {
            var kx = bounds.Width / samples.Values.Length;
            var ky = bounds.Height / 2;
            var cy = bounds.Top + bounds.Height/2;
            var cx = bounds.Left;

            var values = samples.Values;
            if (normalize)
            {
                var s = samples.Clone();
                s.Normalize();
                values = s.Values;
            }

            using(var pen = new Pen(ForeColor))
            for (int i = 0; i < values.Length; i++)
            {
                var x = i*kx;
                var y = values[i];
                if (y > 1) y = 1;
                if (y < -1) y = -1;
                y = y * ky;
                gr.DrawLine(pen, cx + x, cy + y, cx + x, cy - y);
            }
        }

        public void DrawOneSide(Samples samples, Graphics gr, Rectangle bounds, bool normalize = false)
        {
            var kx = bounds.Width / samples.Values.Length;
            var ky = bounds.Height;
            var cy = bounds.Bottom;
            var cx = bounds.Left;

            var values = samples.Values;
            if (normalize)
            {
                var s = samples.Clone();
                s.Normalize();
                values = s.Values;
            }

            using (var pen = new Pen(ForeColor))
            for (int i = 0; i < values.Length; i++)
            {
                var x = i * kx;
                var y = values[i];
                if (y > 1) y = 1;
                if (y < -1) continue;
                y = y * ky;
                gr.DrawLine(pen, cx + x, cy, cx + x, cy - y);
            }
        }
    }
}
