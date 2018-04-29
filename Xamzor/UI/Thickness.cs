using System;

namespace Xamzor.UI
{
    public struct Thickness : IEquatable<Thickness>
    {
        public static readonly Thickness Zero = new Thickness(0);

        public double Left { get; }

        public double Top { get; }

        public double Right { get; }

        public double Bottom { get; }

        public double HorizontalThickness => Left + Right;

        public double VerticalThickness => Top + Bottom;

        public Point TopLeft => new Point(Left, Top);

        public Point Size => new Point(Left + Right, Top + Bottom);

        internal bool IsDefault => Left == 0 && Top == 0 && Right == 0 && Bottom == 0;

        public Thickness(double uniformSize) : this(uniformSize, uniformSize, uniformSize, uniformSize)
        {
        }

        public Thickness(double horizontalSize, double verticalSize) : this(horizontalSize, verticalSize, horizontalSize, verticalSize)
        {
        }

        public Thickness(double left, double top, double right, double bottom) : this()
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static implicit operator Thickness(double uniformSize) =>
            new Thickness(uniformSize);

        public static implicit operator Thickness((double horizontalSize, double verticalSize) t) =>
            new Thickness(t.horizontalSize, t.verticalSize);

        public static implicit operator Thickness((double left, double top, double right, double bottom) t) =>
            new Thickness(t.left, t.top, t.right, t.bottom);

        public static bool operator ==(Thickness a, Thickness b) => a.Equals(b);

        public static bool operator !=(Thickness a, Thickness b) => !(a == b);

        public override string ToString() => $"[{Left}, {Top}, {Right}, {Bottom}]";

        public override bool Equals(object obj) => obj is Thickness t && Equals(t);

        public bool Equals(Thickness other) =>
            Left == other.Left &&
            Top == other.Top &&
            Right == other.Right &&
            Bottom == other.Bottom;

        public override int GetHashCode()
        {
            var hashCode = -1819631549;
            hashCode = hashCode * -1521134295 + Left.GetHashCode();
            hashCode = hashCode * -1521134295 + Top.GetHashCode();
            hashCode = hashCode * -1521134295 + Right.GetHashCode();
            hashCode = hashCode * -1521134295 + Bottom.GetHashCode();
            return hashCode;
        }
    }
}
