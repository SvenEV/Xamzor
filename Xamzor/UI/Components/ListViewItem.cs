using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;

namespace Xamzor.UI.Components
{
    public class ListViewItem : ContentControl
    {
        public EventHandler<UIMouseEventArgs> Clicked { get; set; }

        protected override void BuildContentRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<XButton>(0);
            builder.AddAttribute(1, nameof(Parent), this);
            builder.AddAttribute(2, nameof(XButton.Clicked), new EventHandler<UIMouseEventArgs>(OnClicked));
            builder.AddAttribute(3, nameof(ChildContent), (RenderFragment)base.BuildContentRenderTree);
            builder.CloseComponent();
        }

        private void OnClicked(object sender, UIMouseEventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }
}