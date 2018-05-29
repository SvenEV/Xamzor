using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;
using System.Collections;
using System.Linq;

namespace Xamzor.UI.Components
{
    public class ItemsControl : UIElement
    {
        public static readonly PropertyKey ItemsProperty = PropertyKey.Create<IEnumerable, ItemsControl>(nameof(Items));
        public static readonly PropertyKey ItemTemplateProperty = PropertyKey.Create<Type, ItemsControl>(nameof(ItemTemplate));
        public static readonly PropertyKey ItemsPanelTemplateProperty = PropertyKey.Create<Type, ItemsControl>(nameof(ItemsPanelTemplate));

        /// <summary>
        /// The data items to display.
        /// </summary>
        [Parameter]
        protected IEnumerable Items
        {
            get => Properties.Get<IEnumerable>(ItemsProperty);
            set => Properties.Set(ItemsProperty, value);
        }

        /// <summary>
        /// The component type used to render each item. Should implement <see cref="IDataTemplate{T}"/>.
        /// If null, a string representation of each item is rendered using a <see cref="TextBlock"/>.
        /// </summary>
        [Parameter]
        protected Type ItemTemplate
        {
            get => Properties.Get<Type>(ItemTemplateProperty);
            set => Properties.Set(ItemTemplateProperty, value);
        }

        /// <summary>
        /// The component type used to render the container panel for all items.
        /// If null, <see cref="ItemsControlDefaultItemsPanel"/> is used which
        /// renders a vertical stack of items.
        /// </summary>
        [Parameter]
        protected Type ItemsPanelTemplate
        {
            get => Properties.Get<Type>(ItemsPanelTemplateProperty);
            set => Properties.Set(ItemsPanelTemplateProperty, value);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "id", Id);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "style", CssStyle);
            builder.AddElementReferenceCapture(4, r => LayoutRoot = r);

            BuildItemsPanelRenderTree(builder, innerBuilder =>
            {
                foreach (var item in Items ?? Enumerable.Empty<object>())
                    BuildItemRenderTree(innerBuilder, item);
            });

            builder.CloseElement();
        }

        protected virtual void BuildItemsPanelRenderTree(RenderTreeBuilder builder, RenderFragment renderItems)
        {
            builder.OpenComponent(5, ItemsPanelTemplate ?? typeof(ItemsControlDefaultItemsPanel));
            builder.AddAttribute(6, ParentProperty.Name, this);
            builder.AddAttribute(7, ChildContentProperty.Name, renderItems);
            builder.CloseComponent();
        }

        /// <summary>
        /// Directly renders the component referred to by <see cref="ItemTemplate"/>.
        /// By overriding this method, subtypes can render a container hosting the template.
        /// </summary>
        protected virtual void BuildItemRenderTree(RenderTreeBuilder builder, object item)
        {
            if (ItemTemplate != null)
            {
                builder.OpenComponent(8, ItemTemplate);
                builder.AddAttribute(9, ParentProperty.Name, Helpers.PARENT);
                builder.AddAttribute(10, DataTemplate.DataContextProperty.Name, item);
                builder.CloseComponent();
            }
            else if (item != null)
            {
                builder.OpenComponent<TextBlock>(3);
                builder.AddAttribute(11, ParentProperty.Name, Helpers.PARENT);
                builder.AddAttribute(12, TextBlock.TextProperty.Name, item?.ToString());
                builder.CloseComponent();
            }
        }
    }
}