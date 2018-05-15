using System;

namespace Xamzor.UI
{
    public static class Css
    {
        public static string FromAlignment(Alignment alignment, bool allowStretch)
        {
            switch (alignment)
            {
                case Alignment.Start: return "start";
                case Alignment.End: return "end";
                case Alignment.Center: return "center";
                case Alignment.Stretch: return allowStretch ? "stretch" : "center";
                default: throw new NotImplementedException();
            }
        }

        public static string FromThickness(Thickness t) =>
            $"{t.Top}px {t.Right}px {t.Bottom}px {t.Left}px";
    }
}
