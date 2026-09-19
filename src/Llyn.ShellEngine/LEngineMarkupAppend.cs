using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private static LEntryDraft LEngineMarkupAppend(LEntryDraft loaded, LEntryDraft parsed)
    {
        return loaded with
        {
            LEntryDraftPronunciations = LEngineMarkupAppend(
                loaded.LEntryDraftPronunciations,
                parsed.LEntryDraftPronunciations,
                static row => row.LPronunciationDraftIpa),
            LEntryDraftTranscriptions = LEngineMarkupAppend(
                loaded.LEntryDraftTranscriptions,
                parsed.LEntryDraftTranscriptions,
                static row => row.LTranscriptionDraftText),
            LEntryDraftReflexes = LEngineMarkupAppend(
                loaded.LEntryDraftReflexes,
                parsed.LEntryDraftReflexes,
                static row => string.Join(
                    ' ',
                    row.LReflexDraftLanguage,
                    row.LReflexDraftRegion,
                    row.LReflexDraftKind,
                    row.LReflexDraftText,
                    row.LReflexDraftNote)),
            LEntryDraftSpeeches = LEngineMarkupAppend(
                loaded.LEntryDraftSpeeches,
                parsed.LEntryDraftSpeeches,
                static row => row.LSpeechDraftName),
            LEntryDraftForms = LEngineMarkupAppend(
                loaded.LEntryDraftForms,
                parsed.LEntryDraftForms,
                static row => row.LFormText),
            LEntryDraftInflections = LEngineMarkupAppend(
                loaded.LEntryDraftInflections,
                parsed.LEntryDraftInflections,
                static row => row.LInflectionText),
            LEntryDraftMeanings = LEngineMarkupPlace(
                LEngineMarkupAppend(loaded.LEntryDraftMeanings, parsed.LEntryDraftMeanings)),
            LEntryDraftCollocations = LEngineMarkupPlace(
                LEngineMarkupAppend(loaded.LEntryDraftCollocations, parsed.LEntryDraftCollocations)),
            LEntryDraftNote = LEngineMarkupAppend(loaded.LEntryDraftNote, parsed.LEntryDraftNote),
        };
    }

    private static IReadOnlyList<LEngineRow> LEngineMarkupAppend<LEngineRow>(
        IReadOnlyList<LEngineRow> loaded, IReadOnlyList<LEngineRow> parsed, Func<LEngineRow, string> key)
    {
        List<LEngineRow> merged = [.. loaded];
        HashSet<string> known = [];
        foreach (LEngineRow row in loaded)
        {
            known.Add(LCatalog.LCatalogTextNormalize(key(row)));
        }

        foreach (LEngineRow row in parsed)
        {
            string name = LCatalog.LCatalogTextNormalize(key(row));
            if (name.Length == 0 || known.Add(name))
            {
                merged.Add(row);
            }
        }

        return merged;
    }

    private static string LEngineMarkupAppend(string loaded, string parsed)
    {
        if (string.IsNullOrWhiteSpace(parsed) || string.Equals(loaded, parsed, StringComparison.Ordinal))
        {
            return loaded;
        }

        return string.IsNullOrWhiteSpace(loaded) ? parsed : $"{loaded}\n\n{parsed}";
    }

    private static IReadOnlyList<LCardDraft> LEngineMarkupAppend(
        IReadOnlyList<LCardDraft> loaded, IReadOnlyList<LCardDraft> parsed)
    {
        List<LCardDraft> merged = [.. loaded];
        foreach (LCardDraft card in parsed)
        {
            int index = LEngineCardFind(merged, card);
            if (index < 0)
            {
                merged.Add(card);
            }
            else
            {
                merged[index] = LEngineMarkupAppend(merged[index], card);
            }
        }

        return merged;
    }

    private static int LEngineCardFind(IReadOnlyList<LCardDraft> cards, LCardDraft card)
    {
        string wanted = LEngineCardFormat(card);
        if (wanted.Length == 0)
        {
            return -1;
        }

        for (int index = 0; index < cards.Count; index++)
        {
            if (LEngineCardFormat(cards[index]) == wanted)
            {
                return index;
            }
        }

        return -1;
    }

    private static string LEngineCardFormat(LCardDraft card)
    {
        string title = LCatalog.LCatalogTextNormalize(card.LCardDraftTitle.LStateValueShow());
        string expression = LCatalog.LCatalogTextNormalize(card.LCardDraftExpression.LStateValueShow());
        string meaning = LCatalog.LCatalogTextNormalize(card.LCardDraftMeaning.LStateValueShow());
        return title.Length + expression.Length + meaning.Length == 0
            ? string.Empty
            : $"{title}\n{expression}\n{meaning}";
    }

    private static LCardDraft LEngineMarkupAppend(LCardDraft loaded, LCardDraft parsed)
    {
        return loaded with
        {
            LCardDraftSentence = LEngineMarkupAppend(
                loaded.LCardDraftSentence,
                parsed.LCardDraftSentence,
                static row => row.LSentenceDraftExample?.LExampleDraftText.LStateValueShow() ?? string.Empty),
            LCardDraftSituation = LEngineMarkupAppend(
                loaded.LCardDraftSituation,
                parsed.LCardDraftSituation,
                static row => row.LSituationDraftTitle.LStateValueShow()),
            LCardDraftRegister = LEngineMarkupAppend(
                loaded.LCardDraftRegister,
                parsed.LCardDraftRegister,
                static row => row.LRegisterDraftName.LStateValueShow()),
            LCardDraftTranslation = LEngineMarkupAppend(
                loaded.LCardDraftTranslation,
                parsed.LCardDraftTranslation,
                static row => row.ToString(CultureInfo.InvariantCulture)),
            LCardDraftTag = LEngineMarkupAppend(
                loaded.LCardDraftTag,
                parsed.LCardDraftTag,
                static row => row.LTagDraftText),
            LCardDraftImage = LEngineMarkupAppend(
                loaded.LCardDraftImage,
                parsed.LCardDraftImage,
                static row => row.LImageDraftLocation.LStateValueShow()),
            LCardDraftVideo = LEngineMarkupAppend(
                loaded.LCardDraftVideo,
                parsed.LCardDraftVideo,
                static row => row.LVideoDraftLocation.LStateValueShow()),
            LCardDraftChild = LEngineMarkupAppend(loaded.LCardDraftChild, parsed.LCardDraftChild),
        };
    }

    private static IReadOnlyList<LCardDraft> LEngineMarkupPlace(IReadOnlyList<LCardDraft> cards)
    {
        List<LCardDraft> placed = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            placed.Add(card with
            {
                LCardDraftPosition = placed.Count + 1,
                LCardDraftChild = LEngineMarkupPlace(card.LCardDraftChild),
            });
        }

        return placed;
    }
}
