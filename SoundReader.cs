using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NAudio.Wave;

namespace HOLO
{
    class SoundReader
    {
        public Mp3FileReader FR;

        public string OpenFile(string path)
        {
            try
            {
                FR = new Mp3FileReader(path);
            }
            catch (Exception E)
            {
                return E.Message;
            }
            return "OK";
        }

        public bool CloseFile()
        {
            try
            {
                FR.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool CheckFile()
        {
            if ((FR.WaveFormat.SampleRate != 44100) || (FR.WaveFormat.BitsPerSample != 16))
                return false;

            return true;
        }

        public bool SetPosition(long samples)
        {
            try
            {
                FR.Seek(samples, SeekOrigin.Begin);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public double[][] GetNextChunk(int samplesDesired, Mp3FileReader fr, bool fullnormalize = false)
        {
            byte[] buffer = new byte[samplesDesired * 4];
            short[] left = new short[samplesDesired];
            //short[] right = new short[samplesDesired];
            double[] leftd = new double[samplesDesired];
            //double[] rightd = new double[samplesDesired];

            int bytesRead = 0;

            try
            {
                bytesRead = fr.Read(buffer, 0, 4 * samplesDesired);
            }
            catch
            {
                new Exception("An error occurred while reading file");
            }

            int index = 0;
            for (int sample = 0; sample < bytesRead / 4; sample++)
            {
                left[sample] = BitConverter.ToInt16(buffer, index); index += 2;
                //right[sample] = BitConverter.ToInt16(buffer, index); 
                index += 2;
            }

            if (fullnormalize)
                leftd = Utilities.NormalizeFull(ref left, left.Length);
            else
                leftd = Utilities.Normalize(ref left, left.Length);            
            
            return new double[][] { leftd, null }; //rightd };
        }
    }
}
