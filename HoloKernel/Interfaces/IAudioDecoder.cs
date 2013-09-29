using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HoloKernel
{
    public interface IAudioDecoder : IDisposable
    {
        AudioInfo Decode(Stream stream, float targetBitrate, string fileExt);
        bool AllowsMultithreading { get; }
    }

    public class AudioInfo
    {
        public Samples Samples { get; set; }
        public Dictionary<string, object> Tags { get; private set; }

        public AudioInfo()
        {
            Tags = new Dictionary<string, object>();
        }
    }
}
