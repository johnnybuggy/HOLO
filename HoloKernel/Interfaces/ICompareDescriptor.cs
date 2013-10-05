namespace HoloKernel
{
    /// <summary>
    /// Comparable descriptor
    /// </summary>
    /// <remarks>
    /// Descriptor describes some of the characteristics of the sound.
    /// Descriptors can be compared with each other.
    /// ICompareDescriptor can compare by more-less. It is used to sort of sounds.
    /// </remarks>
    public interface ICompareDescriptor
    {
        /// <summary>
        /// Compare with other descriptor (same type).
        /// Returns 0 if equal, +1 if it is more, -1 if less.
        /// </summary>
        int Compare(ICompareDescriptor other);
        /// <summary>
        /// Weight of the descriptor.
        /// It is used to mix this descriptor with other measures.
        /// By default, it have to return 1.
        /// Possible values: from 0 to infinite.
        /// </summary>
        float Weight { get; }
    }
}