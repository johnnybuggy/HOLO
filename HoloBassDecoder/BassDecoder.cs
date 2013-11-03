using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Holo.Core;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;

namespace HoloBassDecoder
{
    public class BassDecoder : IAudioDecoder, IDisposable
    {
        public BassDecoder()
        {
            BassNet.Registration("pavel_torgashov@mail.ru", "2X25317232238");

            if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_LATENCY, IntPtr.Zero))
            {
                BASS_INFO info = new BASS_INFO();
                Bass.BASS_GetInfo(info);
            }
            else
                throw new Exception("Bass_Init error!");
        }

        public AudioInfo Decode(System.IO.Stream stream, float targetBitrate, string fileExt)
        {
            var length = stream.Length;
            byte[] source = new byte[length];
            // read the file into the buffer
            stream.Read(source, 0, (int)length);

            // now create a pinned handle, so that the Garbage Collector will not move this object
            var _hGCFile = GCHandle.Alloc(source, GCHandleType.Pinned);
            try
            {
                var buffer = ReadMonoFromStream(_hGCFile.AddrOfPinnedObject(), source.Length, (int) targetBitrate, -1, 0);

                var result = new AudioInfo();
                result.Samples = new Samples() { Values = buffer, Bitrate = (int)targetBitrate };
                return result;
            }
            catch(Exception ex)
            {
                return null;
            }
            finally
            {
                _hGCFile.Free();
            }
        }

        public bool AllowsMultithreading
        {
            get { return true; }
        }

        /// <summary>
        /// Read mono from stream
        /// </summary>
        float[] ReadMonoFromStream(IntPtr streamPtr, int length, int samplerate, int milliseconds, int startmillisecond)
        {
            int totalmilliseconds = milliseconds <= 0 ? Int32.MaxValue : milliseconds + startmillisecond;
            float[] data = null;
            //create streams for re-sampling
            int stream = Bass.BASS_StreamCreateFile(streamPtr, 0, length,
                BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_MONO |
                BASSFlag.BASS_SAMPLE_FLOAT); //Decode the stream

            int mixerStream = 0;

            try
            {
                if (stream == 0)
                    throw new Exception(Bass.BASS_ErrorGetCode().ToString());
                mixerStream = BassMix.BASS_Mixer_StreamCreate(samplerate, 1,
                                                                  BASSFlag.BASS_STREAM_DECODE |
                                                                  BASSFlag.BASS_SAMPLE_MONO |
                                                                  BASSFlag.BASS_SAMPLE_FLOAT);
                if (mixerStream == 0)
                    throw new Exception(Bass.BASS_ErrorGetCode().ToString());
                if (BassMix.BASS_Mixer_StreamAddChannel(mixerStream, stream, BASSFlag.BASS_MIXER_FILTER))
                {
                    int bufferSize = samplerate*10*4; /*read 10 seconds at each iteration*/
                    float[] buffer = new float[bufferSize];
                    List<float[]> chunks = new List<float[]>();
                    int size = 0;
                    while ((float) (size)/samplerate*1000 < totalmilliseconds)
                    {
                        //get re-sampled/mono data
                        int bytesRead = Bass.BASS_ChannelGetData(mixerStream, buffer, bufferSize);
                        if (bytesRead == 0)
                            break;
                        float[] chunk = new float[bytesRead/4]; //each float contains 4 bytes
                        Array.Copy(buffer, chunk, bytesRead/4);
                        chunks.Add(chunk);
                        size += bytesRead/4; //size of the data
                    }

                    if ((float) (size)/samplerate*1000 < (milliseconds + startmillisecond))
                        return null; /*not enough samples to return the requested data*/
                    int start = (int) ((float) startmillisecond*samplerate/1000);
                    int end = (milliseconds <= 0)
                                  ? size
                                  : (int) ((float) (startmillisecond + milliseconds)*samplerate/1000);
                    data = new float[size];
                    int index = 0;
                    /*Concatenate*/
                    foreach (float[] chunk in chunks)
                    {
                        Array.Copy(chunk, 0, data, index, chunk.Length);
                        index += chunk.Length;
                    }
                    /*Select specific part of the song*/
                    if (start != 0 || end != size)
                    {
                        float[] temp = new float[end - start];
                        Array.Copy(data, start, temp, 0, end - start);
                        data = temp;
                    }
                }
                else
                    throw new Exception(Bass.BASS_ErrorGetCode().ToString());
            }finally
            {
                Bass.BASS_StreamFree(stream);
                Bass.BASS_StreamFree(mixerStream);
            }
            return data;
        }

        public void Dispose()
        {
            Bass.BASS_Free();
        }
    }
}
