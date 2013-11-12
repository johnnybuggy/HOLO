using System;
using Holo.Core;
using HoloDB;

namespace Holo.Processing
{
    /// <summary>
    /// The SHA1HashProcessor annotates each audio with the SHA1 hash computed from the audio file content.
    /// </summary>
    public class SHA1HashProcessor : ISampleProcessor
    {
        public void Process(Audio item, AudioInfo info)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            SHA1HashDescriptor Descriptor = new SHA1HashDescriptor(item);

            item.Data.Add(Descriptor);
        }
    }
}
