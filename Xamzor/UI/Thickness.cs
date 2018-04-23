namespace Xamzor.UI
{
    public struct Thickness
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

        public override string ToString() => $"[{Left}, {Top}, {Right}, {Bottom}]";
    }
}
