using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HoloDB;
using HoloKernel;

namespace HoloProcessors
{
    /// <summary>
    /// Describes tempo of sound
    /// </summary>
    public class Tempogram : CDF, ITextDescriptor, ICompareDescriptor
    {
        private const int size = 32;
        /// <summary>
        /// Tempo (Hz)
        /// </summary>
        public float Tempo { get; internal set; }

        /// <summary>
        /// Rhythm level (power of Rhythm of sound)
        /// </summary>
        public float RhythmLevel { get; internal set; }

        public Tempogram():base(size)
        {   
        }

        public override void Store(BinaryWriter bw)
        {
            bw.Write((byte) 0);//version
            bw.Write(Tempo);
            bw.Write(RhythmLevel);
            base.Store(bw);
        }

        public override void Load(BinaryReader br)
        {
            br.ReadByte();//version
            Tempo = br.ReadSingle();
            RhythmLevel = br.ReadSingle();
            base.Load(br);
        }

        public string Description
        {
            //get { return string.Format("Tmp {0:N2} Lvl {1:.000}", Tempo, RhythmLevel); }
            get { return string.Format("Tmp {0:N2}", Tempo); }
        }

        public int Compare(ICompareDescriptor other)
        {
            if (other == null) return 1;
            return Tempo.CompareTo((other as Tempogram).Tempo);
        }
    }
}
