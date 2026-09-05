using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static partial class LMarkup
{
    public readonly record struct LMarkupEntry(
        LEntryDraft LMarkupEntryDraft,
        IReadOnlyList<LMarkupReference> LMarkupEntrySource);

    public static IReadOnlyList<LMarkupEntry> LMarkupEntryRead(string text)
    {
        IReadOnlyList<LMarkupToken> tokens = LMarkupScan(text);
        List<LMarkupEntry> entries = new List<LMarkupEntry>();
        int position = 0;

        while (position < tokens.Count)
        {
            if (tokens[position].LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenEnter ||
                !string.Equals(tokens[position].LMarkupTokenName, "entry", StringComparison.Ordinal))
            {
                position++;
                continue;
            }

            int close = LMarkupBlockFind(tokens, position);
            entries.Add(LMarkupEntryCreate(tokens, position, close, entries.Count + 1));
            position = close + 1;
        }

        return entries;
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
        List<LCardDraft> senses = new List<LCardDraft>();
        List<LCardDraft> collocations = new List<LCardDraft>();
        List<LMarkupReference> sources = new List<LMarkupReference>();

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
                        senses.Add(LMarkupCardRead(block, senses.Count + 1));
                        break;
                    case "collocation":
                        collocations.Add(LMarkupCardRead(block, collocations.Count + 1));
                        break;
                    case "source":
                        sources.Add(LMarkupReferenceRead(block));
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

        HashSet<string> registry = new HashSet<string>(StringComparer.Ordinal);
        foreach (LMarkupReference source in sources)
        {
            if (!registry.Add(source.LMarkupReferenceId))
            {
                throw new FormatException(
                    $"Entry {place} declares the source id '{source.LMarkupReferenceId}' twice.");
            }
        }

        LMarkupCitationCheck(senses, registry);
        LMarkupCitationCheck(collocations, registry);

        LEntryDraft draft = new LEntryDraft(
            word,
            LMarkupStateRead(language).LStateValueShow(),
            LMarkupStateRead(pronunciation).LStateValueShow(),
            LMarkupStateRead(note).LStateValueShow(),
            senses,
            collocations,
            LMarkupStateRead(audio).LStateValueShow(),
            null,
            LMarkupSpeechRead(speech));

        return new LMarkupEntry(draft, sources);
    }

    private static void LMarkupCitationCheck(
        IReadOnlyList<LCardDraft> cards, IReadOnlySet<string> registry)
    {
        foreach (LCardDraft card in cards)
        {
            foreach (LExampleDraft example in card.LCardDraftExample)
            {
                LMarkupSourceCheck(example.LExampleDraftReference, registry);
            }

            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                LMarkupSourceCheck(situation.LSituationDraftReference, registry);
            }
        }
    }

    private static void LMarkupSourceCheck(LStateValue citation, IReadOnlySet<string> registry)
    {
        if (citation.LStateValueState != LState.LStateSpecified)
        {
            return;
        }

        string key = citation.LStateValueText ?? string.Empty;
        if (!registry.Contains(key))
        {
            throw new FormatException($"Citation names no source declared in its entry: '{key}'.");
        }
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
