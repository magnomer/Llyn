using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private static bool LEngineDraftMatch(LEntryDraft one, LEntryDraft other)
    {
        return string.Equals(one.LEntryDraftHeadword, other.LEntryDraftHeadword, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftLanguage, other.LEntryDraftLanguage, StringComparison.Ordinal)
            && LEngineSoundMatch(one.LEntryDraftPronunciations, other.LEntryDraftPronunciations)
            && LEngineSpellingMatch(one.LEntryDraftTranscriptions, other.LEntryDraftTranscriptions)
            && string.Equals(one.LEntryDraftNote, other.LEntryDraftNote, StringComparison.Ordinal)
            && LEngineSpeechMatch(one.LEntryDraftSpeeches, other.LEntryDraftSpeeches)
            && LEngineFormMatch(one.LEntryDraftForms, other.LEntryDraftForms)
            && LEngineInflectionMatch(one.LEntryDraftInflections, other.LEntryDraftInflections)
            && LEngineCardMatch(one.LEntryDraftMeanings, other.LEntryDraftMeanings)
            && LEngineCardMatch(one.LEntryDraftCollocations, other.LEntryDraftCollocations);
    }

    private static bool LEngineSoundMatch(
        IReadOnlyList<LPronunciationDraft> one, IReadOnlyList<LPronunciationDraft> other)
    {
        one = [.. LEnginePronunciationScan(one)];
        other = [.. LEnginePronunciationScan(other)];
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (!LEngineSoundMatch(one[index], other[index]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineSoundMatch(LPronunciationDraft one, LPronunciationDraft other)
    {
        if (one.LPronunciationDraftId != other.LPronunciationDraftId
            || !string.Equals(one.LPronunciationDraftIpa, other.LPronunciationDraftIpa, StringComparison.Ordinal)
            || !string.Equals(
                one.LPronunciationDraftVariety, other.LPronunciationDraftVariety, StringComparison.Ordinal)
            || !string.Equals(
                one.LPronunciationDraftAudio, other.LPronunciationDraftAudio, StringComparison.Ordinal)
            || !string.Equals(
                one.LPronunciationDraftSource, other.LPronunciationDraftSource, StringComparison.Ordinal)
            || one.LPronunciationDraftSyllables.Count != other.LPronunciationDraftSyllables.Count)
        {
            return false;
        }

        for (int index = 0; index < one.LPronunciationDraftSyllables.Count; index++)
        {
            if (one.LPronunciationDraftSyllables[index] != other.LPronunciationDraftSyllables[index])
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineSpellingMatch(
        IReadOnlyList<LTranscriptionDraft> one, IReadOnlyList<LTranscriptionDraft> other)
    {
        one = [.. LEngineTranscriptionScan(one)];
        other = [.. LEngineTranscriptionScan(other)];
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index] != other[index])
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineSpeechMatch(
        IReadOnlyList<LSpeechDraft> one, IReadOnlyList<LSpeechDraft> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index] != other[index])
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineCardMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)
    {
        IReadOnlyList<LCardDraft> first = LEngineCardScan(one);
        IReadOnlyList<LCardDraft> second = LEngineCardScan(other);

        if (first.Count != second.Count)
        {
            return false;
        }

        for (int index = 0; index < first.Count; index++)
        {
            LCardDraft written = first[index];
            LCardDraft held = second[index];
            if (written.LCardDraftPosition != held.LCardDraftPosition
                || written.LCardDraftTitle != held.LCardDraftTitle
                || written.LCardDraftExpression != held.LCardDraftExpression
                || written.LCardDraftMeaning != held.LCardDraftMeaning
                || written.LCardDraftId != held.LCardDraftId
                || !LEngineSentenceMatch(written.LCardDraftSentence, held.LCardDraftSentence)
                || !LEngineSituationMatch(written.LCardDraftSituation, held.LCardDraftSituation)
                || !LEngineRegisterMatch(written.LCardDraftRegister, held.LCardDraftRegister)
                || !LEngineLinkMatch(written.LCardDraftTranslation, held.LCardDraftTranslation)
                || !LEngineTagMatch(written.LCardDraftTag, held.LCardDraftTag)
                || !LEngineImageMatch(written.LCardDraftImage, held.LCardDraftImage)
                || !LEngineVideoMatch(written.LCardDraftVideo, held.LCardDraftVideo)
                || !LEngineCardMatch(written.LCardDraftChild, held.LCardDraftChild))
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineVideoMatch(IReadOnlyList<LVideoDraft> one, IReadOnlyList<LVideoDraft> other)
    {
        one = [.. LEngineVideoRead(one)];
        other = [.. LEngineVideoRead(other)];
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index] != other[index])
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<LCardDraft> LEngineCardScan(IReadOnlyList<LCardDraft> cards)
    {
        List<LCardDraft> filled = [];
        foreach (LCardDraft card in cards)
        {
            if (!LEngineCardCheck(card))
            {
                filled.Add(card);
            }
        }

        return filled;
    }

    private static bool LEngineCardCheck(LCardDraft card)
    {
        return card.LCardDraftTitle.LStateValueEmpty
            && card.LCardDraftExpression.LStateValueEmpty
            && card.LCardDraftMeaning.LStateValueEmpty
            && card.LCardDraftSentence.All(static row => row.LSentenceDraftEmpty)
            && card.LCardDraftSituation.Count == 0
            && card.LCardDraftRegister.Count == 0
            && card.LCardDraftTranslation.Count == 0
            && card.LCardDraftTag.Count == 0
            && card.LCardDraftImage.All(static row => row.LImageDraftEmpty)
            && card.LCardDraftVideo.All(static row => row.LVideoDraftEmpty)
            && card.LCardDraftChild.Count == 0;
    }

    private static bool LEngineSentenceMatch(
        IReadOnlyList<LSentenceDraft> one, IReadOnlyList<LSentenceDraft> other)
    {
        one = [.. LEngineSentenceRead(one)];
        other = [.. LEngineSentenceRead(other)];
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LSentenceDraftId != other[index].LSentenceDraftId
                || one[index].LSentenceDraftParticle != other[index].LSentenceDraftParticle
                || one[index].LSentenceDraftDependence != other[index].LSentenceDraftDependence
                || !LEngineExampleMatch(
                    one[index].LSentenceDraftExample, other[index].LSentenceDraftExample))
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineExampleMatch(LExampleDraft? one, LExampleDraft? other)
    {
        if (one is null || other is null)
        {
            return one is null && other is null;
        }

        return one.LExampleDraftText == other.LExampleDraftText
            && one.LExampleDraftId == other.LExampleDraftId
            && one.LExampleDraftReference == other.LExampleDraftReference
            && one.LExampleDraftTranslation == other.LExampleDraftTranslation
            && string.Equals(
                one.LExampleDraftLanguage, other.LExampleDraftLanguage, StringComparison.Ordinal)
            && one.LExampleDraftMention.SequenceEqual(other.LExampleDraftMention);
    }

    private static bool LEngineSituationMatch(
        IReadOnlyList<LSituationDraft> one, IReadOnlyList<LSituationDraft> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LSituationDraftTitle != other[index].LSituationDraftTitle
                || one[index].LSituationDraftDescription != other[index].LSituationDraftDescription
                || one[index].LSituationDraftKind != other[index].LSituationDraftKind
                || one[index].LSituationDraftId != other[index].LSituationDraftId)
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineImageMatch(IReadOnlyList<LImageDraft> one, IReadOnlyList<LImageDraft> other)
    {
        one = [.. LEngineImageRead(one)];
        other = [.. LEngineImageRead(other)];
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index] != other[index])
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineLinkMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index] != other[index])
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineTagMatch(IReadOnlyList<LTagDraft> one, IReadOnlyList<LTagDraft> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LTagDraftId != other[index].LTagDraftId
                || !string.Equals(one[index].LTagDraftText, other[index].LTagDraftText, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
