using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HoloKernel
{
    public interface IAudioDecoder
    {
        AudioSourceInfo Decode(Stream stream, float targetBitrate, string fileExt);
        bool AllowsMultithreading { get; }
    }

    public class AudioSourceInfo
    {
        public Samples Samples { get; set; }
        public Dictionary<string, object> Tags { get; private set; }

        public AudioSourceInfo()
        {
            Tags = new Dictionary<string, object>();
        }
    }
}
