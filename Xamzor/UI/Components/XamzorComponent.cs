using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamzor.UI.Components
{
    public class XamzorComponent : BlazorComponent, IDisposable
    {
        private static long _nextFreeId = 0;

        public static readonly PropertyKey TagProperty = PropertyKey.Create<string, XamzorComponent>(nameof(Tag));
        public static readonly PropertyKey ParentProperty = PropertyKey.Create<XamzorComponent, XamzorComponent>(nameof(Parent));
        public static readonly PropertyKey ChildContentProperty = PropertyKey.Create<RenderFragment, XamzorComponent>(nameof(Parent));

        public readonly PropertyContainer Properties = new PropertyContainer();
        private readonly string _cssClasses;
        private bool _debugRenderCount = false;

        protected string CssClass => Application.IsDebugOutlineEnabled
            ? _cssClasses + (_debugRenderCount ? " debug1" : " debug2")
            : _cssClasses;

        public string Id { get; }

        [Parameter]
        protected RenderFragment ChildContent
        {
            get => Properties.Get<RenderFragment>(ChildContentProperty);
            set => Properties.Set(ChildContentProperty, value);
        }

        [Parameter]
        protected string Tag
        {
            get => Properties.Get<string>(TagProperty);
            set => Properties.Set(TagProperty, value);
        }

        // HACK
        [Parameter]
        protected XamzorComponent Parent
        {
            get => Properties.Get<XamzorComponent>(ParentProperty);
            set => Properties.Set(ParentProperty, value);
        }
        
        public XamzorComponent()
        {
            Id = GetType().Name + "-" + _nextFreeId++;
            _cssClasses = string.Join(" ", GetBaseTypes(GetType()).Select(t => t.Name));

            IEnumerable<Type> GetBaseTypes(Type type)
            {
                while (type != typeof(UIElement))
                {
                    yield return type;
                    type = type.BaseType;
                }
                yield return typeof(UIElement);
            }
        }

        protected override void OnInit()
        {
            UILog.Write("LIFECYCLE", $"Initialize '{this}'");
        }

        public override void SetParameters(ParameterCollection parameters)
        {
            // Assign to properties
            base.SetParameters(parameters);

            // Inject helper code into ChildContent
            if (ChildContent is RenderFragment originalChildContent)
            {
                ChildContent = builder =>
                {
                    var temp = Helpers.PARENT;
                    Helpers.PARENT = this;
                    originalChildContent(builder);
                    Helpers.PARENT = temp;
                };
            }

            if (!parameters.TryGetValue(nameof(Parent), out object _))
                UILog.Write("WARNING", $"No parent assigned to '{this}'");

            UILog.Write("LIFECYCLE", $"SetParameters '{this}' (parent is '{Parent}')");
        }

        public override string ToString() => Id + (Tag == null ? "" : "-" + Tag);

        public virtual void Dispose()
        {
            UILog.Write("LIFECYCLE", $"Dispose '{this}'");
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            UILog.Write("LIFECYCLE", $"BuildRenderTree '{this}'");
            _debugRenderCount = !_debugRenderCount;
        }
    }
}
