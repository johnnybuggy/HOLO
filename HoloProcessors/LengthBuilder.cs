using System;
using HoloDB;
using HoloKernel;

namespace HoloProcessors
{
    public class AudioLengthDescriptorBuilder : ISampleProcessor
    {
        public void Process(Audio item, AudioInfo info)
        {
            //create new descriptor
            var desc = new AudioLengthDescriptor();

            //calc length of audio (sec)
            desc.Duration = (int)(info.Samples.Values.Length/info.Samples.Bitrate);

            //add desriptor to audio item
            item.Data.Add(desc);
        }
    }

    public class AudioLengthDescriptor : ICompareDescriptor, IDistanceDescriptor, ITextDescriptor, IStorable
    {
        /// <summary>
        /// Duration of audio (sec)
        /// </summary>
        public int Duration;

        public int Compare(ICompareDescriptor other)
        {
            return Duration.CompareTo((other as AudioLengthDescriptor).Duration);
        }

        public float Weight
        {
            get { return 1f; }
        }

        public float Distance(IDistanceDescriptor other)
        {
            return Math.Abs(Duration - (other as AudioLengthDescriptor).Duration);
        }

        public string Description
        {
            get { return string.Format("Length: {0}s", Duration ); }
        }

        public void Store(System.IO.BinaryWriter bw)
        {
            bw.Write((byte)0);//version
            bw.Write(Duration);
        }

        public void Load(System.IO.BinaryReader br)
        {
            br.ReadByte();//version
            Duration = br.ReadInt32();
        }
    }
}
