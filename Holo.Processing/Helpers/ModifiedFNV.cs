namespace Holo.Processing.Helpers
{
    /// <summary>
    /// http://bretm.home.comcast.net/~bretm/hash/6.html
    /// </summary>
    public static class ModifiedFNV
    {
        public static uint ComputeHash(byte[] data)
        {
            const uint P = 16777619;
            uint Hash = 2166136261;

            foreach (byte B in data)
            {
                Hash = (Hash ^ B) * P;
            }

            Hash += Hash << 13;
            Hash ^= Hash >> 7;
            Hash += Hash << 3;
            Hash ^= Hash >> 17;
            Hash += Hash << 5;

            return Hash;
        }
    }
}
