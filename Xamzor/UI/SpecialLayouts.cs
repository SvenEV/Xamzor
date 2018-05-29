using Layman;
using System;
using static Layman.Layouts;

namespace Xamzor.UI
{
    public static class SpecialLayouts
    {
        /// <summary>
        /// Equivalent to <see cref="Inset(Thickness, LayoutFunc)"/> except that no offset is
        /// applied when arranging the child layout. This is needed for HTML borders because
        /// CSS absolute positioning already accounts for border thickness.
        /// </summary>
        public static LayoutFunc InsetWithoutOffset(Thickness thickness, LayoutFunc child = null) => Trace(
            $"InsetWithoutOffset[{thickness}]",
            context =>
            {
                var constrainedSpace = Vector2.Max(Vector2.Zero, context.Space - thickness.Size);

                switch (context.Phase)
                {
                    case LayoutPhase.Measure:
                        var childMeasureInput = context.ForMeasure(constrainedSpace);
                        var childSize = child?.Invoke(childMeasureInput).DesiredSize ?? Vector2.Zero;
                        return childSize + thickness.Size;

                    case LayoutPhase.Arrange:
                        if (child == null)
                            return context.Rect;

                        var childArrangeInput = context.ForArrange(
                            context.Offset,
                            constrainedSpace);

                        var childBounds = child(childArrangeInput).Bounds;

                        return new Rect(
                            childBounds.TopLeft,
                            childBounds.Size + thickness.Size);

                    default:
                        throw new NotImplementedException();
                }
            });
    }
}
