using Microsoft.AspNetCore.Blazor.RenderTree;
using System;

namespace Xamzor.UI.Components
{
    public class ContentControl : UIElement
    {
        /// <summary>
        /// The data object to display.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// The component type used to render each item. Should implement <see cref="IDataTemplate{T}"/>.
        /// If null, a string representation of <see cref="Content"/> is rendered using a
        /// <see cref="TextBlock"/>.
        /// </summary>
        public Type ContentTemplate { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "id", Id);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "style", LayoutCss);

            BuildContentRenderTree(builder);

            builder.CloseElement();
        }

        protected virtual void BuildContentRenderTree(RenderTreeBuilder builder)
        {
            if (ContentTemplate != null)
            {
                builder.OpenComponent(0, ContentTemplate);
                builder.AddAttribute(1, nameof(Parent), Helpers.PARENT);
                builder.AddAttribute(2, nameof(IDataTemplate<object>.DataContext), Content);
                builder.CloseComponent();
            }
            else if (Content != null)
            {
                builder.OpenComponent<TextBlock>(3);
                builder.AddAttribute(4, nameof(Parent), Helpers.PARENT);
                builder.AddAttribute(5, nameof(TextBlock.Text), Content?.ToString());
                builder.CloseComponent();
            }
        }
    }
}