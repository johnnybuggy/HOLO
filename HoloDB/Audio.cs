using System;
using System.Collections.Generic;
using System.IO;

namespace HoloDB
{
    /// <summary>
    /// Audio item
    /// </summary>
    public class Audio : IStorable
    {
        /// <summary>
        /// Full path (or URL) to audio source
        /// </summary>
        public virtual string FullPath { get; set; }

        /// <summary>
        /// Short name
        /// </summary>
        public virtual string ShortName
        {
            get { return Path.GetFileName(FullPath); }
        }

        /// <summary>
        /// Processing state
        /// </summary>
        public virtual AudioState State { get; set; }

        /// <summary>
        /// User Data
        /// </summary>
        public List<IStorable> Data { get; protected set; }

        public Audio()
        {
            Data = new List<IStorable>();
        }

        public virtual void Store(BinaryWriter bw)
        {
            bw.Write((byte)0);//version
            bw.Write(FullPath ?? "");
            bw.Write((byte) State);
            //save Data
            bw.Write(Data.Count);
            foreach (var data in Data)
            {
                var id = DB.GetIdByType(data.GetType());//id of type
                bw.Write(id);
                data.Store(bw);
            }
        }

        public virtual void Load(BinaryReader br)
        {
            br.ReadByte();//version
            FullPath = br.ReadString();
            State = (AudioState) br.ReadByte();
            var count = br.ReadInt32();
            Data = new List<IStorable>();
            for(int i=0;i<count;i++)
            {
                var id = br.ReadInt32();//read id ob type
                var type = DB.GetTypeById(id);//get type
                var data = (IStorable)type.GetConstructor(new Type[0]).Invoke(null);//create object
                data.Load(br);//object loads himself from stream
                Data.Add(data);
            }
        }

        public virtual Stream GetSourceStream()
        {
            return File.Open(FullPath, FileMode.Open, FileAccess.Read);
        }

        public virtual string GetSourceExtension()
        {
            return Path.GetExtension(FullPath);
        }

        /// <summary>
        /// Returns data by Id
        /// </summary>
        public virtual IStorable GetDataById(int id)
        {
            var type = DB.GetTypeById(id);
            foreach (var data in Data)
                if (data.GetType() == type)
                    return data;

            return null;
        }

        /// <summary>
        /// Returns data by given type
        /// </summary>
        public virtual T GetData<T>()
        {
            foreach (var data in Data)
                if (data.GetType() == typeof(T))
                    return (T)data;

            return default(T);
        }
    }

    public enum AudioState : byte
    {
        Unprocessed = 0,
        Processed = 1,
        Bad = 2
    }
}
