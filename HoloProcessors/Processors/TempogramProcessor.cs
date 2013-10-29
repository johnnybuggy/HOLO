using System;
using System.Collections.Generic;
using System.Text;
using HoloDB;
using HoloKernel;

namespace HoloProcessors
{
    public class TempogramProcessor : ISampleProcessor
    {
        private const float maxRithmDuration = 6;//seconds
        private const float minAmplitudeChangeForIntensityRate = 0.2f;
        private Factory factory;

        public TempogramProcessor(Factory factory)
        {
            this.factory = factory;
        }

        public void Process(Audio item, AudioInfo info)
        {
            var tempogram = new Tempogram();

            var s = info.Samples;

            s = new EnvelopeProcessor(factory).Build(info.Samples, 32, false);
            var s2 = new Samples() { Values = new float[s.Values.Length], Bitrate = s.Bitrate };
            var intensity = 0; 

            for (int i = 0; i < s.Values.Length - 1; i++)
            {
                var d = s.Values[i + 1] - s.Values[i];
                var dd = d > 0 ? d : 0;
                s.Values[i] = dd;
                s2.Values[i] = d;
                if (d > minAmplitudeChangeForIntensityRate)
                    intensity++;
            }
            s.Values[s.Values.Length - 1] = 0;
            s2.Values[s.Values.Length - 1] = 0;

            var time = s.Values.Length / s.Bitrate;//time of sound

            var maxShift = (int)(s.Values.Length * (maxRithmDuration / time));

            var autoCorr1 = AutoCorr(s.Values, maxShift, 5);
            var autoCorr2 = AutoCorr(s2.Values, maxShift, 2);
            var l = (float)autoCorr1.Length;
            var k = Math.Log(2);
            var list1 = new List<KeyValuePair<float, float>>();
            var list2 = new List<KeyValuePair<float, float>>();
            for (int i = 0; i < l; i++)
            {
                var j = i / (float)l;
                j = (float)(Math.Log(j + 1) / k);
                list1.Add(new KeyValuePair<float, float>(j, autoCorr1[i]));

                var v = autoCorr2[i];
                list2.Add(new KeyValuePair<float, float>(j, v > 0 ? v : 0));
            }
            tempogram.LongTempogram.Build(list1);
            tempogram.ShortTempogram.Build(list2);

            tempogram.Intensity = (float)intensity / time;

            CalcTempo(tempogram);

            //save to audio item
            item.Data.Add(tempogram);
        }

        public static void CalcTempo(Tempogram tempogram)
        {
            var step = 1f/tempogram.LongTempogram.Size;
            var hist = tempogram.LongTempogram;
            //find main frequency 
            var max = 0f;
            var best = 0f;
            for(float i=0;i<=1;i+=step)
            {
                var v = hist[i] * (1 - 0.8f * i);
                if(v > max)
                {
                    max = v;
                    best = i;
                }
            }

            var k = Math.Log(2);
            //j = (float)(Math.Log(j + 1) / k);
            best = (float)Math.Exp(best*k) - 1;

            tempogram.LongRhythm = 1/(best * maxRithmDuration);//hz
            tempogram.LongRhythmLevel = max;
        }

        protected virtual float[] AutoCorr(float[] values, int maxShift, int pow = 2)
        {
            float[] autoCorr = new float[maxShift - 1];
            var l = values.Length;


            for (int shift = 1; shift < maxShift; shift++)
            {
                var sum = 0f;
                var count = values.Length - (pow - 1)*shift;
                for (int i = 0; i < count; i++)
                {
                    var v = values[i];

                    for (int p = 1; p < pow; p++)
                        v *= values[i + p * shift];

                    sum += v;
                }
                autoCorr[shift - 1] = sum;
            }
            return autoCorr;
        }
    }
}
