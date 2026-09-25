using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Llyn.Core;

public sealed record LGlyph(
    string LGlyphName,
    string LGlyphLanguage,
    IReadOnlyList<LSourceSpec> LGlyphSources,
    LFont? LGlyphFont = null)
{
    public static IReadOnlyList<string> LGlyphScan(string text)
    {
        List<string> characters = [];
        HashSet<string> seen = [];
        foreach (Rune rune in (text ?? string.Empty).EnumerateRunes())
        {
            if (!LGlyphHanCheck(rune.Value))
            {
                continue;
            }

            string character = rune.ToString();
            if (seen.Add(character))
            {
                characters.Add(character);
            }
        }

        return characters;
    }

    public static bool LGlyphSingleCheck(string text)
    {
        return LGlyphScan(text).Count == 1;
    }

    public bool LGlyphSourced => LGlyphSources.Count > 0;

    public static bool LGlyphRowCheck(LGlyph? glyph, IReadOnlyList<LTranscriptionDraft> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return glyph is not null && rows.Any(spelled => glyph.LGlyphSchemeCheck(spelled.LTranscriptionDraftScheme));
    }

    public static bool LGlyphOtherCheck(LGlyph? glyph, IReadOnlyList<LTranscriptionDraft> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows.Any(spelled => glyph?.LGlyphSchemeCheck(spelled.LTranscriptionDraftScheme) != true);
    }

    public bool LGlyphSchemeCheck(string scheme)
    {
        return string.Equals(LGlyphName, scheme, StringComparison.Ordinal);
    }

    public IReadOnlyList<LGlyphCell> LGlyphDivide(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        string text = draft.LEntryDraftHeadword;
        foreach (LTranscriptionDraft spelled in draft.LEntryDraftTranscriptions)
        {
            if (!spelled.LTranscriptionDraftEmpty && LGlyphSchemeCheck(spelled.LTranscriptionDraftScheme))
            {
                text = spelled.LTranscriptionDraftText;
                break;
            }
        }

        List<LGlyphCell> cells = [];
        foreach (Rune rune in text.EnumerateRunes())
        {
            string character = rune.ToString();
            cells.Add(new LGlyphCell(character, LGlyphSingleCheck(character) ? LGlyphLanguage : string.Empty));
        }

        return cells;
    }

    public IReadOnlyList<LSourceSpec>? LGlyphSourceRead(string scheme)
    {
        return string.Equals(LGlyphName, scheme, StringComparison.Ordinal) ? LGlyphSources : null;
    }

    private static bool LGlyphHanCheck(int value)
    {
        return value is (>= 0x4E00 and <= 0x9FFF)
            or (>= 0x3400 and <= 0x4DBF)
            or (>= 0xF900 and <= 0xFAFF)
            or (>= 0x20000 and <= 0x3134F)
            or (>= 0x2F800 and <= 0x2FA1F);
    }
}
