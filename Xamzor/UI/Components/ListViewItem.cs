using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;

namespace Xamzor.UI.Components
{
    public class ListViewItem : ContentControl
    {
        public static readonly PropertyKey ClickedProperty = PropertyKey.Create<EventHandler<UIMouseEventArgs>, ListViewItem>(nameof(Clicked));

        [Parameter]
        protected EventHandler<UIMouseEventArgs> Clicked
        {
            get => Properties.Get<EventHandler<UIMouseEventArgs>>(ClickedProperty);
            set => Properties.Set(ClickedProperty, value);
        }

        protected override void BuildContentRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<XButton>(0);
            builder.AddAttribute(1, ParentProperty.Name, this);
            builder.AddAttribute(2, XButton.ClickedProperty.Name, new EventHandler<UIMouseEventArgs>(OnClicked));
            builder.AddAttribute(3, ChildContentProperty.Name, (RenderFragment)base.BuildContentRenderTree);
            builder.CloseComponent();
        }

        private void OnClicked(object sender, UIMouseEventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }
}