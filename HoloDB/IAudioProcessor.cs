using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HoloKernel
{
    public interface IAudioProcessor
    {
        event EventHandler<ProgressChangedEventArgs> Progress;
        void Process(IList<AudioSource> list);
    }
}