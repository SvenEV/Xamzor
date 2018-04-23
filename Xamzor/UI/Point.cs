using System;

namespace Xamzor.UI
{
    public struct Point
    {
        public static readonly Point Zero = new Point(0, 0);
        public static readonly Point One = new Point(1, 1);
        public static readonly Point UnitX = new Point(1, 0);
        public static readonly Point UnitY = new Point(0, 1);
        public static readonly Point PositiveInfinity = new Point(double.PositiveInfinity, double.PositiveInfinity);

        public double X { get; }
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double this[int dimension]
        {
            get
            {
                switch (dimension)
                {
                    case 0: return X;
                    case 1: return Y;
                    default: throw new ArgumentOutOfRangeException(nameof(dimension));
                }
            }
        }

        public Point WithX(double x) => new Point(x, Y);
        public Point WithY(double y) => new Point(X, y);
        public Point OrWhereNaN(Point fallbackValue) =>
            new Point(X.OrIfNan(fallbackValue.X), Y.OrIfNan(fallbackValue.Y));

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => a + -b;
        public static Point operator -(Point p) => new Point(-p.X, -p.Y);
        public static Point operator *(Point a, Point b) => new Point(a.X * b.X, a.Y * b.Y);
        public static Point operator *(double d, Point p) => new Point(d * p.X, d * p.Y);
        public static Point operator *(Point p, double d) => new Point(d * p.X, d * p.Y);

        public static bool operator ==(Point a, Point b) => a.Equals(b);
        public static bool operator !=(Point a, Point b) => !a.Equals(b);

        public static Point Min(Point a, Point b) => new Point(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        public static Point Max(Point a, Point b) => new Point(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        public static Point Clamp(Point point, Point min, Point max) => new Point(
            Helpers.Clamp(point.X, min.X, max.X),
            Helpers.Clamp(point.Y, min.Y, max.Y));

        public override string ToString() => $"({X}, {Y})";
    }
}
