using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Holo.Core.Helpers
{
    internal static class FileScanner
    {
        public static IEnumerable<Info> Scan(string directory)
        {
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            WIN32_FIND_DATAW findData;
            IntPtr findHandle = INVALID_HANDLE_VALUE;

            try
            {
                findHandle = FindFirstFileW(directory + @"\*", out findData);
                if (findHandle != INVALID_HANDLE_VALUE)
                {

                    do
                    {
                        if (findData.cFileName == "." || findData.cFileName == "..") continue;

                        string fullpath = directory + (directory.EndsWith("\\") ? "" : "\\") + findData.cFileName;

                        bool isDir = false;

                        if ((findData.dwFileAttributes & FileAttributes.Directory) != 0)
                        {
                            isDir = true;
                            foreach (var item in Scan(fullpath))
                                yield return item;
                        }

                        yield return new Info()
                        {
                            CreatedDate = ToDateTime(findData.ftCreationTime),
                            ModifiedDate = ToDateTime(findData.ftLastWriteTime),
                            IsDirectory = isDir,
                            Path = fullpath
                        };
                    }
                    while (FindNextFile(findHandle, out findData));
                }
            }
            finally
            {
                if (findHandle != INVALID_HANDLE_VALUE) FindClose(findHandle);
            }
        }

        public class Info
        {
            public DateTime CreatedDate;
            public DateTime ModifiedDate;
            public bool IsDirectory;
            public string Path;
        }

        public static DateTime ToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME filetime)
        {
            long highBits = filetime.dwHighDateTime;
            highBits = highBits << 32;
            return DateTime.FromFileTimeUtc(highBits + (long)filetime.dwLowDateTime);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll")]
        public static extern bool FindClose(IntPtr hFindFile);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WIN32_FIND_DATAW {
            public FileAttributes dwFileAttributes;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }
    }

    public static class AudioFileScanner
    {
        private const string allowedExts = ".mp3;.wav";

        public static Dictionary<string, string> AllowedExtsDict = new Dictionary<string,string>();

        static AudioFileScanner()
        {
            Init();
        }

        private static void Init()
        {
            foreach(var e in allowedExts.Split(';'))
                AllowedExtsDict.Add(e, e);
        }

        /// <summary>
        /// Scans given folder and return full file pathes for audio files
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static IEnumerable<string> Scan(string folder)
        {
            foreach(var info in FileScanner.Scan(folder))
                if(!info.IsDirectory)
                {
                    var ext = Path.GetExtension(info.Path).ToLower();
                    if (AllowedExtsDict.ContainsKey(ext))
                        yield return info.Path;
                }
        }

        /// <summary>
        /// Scans given pathes and return full file pathes for audio files
        /// </summary>
        /// <param name="pathes">List of folders or files</param>
        /// <returns></returns>
        public static IEnumerable<string> Scan(IEnumerable<string> pathes)
        {
            foreach (var path in pathes)
                if (File.Exists(path))
                {
                    //this is file, check ext
                    var ext = Path.GetExtension(path).ToLower();
                    if (AllowedExtsDict.ContainsKey(ext))
                        yield return path;
                }
                else
                //this is folder, scan it
                foreach (var p in Scan(path))
                    yield return p;
        }
    }
}
