using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HoloDB;
using HoloKernel;

namespace HoloProcessors
{
    public class Tempogram : IStorable
    {
        public float NotesPerSecond { get; internal set; }
        public Samples Values { get; internal set; }

        public void Store(BinaryWriter bw)
        {
            bw.Write((byte) 0);//version
            bw.Write(NotesPerSecond);
            Values.Store(bw);
        }

        public void Load(BinaryReader br)
        {
            br.ReadByte();//version
            NotesPerSecond = br.ReadSingle();
            Values = new Samples();
            Values.Load(br);
        }
    }
}
