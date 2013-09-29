using System;
using System.Collections.Generic;
using System.Text;
using HoloDB;
using HoloKernel;

namespace HoloProcessors
{
    public class TempogramBuilder : ISampleProcessor
    {
        private Factory factory;

        public TempogramBuilder(Factory factory)
        {
            this.factory = factory;
        }

        public void Process(Audio item, AudioInfo info)
        {
            var values = info.Samples.Values;
            var tempogram = new Tempogram();

            //build amplitude envelope
            var eb = new EnvelopeBuilder(factory);
            var s = eb.Build(info.Samples, 32);
            values = s.Values;

            //diff
            var diff = new float[values.Length - 10];
            for (int i = 0; i < diff.Length; i++)
            {
                var v = values[i + 1] - values[i];
                if (v > 0)
                    diff[i] = v;
            }

            values = diff;

            //count of notes per second
            var count = 0;
            foreach (var v in values)
                if (v > 0.2f)
                    count++;

            var time = values.Length / s.Bitrate;//time of sound
            tempogram.NotesPerSecond = count / time;

            var sec = 4;
            var maxShift = (int)(sec * s.Bitrate);
            values = AutoCorr(values, maxShift, 4);

            var tempogramValues = new Samples() { Bitrate = s.Bitrate, Values = values };
            tempogramValues = new Resampler().Resample(tempogramValues, 16 * values.Length / (s.Bitrate * sec));

            tempogram.Values = tempogramValues;

            //save to audio item
            item.Data.Add(tempogram);
        }

        protected virtual float[] AutoCorr(float[] values, int maxShift, int pow = 2)
        {
            float[] autoCorr = new float[maxShift - 1];
            var l = values.Length;


            for (int shift = 1; shift < maxShift; shift++)
            {
                var sum = 0f;
                for (int i = 0; i < values.Length - (pow - 1) * shift; i++)
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
