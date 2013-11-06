namespace Holo.Core
{
    public interface IResampler
    {
        Samples Resample(Samples source, float targetBitrate);
    }
}
