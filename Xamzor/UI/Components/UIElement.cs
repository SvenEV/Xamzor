using Layman;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Browser.Interop;
using Microsoft.AspNetCore.Blazor.Components;
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

        protected string CssStyle { get; private set; }

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

        public UIElement()
        {
            LayoutCache.MeasureInvalidated += _ => LayoutManager.Instance.InvalidateArrange(this);
            LayoutCache.ArrangeInvalidated += _ => LayoutManager.Instance.InvalidateArrange(this);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            var sb = new StringBuilder();
            ComputeCss(sb);
            CssStyle = sb.ToString();

            LayoutCache.InvalidateMeasure();
        }

        protected virtual void ComputeCss(StringBuilder sb)
        {
            if (Opacity != 1)
                sb.Append($"opacity: {Opacity}; ");
        }

        protected override void OnAfterRender()
        {
            base.OnAfterRender();
            ApplyBounds();
        }

        protected virtual void ApplyBounds()
        {
            // Note: Some UIElements may not have a root div. TODO: Should we change this?
            var bounds = LayoutCache.RelativeBounds;
            RegisteredFunction.Invoke<object>("Xamzor.layout", LayoutRoot, bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        public void RaiseStateHasChanged() => StateHasChanged();
        
        public LayoutCache LayoutCache = new LayoutCache();

        public virtual LayoutFunc Layout =>
            Layouts.TraceSpecial(ToString(),
                Layouts.Callback(onArrangeOut: (_, __) => ApplyBounds(), child:
                    Layouts.StandardLayout(
                        new StandardLayoutProperties(
                            Margin, Padding,
                            new Vector2(Width, Height),
                            new Vector2(MinWidth, MinHeight),
                            new Vector2(MaxWidth, MaxHeight),
                            HorizontalAlignment,
                            VerticalAlignment),
                        LayoutCache, Layouts.FillSpace(FillSpaceMode.Exact, ChildLayout))));

        protected virtual LayoutFunc ChildLayout => Layouts.Overlay(Children.OfType<UIElement>().Select(child => child.Layout));
    }
}
