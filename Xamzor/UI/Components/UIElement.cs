using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Browser.Interop;
using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Linq;
using System.Text;

namespace Xamzor.UI.Components
{
    public class UIElement : XamzorComponent
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

        protected ElementRef LayoutRoot { get; set; }

        protected string LayoutCss =>
            //$"left: {Bounds.X}px; top: {Bounds.Y}px; width: {Bounds.Width}px; height: {Bounds.Height}px; " +
            //$"clip: rect({ClippedBounds.Y - Bounds.Y}px, {ClippedBounds.X - Bounds.X + ClippedBounds.Width}px, {ClippedBounds.Y - Bounds.Y + ClippedBounds.Height}px, {ClippedBounds.X - Bounds.X}px); " + 
            StyleCss;

        protected string StyleCss { get; private set; }

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


        public Point Size => new Point(Width, Height);
        public Point MinSize => new Point(MinWidth, MinHeight);
        public Point MaxSize => new Point(MaxWidth, MaxHeight);
        public bool IsMeasureValid { get; internal set; }
        public bool IsArrangeValid { get; internal set; }
        public Point? PreviousMeasureInput { get; private set; }
        public Rect? PreviousArrangeInput { get; private set; }
        public Point DesiredSize { get; private set; } // includes margins, computed by Measure()
        public Rect Bounds { get; private set; } // size excludes margins, computed by Arrange()
        public Rect ClippedBounds { get; private set; } // bounds clipped to the finalRect size

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            UpdateLayoutCss();
            InvalidateMeasure();
        }

        private void UpdateLayoutCss()
        {
            var sb = new StringBuilder();
            ComputeOwnLayoutCss(sb);
            (Parent as UIElement)?.ComputeChildLayoutCss(sb, this);
            StyleCss = sb.ToString();
        }

        protected virtual void ComputeOwnLayoutCss(StringBuilder sb)
        {
            if (Opacity != 1)
                sb.Append($"opacity: {Opacity}; ");
        }

        protected virtual void ComputeChildLayoutCss(StringBuilder sb, UIElement child)
        {
        }

        private void ApplyBounds()
        {
            RegisteredFunction.Invoke<object>("Xamzor.layout", LayoutRoot,
                Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
        }






        public void RaiseStateHasChanged() => StateHasChanged();

        public void InvalidateMeasure()
        {
            if (IsMeasureValid)
            {
                IsMeasureValid = false;
                IsArrangeValid = false;
                LayoutManager.Instance.InvalidateMeasure(this);
            }
        }

        public void InvalidateArrange()
        {
            if (IsArrangeValid)
            {
                IsArrangeValid = false;
                LayoutManager.Instance.InvalidateArrange(this);
            }
        }

        public Point Measure(Point availableSize)
        {
            if (IsInvalidInput(availableSize))
                throw new LayoutException($"Invalid input for '{GetType().Name}.Measure': {availableSize}");

            // If possible, use cached desired size
            // DEBUG: Temporarily, never use cached results
            if (false && IsMeasureValid && PreviousMeasureInput == availableSize)
            {
                UILog.Write("DEBUG", $"Using cached DesiredSize of '{this}'");
                return DesiredSize;
            }

            using (UILog.BeginScope("LAYOUT",
                $"{this}.Measure{availableSize}...",
                () => $"<<< {this}.{nameof(DesiredSize)} = {DesiredSize}"))
            {
                DesiredSize = Point.Min(MeasureCore(availableSize), availableSize);
                PreviousMeasureInput = availableSize;
                IsMeasureValid = true;
            }

            if (IsInvalidOutput(DesiredSize))
                throw new LayoutException($"Invalid result from '{GetType().Name}.Measure({availableSize})': {DesiredSize}");

            return DesiredSize;

            // Available size must not be NaN or negative (but can be infinity)
            bool IsInvalidInput(Point size) =>
                size.X < 0 || size.Y < 0 ||
                double.IsNaN(size.X) || double.IsNaN(size.Y);

            // Desired size must be >=0 (not NaN and not infinity)
            bool IsInvalidOutput(Point size) =>
                size.X < 0 || size.Y < 0 ||
                double.IsInfinity(size.X) || double.IsInfinity(size.Y) ||
                double.IsNaN(size.X) || double.IsNaN(size.Y);
        }

        public Rect Arrange(Rect finalRect)
        {
            if (IsInvalidInput(finalRect))
                throw new LayoutException($"Invalid input for '{GetType().Name}.Arrange': {finalRect}");

            if (!IsMeasureValid)
                Measure(PreviousMeasureInput ?? finalRect.Size);

            // If possible, use cached rect
            // DEBUG: Temporarily, never use cached results
            if (false && IsArrangeValid && PreviousArrangeInput == finalRect)
            {
                UILog.Write("DEBUG", $"Using cached Bounds of '{this}'");
                return Bounds;
            }

            using (UILog.BeginScope("LAYOUT",
                $"{this}.Arrange{finalRect}...",
                () => $"<<< {this}.{nameof(Bounds)} = {Bounds}"))
            {
                Bounds = ArrangeCore(finalRect);
                ClippedBounds = finalRect; // TODO: Is this correct? Shouldn't it incorporate Bounds?
                PreviousArrangeInput = finalRect;
                IsArrangeValid = true;
            }

            // TODO: We can't call StateHasChanged() here, probably because it might run in the scope
            //       of an OnAfterRender() call
            // StateHasChanged()
            ApplyBounds();
            return Bounds;

            // Position and size must not be NaN or infinity, and size must be >=0
            bool IsInvalidInput(Rect rect) =>
                rect.Width < 0 || rect.Height < 0 ||
                double.IsInfinity(rect.X) || double.IsInfinity(rect.Y) ||
                double.IsInfinity(rect.Width) || double.IsInfinity(rect.Height) ||
                double.IsNaN(rect.X) || double.IsNaN(rect.Y) ||
                double.IsNaN(rect.Width) || double.IsNaN(rect.Height);
        }

        private Point MeasureCore(Point availableSize)
        {
            // Effective min/max size accounts for explicitly set Width/Height
            var effectiveMinSize = Point.Clamp(Size.OrWhereNaN(Point.Zero), MinSize, MaxSize);
            var effectiveMaxSize = Point.Clamp(Size.OrWhereNaN(Point.PositiveInfinity), MinSize, MaxSize);
            var constrainedSize = Point.Clamp(availableSize - Margin.Size, effectiveMinSize, effectiveMaxSize);

            var measuredSize = MeasureOverride(constrainedSize);
            var desiredSize = Point.Clamp(Size.OrWhereNaN(measuredSize), MinSize, MaxSize);
            return Point.Max(Point.Zero, desiredSize + Margin.Size);
        }

        private Rect ArrangeCore(Rect finalRect)
        {
            var availableSizeMinusMargins = Point.Max(Point.Zero, finalRect.Size - Margin.Size);
            var finalSize = ComputeSize();
            var finalOffset = ComputeOffset(finalSize);
            return new Rect(finalOffset, finalSize);

            Point ComputeSize()
            {
                // On 'Stretch' start with full available size, otherwise start with DesiredSize
                var arrangeSize = new Point(
                    HorizontalAlignment == Alignment.Stretch ? availableSizeMinusMargins.X : DesiredSize.X,
                    VerticalAlignment == Alignment.Stretch ? availableSizeMinusMargins.Y : DesiredSize.Y);

                // Effective min/max size accounts for explicitly set Width/Height
                var effectiveMinSize = Point.Clamp(Size.OrWhereNaN(Point.Zero), MinSize, MaxSize);
                var effectiveMaxSize = Point.Clamp(Size.OrWhereNaN(Point.PositiveInfinity), MinSize, MaxSize);
                arrangeSize = Point.Clamp(arrangeSize, effectiveMinSize, effectiveMaxSize);

                // Note: Returned size may exceed available size so that content gets clipped
                return Point.Clamp(ArrangeOverride(arrangeSize), effectiveMinSize, effectiveMaxSize);
            }

            Point ComputeOffset(Point size)
            {
                // If size returned by ArrangeOverride() exceeds available space,
                // content will be clipped and we fall back to top/left alignment
                var effectiveHAlign = (HorizontalAlignment == Alignment.Stretch && size.X > availableSizeMinusMargins.X)
                    ? Alignment.Start
                    : HorizontalAlignment;

                var effectiveVAlign = (VerticalAlignment == Alignment.Stretch && size.Y > availableSizeMinusMargins.Y)
                    ? Alignment.Start
                    : VerticalAlignment;

                var offset = finalRect.TopLeft + Margin.TopLeft;

                switch (effectiveHAlign)
                {
                    case Alignment.Center:
                    case Alignment.Stretch:
                        offset += new Point((availableSizeMinusMargins.X - size.X) / 2, 0);
                        break;

                    case Alignment.End:
                        offset += new Point(availableSizeMinusMargins.X - size.X, 0);
                        break;
                }

                switch (effectiveVAlign)
                {
                    case Alignment.Center:
                    case Alignment.Stretch:
                        offset += new Point(0, (availableSizeMinusMargins.Y - size.Y) / 2);
                        break;

                    case Alignment.End:
                        offset += new Point(0, availableSizeMinusMargins.Y - size.Y);
                        break;
                }

                return offset;
            }
        }

        /// <param name="availableSize">Size available for the element, excluding margin</param>
        protected virtual Point MeasureOverride(Point availableSize)
        {
            // By default: Return bounding box size of all children positioned at (0, 0)
            var size = Point.Zero;

            foreach (var child in Children.OfType<UIElement>())
            {
                child.Measure(availableSize - Padding.Size);
                size = Point.Max(size, child.DesiredSize);
            }

            return size + Padding.Size;
        }

        protected virtual Point ArrangeOverride(Point finalSize)
        {
            // By default: Position all children at (0, 0)
            foreach (var child in Children.OfType<UIElement>())
                child.Arrange(new Rect(Padding.TopLeft, finalSize - Padding.Size));

            return finalSize;
        }
    }
}
