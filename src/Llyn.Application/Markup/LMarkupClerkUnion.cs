using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LMarkupClerkUnion
{
    public static LEntryDraft LMarkupUnionRead(LEntryDraft loaded, LEntryDraft parsed)
    {
        ArgumentNullException.ThrowIfNull(loaded);
        ArgumentNullException.ThrowIfNull(parsed);

        Dictionary<long, long> absorbed = [];
        IReadOnlyList<LCardDraft> meanings = LMarkupCardPlace(
            LMarkupCardAppend(loaded.LEntryDraftMeanings, parsed.LEntryDraftMeanings, absorbed));
        List<LCardDraft> collocations = new(parsed.LEntryDraftCollocations.Count);
        foreach (LCardDraft card in parsed.LEntryDraftCollocations)
        {
            collocations.Add(card with
            {
                LCardDraftSense = [.. card.LCardDraftSense.Select(id => absorbed.GetValueOrDefault(id, id)).Distinct()],
            });
        }

        return loaded with
        {
            LEntryDraftPronunciations = LMarkupRowAppend(
                loaded.LEntryDraftPronunciations,
                parsed.LEntryDraftPronunciations,
                static row => string.IsNullOrWhiteSpace(row.LPronunciationDraftIpa)
                    ? $"{row.LPronunciationDraftIpa}\n{row.LPronunciationDraftAudio}"
                    : row.LPronunciationDraftIpa),
            LEntryDraftTranscriptions = LMarkupRowAppend(
                loaded.LEntryDraftTranscriptions,
                parsed.LEntryDraftTranscriptions,
                static row => row.LTranscriptionDraftScheme,
                static scheme => scheme.Trim()),
            LEntryDraftReflexes = LMarkupRowAppend(
                loaded.LEntryDraftReflexes,
                parsed.LEntryDraftReflexes,
                static row => string.Join(
                    ' ',
                    row.LReflexDraftLanguage,
                    row.LReflexDraftRegion,
                    row.LReflexDraftKind,
                    row.LReflexDraftText,
                    row.LReflexDraftRomanization)),
            LEntryDraftSpeeches = LMarkupRowAppend(
                loaded.LEntryDraftSpeeches,
                parsed.LEntryDraftSpeeches,
                static row => row.LSpeechDraftName),
            LEntryDraftForms = LMarkupRowAppend(
                loaded.LEntryDraftForms,
                parsed.LEntryDraftForms,
                static row => row.LFormText),
            LEntryDraftInflections = LMarkupRowAppend(
                loaded.LEntryDraftInflections,
                parsed.LEntryDraftInflections,
                static row => row.LInflectionText),
            LEntryDraftMeanings = meanings,
            LEntryDraftCollocations = LMarkupCardPlace(
                LMarkupCardAppend(loaded.LEntryDraftCollocations, collocations, [])),
            LEntryDraftNote = LMarkupNoteAppend(loaded.LEntryDraftNote, parsed.LEntryDraftNote),
            LEntryDraftAncestors = loaded.LEntryDraftAncestors.Count == 0
                ? parsed.LEntryDraftAncestors
                : loaded.LEntryDraftAncestors,
            LEntryDraftUnit = loaded.LEntryDraftUnit == LUnit.LUnitEmpty
                ? parsed.LEntryDraftUnit
                : loaded.LEntryDraftUnit,
            LEntryDraftFreedom = loaded.LEntryDraftFreedom == LFreedom.LFreedomEmpty
                ? parsed.LEntryDraftFreedom
                : loaded.LEntryDraftFreedom,
        };
    }

    private static IReadOnlyList<LMarkupRow> LMarkupRowAppend<LMarkupRow>(
        IReadOnlyList<LMarkupRow> loaded, IReadOnlyList<LMarkupRow> parsed, Func<LMarkupRow, string> key)
    {
        return LMarkupRowAppend(loaded, parsed, key, LCatalog.LCatalogTextNormalize);
    }

    private static IReadOnlyList<LMarkupRow> LMarkupRowAppend<LMarkupRow>(
        IReadOnlyList<LMarkupRow> loaded,
        IReadOnlyList<LMarkupRow> parsed,
        Func<LMarkupRow, string> key,
        Func<string, string> fold)
    {
        List<LMarkupRow> merged = [.. loaded];
        HashSet<string> known = new(StringComparer.Ordinal);
        foreach (LMarkupRow row in loaded)
        {
            known.Add(fold(key(row)));
        }

        foreach (LMarkupRow row in parsed)
        {
            string name = fold(key(row));
            if (name.Length == 0 || known.Add(name))
            {
                merged.Add(row);
            }
        }

        return merged;
    }

    private static string LMarkupNoteAppend(string loaded, string parsed)
    {
        if (string.IsNullOrWhiteSpace(parsed) || LMarkupNoteMatch(loaded, parsed))
        {
            return loaded;
        }

        return string.IsNullOrWhiteSpace(loaded) ? parsed : $"{loaded}\n\n{parsed}";
    }

    private static bool LMarkupNoteMatch(string loaded, string parsed)
    {
        string[] stored = LMarkupNoteParse(loaded);
        string[] wanted = LMarkupNoteParse(parsed);
        for (int start = 0; start + wanted.Length <= stored.Length; start++)
        {
            if (stored.AsSpan(start, wanted.Length).SequenceEqual(wanted))
            {
                return true;
            }
        }

        return false;
    }

    private static string[] LMarkupNoteParse(string note)
    {
        return LMarkdown.LMarkdownNormalize(note)
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
            .Select(static block => block.Trim('\n'))
            .Where(static block => block.Length > 0)
            .ToArray();
    }

    private static IReadOnlyList<LCardDraft> LMarkupCardAppend(
        IReadOnlyList<LCardDraft> loaded, IReadOnlyList<LCardDraft> parsed, Dictionary<long, long> absorbed)
    {
        List<LCardDraft> merged = [.. loaded];
        foreach (LCardDraft card in parsed)
        {
            int index = LMarkupCardFind(merged, card);
            if (index < 0)
            {
                merged.Add(card);
            }
            else
            {
                if (card.LCardDraftId != 0)
                {
                    absorbed[card.LCardDraftId] = merged[index].LCardDraftId;
                }

                merged[index] = LMarkupCardAppend(merged[index], card, absorbed);
            }
        }

        return merged;
    }

    private static int LMarkupCardFind(IReadOnlyList<LCardDraft> cards, LCardDraft card)
    {
        string wanted = LMarkupCardFormat(card);
        if (wanted.Length == 0)
        {
            return -1;
        }

        for (int index = 0; index < cards.Count; index++)
        {
            if (LMarkupCardFormat(cards[index]) == wanted)
            {
                return index;
            }
        }

        return -1;
    }

    private static string LMarkupCardFormat(LCardDraft card)
    {
        string title = LCatalog.LCatalogTextNormalize(card.LCardDraftTitle.LStateValueShow());
        string expression = LCatalog.LCatalogTextNormalize(card.LCardDraftExpression.LStateValueShow());
        string meaning = LCatalog.LCatalogTextNormalize(card.LCardDraftMeaning.LStateValueShow());
        return title.Length + expression.Length + meaning.Length == 0
            ? string.Empty
            : $"{title}\n{expression}\n{meaning}";
    }

    private static string LMarkupStateFormat(LStateValue value)
    {
        string shown = LCatalog.LCatalogTextNormalize(value.LStateValueShow());
        return shown.Length == 0 && !value.LStateValueUncertain
            ? string.Empty
            : $"{shown}\n{value.LStateValueUncertain}";
    }

    private static LCardDraft LMarkupCardAppend(LCardDraft loaded, LCardDraft parsed, Dictionary<long, long> absorbed)
    {
        return loaded with
        {
            LCardDraftSentence = LMarkupRowAppend(
                loaded.LCardDraftSentence,
                parsed.LCardDraftSentence,
                static row => row.LSentenceDraftExample?.LExampleDraftText.LStateValueShow() ?? string.Empty),
            LCardDraftSituation = LMarkupRowAppend(
                loaded.LCardDraftSituation,
                parsed.LCardDraftSituation,
                static row => LMarkupStateFormat(row.LSituationDraftTitle)),
            LCardDraftRegister = LMarkupRowAppend(
                loaded.LCardDraftRegister,
                parsed.LCardDraftRegister,
                static row => LMarkupStateFormat(row.LRegisterDraftName)),
            LCardDraftTranslation = LMarkupRowAppend(
                loaded.LCardDraftTranslation,
                parsed.LCardDraftTranslation,
                static row => row.ToString(CultureInfo.InvariantCulture)),
            LCardDraftTag = LMarkupRowAppend(
                loaded.LCardDraftTag,
                parsed.LCardDraftTag,
                static row => row.LTagDraftText),
            LCardDraftImage = LMarkupRowAppend(
                loaded.LCardDraftImage,
                parsed.LCardDraftImage,
                static row => LMarkupStateFormat(row.LImageDraftLocation)),
            LCardDraftVideo = LMarkupRowAppend(
                loaded.LCardDraftVideo,
                parsed.LCardDraftVideo,
                static row => LMarkupStateFormat(row.LVideoDraftLocation)),
            LCardDraftSense = LMarkupRowAppend(
                loaded.LCardDraftSense,
                parsed.LCardDraftSense,
                static row => row.ToString(CultureInfo.InvariantCulture)),
            LCardDraftChild = LMarkupCardAppend(loaded.LCardDraftChild, parsed.LCardDraftChild, absorbed),
        };
    }

    private static IReadOnlyList<LCardDraft> LMarkupCardPlace(IReadOnlyList<LCardDraft> cards)
    {
        List<LCardDraft> placed = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            placed.Add(card with
            {
                LCardDraftPosition = placed.Count + 1,
                LCardDraftChild = LMarkupCardPlace(card.LCardDraftChild),
            });
        }

        return placed;
    }
}
