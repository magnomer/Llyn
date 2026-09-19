using System.Windows;
using System.Windows.Input;

namespace Llyn.UIVeneer;

internal static class PSender
{
    internal static PSenderItem? PSenderItemRead<PSenderItem>(object sender)
        where PSenderItem : class
    {
        return sender is FrameworkElement { DataContext: PSenderItem item } ? item : null;
    }

    internal static string? PSenderTagRead(object sender)
    {
        return sender is FrameworkElement { Tag: string tag } ? tag : null;
    }

    internal static PSenderItem? PSenderParameterRead<PSenderItem>(ExecutedRoutedEventArgs e)
        where PSenderItem : class
    {
        return e.Parameter as PSenderItem;
    }
}
