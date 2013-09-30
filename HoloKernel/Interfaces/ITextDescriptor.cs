namespace HoloKernel
{
    /// <summary>
    /// Descriptor with text 
    /// </summary>
    /// <remarks>
    /// Descriptor describes some of the characteristics of the sound.
    /// ITextDescriptor returns text description of the sound for UI.
    /// </remarks>
    public interface ITextDescriptor
    {
        /// <summary>
        /// Description of the sound
        /// </summary>
        string Description { get; }
    }
}
