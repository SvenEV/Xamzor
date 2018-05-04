using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Text;

namespace Xamzor.UI.Components
{
    public class UIElement : XamzorComponent, IDisposable
    {
        public static readonly PropertyKey WidthProperty = PropertyKey.Create<double, UIElement>(nameof(Width), double.NaN);
        public static readonly PropertyKey HeightProperty = PropertyKey.Create<double, UIElement>(nameof(Height), double.NaN);
        public static readonly PropertyKey MinWidthProperty = PropertyKey.Create<double, UIElement>(nameof(MinWidth), 0);
        public static readonly PropertyKey MinHeightProperty = PropertyKey.Create<double, UIElement>(nameof(MinHeight), 0);
        public static readonly PropertyKey MaxWidthProperty = PropertyKey.Create<double, UIElement>(nameof(MaxWidth), double.PositiveInfinity);
        public static readonly PropertyKey MaxHeightProperty = PropertyKey.Create<double, UIElement>(nameof(MaxHeight), double.PositiveInfinity);
        public static readonly PropertyKey MarginProperty = PropertyKey.Create<Thickness, UIElement>(nameof(Margin), Thickness.Zero);
        public static readonly PropertyKey PaddingProperty = PropertyKey.Create<Thickness, UIElement>(nameof(Padding), Thickness.Zero);
        public static readonly PropertyKey HorizontalAlignmentProperty = PropertyKey.Create<Alignment, UIElement>(nameof(HorizontalAlignment), Alignment.Stretch);
        public static readonly PropertyKey VerticalAlignmentProperty = PropertyKey.Create<Alignment, UIElement>(nameof(VerticalAlignment), Alignment.Stretch);
        public static readonly PropertyKey OpacityProperty = PropertyKey.Create<double, UIElement>(nameof(Opacity), 1);

        protected string LayoutCss { get; private set; }

        [Parameter] protected double Width { get => Properties.Get<double>(WidthProperty); set => Properties.Set(WidthProperty, value); }
        [Parameter] protected double Height { get => Properties.Get<double>(HeightProperty); set => Properties.Set(HeightProperty, value); }
        [Parameter] protected double MinWidth { get => Properties.Get<double>(MinWidthProperty); set => Properties.Set(MinWidthProperty, value); }
        [Parameter] protected double MinHeight { get => Properties.Get<double>(MinHeightProperty); set => Properties.Set(MinHeightProperty, value); }
        [Parameter] protected double MaxWidth { get => Properties.Get<double>(MaxWidthProperty); set => Properties.Set(MaxWidthProperty, value); }
        [Parameter] protected double MaxHeight { get => Properties.Get<double>(MaxHeightProperty); set => Properties.Set(MaxHeightProperty, value); }
        [Parameter] protected Thickness Margin { get => Properties.Get<Thickness>(MarginProperty); set => Properties.Set(MarginProperty, value); }
        [Parameter] protected Thickness Padding { get => Properties.Get<Thickness>(PaddingProperty); set => Properties.Set(PaddingProperty, value); }
        [Parameter] protected Alignment HorizontalAlignment { get => Properties.Get<Alignment>(HorizontalAlignmentProperty); set => Properties.Set(HorizontalAlignmentProperty, value); }
        [Parameter] protected Alignment VerticalAlignment { get => Properties.Get<Alignment>(VerticalAlignmentProperty); set => Properties.Set(VerticalAlignmentProperty, value); }
        [Parameter] protected double Opacity { get => Properties.Get<double>(OpacityProperty); set => Properties.Set(OpacityProperty, value); }

