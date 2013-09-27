using System;
using System.Collections.Generic;
using System.Text;

namespace HoloKernel
{
    public class EnvelopeBuilder
    {
        unsafe public Samples BuildEnvelope(Samples source)
        {
            var values = source.Values;
            var resValues = new float[values.Length];

            Array.Copy(values, resValues, values.Length);

            fixed (float* resValuesPtr = resValues)
            {
                var ptr = resValuesPtr;
                for (int i = resValues.Length; i >= 0; i--)
                {
                    var f = *ptr;
                    if (f < 0)
                        *ptr = -f;

                    ptr++;
                }
            }

            return new Samples() { Bitrate = source.Bitrate, Values = resValues };
        }
    }
}
