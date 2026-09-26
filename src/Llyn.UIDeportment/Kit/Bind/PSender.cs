using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal static class PSender
{
    internal static PSenderItem? PSenderItemRead<PSenderItem>(object sender)
        where PSenderItem : class
    {
        return sender is FrameworkElement { DataContext: PSenderItem item } ? item : null;
    }

    internal static PSenderItem? PSenderSourceRead<PSenderItem>(RoutedEventArgs e)
        where PSenderItem : class
    {
        return e.OriginalSource is FrameworkElement { DataContext: PSenderItem item } ? item : null;
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

    internal static string? PSenderTextRead(ExecutedRoutedEventArgs e)
    {
        return e.Parameter as string;
    }

    internal static bool? PSenderFlagRead(ExecutedRoutedEventArgs e)
    {
        return e.Parameter as bool?;
    }

    internal static PSenderItem? PSenderFocusRead<PSenderItem>()
        where PSenderItem : class
    {
        return Keyboard.FocusedElement is FrameworkElement { DataContext: PSenderItem item } ? item : null;
    }

    internal static string PSenderKeyRead(KeyEventArgs e)
    {
        return e.Key switch
        {
            Key.Enter => "Enter",
            Key.Escape => "Escape",
            Key.Down => "Down",
            Key.Up => "Up",
            _ => string.Empty,
        };
    }
}
