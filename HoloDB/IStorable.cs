using System.IO;

namespace HoloDB
{
    /// <summary>
    /// Can save/load himself into/from stream
    /// </summary>
    public interface IStorable
    {
        void Store(BinaryWriter bw);
        void Load(BinaryReader br);
    }
}
