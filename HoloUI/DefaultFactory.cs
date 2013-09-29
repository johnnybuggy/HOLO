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
        }
    }
}
