using System;

namespace Xamzor.UI
{
    public class LayoutException : InvalidOperationException
    {
        public LayoutException(string message) : base(message)
        {
        }

        public LayoutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
