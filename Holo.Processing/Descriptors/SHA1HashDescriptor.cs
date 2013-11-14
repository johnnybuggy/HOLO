using System;
using System.IO;
using Holo.Core;
using HoloDB;

namespace Holo.Processing
{
    /// <summary>
    /// The SHA1HashDescriptor is used to annotate an <see cref="Audio"/> object with a SHA1 hash value, 
    /// which is represented by an instance of <see cref="SHA1Hash"/> class.
    /// </summary>
    public class SHA1HashDescriptor : ITextDescriptor, IStorable
    {
        public SHA1HashDescriptor()
        {
        }

        public SHA1HashDescriptor(Audio audio)
        {
            if (audio == null)
            {
                throw new ArgumentNullException("audio");
            }

            Hash = SHA1Hash.FromStream(audio.GetSourceStream());
        }

        public SHA1Hash Hash
        {
            get;
            private set;
        }

        public string Description
        {
            get
            {
                if (Hash != null)
                {
                    return Hash.ToString();
                }
                else
                {
                    return "Hash is not initialized.";
                }
            }
        }

        public void Store(BinaryWriter bw)
        {
            bw.Write((byte)0); // version
            bw.Write(Hash.GetBytes());
        }

        public void Load(BinaryReader br)
        {
            br.ReadByte(); // version
            byte[] Bytes = br.ReadBytes(20);

            Hash = SHA1Hash.FromBytes(Bytes);
        }
    }
}
