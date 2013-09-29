using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace HoloDB
{
    /// <summary>
    /// Database engine for saing and loading Audios
    /// </summary>
    public class DB
    {
        private static readonly Dictionary<int, Type> typesById = new Dictionary<int, Type>();
        private static readonly Dictionary<Type, int> idByType = new Dictionary<Type, int>();

        public Audios Audios { get; set; }

        public bool IsChanged { get; set; }

        public DB()
        {
            Audios = new Audios();
        }

        public static void Register(int id, Type type)
        {
            typesById.Add(id, type);
            idByType.Add(type, id);
        }

        public static Type GetTypeById(int id)
        {
            return typesById[id];
        }

        public static int GetIdByType(Type type)
        {
            return idByType[type];
        }

        public virtual void Save(string fullPath)
        {
            var tempFullPath = fullPath + ".temp";

            using (var fs = new FileStream(tempFullPath, FileMode.Create, FileAccess.Write))
            using (var zip = new GZipStream(fs, CompressionMode.Compress, false))
            using (var buff = new BufferedStream(zip, 8192))
            using (var bw = new BinaryWriter(buff, Encoding.UTF8))
            {
                bw.Write((byte)0);//version
                Audios.Store(bw);
            }

            if (File.Exists(fullPath))
                File.Delete(fullPath);
            File.Move(tempFullPath, fullPath);
        }

        /// <summary>
        /// Loads database form file to mempry.
        /// </summary>
        /// <param name="fileName">File name of database</param>
        /// <param name="wellKnownTypes">Dictionary of types, are contained in database (from outside libraries)</param>
        /// <returns></returns>
        /// <remarks>
        /// You need to register all outside types, will saved into database.
        /// For this you need to pass dictionary of these types.
        /// </remarks>
        public static DB Load(string fileName, Dictionary<int, Type> wellKnownTypes)
        {
            foreach (var pair in wellKnownTypes)
                Register(pair.Key, pair.Value);

            var result = new DB();

            if (File.Exists(fileName))
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var zip = new GZipStream(fs, CompressionMode.Decompress, false))
            using (var br = new BinaryReader(zip, Encoding.UTF8))
                try
                {
                    br.ReadByte();//version
                    result.Audios.Load(br);
                }
                catch (EndOfStreamException) {/*end of stream*/}

            return result;
        }
    }
}
