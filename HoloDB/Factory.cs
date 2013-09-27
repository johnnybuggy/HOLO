using System;
using System.Collections.Generic;
using System.Text;

namespace HoloKernel
{
    public abstract class Factory
    {
        public abstract IAudioDecoder CreateAudioDecoder();
        public virtual IAudioProcessor CreateAudioProcessor()
        {
            return new AudioProcessor(this);
        }

        public virtual IResampler CreateResampler()
        {
            return new Resampler();
        }
    }
}
