﻿using System;
using System.Collections.Generic;
using System.Text;
using HoloDB;
using HoloKernel;

namespace HoloProcessors
{
    /// <summary>
    /// Builds amplitude envelope
    /// </summary>
    public class EnvelopeBuilder : ISampleProcessor
    {
        private Factory factory;
        const int EnvelopeLength = 64;

        public EnvelopeBuilder(Factory factory)
        {
            this.factory = factory;   
        }

        /// <summary>
        /// Amplitude envelope builder
        /// </summary>
        public Samples Build(Samples source, int targetBitrate = 20)
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

        public virtual void Process(Audio item, AudioInfo info)
        {
            //build amplitude envelope
            var s = Build(info.Samples);
            //resample
            var resampler = factory.CreateResampler();
            var resampled = resampler.Resample(s, info.Samples.Bitrate * ((float)EnvelopeLength / info.Samples.Values.Length));
            //build packed array
            var envelope = new Envelope(resampled);
            //save into audio item
            item.Data.Add(envelope);
        }
    }
}
