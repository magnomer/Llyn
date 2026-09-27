using System;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    internal void PWindowGlyphShow(string character, string language)
    {
        long entry;
        try
        {
            entry = PWindowAtelier.CAtelierCatalog.CCatalogGlyphResolve(character, language);
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Glyph.OpenFailed", exception);
            return;
        }

        PWindowEntryShow(entry);
    }
}
