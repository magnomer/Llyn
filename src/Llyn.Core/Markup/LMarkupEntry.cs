using System;
using System.Collections.Generic;

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
                entries.Add(LMarkupEntryCreate(tokens, position, close, entries.Count + 1));
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
        IReadOnlyList<LMarkupToken> tokens, int first, int last, int place)
    {
        LMarkupToken? headword = null;
        LMarkupToken? language = null;
        LMarkupToken? pronunciation = null;
        LMarkupToken? audio = null;
        LMarkupToken? speech = null;
        LMarkupToken? note = null;
        List<LCardDraft> meanings = new List<LCardDraft>();
        List<LCardDraft> collocations = new List<LCardDraft>();

        for (int position = first + 1; position < last; position++)
        {
            LMarkupToken token = tokens[position];

            if (token.LMarkupTokenKind == LMarkupTokenKind.LMarkupTokenEnter)
            {
                int close = LMarkupBlockFind(tokens, position);
                IReadOnlyList<LMarkupToken> block = LMarkupBlockRead(tokens, position, close);

                switch (token.LMarkupTokenName)
                {
                    case "sense":
                        meanings.Add(LMarkupCardRead(block, meanings.Count + 1));
                        break;
                    case "collocation":
                        collocations.Add(LMarkupCardRead(block, collocations.Count + 1));
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
                case "ipa":
                    pronunciation = token;
                    break;
                case "audio":
                    audio = token;
                    break;
                case "pos":
                    speech = token;
                    break;
                case "note":
                    note = token;
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
            LMarkupStateRead(pronunciation).LStateValueShow(),
            LMarkupStateRead(note).LStateValueShow(),
            meanings,
            collocations,
            LMarkupStateRead(audio).LStateValueShow(),
            null,
            LMarkupSpeechRead(speech));

        return new LMarkupEntry(draft, tokens[first].LMarkupTokenRead("id") ?? string.Empty);
    }

    private static IReadOnlyList<string> LMarkupSpeechRead(LMarkupToken? token)
    {
        List<string> speeches = new List<string>();
        if (token is null || token.Value.LMarkupTokenEmpty)
        {
            return speeches;
        }

        foreach (string part in token.Value.LMarkupTokenText.Split(','))
        {
            string name = part.Trim();
            if (name.Length > 0)
            {
                speeches.Add(name);
            }
        }

        return speeches;
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