        // Temporary properties - we'll invent some form of "attached properties" in the future
        [Parameter] protected int Row { get => Properties.Get<int>(Grid.RowProperty); set => Properties.Set(Grid.RowProperty, value); }
        [Parameter] protected int Column { get => Properties.Get<int>(Grid.ColumnProperty); set => Properties.Set(Grid.ColumnProperty, value); }
        [Parameter] protected int RowSpan { get => Properties.Get<int>(Grid.RowSpanProperty); set => Properties.Set(Grid.RowSpanProperty, value); }
        [Parameter] protected int ColumnSpan { get => Properties.Get<int>(Grid.ColumnSpanProperty); set => Properties.Set(Grid.ColumnSpanProperty, value); }

        public override void SetParameters(ParameterCollection parameters)
        {
            base.SetParameters(parameters);
            UpdateLayoutCss();
        }

        private void UpdateLayoutCss()
        {
            var sb = new StringBuilder();
            ComputeOwnLayoutCss(sb);
            (Parent as UIElement)?.ComputeChildLayoutCss(sb, this);
            LayoutCss = sb.ToString();
        }

        protected virtual void ComputeOwnLayoutCss(StringBuilder sb)
        {
            if (!double.IsNaN(Width))
                sb.Append($"width: {Width}px; ");

            if (!double.IsNaN(Height))
                sb.Append($"height: {Height}px; ");

            if (MinWidth > 0)
                sb.Append($"min-width: {MinWidth}px; ");

            if (MinHeight > 0)
                sb.Append($"min-height: {MinHeight}px; ");

            if (MaxWidth != double.PositiveInfinity)
                sb.Append($"max-width: {MaxWidth}px; ");
            else if (HorizontalAlignment != Alignment.Stretch)
                sb.Append($"max-width: calc(100% - {Margin.HorizontalThickness}px); ");

            if (MaxHeight != double.PositiveInfinity)
                sb.Append($"max-height: {MaxHeight}px; ");
            else if (VerticalAlignment != Alignment.Stretch)
                sb.Append($"max-height: calc(100% - {Margin.VerticalThickness}px); ");

            if (Opacity != 1)
                sb.Append($"opacity: {Opacity}; ");

            if (Margin != Thickness.Zero)
                sb.Append($"margin: {ThicknessToCss(Margin)}; ");

            if (Padding != Thickness.Zero)
                sb.Append($"padding: {ThicknessToCss(Padding)}; ");

            // If the element cannot effectively stretch due to size constraints (Width/MaxWidth),
            // CSS will align it at 'start' instead. To get the typical XAML behavior ('center'),
            // we explicitly set the CSS alignment to 'center' instead of 'stretch' in such cases.
            var canStretchH =
                HorizontalAlignment == Alignment.Stretch &&
                double.IsNaN(Width) && MaxWidth == double.PositiveInfinity;

            var canStretchV =
                VerticalAlignment == Alignment.Stretch &&
                double.IsNaN(Height) && MaxHeight == double.PositiveInfinity;

            // TODO: Remaining problem:
            //       If only MaxWidth is set (but no Width) and HorizontalAlignment == Stretch,
            //       the MaxWidth is not exhausted. Instead the element will size to content.

            // Here's an attempt at a workaround, but it breaks if MaxWidth exceeds available width
            //if (!canStretchH && MaxWidth != double.PositiveInfinity)
            //    sb.Append($"width: {MaxWidth}px; ");
            //if (!canStretchV && MaxHeight != double.PositiveInfinity)
            //    sb.Append($"height: {MaxHeight}px; ");

            var justifySelf = AlignmentToCss(HorizontalAlignment, canStretchH);
            var alignSelf = AlignmentToCss(VerticalAlignment, canStretchV);

            if (justifySelf != "stretch")
                sb.Append($"justify-self: {justifySelf}; ");

            if (alignSelf != "stretch")
                sb.Append($"align-self: {alignSelf}; ");
        }

        protected virtual void ComputeChildLayoutCss(StringBuilder sb, UIElement child)
        {
        }

        protected string AlignmentToCss(Alignment alignment, bool allowStretch)
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

        protected string ThicknessToCss(Thickness t) =>
            $"{t.Top}px {t.Right}px {t.Bottom}px {t.Left}px";
    }
}
