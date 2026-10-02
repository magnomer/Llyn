using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class QGlyphCommand
{
    public static RoutedCommand QGlyphCommandNotation { get; } =
        new(nameof(QGlyphCommandNotation), typeof(QGlyphCommand));

    public static RoutedCommand QGlyphCommandEntry { get; } =
        new(nameof(QGlyphCommandEntry), typeof(QGlyphCommand));
}
