using Layman;
using Layman.Diagnostics;
using System.Collections.Generic;
using Xamzor.UI.Components;

namespace Xamzor.UI
{
    public class LayoutManager
    {
        private readonly Queue<UIElement> _toArrange = new Queue<UIElement>();
        private bool _queued;
        private bool _running;

        public static LayoutManager Instance { get; } = new LayoutManager();

        public ILayoutTraceWriter TraceWriter { get; set; }
        
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
                    $"Started layout pass. To arrange: {_toArrange.Count}",
                    () => $"Layout done"))
                {
                    try
                    {
                        for (var pass = 0; pass < MaxPasses; ++pass)
                        {
                            ExecuteArrangePass();

                            if (_toArrange.Count == 0)
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
            Arrange(root);

            // Running the initial layout pass may have caused some control to be invalidated
            // so run a full layout pass now (this usually due to scrollbars; its not known 
            // whether they will need to be shown until the layout pass has run and if the
            // first guess was incorrect the layout will need to be updated).
            ExecuteLayoutPass();
        }

        private void ExecuteArrangePass()
        {
            System.Console.WriteLine("ExecuteArrangePass");

            while (_toArrange.Count > 0)
            {
                var control = _toArrange.Dequeue();
                System.Console.WriteLine("About to arrange " + control + ": " + control.LayoutCache.IsArrangeValid);

                if (!control.LayoutCache.IsArrangeValid)
                    Arrange(control);
            }
        }

        private void Arrange(UIElement control)
        {
            if (control.Properties.Get<XamzorComponent>(XamzorComponent.ParentProperty) is UIElement parent)
            {
                Arrange(parent);
            }

            if (!control.LayoutCache.IsArrangeValid)
            {
                var rect = (control is XamzorView root)
                    ? new Rect(Vector2.Zero, root.DetermineRootSize())
                    : control.LayoutCache.PreviousArrangeInput.Value;

                UILog.Write("LAYMAN", $"Arranging '{control}' with {rect}");

                var context = LayoutContext.CreateForArrange(rect, TraceWriter);
                control.Layout(context);
            }
        }

        private void QueueLayoutPass()
        {
            if (!_queued && !_running)
                _queued = true;
        }

        public void RunQueuedLayoutPass()
        {
            if (_queued)
                ExecuteLayoutPass();
        }
    }
}
