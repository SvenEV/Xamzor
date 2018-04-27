using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;

namespace Xamzor.UI.Components
{
    public class ListView : ItemsControl
    {
        public EventHandler<object> ItemClicked { get; set; }

        protected override void BuildItemsPanelRenderTree(RenderTreeBuilder builder, RenderFragment renderItems)
        {
            // In contrast to ItemsControl, we want ListView to include a ScrollViewer by default
            builder.OpenComponent(4, ItemsPanelTemplate ?? typeof(ListViewDefaultItemsPanel));
            builder.AddAttribute(5, nameof(Parent), this);
            builder.AddAttribute(6, nameof(ChildContent), renderItems);
            builder.CloseComponent();
        }

        protected override void BuildItemRenderTree(RenderTreeBuilder builder, object item)
        {
            builder.OpenComponent<ListViewItem>(0);
            builder.AddAttribute(1, nameof(Parent), Helpers.PARENT);
            builder.AddAttribute(2, nameof(ListViewItem.Content), item);
            builder.AddAttribute(3, nameof(ListViewItem.ContentTemplate), ItemTemplate);
            builder.AddAttribute(4, nameof(ListViewItem.Clicked), new EventHandler<UIMouseEventArgs>(OnItemClicked));
            builder.CloseComponent();
        }

        private void OnItemClicked(object sender, UIMouseEventArgs e)
        {
            var item = ((ListViewItem)sender).Content;
            ItemClicked?.Invoke(this, item);
        }
    }
}