using System;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    internal void PWindowGlyphShow(string character, string language)
    {
        long entry;
        try
        {
            entry = _lWindow.LWindowWorkspace.QWorkspaceGlyphResolve(character, language);
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Glyph.OpenFailed", exception);
            return;
        }

        PWindowEntryShow(entry);
    }
}
