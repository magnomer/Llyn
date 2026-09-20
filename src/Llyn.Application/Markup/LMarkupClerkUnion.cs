using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.Application;

public static class LMarkupClerkUnion
{
    public static LEntryDraft LMarkupUnionRead(LEntryDraft loaded, LEntryDraft parsed)
    {
        ArgumentNullException.ThrowIfNull(loaded);
        ArgumentNullException.ThrowIfNull(parsed);

        return loaded with
        {
            LEntryDraftPronunciations = LMarkupRowAppend(
                loaded.LEntryDraftPronunciations,
                parsed.LEntryDraftPronunciations,
                static row => row.LPronunciationDraftIpa),
            LEntryDraftTranscriptions = LMarkupRowAppend(
                loaded.LEntryDraftTranscriptions,
                parsed.LEntryDraftTranscriptions,
                static row => row.LTranscriptionDraftText),
            LEntryDraftReflexes = LMarkupRowAppend(
                loaded.LEntryDraftReflexes,
                parsed.LEntryDraftReflexes,
                static row => string.Join(
                    ' ',
                    row.LReflexDraftLanguage,
                    row.LReflexDraftRegion,
                    row.LReflexDraftKind,
                    row.LReflexDraftText,
                    row.LReflexDraftNote)),
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
            LEntryDraftMeanings = LMarkupCardPlace(
                LMarkupCardAppend(loaded.LEntryDraftMeanings, parsed.LEntryDraftMeanings)),
            LEntryDraftCollocations = LMarkupCardPlace(
                LMarkupCardAppend(loaded.LEntryDraftCollocations, parsed.LEntryDraftCollocations)),
            LEntryDraftNote = LMarkupNoteAppend(loaded.LEntryDraftNote, parsed.LEntryDraftNote),
        };
    }

    private static IReadOnlyList<LMarkupRow> LMarkupRowAppend<LMarkupRow>(
        IReadOnlyList<LMarkupRow> loaded, IReadOnlyList<LMarkupRow> parsed, Func<LMarkupRow, string> key)
    {
        List<LMarkupRow> merged = [.. loaded];
        HashSet<string> known = [];
        foreach (LMarkupRow row in loaded)
        {
            known.Add(LCatalog.LCatalogTextNormalize(key(row)));
        }

        foreach (LMarkupRow row in parsed)
        {
            string name = LCatalog.LCatalogTextNormalize(key(row));
            if (name.Length == 0 || known.Add(name))
            {
                merged.Add(row);
            }
        }

        return merged;
    }

    private static string LMarkupNoteAppend(string loaded, string parsed)
    {
        if (string.IsNullOrWhiteSpace(parsed) || string.Equals(loaded, parsed, StringComparison.Ordinal))
        {
            return loaded;
        }

        return string.IsNullOrWhiteSpace(loaded) ? parsed : $"{loaded}\n\n{parsed}";
    }

    private static IReadOnlyList<LCardDraft> LMarkupCardAppend(
        IReadOnlyList<LCardDraft> loaded, IReadOnlyList<LCardDraft> parsed)
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
                merged[index] = LMarkupCardAppend(merged[index], card);
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

    private static LCardDraft LMarkupCardAppend(LCardDraft loaded, LCardDraft parsed)
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
                static row => row.LSituationDraftTitle.LStateValueShow()),
            LCardDraftRegister = LMarkupRowAppend(
                loaded.LCardDraftRegister,
                parsed.LCardDraftRegister,
                static row => row.LRegisterDraftName.LStateValueShow()),
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
                static row => row.LImageDraftLocation.LStateValueShow()),
            LCardDraftVideo = LMarkupRowAppend(
                loaded.LCardDraftVideo,
                parsed.LCardDraftVideo,
                static row => row.LVideoDraftLocation.LStateValueShow()),
            LCardDraftChild = LMarkupCardAppend(loaded.LCardDraftChild, parsed.LCardDraftChild),
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
