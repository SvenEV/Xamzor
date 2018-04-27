using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.RenderTree;

namespace Xamzor.UI.Components
{
    public class FlipView : ItemsControl
    {
        protected override void BuildItemsPanelRenderTree(RenderTreeBuilder builder, RenderFragment renderItems)
        {
            // TODO: Should we expose 'ItemsPanelTemplate' at all in 'ItemsControl', even if
            // a derived class does not honor it (as in this case)?
            builder.OpenComponent(4, typeof(FlipViewItemsPanel));
            builder.AddAttribute(5, nameof(Parent), this);
            builder.AddAttribute(6, nameof(ChildContent), renderItems);
            builder.CloseComponent();
        }
    }
}