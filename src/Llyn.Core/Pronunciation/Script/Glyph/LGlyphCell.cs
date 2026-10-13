namespace Llyn.Core;

public sealed record LGlyphCell(string LGlyphCellText, string LGlyphCellLanguage)
{
    public bool LGlyphCellLinked => LGlyphCellLanguage.Length > 0;
}
