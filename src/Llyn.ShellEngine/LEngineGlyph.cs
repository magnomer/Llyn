using System;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LGlyph? LEngineGlyphRead(string language)
    {
        return string.IsNullOrWhiteSpace(language) ? null : LEngineLanguageLoad(language).LLanguageGlyph;
    }

    public LEntry LEngineGlyphResolve(string character, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(character);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string headword = character.Trim();
        lock (_lEngineGate)
        {
            foreach (LEntry entry in _lEngineEntries.LEntryFind(headword))
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
