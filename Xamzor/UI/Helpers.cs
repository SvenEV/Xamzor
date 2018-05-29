using Layman;
using Microsoft.AspNetCore.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamzor.UI.Components;

namespace Xamzor.UI
{
    /// <summary>
    /// Constants and static methods to simplify writing Xamzor views.
    /// </summary>
    public static class Helpers
    {
        public const Orientation Horizontal = Orientation.Horizontal;
        public const Orientation Vertical = Orientation.Vertical;
        public const Alignment Left = Alignment.Start;
        public const Alignment Top = Alignment.Start;
        public const Alignment Right = Alignment.End;
        public const Alignment Bottom = Alignment.End;
        public const Alignment Center = Alignment.Center;
        public const Alignment Stretch = Alignment.Stretch;
        public const StretchMode None = StretchMode.None;
        public const StretchMode Fill = StretchMode.Fill;
        public const StretchMode Uniform = StretchMode.Uniform;
        public const StretchMode UniformToFill = StretchMode.UniformToFill;

        // HACK
        public static XamzorComponent PARENT { get; set; }

        public static RenderFragment SurroundWithParent(this RenderFragment fragment, XamzorComponent parent) => builder =>
        {
            var temp = PARENT;
            PARENT = parent;
            fragment?.Invoke(builder);
            PARENT = temp;
        };

        public static IReadOnlyList<GridTrackDefinition> Rows(params string[] sizeStrings) =>
            sizeStrings.Select(s => new GridTrackDefinition(GridLength.Parse(s))).ToList();

        public static IReadOnlyList<GridTrackDefinition> Columns(params string[] sizeStrings) =>
            sizeStrings.Select(s => new GridTrackDefinition(GridLength.Parse(s))).ToList();

        public static double Clamp(double value, double min, double max) =>
            value < min ? min : (value > max ? max : value);

        public static double OrIfNan(this double value, double fallbackValue) =>
            double.IsNaN(value) ? fallbackValue : value;

        public static bool EqualsApprox(double a, double b, double tolerance = .001) =>
            Math.Abs(a - b) <= tolerance;

        public static Thickness T(double uniformSize) =>
            new Thickness(uniformSize);

        public static Thickness T(double horizontalSize, double verticalSize) =>
            new Thickness(horizontalSize, verticalSize, horizontalSize, verticalSize);

        public static Thickness T(double left, double top, double right, double bottom) =>
            new Thickness(left, top, right, bottom);
    }
}
