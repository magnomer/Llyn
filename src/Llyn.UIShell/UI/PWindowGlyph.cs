using System;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PWindow
{
    internal void PWindowGlyphShow(string character, string language)
    {
        LEntry entry;
        try
        {
            entry = _lEngine.LEngineGlyphResolve(character, language);
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Glyph.OpenFailed", exception);
            return;
        }

        PWindowEntryShow(entry.LEntryId);
    }
}
