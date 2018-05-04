using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;

namespace Xamzor.UI.Components
{
    public class ListView : ItemsControl
    {
        public static PropertyKey ItemClickedProperty = PropertyKey.Create<EventHandler<object>, ListView>(nameof(ItemClicked));

        [Parameter]
        protected EventHandler<object> ItemClicked
        {
            get => Properties.Get<EventHandler<object>>(ItemClickedProperty);
            set => Properties.Set(ItemsProperty, value);
        }

        protected override void BuildItemsPanelRenderTree(RenderTreeBuilder builder, RenderFragment renderItems)
        {
            // In contrast to ItemsControl, we want ListView to include a ScrollViewer by default
            builder.OpenComponent(4, ItemsPanelTemplate ?? typeof(ListViewDefaultItemsPanel));
            builder.AddAttribute(5, ParentProperty.Name, this);
            builder.AddAttribute(6, ChildContentProperty.Name, renderItems);
            builder.CloseComponent();
        }

        protected override void BuildItemRenderTree(RenderTreeBuilder builder, object item)
        {
            builder.OpenComponent<ListViewItem>(0);
            builder.AddAttribute(1, ParentProperty.Name, Helpers.PARENT);
            builder.AddAttribute(2, ContentControl.ContentProperty.Name, item);
            builder.AddAttribute(3, ContentControl.ContentTemplateProperty.Name, ItemTemplate);
            builder.AddAttribute(4, ListViewItem.ClickedProperty.Name, new EventHandler<UIMouseEventArgs>(OnItemClicked));
            builder.CloseComponent();
        }

        private void OnItemClicked(object sender, UIMouseEventArgs e)
        {
            var item = ((ListViewItem)sender).Properties.Get<object>(ContentControl.ContentProperty);
            ItemClicked?.Invoke(this, item);
        }
    }
}