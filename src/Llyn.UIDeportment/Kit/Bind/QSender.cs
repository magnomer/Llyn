using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal static class QSender
{
    internal static QSenderItem? QSenderItemRead<QSenderItem>(object sender)
        where QSenderItem : class
    {
        return sender is FrameworkElement { DataContext: QSenderItem item } ? item : null;
    }

    internal static QSenderItem? QSenderSourceRead<QSenderItem>(RoutedEventArgs e)
        where QSenderItem : class
    {
        return e.OriginalSource is FrameworkElement { DataContext: QSenderItem item } ? item : null;
    }

    internal static string? QSenderTagRead(object sender)
    {
        return sender is FrameworkElement { Tag: string tag } ? tag : null;
    }

    internal static QSenderItem? QSenderParameterRead<QSenderItem>(ExecutedRoutedEventArgs e)
        where QSenderItem : class
    {
        return e.Parameter as QSenderItem;
    }

    internal static string? QSenderTextRead(ExecutedRoutedEventArgs e)
    {
        return e.Parameter as string;
    }

    internal static bool? QSenderFlagRead(ExecutedRoutedEventArgs e)
    {
        return e.Parameter as bool?;
    }

    internal static QSenderItem? QSenderFocusRead<QSenderItem>()
        where QSenderItem : class
    {
        return Keyboard.FocusedElement is FrameworkElement { DataContext: QSenderItem item } ? item : null;
    }

    internal static string QSenderKeyRead(KeyEventArgs e)
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
