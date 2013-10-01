using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HoloDB;
using HoloKernel;
using System.Diagnostics;

namespace HoloProcessors
{

    public class Tester
    {
        private Factory factory;

        public Tester(Factory factory)
        {
            this.factory = factory;
        }

        public static void ToWav(string fileName, Samples samples)
        {
            samples.Normalize();

            var data = new short[samples.Values.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = (short)(samples.Values[i]*short.MaxValue);

            var sampleRate = (int)samples.Bitrate;

            using(Stream stream = File.OpenWrite(fileName))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                short frameSize = (short) (16/8);
                writer.Write(0x46464952);
                writer.Write(36 + data.Length*frameSize);
                writer.Write(0x45564157);
                writer.Write(0x20746D66);
                writer.Write(16);
                writer.Write((short) 1);
                writer.Write((short) 1);
                writer.Write(sampleRate);
                writer.Write(sampleRate*frameSize);
                writer.Write(frameSize);
                writer.Write((short) 16);
                writer.Write(0x61746164);
                writer.Write(data.Length*frameSize);
                for (int index = 0; index < data.Length; index++)
                {
                    foreach (byte element in BitConverter.GetBytes(data[index]))
                    {
                        stream.WriteByte(element);
                    }
                }
            }
        }

        public static void Out(float[] data)
        {
            Console.WriteLine();
            foreach (var v in data) Console.Write(v.ToString("0.00") + "\t");
        }

        public static void ToCSV(float[] array)
        {
            var sb = new StringBuilder();
            foreach (var v in array)
                sb.AppendLine(v.ToString("0.000"));
            var temp = Path.GetTempFileName() + ".csv";
            File.WriteAllText(temp, sb.ToString());
            System.Diagnostics.Process.Start(temp);
        }

        public static void ToCSVWithX(float[] array, float stepX = 1, float startX = 0)
        {
            var x = startX;
            var sb = new StringBuilder();
            foreach (var v in array)
            {
                sb.AppendLine(x.ToString("0.00000") + ";" + v.ToString("0.000"));
                x += stepX;
            }
            var temp = Path.GetTempFileName() + ".csv";
            File.WriteAllText(temp, sb.ToString());
            System.Diagnostics.Process.Start(temp);
        }

        public void TestFFT()
        {
            var fft = new FFTCalculator();
            var data = new float[] {1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0};
            Out(data);
            fft.RealFFT(data, true);
            data = fft.Norm(data);
            Out(data);
        }

        public void Process(Audio item)
        {
            using (IAudioDecoder decoder = factory.CreateAudioDecoder())
                Process(item, decoder);
        }

        internal void Process(Audio item, IAudioDecoder decoder)
        {
            AudioInfo info;
            using (var stream = item.GetSourceStream())
                info = decoder.Decode(stream, 8000, item.GetSourceExtension());

            info.Samples.Normalize();
            var s = info.Samples;

            s = new EnvelopeBuilder(factory).Build(info.Samples, 32, false);
            var s2 = new Samples() {Values = new float[s.Values.Length], Bitrate = s.Bitrate};
            
            for(int i=0;i<s.Values.Length - 1;i++)
            {
                var d = s.Values[i + 1] - s.Values[i];
                s.Values[i] = d > 0 ? d : 0;
                s2.Values[i] = d;
            }
            s.Values[s.Values.Length - 1] = 0;
            s2.Values[s.Values.Length - 1] = 0;


            //ToWav("c:\\temp2.wav", s);

            var values = s.Values;


            var time = values.Length / s.Bitrate;//time of sound

            var maxShift = (int)(values.Length* ( 5f / time));
            var autoCorr = AutoCorr(s2.Values, maxShift, 2);

            ToCSVWithX(autoCorr, 1f / s.Bitrate);

            maxShift = (int)(values.Length * (5f / time));
            autoCorr = AutoCorr(s.Values, maxShift, 5);

            ToCSVWithX(autoCorr, 1f / s.Bitrate);
        }

        protected virtual float[] AutoCorr(float[] values, int maxShift, int pow = 2)
        {
            float[] autoCorr = new float[maxShift - 1];
            var l = values.Length;


            for (int shift = 1; shift < maxShift; shift++)
            {
                var sum = 0f;
                var count = values.Length - (pow - 1) * shift;
                for (int i = 0; i < count; i++)
                {
                    var v = values[i];

                    for (int p = 1; p < pow; p++)
                        v *= values[i + p * shift];

                    sum += v;
                }
                autoCorr[shift - 1] = sum;
            }
            return autoCorr;
        }


        protected unsafe virtual float[] AutoCorrPow2(float[] values, int maxShift)
        {
            float[] autoCorr = new float[maxShift - 1];
            var l = values.Length;

            fixed(float* valuesPtr = @values)
            for (int shift = 1; shift < maxShift; shift++)
            {
                var p1 = valuesPtr;
                var p2 = valuesPtr + shift;

                var sum = 0f;
                var count = values.Length - maxShift;
                for (int i = 0; i < count; i++)
                {
                    sum += (*p1)*(*p2);
                    p1++;
                    p2++;
                }

                autoCorr[shift - 1] = sum;
            }

            return autoCorr;
        }

    }
}
