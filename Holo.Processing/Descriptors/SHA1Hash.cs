using System;
using System.IO;
using System.Security.Cryptography;
using Holo.Processing.Helpers;
using HoloDB;

namespace Holo.Processing
{
    /// <summary>
    /// Represents a SHA1 hash value for the given <see cref="Audio"/> file.
    /// It can be used as key in dicionaries and sets since it overrides <see cref="Object.Equals(object)"/> 
    /// and <see cref="Object.GetHashCode"/>.
    /// </summary>
    public class SHA1Hash : IEquatable<SHA1Hash>
    {
        private readonly byte[] HashValue;

        // Cached value for GetHashCode
        private readonly int HashCode;

        protected SHA1Hash(byte[] hashValue)
        {
            if (hashValue == null)
            {
                throw new ArgumentNullException("hashValue");
            }

            HashValue = (byte[])hashValue.Clone();

            HashCode = (int)ModifiedFNV.ComputeHash(HashValue);
        }

        public static SHA1Hash FromBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (bytes.Length != 20)
            {
                throw new ArgumentOutOfRangeException("bytes", "SHA1 length must be 20 bytes.");
            }

            return new SHA1Hash(bytes);
        }

        public static SHA1Hash FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value can not be null or empty.", "value");
            }

            if (value.Length != 40)
            {
                throw new ArgumentOutOfRangeException("value", "Value length must be 40.");
            }

            int NumberChars = value.Length;
            byte[] Bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                Bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
            return new SHA1Hash(Bytes);
        }

        public static SHA1Hash FromAudio(Audio audio)
        {
            if (audio == null)
            {
                throw new ArgumentNullException("audio");
            }

            using (Stream FileStream = audio.GetSourceStream())
            {
                byte[] HashValue = SHA1.Create().ComputeHash(FileStream);
                
                return new SHA1Hash(HashValue);
            }
        }

        public byte[] GetBytes()
        {
            return (byte[])HashValue.Clone();
        }

        public override bool Equals(object obj)
        {
            return (obj is SHA1Hash) && Equals((SHA1Hash)obj);
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public bool Equals(SHA1Hash other)
        {
            // .NET 2.0 friendly implementation
            if (HashValue == other.HashValue)
            {
                return true;
            }

            if ((HashValue != null) && (other.HashValue != null))
            {
                if (HashValue.Length != other.HashValue.Length)
                {
                    return false;
                }

                for (int i = 0; i < HashValue.Length; i++)
                {
                    if (HashValue[i] != other.HashValue[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public override string ToString()
        {
            if (HashValue != null)
            {
                return BitConverter.ToString(HashValue).Replace("-", "").ToLower();
            }
            else
            {
                return "null";
            }
        }
    }
}
