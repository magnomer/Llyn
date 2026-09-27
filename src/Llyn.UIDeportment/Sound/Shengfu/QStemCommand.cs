using System.Windows.Input;

namespace Llyn.UIDeportment;

internal static class QStemCommand
{
    internal static RoutedCommand QStemCommandEntry { get; } =
        new(nameof(QStemCommandEntry), typeof(QStemCommand));
}
