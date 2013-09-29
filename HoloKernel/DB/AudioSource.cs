using System.Collections.Generic;
using System.IO;

namespace HoloKernel
{
    public class AudioSources : List<AudioSource>, IStorable
    {
        public AudioSources(IEnumerable<AudioSource> audioSources) : base(audioSources)
        {
        }

        public AudioSources()
        {
        }

        public void Store(BinaryWriter bw)
        {
            bw.Write((byte) 0);//version
            bw.Write(Count);
            foreach (var item in this)
                item.Store(bw);
        }

        public void Load(BinaryReader br)
        {
            br.ReadByte();//version
            var count = br.ReadInt32();
            for(int i = 0; i<count;i++)
            {
                var item = new AudioSource();
                item.Load(br);
                Add(item);
            }
        }

        public Dictionary<string, int> GetIndexesByFullPath()
        {
            var result = new Dictionary<string, int>();
            for (int i = 0; i < Count; i++)
                result[this[i].FullPath] = i;

            return result;
        }

        public void RemoveRange(IEnumerable<AudioSource> items)
        {
            foreach (var item in items)
                Remove(item);
        }
    }

    public class AudioSource : IStorable
    {
        /// <summary>
        /// Full path (or URL) to audio source
        /// </summary>
        public virtual string FullPath { get; set; }

        /// <summary>
        /// Short name
        /// </summary>
        public virtual string ShortName
        {
            get { return Path.GetFileName(FullPath); }
        }

        /// <summary>
        /// Processing state
        /// </summary>
        public virtual AudioSourceState State { get; set; }

        /// <summary>
        /// Amplitude envelope (4 bit per sample, 128 samples = 32 bytes)
        /// </summary>
        public virtual byte[] Envelope { get;set;}

        public float NotesPerSecond { get; set; }

        public Samples Tempogram { get;set;}

        public virtual void Store(BinaryWriter bw)
        {
            bw.Write((byte)0);//version
            bw.Write(FullPath ?? "");
            bw.Write((byte) State);
            if (Envelope == null || Envelope.Length == 0)
                bw.Write((ushort)0);
            else
            {
                bw.Write((ushort) Envelope.Length);
                bw.Write(Envelope, 0, Envelope.Length);
            }

            bw.Write(NotesPerSecond);
            Tempogram.Store(bw);
        }

        public virtual void Load(BinaryReader br)
        {
            br.ReadByte();//version
            FullPath = br.ReadString();
            State = (AudioSourceState) br.ReadByte();
            var count = br.ReadUInt16();
            if(count > 0)
                Envelope = br.ReadBytes(count);

            NotesPerSecond = br.ReadSingle();
            Tempogram = new Samples();
            Tempogram.Load(br);
        }

        public virtual Stream GetSourceStream()
        {
            return File.Open(FullPath, FileMode.Open, FileAccess.Read);
        }

        public virtual string GetSourceExtension()
        {
            return Path.GetExtension(FullPath);
        }
    }

    public enum AudioSourceState : byte
    {
        Unprocessed = 0,
        Processed = 1,
        Bad = 2
    }
}
