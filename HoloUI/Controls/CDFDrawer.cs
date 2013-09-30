using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using HoloProcessors;

namespace HoloUI
{
    public class CDFDrawer
    {
        public Color ForeColor = Color.Silver;

        public void Draw(CDF cdf, Graphics gr, Rectangle bounds, bool differential = true)
        {
            var w = bounds.Width;
            var kx = 1f / bounds.Width;
            var ky = 1f * bounds.Height;
            var cy = bounds.Top + bounds.Height;
            var cx = bounds.Left;

            var diffs = new List<float>();
            var maxDiff = 0f;
            if(differential)
            for (float i = 0; i <= 1; i += 1f / w)
            {
                var d = cdf[i + 1f/w] - cdf[i];
                diffs.Add(d);
                if (d > maxDiff)
                    maxDiff = d;
            }

            using (var pen = new Pen(ForeColor))
                for (float i = 0; i <= 1; i += 1f/w)
                {
                    var x = i * w;
                    var y = (differential ? (diffs[(int)x]  / maxDiff) : cdf[i]) * ky;
                    gr.DrawLine(pen, cx + x, cy, cx + x, cy - y);
                }

            using (var pen = new Pen(ForeColor))
                gr.DrawLine(pen, cx, cy, cx + w, cy);
        }
    }
}
