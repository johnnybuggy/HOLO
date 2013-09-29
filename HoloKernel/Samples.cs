using System;

namespace HoloKernel
{
    public class Samples : IStorable
    {
        public float[] Values;
        public float Bitrate;

        /// <summary>
        /// Returns sample for any point (with linear interpolation)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual float this[float index]
        {
            get { 
                var intIndex = (int) index;
                if (intIndex < 0) return 0;
                if (index > Values.Length) return 0;
                if (intIndex == Values.Length) return Values[intIndex];

                var rest = index - intIndex;
                return (1 - rest)*Values[intIndex] + rest*Values[intIndex + 1];//linear interpolation
            }
        }

        /// <summary>
        /// Normalizes amplitude (by default from -1 to +1)
        /// </summary>
        unsafe public void Normalize(float k = 1f)
        {
            var l = Values.Length;

            //find abs max sample
            float max = 0;
            fixed (float* valuesPtr = Values)
            {
                var ptr = valuesPtr;
                for (int i = 0; i < l; i++)
                {
                    var v = *ptr > 0 ? *ptr : -*ptr;
                    if (v > max)
                        max = v;
                    ptr++;
                }
            }

            //normalize
            if (max > float.Epsilon)
                Scale(k/max);
        }

        /// <summary>
        /// Scales amplitude of samples
        /// </summary>
        unsafe public void Scale(float volumeKoeff)
        {
            var l = Values.Length;

            fixed (float* valuesPtr = Values)
            {
                var ptr = valuesPtr;
                for (int i = 0; i < l; i++)
                {
                    *ptr *= volumeKoeff;
                    ptr++;
                }
            }
        }

        public void Store(System.IO.BinaryWriter bw)
        {
            bw.Write((byte)0);
            bw.Write(Bitrate);
            if (Values == null || Values.Length == 0)
                bw.Write((int) 0);
            else
            {
                bw.Write((int)Values.Length);
                foreach (var v in Values)
                    bw.Write(v);
            }
        }

        public void Load(System.IO.BinaryReader br)
        {
            br.ReadByte();//version
            Bitrate = br.ReadSingle();
            var count = br.ReadInt32();
            Values = new float[count];
            for (int i = 0; i < count; i++)
                Values[i] = br.ReadSingle();
        }

        public Samples Clone()
        {
            var result = new Samples();
            result.Bitrate = Bitrate;
            result.Values = new float[Values.Length];
            Array.Copy(Values, result.Values, Values.Length);

            return result;
        }
    }
}