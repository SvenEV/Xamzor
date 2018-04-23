using System;

namespace Xamzor.UI
{
    public static class Application
    {
        public static bool IsDebugOutlineEnabled { get; set; } = false;

        public static event Action WindowResized;

        public static void JSNotifyWindowResized() => 
            WindowResized?.Invoke();
    }
}
