using System;
using System.Collections.Generic;
using System.Text;
using HoloDB;
using HoloKernel;

namespace HoloProcessors
{
    public class SimpleDescriptorsBuilder : ISampleProcessor
    {
        public void Process(Audio item, AudioInfo info)
        {
            /*
            var volDesc = new VolumeDescriptor();
            volDesc.Build(Abs(info.Samples.Values));

            item.Data.Add(volDesc);*/
        }
    }
}
