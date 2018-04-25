using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamzor.UI.Components
{
    public class ListView : UIElement
    {
        public IEnumerable<object> ItemsSource { get; set; }

        /// <summary>
        /// Should have a property "DataContext".
        /// </summary>
        public Type ItemTemplate { get; set; }

        public Type ItemsPanelTemplate { get; set; } = typeof(ListViewDefaultItemsPanel);

        public EventHandler<object> ItemClicked { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            if (ItemTemplate == null || ItemsPanelTemplate == null)
                return;

            builder.OpenComponent(0, ItemsPanelTemplate);
            builder.AddAttribute(1, nameof(Parent), this);
            builder.AddAttribute(2, nameof(ChildContent), (RenderFragment)(builder2 =>
            {
                foreach (var item in ItemsSource ?? Enumerable.Empty<object>())
                {
                    builder2.OpenComponent<ListViewItem>(3);
                    builder2.AddAttribute(4, nameof(Parent), Helpers.PARENT);
                    builder2.AddAttribute(5, nameof(ListViewItem.Content), item);
                    builder2.AddAttribute(6, nameof(ListViewItem.ContentTemplate), ItemTemplate);
                    builder2.AddAttribute(7, nameof(ListViewItem.Clicked), new EventHandler<UIMouseEventArgs>(OnItemClicked));
                    builder2.CloseComponent();
                }
            }));
            builder.CloseComponent();
        }

        private void OnItemClicked(object sender, UIMouseEventArgs e)
        {
            var item = ((ListViewItem)sender).Content;
            ItemClicked?.Invoke(this, item);
        }
    }
}