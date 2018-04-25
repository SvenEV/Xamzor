using Microsoft.AspNetCore.Blazor.RenderTree;
using System;

namespace Xamzor.UI.Components
{
    public class ContentControl : UIElement
    {
        public object Content { get; set; }

        /// <summary>
        /// Should have a property "DataContext"
        /// </summary>
        public Type ContentTemplate { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            if (Content == null || ContentTemplate == null)
                return;
            
            builder.OpenComponent(4, ContentTemplate);
            builder.AddAttribute(5, nameof(Parent), Helpers.PARENT);
            builder.AddAttribute(6, nameof(IDataTemplate<object>.DataContext), Content);
            builder.CloseComponent();
        }
    }
}