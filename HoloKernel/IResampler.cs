using System;
using System.Collections.Generic;
using System.Text;

namespace HoloKernel
{
    public interface IResampler
    {
        Samples Resample(Samples source, float targetBitrate);
    }
}
