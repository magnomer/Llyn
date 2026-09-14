using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LGlyph? LEngineGlyphRead(string language)
    {
        return LEngineLanguageLoad(language).LLanguageGlyph;
    }

    public LEntry LEngineGlyphResolve(string character, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(character);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string headword = character.Trim();
        lock (_lEngineGate)
        {
            foreach (LEntry entry in new LEntryArchive(_lEngineDatabase).LEntryFind(headword))
            {
                if (string.Equals(entry.LEntryHeadword, headword, StringComparison.Ordinal)
                    && string.Equals(entry.LEntryLanguage, language, StringComparison.Ordinal))
                {
                    return entry;
                }
            }

            return LEngineTranslationCreate(headword, language);
        }
    }
}
