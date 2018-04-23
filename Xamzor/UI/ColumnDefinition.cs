namespace Xamzor.UI
{
    public class ColumnDefinition
    {
        public GridLength Width { get; set; }

        public double MinWidth { get; set; } = 0;

        public double MaxWidth { get; set; } = double.PositiveInfinity;

        public ColumnDefinition(GridLength size) => Width = size;
    }
}
