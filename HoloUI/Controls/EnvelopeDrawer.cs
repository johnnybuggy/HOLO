using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace HoloUI.Controls
{
    class EnvelopeDrawer
    {
        public Color ForeColor = Color.Silver;

        public void DrawEnvelope(byte[] packedEnvelope, Graphics gr, Rectangle bounds)
        {
            var kx = bounds.Width/(packedEnvelope.Length*2);
            var ky = bounds.Height/(16*2);
            var cy = bounds.Top + bounds.Height/2;
            var cx = bounds.Left;

            using(var pen = new Pen(ForeColor))
            for(int i=0;i<packedEnvelope.Length;i++)
            {
                var x = i*kx;
                var y = (packedEnvelope[i] >> 4) * ky;
                gr.DrawLine(pen, cx + x, cy + y, cx + x, cy - y);

                x = (i + 1) * kx;
                y = (packedEnvelope[i] & 0xf) * ky;
                gr.DrawLine(pen, cx + x, cy + y, cx + x, cy - y - 1);
            }
        }
    }
}
