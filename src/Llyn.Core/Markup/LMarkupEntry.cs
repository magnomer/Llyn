using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public static partial class LMarkup
{
    public readonly record struct LMarkupEntry(
        LEntryDraft LMarkupEntryDraft,
        string LMarkupEntryKey);

    public readonly record struct LMarkupDocument(
        LMarkupCatalog LMarkupDocumentCatalog,
        IReadOnlyList<LMarkupEntry> LMarkupDocumentEntry);

    public static LMarkupDocument LMarkupEntryRead(string text)
    {
        IReadOnlyList<LMarkupToken> tokens = LMarkupScan(text);

        if (tokens.Count == 0
            || tokens[0].LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenEnter
            || !string.Equals(tokens[0].LMarkupTokenName, "llyn", StringComparison.Ordinal))
        {
            throw new FormatException("The document does not open with a '<llyn>' element.");
        }

        int root = LMarkupBlockFind(tokens, 0);
        LMarkupCatalog catalog = LMarkupCatalogFind(tokens, root);
        catalog = catalog with { LMarkupCatalogRow = LMarkupKeyRead(tokens, catalog) };
        LMarkupCitationValidate(tokens, catalog.LMarkupCatalogRow);

        List<LMarkupEntry> entries = new List<LMarkupEntry>();

        for (int position = 1; position < root; position++)
        {
            if (tokens[position].LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenEnter)
            {
                continue;
            }

            int close = LMarkupBlockFind(tokens, position);
            if (string.Equals(tokens[position].LMarkupTokenName, "entry", StringComparison.Ordinal))
            {
                entries.Add(LMarkupEntryCreate(catalog, tokens, position, close, entries.Count + 1));
            }

            position = close;
        }

        return new LMarkupDocument(catalog, entries);
    }

    private static LMarkupCatalog LMarkupCatalogFind(IReadOnlyList<LMarkupToken> tokens, int root)
    {
        for (int position = 1; position < root; position++)
        {
            if (tokens[position].LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenEnter)
            {
                continue;
            }

            int close = LMarkupBlockFind(tokens, position);
            if (string.Equals(tokens[position].LMarkupTokenName, "catalog", StringComparison.Ordinal))
            {
                return LMarkupCatalogRead(tokens, position, close);
            }

            position = close;
        }

        return LMarkupCatalog.LMarkupCatalogCreate();
    }

    private static LMarkupEntry LMarkupEntryCreate(
        LMarkupCatalog catalog, IReadOnlyList<LMarkupToken> tokens, int first, int last, int place)
    {
        LMarkupToken? headword = null;
        LMarkupToken? language = null;
        LMarkupToken? note = null;
        LPronunciationDraft? pronunciation = null;
        List<LSpeechDraft> speeches = new List<LSpeechDraft>();
        List<LForm> forms = new List<LForm>();
        List<LInflection> inflections = new List<LInflection>();
        List<LCardDraft> meanings = new List<LCardDraft>();
        List<LCardDraft> collocations = new List<LCardDraft>();

        for (int position = first + 1; position < last; position++)
        {
            LMarkupToken token = tokens[position];

            if (token.LMarkupTokenKind == LMarkupTokenKind.LMarkupTokenEnter)
            {
                int close = LMarkupBlockFind(tokens, position);

                switch (token.LMarkupTokenName)
                {
                    case "sense":
                        meanings.Add(LMarkupCardRead(
                            catalog, tokens, position, close, meanings.Count + 1, false));
                        break;
                    case "collocation":
                        collocations.Add(LMarkupCardRead(
                            catalog, tokens, position, close, collocations.Count + 1, true));
                        break;
                    case "inflection":
                        LMarkupInflectionRead(
                            inflections, token, LMarkupBlockRead(tokens, position, close));
                        break;
                    case "pronunciation":
                        pronunciation = LMarkupSoundRead(
                            token, LMarkupBlockRead(tokens, position, close), place);
                        break;
                    default:
                        break;
                }

                position = close;
                continue;
            }

            if (token.LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenText)
            {
                continue;
            }

            switch (token.LMarkupTokenName)
            {
                case "headword":
                    headword = token;
                    break;
                case "lang":
                    language = token;
                    break;
                case "note":
                    note = token;
                    break;
                case "pos":
                    LMarkupSpeechRead(speeches, token, place);
                    break;
                case "form":
                    LMarkupFormRead(forms, token);
                    break;
                default:
                    break;
            }
        }

        string word = LMarkupStateRead(headword).LStateValueShow();
        if (word.Length == 0)
        {
            throw new FormatException(headword is null
                ? $"Entry {place} has no headword."
                : $"Entry {place} has an unreadable headword.");
        }

        LEntryDraft draft = new LEntryDraft(
            word,
            LMarkupStateRead(language).LStateValueShow(),
            pronunciation,
            LMarkupStateRead(note).LStateValueShow(),
            meanings,
            collocations,
            speeches,
            forms,
            inflections);

        return new LMarkupEntry(draft, tokens[first].LMarkupTokenRead("id") ?? string.Empty);
    }

    private static LPronunciationDraft? LMarkupSoundRead(
        LMarkupToken token, IReadOnlyList<LMarkupToken> block, int place)
    {
        string ipa = string.Empty;
        string file = string.Empty;
        string? source = null;
        List<LSyllable> syllables = new List<LSyllable>();
        List<LRepresentation> representations = new List<LRepresentation>();

        foreach (LMarkupToken leaf in LMarkupLeafRead(block))
        {
            switch (leaf.LMarkupTokenName)
            {
                case "ipa":
                    ipa = leaf.LMarkupTokenText;
                    break;
                case "audio":
                    file = leaf.LMarkupTokenText;
                    source = leaf.LMarkupTokenRead("source");
                    break;
                case "syllable":
                    syllables.Add(LMarkupSyllableRead(leaf, syllables.Count, place));
                    break;
                case "representation":
                    representations.Add(LMarkupRepresentationRead(leaf, representations.Count));
                    break;
                default:
                    break;
            }
        }

        LPronunciationDraft written = new LPronunciationDraft(
            ipa,
            token.LMarkupTokenRead("level"),
            syllables,
            representations,
            file,
            source);

        return written.LPronunciationDraftEmpty ? null : written;
    }

    private static LSyllable LMarkupSyllableRead(LMarkupToken token, int position, int place)
    {
        string? nucleus = token.LMarkupTokenRead("nucleus");
        if (string.IsNullOrEmpty(nucleus))
        {
            throw new FormatException($"Entry {place} carries a '<syllable>' with no nucleus.");
        }

        return new LSyllable(
            string.Empty,
            position,
            token.LMarkupTokenRead("orthography"),
            token.LMarkupTokenRead("local"),
            token.LMarkupTokenRead("onset"),
            token.LMarkupTokenRead("medial"),
            nucleus,
            token.LMarkupTokenRead("coda"),
            LMarkupToneRead(token.LMarkupTokenRead("tone")),
            token.LMarkupTokenRead("tone-local"),
            token.LMarkupTokenRead("tone-points"));
    }

    private static int? LMarkupToneRead(string? tone)
    {
        return int.TryParse(tone, NumberStyles.Integer, CultureInfo.InvariantCulture, out int number)
            ? number
            : null;
    }

    private static LRepresentation LMarkupRepresentationRead(LMarkupToken token, int position)
    {
        return new LRepresentation(
            string.Empty,
            position,
            token.LMarkupTokenRead("system") ?? string.Empty,
            token.LMarkupTokenRead("role") ?? string.Empty,
            token.LMarkupTokenText,
            token.LMarkupTokenRead("tone"));
    }

    private static void LMarkupInflectionRead(
        List<LInflection> inflections, LMarkupToken token, IReadOnlyList<LMarkupToken> block)
    {
        string text = string.Empty;
        List<LFeature> features = new List<LFeature>();

        foreach (LMarkupToken leaf in LMarkupLeafRead(block))
        {
            switch (leaf.LMarkupTokenName)
            {
                case "text":
                    text = leaf.LMarkupTokenText;
                    break;
                case "feature":
                    features.Add(new LFeature(
                        leaf.LMarkupTokenRead("id") ?? string.Empty,
                        leaf.LMarkupTokenRead("value") ?? string.Empty));
                    break;
                default:
                    break;
            }
        }

        if (text.Length == 0)
        {
            return;
        }

        inflections.Add(new LInflection(
            string.Empty,
            inflections.Count,
            text,
            token.LMarkupTokenRead("local"),
            token.LMarkupTokenRead("pos"),
            features));
    }

    private static void LMarkupFormRead(List<LForm> forms, LMarkupToken token)
    {
        if (token.LMarkupTokenEmpty)
        {
            return;
        }

        forms.Add(new LForm(
            string.Empty,
            forms.Count,
            token.LMarkupTokenText,
            token.LMarkupTokenRead("local"),
            token.LMarkupTokenRead("role") ?? string.Empty));
    }

    private static void LMarkupSpeechRead(List<LSpeechDraft> speeches, LMarkupToken token, int place)
    {
        string? value = token.LMarkupTokenRead("id");
        string named = token.LMarkupTokenEmpty ? string.Empty : token.LMarkupTokenText.Trim();

        if (!string.IsNullOrEmpty(value) && named.Length > 0)
        {
            throw new FormatException(
                $"Entry {place} carries a '<pos>' holding both an id and a name.");
        }

        if (string.IsNullOrEmpty(value) && named.Length == 0)
        {
            throw new FormatException(
                $"Entry {place} carries a '<pos>' holding neither an id nor a name.");
        }

        speeches.Add(string.IsNullOrEmpty(value)
            ? new LSpeechDraft(null, named, named)
            : new LSpeechDraft(value, null, value));
    }

    private static int LMarkupBlockFind(IReadOnlyList<LMarkupToken> tokens, int first)
    {
        int depth = 0;

        for (int position = first; position < tokens.Count; position++)
        {
            if (tokens[position].LMarkupTokenKind == LMarkupTokenKind.LMarkupTokenEnter)
            {
                depth++;
                continue;
            }

            if (tokens[position].LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenLeave)
            {
                continue;
            }

            depth--;
            if (depth == 0)
            {
                return position;
            }
        }

        return tokens.Count - 1;
    }

    private static IReadOnlyList<LMarkupToken> LMarkupBlockRead(
        IReadOnlyList<LMarkupToken> tokens, int first, int last)
    {
        List<LMarkupToken> block = new List<LMarkupToken>();

        for (int position = first; position <= last; position++)
        {
            block.Add(tokens[position]);
        }

        return block;
    }
}
