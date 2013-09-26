using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Meta.Numerics.SignalProcessing;
using Meta.Numerics;

namespace HOLO
{
    class SoundProcessing
    {
        public static List<double> AmplitudeFromLR(int snapsize, double[] l, double[] r, double lowpass = 2.5, Dictionary<int, double> mw = null)
        {
            FourierTransformer ft = new FourierTransformer(snapsize);
            var xxa = new Complex[l.Length];

            if (mw.Count == l.Length)
                for (int j = 0; j < l.Length; j++)
                    xxa[j] = new Complex(l[j] * mw[j], r[j] * mw[j]);
            else
                return null;

            var ftt = ft.Transform(xxa).Take((int)(l.Length / lowpass)); // TAKING ONLY LEFT HALF OF FFT RESULT

            List<double> pow_re_im = new List<double>();
            //ftt.ToList().ForEach(t => pow_re_im.Add(Math.Log(ComplexMath.Abs(t))));
            //ftt.ToList().ForEach(t => pow_re_im.Add(Math.Sqrt(Math.Max(Math.Log(ComplexMath.Abs(t)),0))));
            ftt.ToList().ForEach(t => pow_re_im.Add(Math.Pow(Math.Max(Math.Log(ComplexMath.Abs(t)), 0), 2)));

            return pow_re_im;
        }

        public static List<double> AmplitudeFromLR2(int snapsize, double[] l, int i_skip, int i_count, double lowpass = 2.5, Dictionary<int, double> mw = null)
        {
            FourierTransformer ft = new FourierTransformer(snapsize);
            var xxa = new Complex[i_count];

            if (mw.Count == i_count)
                for (int j = 0; j < i_count; j++)
                    xxa[j] = new Complex(l[j + i_skip] * mw[j], l[j + i_skip] * mw[j]);
            else
                return null;

            var ftt = ft.Transform(xxa).Take((int)(i_count / (2 * lowpass))); // TAKING ONLY LEFT HALF OF FFT RESULT

            List<double> pow_re_im = new List<double>();
            //ftt.ToList().ForEach(t => pow_re_im.Add(Math.Log(ComplexMath.Abs(t))));
            //ftt.ToList().ForEach(t => pow_re_im.Add(Math.Sqrt(Math.Max(Math.Log(ComplexMath.Abs(t)),0))));
            ftt.ToList().ForEach(t => pow_re_im.Add(Math.Pow(Math.Max(Math.Log(ComplexMath.Abs(t)), 0), 2)));

            return pow_re_im;
        }
    }
}
