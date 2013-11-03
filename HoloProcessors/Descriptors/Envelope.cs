using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Holo.Core;
using HoloDB;

namespace HoloProcessors
{
    /// <summary>
    /// Stores envelope in packed array
    /// </summary>
    public class Envelope : IStorable
    {
        byte[] packedValues;

        public Envelope()
        {
        }

        public Envelope(Samples samples)
        {
            var values = samples.Values;
            //pack
            packedValues = new byte[values.Length / 2];
            for (int i = 0; i < values.Length; i += 2)
            {
                var v1 = (int)(16 * values[i]);
                var v2 = (int)(16 * values[i + 1]);
                if (v1 > 15) v1 = 15;
                if (v2 > 15) v2 = 15;
                packedValues[i / 2] = (byte)((v1 << 4) + v2);
            }
        }

        public int Length 
        { 
            get { return packedValues.Length*2; }
        }

        public float this[int index]
        {
            get
            {
                var v = packedValues[index/2];
                if (index % 2 == 0)
                    return (v >> 4) / 16f;
                else
                    return (v & 0x0f) / 16f;
            }
        }

        public void Store(BinaryWriter bw)
        {
            bw.Write((byte) 0);//version
            bw.Write(packedValues.Length);
            foreach (var v in packedValues)
                bw.Write(v);
        }

        public void Load(BinaryReader br)
        {
            br.ReadByte();//version
            var count = br.ReadInt32();
            packedValues = new byte[count];
            for (int i = 0; i < count; i++)
                packedValues[i] = br.ReadByte();
        }
    }
}
