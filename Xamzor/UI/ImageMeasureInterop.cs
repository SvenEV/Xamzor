using Layman;
using Microsoft.AspNetCore.Blazor.Browser.Interop;
using System;
using System.Threading.Tasks;

namespace Xamzor.UI
{
    public class ImageMeasureInterop
    {
        public static async Task<Vector2> MeasureImageAsync(string source)
        {
            try
            {
                var measuredSize = await RegisteredFunction.InvokeAsync<string>("Xamzor.measureImageAsync", source);
                var parts = measuredSize.Split(',');
                var size = new Vector2(double.Parse(parts[0]), double.Parse(parts[1]));
                UILog.Write("IMAGE", $"Reported measure result '{measuredSize}' for '{source}'");
                return size;
            }
            catch (JavaScriptException e)
            {
                Console.WriteLine(e);
                return default;
            }
        }
    }
}
