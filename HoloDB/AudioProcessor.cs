using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using HoloKernel;

namespace HoloKernel
{
    public class AudioProcessor : IAudioProcessor
    {
        protected Factory factory;

        public event EventHandler<ProgressChangedEventArgs> Progress;

        public AudioProcessor(Factory factory)
        {
            this.factory = factory;
            TargetBitrate = 8000;
            EnvelopeLength = 128;
        }

        public virtual float TargetBitrate { get; set; }
        public virtual int EnvelopeLength { get; set; }

        private Queue<AudioSource> sourceQueue = new Queue<AudioSource>();
        private int itemsCount;

        public virtual void Process(IList<AudioSource> list)
        {
            if (list.Count == 0)
                return;

            lock (sourceQueue)
            foreach (var item in list)
                sourceQueue.Enqueue(item);

            itemsCount += list.Count;

            var decoder = factory.CreateAudioDecoder();

            if (decoder.AllowsMultithreading)
                ProcessMultithreading(decoder);
            else
                Process(decoder);

            OnProgress(new ProgressChangedEventArgs(100, null));

            if (decoder is IDisposable)
                (decoder as IDisposable).Dispose();
        }

        private void OnProgress(ProgressChangedEventArgs e)
        {
            if (Progress != null)
                try
                {
                    Progress(this, e);
                }catch
                {
                    //ignore handler exceptions
                }
        }

        private void ProcessMultithreading(IAudioDecoder decoder)
        {
            //create threads
            var threadCount = Environment.ProcessorCount;
            for(int i = 0;i<threadCount;i++)
            {
                var t = new Thread(() => Process(decoder)) {IsBackground = true};
                t.Start();
            }

            while (sourceQueue.Count > 0)
                Thread.Sleep(100);
        }

        private void Process(IAudioDecoder decoder)
        {
            int counter = 0;
            AudioSource item;
            while((item = GetItemFromQueue())!=null)
            try
            {
                counter++;
                //decode audio source to samples and mp3 tags extracting
                AudioSourceInfo info = null;
                using (var stream = item.GetSourceStream())
                    info = decoder.Decode(stream, TargetBitrate, item.GetSourceExtension());

                BuildEnvelope(item, info, EnvelopeLength);

                OnProgress(new ProgressChangedEventArgs(100 * (itemsCount - sourceQueue.Count) / itemsCount, null));
                item.State = AudioSourceState.Processed;
            }
            catch (Exception ex)
            {
                item.State = AudioSourceState.Bad;
            }
        }

        AudioSource GetItemFromQueue()
        {
            lock(sourceQueue)
            {
                if (sourceQueue.Count > 0)
                    return sourceQueue.Dequeue();
            }

            return null;
        }

        protected virtual void BuildEnvelope(AudioSource item, AudioSourceInfo info, int envelopeLength)
        {
            //build amplitude envelope
            var eb = new EnvelopeBuilder();
            var s = eb.BuildEnvelope(info.Samples);
            //resample
            var resampled = factory.CreateResampler().Resample(s, info.Samples.Bitrate * ((float)envelopeLength / info.Samples.Values.Length));
            resampled.Normalize();
            var values = resampled.Values;
            //pack
            var packed = new byte[values.Length / 2];
            for(int i=0;i<values.Length;i+=2)
            {
                var v1 = (int) (16*values[i]);
                var v2 = (int) (16*values[i + 1]);
                if (v1 > 15) v1 = 15;
                if (v2 > 15) v2 = 15;
                packed[i/2] = (byte)((v1 << 4) + v2);
            }

            item.Envelope = packed;
        }
    }
}
