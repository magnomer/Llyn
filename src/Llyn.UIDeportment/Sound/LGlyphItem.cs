using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public sealed record LGlyphItem(string LGlyphItemText, string LGlyphItemLanguage)
{
    internal static void LGlyphItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LGlyphItem glyph || PLook.PLookPartFind<Button>(container, "PGlyphChip") is not Button chip)
        {
            return;
        }

        chip.CommandParameter = glyph;
        if (PLook.PLookPartFind<TextBlock>(chip, "PGlyphCharacter") is TextBlock character)
        {
            character.Text = glyph.LGlyphItemText;
        }

        if (glyph.LGlyphItemLanguage.Length == 0)
        {
            chip.IsHitTestVisible = false;
            chip.Cursor = Cursors.Arrow;
            chip.ToolTip = null;
        }
    }
}
