using System;
using System.Collections.Generic;
using System.Text;
using HoloKernel;

namespace HoloUI
{
    public class DefaultFactory : Factory
    {
        public override IAudioDecoder CreateAudioDecoder()
        {
            return new HoloBassDecoder.BassDecoder();
        }
    }
}
