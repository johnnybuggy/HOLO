using System;
using System.Collections.Generic;
using System.Text;
using HoloDB;

namespace HoloKernel
{
    public interface ISampleProcessor
    {
        void Process(Audio item, AudioInfo info);
    }
}
