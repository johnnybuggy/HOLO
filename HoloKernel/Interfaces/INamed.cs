namespace HoloKernel
{
    /// <summary>
    /// Item with name and description for UI
    /// </summary>
    public interface INamed
    {
        string UserInterfaceName { get; }
        string UserInterfaceDescription { get; }
    }
}