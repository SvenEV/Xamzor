namespace Xamzor.UI
{
    public enum ZoomMode
    {
        Disabled,

        /// <summary>
        /// Scroll mode must be <see cref="ScrollMode.Enabled"/> or <see cref="ScrollMode.Auto"/>
        /// in both directions for zooming to be enabled.
        /// Only works in Edge.
        /// </summary>
        Enabled
    }
}
