using System.Collections.Generic;
using System.IO;

namespace HoloDB
{
    /// <summary>
    /// List of Audio items
    /// </summary>
    public class Audios : List<Audio>, IStorable
    {
        public Audios(IEnumerable<Audio> Audios) : base(Audios)
        {
        }

        public Audios()
        {
        }

        public virtual void Store(BinaryWriter bw)
        {
            bw.Write((byte) 0);//version
            bw.Write(Count);
            foreach (var item in this)
                item.Store(bw);
        }

        public virtual void Load(BinaryReader br)
        {
            br.ReadByte();//version
            var count = br.ReadInt32();
            for(int i = 0; i<count;i++)
            {
                var item = new Audio();
                item.Load(br);
                Add(item);
            }
        }

        public Dictionary<string, int> GetIndexesByFullPath()
        {
            var result = new Dictionary<string, int>();
            for (int i = 0; i < Count; i++)
                result[this[i].FullPath] = i;

            return result;
        }

        public void RemoveRange(IEnumerable<Audio> items)
        {
            foreach (var item in items)
                Remove(item);
        }
    }
}