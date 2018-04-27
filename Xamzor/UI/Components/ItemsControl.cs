using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;
using System.Collections;
using System.Linq;

namespace Xamzor.UI.Components
{
    public class ItemsControl : UIElement
    {
        /// <summary>
        /// The data items to display.
        /// </summary>
        public IEnumerable Items { get; set; }

        /// <summary>
        /// The component type used to render each item. Should implement <see cref="IDataTemplate{T}"/>.
        /// If null, a string representation of each item is rendered using a <see cref="TextBlock"/>.
        /// </summary>
        public Type ItemTemplate { get; set; }

        /// <summary>
        /// The component type used to render the container panel for all items.
        /// If null, <see cref="ItemsControlDefaultItemsPanel"/> is used which
        /// renders a scrollable, vertical stack of items.
        /// </summary>
        public Type ItemsPanelTemplate { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "id", Id);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "style", LayoutCss);

            builder.OpenComponent(4, ItemsPanelTemplate ?? typeof(ItemsControlDefaultItemsPanel));
            builder.AddAttribute(5, nameof(Parent), this);
            builder.AddAttribute(6, nameof(ChildContent), (RenderFragment)(builder2 =>
            {
                foreach (var item in Items ?? Enumerable.Empty<object>())
                    BuildItemRenderTree(builder2, item);
            }));
            builder.CloseComponent();

            builder.CloseElement();
        }

        /// <summary>
        /// Directly renders the component referred to by <see cref="ItemTemplate"/>.
        /// By overriding this method, subtypes can render a container hosting the template.
        /// </summary>
        protected virtual void BuildItemRenderTree(RenderTreeBuilder builder, object item)
        {
            if (ItemTemplate != null)
            {
                builder.OpenComponent(0, ItemTemplate);
                builder.AddAttribute(1, nameof(Parent), Helpers.PARENT);
                builder.AddAttribute(2, nameof(IDataTemplate<object>.DataContext), item);
                builder.CloseComponent();
            }
            else if (item != null)
            {
                builder.OpenComponent<TextBlock>(3);
                builder.AddAttribute(4, nameof(Parent), Helpers.PARENT);
                builder.AddAttribute(5, nameof(TextBlock.Text), item?.ToString());
                builder.CloseComponent();
            }
        }
    }
}