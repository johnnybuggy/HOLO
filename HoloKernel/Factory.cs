using System;
using System.Collections.Generic;
using System.Text;

namespace HoloKernel
{
    /// <summary>
    /// Factory of main classes of signal processing
    /// </summary>
    public abstract class Factory
    {
        /// <summary>
        /// Creates audio decoder
        /// </summary>
        public abstract IAudioDecoder CreateAudioDecoder();

        /// <summary>
        /// Create processor of signal
        /// </summary>
        public virtual IAudioProcessor CreateAudioProcessor()
        {
            return new AudioProcessor(this);
        }

        /// <summary>
        /// Ctrates resampler
        /// </summary>
        public virtual IResampler CreateResampler()
        {
            return new Resampler();
        }

        /// <summary>
        /// Enumerates processors of samples of signal
        /// </summary>
        public abstract IEnumerable<ISampleProcessor> CreateSampleProcessors();
    }
}
