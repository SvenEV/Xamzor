using System.Collections.Generic;
using Xamzor.UI.Components;

namespace Xamzor.UI
{
    public class LayoutManager
    {
        private readonly Queue<UIElement> _toMeasure = new Queue<UIElement>();
        private readonly Queue<UIElement> _toArrange = new Queue<UIElement>();
        private bool _queued;
        private bool _running;

        public static LayoutManager Instance { get; } = new LayoutManager();

        public void InvalidateMeasure(UIElement control)
        {
            _toMeasure.Enqueue(control);
            _toArrange.Enqueue(control);
            QueueLayoutPass();
            UILog.Write("LAYOUT", $"InvalidateMeasure on '{control}' - now {_toMeasure.Count} in queue");
        }

        public void InvalidateArrange(UIElement control)
        {
            _toArrange.Enqueue(control);
            QueueLayoutPass();
        }

        public void ExecuteLayoutPass()
        {
            const int MaxPasses = 3;

            if (!_running)
            {
                _running = true;

                using (UILog.BeginScope("LAYMAN",
                    $"Started layout pass. To measure: {_toMeasure.Count} To arrange: {_toArrange.Count}",
                    () => $"Layout done"))
                {
                    try
                    {
                        for (var pass = 0; pass < MaxPasses; ++pass)
                        {
                            ExecuteMeasurePass();
                            ExecuteArrangePass();

                            if (_toMeasure.Count == 0)
                                break;
                        }
                    }
                    finally
                    {
                        _running = false;
                    }
                }
            }

            _queued = false;
        }

        public void ExecuteInitialLayoutPass(XamzorView root)
        {
            Measure(root);
            Arrange(root);

            // Running the initial layout pass may have caused some control to be invalidated
            // so run a full layout pass now (this usually due to scrollbars; its not known 
            // whether they will need to be shown until the layout pass has run and if the
            // first guess was incorrect the layout will need to be updated).
            ExecuteLayoutPass();
        }

        private void ExecuteMeasurePass()
        {
            while (_toMeasure.Count > 0)
            {
                var control = _toMeasure.Dequeue();

                if (!control.IsMeasureValid)
                    Measure(control);
            }
        }

        private void ExecuteArrangePass()
        {
            while (_toArrange.Count > 0 && _toMeasure.Count == 0)
            {
                var control = _toArrange.Dequeue();

                if (!control.IsArrangeValid)
                    Arrange(control);
            }
        }

        private void Measure(UIElement control)
        {
            // Controls closest to the visual root need to be arranged first. We don't try to store
            // ordered invalidation lists, instead we traverse the tree upwards, measuring the
            // controls closest to the root first. This has been shown by benchmarks to be the
            // fastest and most memory-efficent algorithm.
            if (control.Properties.Get<XamzorComponent>(XamzorComponent.ParentProperty) is UIElement parent)
            {
                Measure(parent);
            }

            // If the control being measured has IsMeasureValid == true here then its measure was
            // handed by an ancestor and can be ignored. The measure may have also caused the
            // control to be removed.
            if (!control.IsMeasureValid)
            {
                UILog.Write("LAYMAN", $"Measuring '{control}'");

                if (control is XamzorView root)
                {
                    root.MeasureRoot();
                }
                else
                {
                    control.Measure(control.PreviousMeasureInput.Value);
                }
            }
        }

        private void Arrange(UIElement control)
        {
            var props = new LayoutProperties(control.Properties);

            if (props.Parent is UIElement parent)
            {
                Arrange(parent);
            }

            if (!control.IsArrangeValid)
            {
                UILog.Write("LAYMAN", $"Arranging '{control}'");

                if (control is XamzorView root)
                {
                    root.ArrangeRoot();
                }
                else if (control.PreviousArrangeInput != null)
                {
                    // Has been observed that PreviousArrange sometimes is null, probably a bug somewhere else.
                    // Condition observed: control.VisualParent is Scrollbar, control is Border.
                    control.Arrange(control.PreviousArrangeInput.Value);
                }
            }
        }

        private void QueueLayoutPass()
        {
            if (!_queued && !_running)
            {
                _queued = true;
            }
        }

        public void RunQueuedLayoutPass()
        {
            if (_queued)
                ExecuteLayoutPass();
        }
    }

    public struct LayoutProperties
    {
        private readonly PropertyContainer Properties;

        public LayoutProperties(PropertyContainer props) => Properties = props;

        public XamzorComponent Parent => Properties.Get<XamzorComponent>(XamzorComponent.ParentProperty);
        public ISet<XamzorComponent> Children => Properties.Get<ISet<XamzorComponent>>(XamzorComponent.ChildrenProperty);
        public double Width => Properties.Get<double>(UIElement.WidthProperty);
        public double Height => Properties.Get<double>(UIElement.HeightProperty);
        public double MinWidth => Properties.Get<double>(UIElement.MinWidthProperty);
        public double MinHeight => Properties.Get<double>(UIElement.MinHeightProperty);
        public double MaxWidth => Properties.Get<double>(UIElement.MaxWidthProperty);
        public double MaxHeight => Properties.Get<double>(UIElement.MaxHeightProperty);
        public Thickness Margin => Properties.Get<Thickness>(UIElement.MarginProperty);
        public Thickness Padding => Properties.Get<Thickness>(UIElement.PaddingProperty);
        public Alignment HorizontalAlignment => Properties.Get<Alignment>(UIElement.HorizontalAlignmentProperty);
        public Alignment VerticalAlignment => Properties.Get<Alignment>(UIElement.VerticalAlignmentProperty);
        public double Opacity => Properties.Get<double>(UIElement.OpacityProperty);
    }
}
