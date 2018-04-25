using Xamzor.UI.Components;

namespace Xamzor.UI
{
    public interface IDataTemplate<T>
    {
        T DataContext { get; set; }

        // TODO: Temporary, remove if no longer needed
        UIElement PARENT { get; set; }
    }
}
