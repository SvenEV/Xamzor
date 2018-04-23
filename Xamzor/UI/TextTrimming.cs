namespace Xamzor.UI
{
    public enum TextTrimming
    {
        None,

        /// <summary>
        /// Due to CSS limitations this only works if <see cref="TextWrapping"/>
        /// is set to <see cref="TextWrapping.NoWrap"/>.
        /// </summary>
        Ellipsis
    }
}
