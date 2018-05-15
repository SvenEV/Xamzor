using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;

namespace Xamzor.UI.Components
{
    public class ContentControl : UIElement
    {
        public static readonly PropertyKey ContentProperty = PropertyKey.Create<object, ContentControl>(nameof(Content));
        public static readonly PropertyKey ContentTemplateProperty = PropertyKey.Create<Type, ContentControl>(nameof(ContentTemplate));

        /// <summary>
        /// The data object to display.
        /// </summary>
        [Parameter]
        protected object Content { get => Properties.Get<object>(ContentProperty); set => Properties.Set(ContentProperty, value); }

        /// <summary>
        /// The component type used to render each item. Should implement <see cref="IDataTemplate{T}"/>.
        /// If null, a string representation of <see cref="Content"/> is rendered using a
        /// <see cref="TextBlock"/>.
        /// </summary>
        [Parameter]
        protected Type ContentTemplate { get => Properties.Get<Type>(ContentTemplateProperty); set => Properties.Set(ContentTemplateProperty, value); }

        protected virtual bool OverridesContainer => false;

        protected RenderFragment ActualContent => BuildContentRenderTree;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            if (!OverridesContainer)
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "id", Id);
                builder.AddAttribute(2, "class", CssClass);
                builder.AddAttribute(3, "style", LayoutCss);
                builder.AddElementReferenceCapture(4, r => LayoutRoot = r);
                BuildContentRenderTree(builder);
                builder.CloseElement();
            }
        }

        protected virtual void BuildContentRenderTree(RenderTreeBuilder builder)
        {
            if (ChildContent != null)
            {
                ChildContent(builder);
            }
            else if (ContentTemplate != null)
            {
                builder.OpenComponent(0, ContentTemplate);
                builder.AddAttribute(1, ParentProperty.Name, this);
                builder.AddAttribute(2, DataTemplate.DataContextProperty.Name, Content);
                builder.CloseComponent();
            }
            else if (Content != null)
            {
                builder.OpenComponent<TextBlock>(3);
                builder.AddAttribute(4, ParentProperty.Name, this);
                builder.AddAttribute(5, TextBlock.TextProperty.Name, Content?.ToString());
                builder.CloseComponent();
            }
        }
    }
}