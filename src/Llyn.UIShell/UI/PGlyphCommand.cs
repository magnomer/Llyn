using System.Windows.Input;

namespace Llyn.UIShell;

public static class PGlyphCommand
{
    public static RoutedCommand PGlyphCommandNotation { get; } =
        new(nameof(PGlyphCommandNotation), typeof(PGlyphCommand));

    public static RoutedCommand PGlyphCommandEntry { get; } =
        new(nameof(PGlyphCommandEntry), typeof(PGlyphCommand));
}
