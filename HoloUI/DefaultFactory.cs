using System;
using System.Collections.Generic;
using System.Text;
using HoloKernel;
using HoloProcessors;

namespace HoloUI
{
    /// <summary>
    /// Implements factory of classes of processing of signals
    /// </summary>
    public class DefaultFactory : Factory
    {
        public override IAudioDecoder CreateAudioDecoder()
        {
            return new HoloBassDecoder.BassDecoder();
        }

        /// <summary>
        /// Enumerates processors of samples of signal
        /// </summary>
        public override IEnumerable<ISampleProcessor> CreateSampleProcessors()
        {
            //build envelope
            yield return new EnvelopeBuilder(this);
            //build tempogram
            yield return new TempogramBuilder(this);
            //
            yield return new SimpleDescriptorsBuilder();
        }

        public static Dictionary<int, Type> GetWellKnownTypes()
        {
            var res = new Dictionary<int, Type>();

            res.Add(0, typeof(Envelope));
            res.Add(1, typeof(Tempogram));
            res.Add(2, typeof(VolumeDescriptor));
            res.Add(3, typeof(SpectrumDescriptor));

            return res;
        }
    }
}
