using System.IO;
using System.IO.Compression;
using System.Text;

namespace HoloKernel
{
    public class DB
    {
        public AudioSources AudioSources { get; set; }

        public bool IsChanged { get; set; }

        public DB()
        {
            AudioSources = new AudioSources();
        }

        public void Save(string fullPath)
        {
            var tempFullPath = fullPath + ".temp";

            using (var fs = new FileStream(tempFullPath, FileMode.Create, FileAccess.Write))
            using (var zip = new GZipStream(fs, CompressionMode.Compress, false))
            using (var buff = new BufferedStream(zip, 8192))
            using (var bw = new BinaryWriter(buff, Encoding.UTF8))
            {
                bw.Write((byte)0);//version
                AudioSources.Store(bw);
            }

            if (File.Exists(fullPath))
                File.Delete(fullPath);
            File.Move(tempFullPath, fullPath);
        }

        public static DB Load(string fileName)
        {
            var result = new DB();

            if (File.Exists(fileName))
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var zip = new GZipStream(fs, CompressionMode.Decompress, false))
            using (var br = new BinaryReader(zip, Encoding.UTF8))
                try
                {
                    br.ReadByte();//version
                    result.AudioSources.Load(br);
                }
                catch (EndOfStreamException) {/*end of stream*/}

            return result;
        }
    }
}
