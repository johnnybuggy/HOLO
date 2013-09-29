using System;
using System.Collections.Generic;
using System.Text;

namespace HoloKernel
{
    public class EnvelopeBuilder
    {
        /// <summary>
        /// Amplitude envelope
        /// </summary>
        public Samples BuildEnvelope(Samples source, int targetBitrate = 20)
        {
            float k = 1f * targetBitrate /source.Bitrate;
            var values = source.Values;
            var resValues = new float[(int)(values.Length * k)+1];

            for (int i = values.Length - 1; i >= 0; i--)
            {
                var ii = (int) (i*k);
                var prev = resValues[ii];
                var f = values[i];
                if(f < 0) f = -f;

                if (prev < f) resValues[ii] = f;
            }

            return new Samples() { Bitrate = targetBitrate, Values = resValues };
        }

        /// <summary>
        /// Signal power
        /// </summary>
        public Samples BuildPower(Samples source, int targetBitrate = 20)
        {
            float k = 1f * targetBitrate / source.Bitrate;
            var values = source.Values;
            var resValues = new float[(int)(values.Length * k) + 1];

            for (int i = values.Length - 1; i >= 0; i--)
            {
                var ii = (int)(i * k);
                var prev = resValues[ii];
                var f = values[i];
                f *= f;

                if (prev < f) resValues[ii] = f;
            }

            return new Samples() { Bitrate = targetBitrate, Values = resValues };
        }
    }
}
