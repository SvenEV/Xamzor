using Microsoft.AspNetCore.Blazor.Components;
using Xamzor.UI.Components;

namespace Xamzor.UI
{
    public class DataTemplate : XamzorComponent
    {
        public static readonly PropertyKey DataContextProperty = PropertyKey.Create<object, DataTemplate>(nameof(DataContext));
        
        [Parameter]
        protected object DataContext
        {
            get => Properties.Get<object>(DataContextProperty);
            set => Properties.Set(DataContextProperty, value);
        }
    }
}
