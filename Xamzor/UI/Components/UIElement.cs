using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamzor.UI.Components
{
    public class UIElement : BlazorComponent, IDisposable
    {
        private static long _nextFreeId = 0;

        private readonly string _cssClasses;
        private bool _debugRenderCount = false;

        protected string LayoutCss { get; private set; }

        protected string CssClass => Application.IsDebugOutlineEnabled
            ? _cssClasses + (_debugRenderCount ? " debug1" : " debug2")
            : _cssClasses;

        public string Id { get; }

        public RenderFragment ChildContent { get; set; }

        public double Width { get; set; } = double.NaN;

        public double Height { get; set; } = double.NaN;

        public double MinWidth { get; set; } = 0;

        public double MinHeight { get; set; } = 0;

        public double MaxWidth { get; set; } = double.PositiveInfinity;

        public double MaxHeight { get; set; } = double.PositiveInfinity;

        public Thickness Margin { get; set; } = Thickness.Zero;

        public Thickness Padding { get; set; } = Thickness.Zero;

        public string Tag { get; set; }

        // HACK
        public UIElement Parent { get; set; }

        public Alignment HorizontalAlignment { get; set; } = Alignment.Stretch;

        public Alignment VerticalAlignment { get; set; } = Alignment.Stretch;

        public double Opacity { get; set; } = 1;

        // Temporary properties - we'll invent some form of "attached properties" in the future
        public int Row { get; set; } = 0;
        public int Column { get; set; } = 0;
        public int RowSpan { get; set; } = 1;
        public int ColumnSpan { get; set; } = 1;

        public UIElement()
        {
            Id = GetType().Name + "-" + _nextFreeId++;
            _cssClasses = string.Join(" ", GetBaseTypes(GetType()).Select(t => t.Name));

            IEnumerable<Type> GetBaseTypes(Type type)
            {
                while (type != typeof(UIElement))
                {
                    yield return type;
                    type = type.BaseType;
                }
                yield return typeof(UIElement);
            }
        }

        protected override void OnInit()
        {
            UILog.Write("LIFECYCLE", $"Initialize '{this}'");
        }

        public override void SetParameters(ParameterCollection parameters)
        {
            // Assign to properties
            base.SetParameters(parameters);

            // Inject helper code into ChildContent
            if (ChildContent is RenderFragment originalChildContent)
            {
                ChildContent = builder =>
                {
                    var temp = Helpers.PARENT;
                    Helpers.PARENT = this;
                    originalChildContent(builder);
                    Helpers.PARENT = temp;
                };
            }

            if (!parameters.TryGetValue(nameof(Parent), out object _))
                UILog.Write("WARNING", $"No parent assigned to '{this}'");

            UpdateLayoutCss();
            UILog.Write("LIFECYCLE", $"SetParameters '{this}' (parent is '{Parent}')");
        }

        public override string ToString() => Id + (Tag == null ? "" : "-" + Tag);

        public virtual void Dispose()
        {
            UILog.Write("LIFECYCLE", $"Dispose '{this}'");
        }

        private void UpdateLayoutCss()
        {
            var sb = new StringBuilder();
            ComputeOwnLayoutCss(sb);
            Parent?.ComputeChildLayoutCss(sb, this);
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

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            UILog.Write("LIFECYCLE", $"BuildRenderTree '{this}'");
            _debugRenderCount = !_debugRenderCount;
        }
    }
}
