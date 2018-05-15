using Layman;
using Microsoft.AspNetCore.Blazor.Browser.Interop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamzor.UI
{
    public class ImageMeasureInterop
    {
        private static readonly Dictionary<string, TaskCompletionSource<Vector2>> _tasks =
            new Dictionary<string, TaskCompletionSource<Vector2>>();

        public static Task<Vector2> MeasureImageAsync(string source)
        {
            if (_tasks.TryGetValue(source, out var existingTcs))
                return existingTcs.Task;

            var tcs = new TaskCompletionSource<Vector2>();
            _tasks.Add(source, tcs);
            RegisteredFunction.Invoke<string>("Xamzor.measureImage", source);
            return tcs.Task;
        }

        // Called from JS after image got loaded and naturalWidth/naturalHeight are available
        public static void NotifyImageMeasured(string source, string measuredSize)
        {
            if (_tasks.TryGetValue(source, out var tcs))
            {
                var parts = measuredSize.Split(',');
                var size = new Vector2(double.Parse(parts[0]), double.Parse(parts[1]));
                tcs.SetResult(size);
                UILog.Write("IMAGE", $"Reported measure result '{measuredSize}' for '{source}'");
            }
        }
    }
}
