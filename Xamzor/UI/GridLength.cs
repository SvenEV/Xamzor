using System;

namespace Xamzor.UI
{
    public struct GridLength
    {
        public double Value { get; }

        public GridUnitType GridUnitType { get; }

        public bool IsAbsolute => GridUnitType == GridUnitType.Absolute;

        public bool IsAuto => GridUnitType == GridUnitType.Auto;

        public bool IsStar => GridUnitType == GridUnitType.Star;

        public GridLength(double value, GridUnitType unitType) : this()
        {
            Value = value;
            GridUnitType = unitType;
        }

        public static GridLength Parse(string s)
        {
            if (s == "*")
                return new GridLength(1, GridUnitType.Star);

            if (s.Equals("auto", StringComparison.OrdinalIgnoreCase))
                return new GridLength(1, GridUnitType.Auto);

            if (double.TryParse(s, out var absSize))
                return new GridLength(absSize, GridUnitType.Absolute);

            if (s.EndsWith("*") && double.TryParse(s.Substring(0, s.Length - 1), out var starSize))
                return new GridLength(starSize, GridUnitType.Star);

            throw new FormatException($"'{s}' is not a valid format for '{nameof(GridLength)}'");
        }
    }
}
