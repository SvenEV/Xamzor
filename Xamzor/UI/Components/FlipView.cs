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
            builder.OpenComponent(0, typeof(FlipViewItemsPanel));
            builder.AddAttribute(1, ParentProperty.Name, this);
            builder.AddAttribute(2, ChildContentProperty.Name, renderItems);
            builder.CloseComponent();
        }

        //protected override void BuildItemRenderTree(RenderTreeBuilder builder, object item)
        //{
        //    // Each item is wrapped in a horizontally & vertically stretching Border
        //    // to ensure that it covers 100% of the FlipView rect
        //    builder.OpenComponent<Border>(0);
        //    builder.AddAttribute(1, ParentProperty.Name, Helpers.PARENT);
        //    builder.AddAttribute(2, ChildContentProperty.Name, 
        //        (RenderFragment)(innerBuilder => base.BuildItemRenderTree(innerBuilder, item)));
        //    builder.CloseComponent();
        //}
    }
}