using System;
using System.Collections.Generic;
using Holo.Core;
using HoloProcessors;

namespace Holo.UI
{
    /// <summary>
    /// Implements factory of signal processing classes.
    /// </summary>
    public class DefaultFactory : Factory
    {
        public override IAudioDecoder CreateAudioDecoder()
        {
            return new HoloBassDecoder.BassDecoder();
        }

        /// <summary>
        /// Enumerates signal samples processors
        /// </summary>
        public override IEnumerable<ISampleProcessor> CreateSampleProcessors()
        {
            //build envelope
            yield return new EnvelopeProcessor(this);
            //build tempogram
            yield return new TempogramProcessor(this);
        }

        public override Dictionary<int, Type> GetKnownTypes()
        {
            var Result = new Dictionary<int, Type>();

            Result.Add(0, typeof(Envelope));
            Result.Add(1, typeof(Tempogram));
            Result.Add(2, typeof(VolumeDescriptor));

            return Result;
        }
    }
}
