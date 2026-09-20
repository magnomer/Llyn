using System;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    internal void PWindowGlyphShow(string character, string language)
    {
        LEntry entry;
        try
        {
            entry = _lWindow.LWindowGlyphResolve(character, language);
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Glyph.OpenFailed", exception);
            return;
        }

        PWindowEntryShow(entry.LEntryId);
    }
}
