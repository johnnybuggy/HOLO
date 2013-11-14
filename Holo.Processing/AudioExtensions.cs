using Holo.Core;
using HoloDB;

namespace Holo.Processing
{
    public static class AudioExtensions
    {
        public static SHA1Hash GetHash(this Audio source)
        {
            return source.GetData<SHA1HashDescriptor>().Hash;
        }
    }
}
