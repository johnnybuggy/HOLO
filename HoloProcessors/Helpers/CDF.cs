using System;
using System.Collections.Generic;
using System.Text;
using HoloDB;
using HoloKernel;

namespace HoloProcessors
{
    /// <summary>
    /// Cumulative distribution function
    /// </summary>
    public class CDF : IStorable, IDistanceDescriptor
    {
        private const int defaultSize = 16;
        private byte[] values;

        public CDF() : this(defaultSize)
        {
        }

        public CDF(int size)
        {
            values = new byte[size];
        }

        public int Size
        {
            get { return values.Length; }
        }

        /// <summary>
        /// Returns value for any point (with linear interpolation).
        /// Index must by from 0 to 1
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Value from 0 to 1</returns>
        public virtual float this[float index]
        {
            get
            {
                var size = Size;
                index = index*size;
                var intIndex = (int)index;
                if (index <= float.Epsilon) return 0f;
                if (index >= size) return 1f;

                float v1 = values[intIndex];
                float v2 = 255;
                if(intIndex < size - 1) v2 = values[intIndex + 1];

                float rest = index - intIndex;

                return ((1 - rest) * v1 + rest * v2)/255f;//linear interpolation
            }
        }

        public virtual void Build(IEnumerable<float> values)
        {
            //calc frequencies
            double sum = 0;
            var size = Size + 1;
            var frequencies = new float[size];
            foreach (var v in values)
            {
                var intIndex = (int)(v * size * 0.99f);
                if (intIndex >= size) intIndex = size - 1;
                if (intIndex < 0) intIndex = 0;
                frequencies[intIndex]++;
                sum ++;
            }

            double cum = 0.0;
            var s = Size;
            if(sum > float.Epsilon)
            for(int i=0;i<s;i++)
            {
                cum += frequencies[i] / sum;
                this.values[i] = (byte)(cum * 255);
            }
        }

        public virtual void Build(IEnumerable<KeyValuePair<float, float>> valueAndWeights)
        {
            //calc frequencies
            double sum = 0;
            var size = Size + 1;
            var frequencies = new float[size];
            foreach (var pair in valueAndWeights)
            {
                var intIndex = (int)(pair.Key * size * 0.99f);
                frequencies[intIndex] += pair.Value;
                sum += pair.Value;
            }

            double cum = 0.0;
            var s = Size;
            if (sum > float.Epsilon)
                for (int i = 0; i < s; i++)
                {
                    cum += frequencies[i] / sum;
                    this.values[i] = (byte)(cum * 255);
                }
        }

        public virtual void Store(System.IO.BinaryWriter bw)
        {
            bw.Write((byte) 0);//version
            bw.Write(values.Length);
            foreach (var v in values)
                bw.Write(v);
        }

        public virtual void Load(System.IO.BinaryReader br)
        {
            br.ReadByte();
            var count = br.ReadInt32();
            values = new byte[count];
            for (int i = 0; i < count; i++)
                values[i] = br.ReadByte();
        }

        public float Distance(IDistanceDescriptor other)
        {
            return Distance((CDF) other);
        }

        public float Distance(CDF other)
        {
            if (other == null)
                return 1;
            var size = Size;
            var res = 0f;
            if (size == other.Size)
                for (int i = 0; i < size; i++)
                    res += Math.Abs(values[i] - other.values[i])/255f;
            else
                for (int i = 0; i < size; i++)
                    res += Math.Abs(values[i]/255f - other[1f * i / size]);

            return res / size;
        }

        public float Weight
        {
            get { return 1f; }
        }
    }
}
