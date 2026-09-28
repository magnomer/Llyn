using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public sealed record QGlyphItem(string QGlyphItemText, string QGlyphItemLanguage, bool QGlyphItemLinked)
{
    internal static void QGlyphItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QGlyphItem glyph || QLook.QLookPartFind<Button>(container, "PGlyphChip") is not Button chip)
        {
            return;
        }

        chip.CommandParameter = glyph;
        if (QLook.QLookPartFind<TextBlock>(chip, "PGlyphCharacter") is TextBlock character)
        {
            character.Text = glyph.QGlyphItemText;
        }

        if (!glyph.QGlyphItemLinked)
        {
            chip.IsHitTestVisible = false;
            chip.Cursor = Cursors.Arrow;
            chip.ToolTip = null;
        }
    }
}
