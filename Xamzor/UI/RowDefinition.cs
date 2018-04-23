namespace Xamzor.UI
{
    public class RowDefinition
    {
        public GridLength Height { get; set; }

        public double MinHeight { get; set; } = 0;

        public double MaxHeight { get; set; } = double.PositiveInfinity;

        public RowDefinition(GridLength size) => Height = size;
    }
}
