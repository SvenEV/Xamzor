namespace Xamzor.UI
{
    public struct Rect
    {
        public double X { get; }
        public double Y { get; }
        public double Width { get; }
        public double Height { get; }

        public Point TopLeft => new Point(X, Y);

        public Point Size => new Point(Width, Height);

        public Rect(double x, double y, double width, double height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rect(Point topLeft, Point size) : this(topLeft.X, topLeft.Y, size.X, size.Y)
        {
        }

        public override string ToString() => $"({X}, {Y}, {Width}, {Height})";

        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Rect a, Rect b) => a.Equals(b);

        public static bool operator !=(Rect a, Rect b) => !a.Equals(b);
    }
}
