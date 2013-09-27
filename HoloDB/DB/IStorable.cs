using System.IO;

namespace HoloKernel
{
    /// <summary>
    /// Can save himself into stream
    /// </summary>
    public interface IStorable
    {
        void Store(BinaryWriter bw);
        void Load(BinaryReader br);
    }
}
