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
        public static readonly PropertyKey ChildrenProperty = PropertyKey.Create<ISet<XamzorComponent>, XamzorComponent>(nameof(Children));
        public static readonly PropertyKey ChildContentProperty = PropertyKey.Create<RenderFragment, XamzorComponent>(nameof(ChildContent));

        public readonly PropertyContainer Properties = new PropertyContainer();
        public bool IsRealized { get; private set; }

        private readonly string _cssClasses;
        private bool _debugRenderCount = false;

        protected virtual string CssClass => Application.IsDebugOutlineEnabled
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

        protected ISet<XamzorComponent> Children =>
            Properties.Get<ISet<XamzorComponent>>(ChildrenProperty);
        
        public XamzorComponent()
        {
            Id = GetType().Name + "-" + _nextFreeId++;
            _cssClasses = string.Join(" ", GetBaseTypes(GetType()).Select(t => t.Name));

            IEnumerable<Type> GetBaseTypes(Type type)
            {
                while (type != typeof(XamzorComponent))
                {
                    yield return type;
                    type = type.BaseType;
                }
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            UILog.Write("LIFECYCLE", $"Initialize '{this}'");
        }

        protected override void OnAfterRender()
        {
            base.OnAfterRender();
            IsRealized = true;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            UILog.Write("LIFECYCLE", $"BuildRenderTree '{this}'");
            _debugRenderCount = !_debugRenderCount;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // Clear list of children
            Properties.Set(ChildrenProperty, new HashSet<XamzorComponent>());

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

            Parent?.Properties.Get<ISet<XamzorComponent>>(ChildrenProperty)?.Add(this);

            UILog.Write("LIFECYCLE", $"OnParametersSet '{this}' (parent is '{Parent}')");
        }

        //public override void SetParameters(ParameterCollection parameters)
        //{
        //    UILog.Write("LIFECYCLE", $"SetParameters '{this}' (parent is '{Parent}', props are {string.Join(", ", parameters.ToDictionary().Select(o => $"{o.Key} = {o.Value}"))})");
        //    if (!parameters.TryGetValue(nameof(Parent), out object _) && !(this is XamzorView))
        //        UILog.Write("WARNING", $"No parent assigned to '{this}'");
        //
        //    // Assign to properties
        //    base.SetParameters(parameters);
        //}

        public virtual void Dispose()
        {
            UILog.Write("LIFECYCLE", $"Dispose '{this}'");
        }

        public override string ToString() => Id + (Tag == null ? "" : "-" + Tag);

        public override int GetHashCode() => Id.GetHashCode();
    }
}
