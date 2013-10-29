using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using HoloKernel;
using HoloDB;

namespace HoloKernel
{
    /// <summary>
    /// Decodes audio sources, calculates chracteristics of signal.
    /// Results of processing are storing in Audio items.
    /// </summary>
    public class AudioProcessor : IAudioProcessor
    {
        protected Factory factory;
        protected Queue<Audio> sourceQueue = new Queue<Audio>();
        protected int itemsCount;

        /// <summary>
        /// Requested bitrate of signal after decoding
        /// </summary>
        public float TargetBitrate { get; set; }
        /// <summary>
        /// Progress of processing event
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> Progress;

        public AudioProcessor(Factory factory)
        {
            this.factory = factory;
            TargetBitrate = 24000;//8000
        }

        /// <summary>
        /// Process list of Audio items
        /// </summary>
        /// <param name="list"></param>
        public virtual void Process(IList<Audio> list)
        {
            if (list.Count == 0)
                return;

            lock (sourceQueue)
            foreach (var item in list)
                sourceQueue.Enqueue(item);

            itemsCount += list.Count;

            using (var decoder = factory.CreateAudioDecoder())
            if (decoder.AllowsMultithreading)
                ProcessMultithreading(decoder);
            else
                Process(decoder);

            OnProgress(new ProgressChangedEventArgs(100, null));
        }

        protected virtual void OnProgress(ProgressChangedEventArgs e)
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

        protected virtual void ProcessMultithreading(IAudioDecoder decoder)
        {
            var threads = new List<Thread>();
            //create threads
            var threadCount = Environment.ProcessorCount;
            for(int i = 0;i<threadCount;i++)
            {
                var t = new Thread(() => Process(decoder)) {IsBackground = true};
                t.Start();
                threads.Add(t);
            }

            while (sourceQueue.Count > 0)
                Thread.Sleep(100);

            foreach (var t in threads)
                t.Join();
        }

        /// <summary>
        /// Gets audio from queue and process it
        /// </summary>
        protected virtual void Process(IAudioDecoder decoder)
        {
            int counter = 0;
            Audio item;
            while((item = GetItemFromQueue())!=null)
            try
            {
                counter++;
                //decode audio source to samples and mp3 tags extracting
                AudioInfo info = null;
                using (var stream = item.GetSourceStream())
                    info = decoder.Decode(stream, TargetBitrate, item.GetSourceExtension());

                //normalize volume level
                info.Samples.Normalize();

                //launch sample processors
                foreach (var processor in factory.CreateSampleProcessors())
                    try
                    {
                        processor.Process(item, info);
                    }catch(Exception ex)
                    {
                        /*ignore errors of processors*/
                        Console.WriteLine(ex.Message);
                    }

                OnProgress(new ProgressChangedEventArgs(100 * (itemsCount - sourceQueue.Count) / itemsCount, null));
                item.State = AudioState.Processed;
            }
            catch (Exception ex)
            {
                item.State = AudioState.Bad;
            }
        }

        protected virtual Audio GetItemFromQueue()
        {
            lock(sourceQueue)
            {
                if (sourceQueue.Count > 0)
                    return sourceQueue.Dequeue();
            }

            return null;
        }
    }
}
