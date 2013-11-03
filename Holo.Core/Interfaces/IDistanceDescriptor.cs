namespace Holo.Core
{
    /// <summary>
    /// Descriptor to calculate distance between sounds.
    /// </summary>
    /// <remarks>
    /// Descriptor describes some of the characteristics of the sound.
    /// Descriptors can be compared with each other.
    /// IDistanceDescriptor can calculate distance between sounds. This is used to search for similar sounds.
    /// </remarks>
    public interface IDistanceDescriptor
    {
        /// <summary>
        /// Calculates distance to other descriptor (same type).
        /// Returns float value from 0(shortest distance) to 1(farest distance).
        /// </summary>
        float Distance(IDistanceDescriptor other);
        /// <summary>
        /// Weight of the descriptor.
        /// It is used to mix this descriptor with other measures.
        /// By default, it have to return 1.
        /// Possible values: from 0 to infinite.
        /// </summary>
        float Weight { get; }
    }
}
