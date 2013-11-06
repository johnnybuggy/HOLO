using HoloDB;

namespace Holo.Core
{
    public interface ISampleProcessor
    {
        void Process(Audio item, AudioInfo info);
    }
}
