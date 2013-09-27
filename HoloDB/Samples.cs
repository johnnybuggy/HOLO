namespace HoloKernel
{
    public class Samples
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
        /// Normalizes amplitude to 1
        /// </summary>
        unsafe public void Normalize()
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
                Scale(1/max);
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
    }
}