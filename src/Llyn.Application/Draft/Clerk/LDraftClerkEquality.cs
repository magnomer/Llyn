using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LDraftClerkEquality
{
    public static bool LDraftMatch(LDraft one, LDraft other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        if (one.LDraftExample is LExample sentence)
        {
            return other.LDraftExample is LExample spoken && LExampleClerk.LExampleClerkMatch(sentence, spoken);
        }

        if (one.LDraftSituation is LSituation situation)
        {
            return other.LDraftSituation is LSituation scene && LSituationClerk.LSituationClerkMatch(situation, scene);
        }

        if (one.LDraftReference is LReference reference)
        {
            return other.LDraftReference is LReference cited
                && LReferenceClerk.LReferenceClerkMatch(reference, cited)
                && LAuthorClerk.LAuthorClerkMatch(one.LDraftAuthor, other.LDraftAuthor);
        }

        if (one.LDraftAuthorHeld is LAuthor author)
        {
            return other.LDraftAuthorHeld is LAuthor named
                && string.Equals(author.LAuthorName.Trim(), named.LAuthorName.Trim(), StringComparison.Ordinal);
        }

        return LDraftMatch(one.LDraftContent, other.LDraftContent);
    }

    public static bool LDraftMatch(LEntryDraft one, LEntryDraft other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        return string.Equals(
                one.LEntryDraftHeadword.Trim(), other.LEntryDraftHeadword.Trim(), StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftLanguage, other.LEntryDraftLanguage, StringComparison.Ordinal)
            && LPronunciationMatch(one.LEntryDraftPronunciations, other.LEntryDraftPronunciations)
            && LTranscriptionMatch(one.LEntryDraftTranscriptions, other.LEntryDraftTranscriptions)
            && LReflexMatch(one.LEntryDraftReflexes, other.LEntryDraftReflexes)
            && LEntryClerkEtymology.LEtymologyMatch(one.LEntryDraftEtymology, other.LEntryDraftEtymology)
            && string.Equals(
                LMarkdown.LMarkdownNormalize(one.LEntryDraftNote),
                LMarkdown.LMarkdownNormalize(other.LEntryDraftNote),
                StringComparison.Ordinal)
            && LSpeechMatch(one.LEntryDraftSpeeches, other.LEntryDraftSpeeches)
            && LEntryClerkField.LFormMatch(one.LEntryDraftForms, other.LEntryDraftForms)
            && LInflectionClerk.LInflectionClerkMatch(one.LEntryDraftInflections, other.LEntryDraftInflections)
            && LCardMatch(one.LEntryDraftMeanings, other.LEntryDraftMeanings)
            && LCardMatch(one.LEntryDraftCollocations, other.LEntryDraftCollocations);
    }

    public static IReadOnlyList<LTranscriptionDraft> LTranscriptionScan(IReadOnlyList<LTranscriptionDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<LTranscriptionDraft> filled = [];
        foreach (LTranscriptionDraft draft in drafts)
        {
            if (!draft.LTranscriptionDraftSeeded || !draft.LTranscriptionDraftEmpty)
            {
                filled.Add(draft);
            }
        }

        return filled;
    }

    public static IReadOnlyList<LReflexDraft> LReflexScan(IReadOnlyList<LReflexDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<LReflexDraft> filled = [];
        foreach (LReflexDraft draft in drafts)
        {
            if (!draft.LReflexDraftEmpty)
            {
                filled.Add(draft);
            }
        }

        return filled;
    }

    private static bool LPronunciationMatch(
        IReadOnlyList<LPronunciationDraft> one, IReadOnlyList<LPronunciationDraft> other)
    {
        one = [.. LPronunciationClerk.LPronunciationClerkScan(one)];
        other = [.. LPronunciationClerk.LPronunciationClerkScan(other)];
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (!LPronunciationMatch(one[index], other[index]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool LPronunciationMatch(LPronunciationDraft one, LPronunciationDraft other)
    {
        if (one.LPronunciationDraftId != other.LPronunciationDraftId
            || !string.Equals(one.LPronunciationDraftIpa, other.LPronunciationDraftIpa, StringComparison.Ordinal)
            || !string.Equals(
                one.LPronunciationDraftRespelling, other.LPronunciationDraftRespelling, StringComparison.Ordinal)
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

    private static bool LTranscriptionMatch(
        IReadOnlyList<LTranscriptionDraft> one, IReadOnlyList<LTranscriptionDraft> other)
    {
        one = LTranscriptionScan(one);
        other = LTranscriptionScan(other);
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

    private static bool LReflexMatch(IReadOnlyList<LReflexDraft> one, IReadOnlyList<LReflexDraft> other)
    {
        one = LReflexScan(one);
        other = LReflexScan(other);
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (!LAnchor.LAnchorMatch(one[index].LReflexDraftAnchors, other[index].LReflexDraftAnchors)
                || one[index] with { LReflexDraftAnchors = [] } != other[index] with { LReflexDraftAnchors = [] })
            {
                return false;
            }
        }

        return true;
    }

    private static bool LSpeechMatch(IReadOnlyList<LSpeechDraft> one, IReadOnlyList<LSpeechDraft> other)
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

    private static bool LCardMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)
    {
        IReadOnlyList<LCardDraft> first = LCardScan(one);
        IReadOnlyList<LCardDraft> second = LCardScan(other);

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
                || !LSentenceMatch(written.LCardDraftSentence, held.LCardDraftSentence)
                || !LSituationMatch(written.LCardDraftSituation, held.LCardDraftSituation)
                || !LRegisterClerk.LRegisterClerkMatch(written.LCardDraftRegister, held.LCardDraftRegister)
                || !LLinkMatch(written.LCardDraftTranslation, held.LCardDraftTranslation)
                || !LTagMatch(written.LCardDraftTag, held.LCardDraftTag)
                || !LImageMatch(written.LCardDraftImage, held.LCardDraftImage)
                || !LVideoMatch(written.LCardDraftVideo, held.LCardDraftVideo)
                || !LCardMatch(written.LCardDraftChild, held.LCardDraftChild))
            {
                return false;
            }
        }

        return true;
    }

    public static bool LVideoMatch(IReadOnlyList<LVideoDraft> one, IReadOnlyList<LVideoDraft> other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        one = [.. LCardClerkField.LVideoRead(one)];
        other = [.. LCardClerkField.LVideoRead(other)];
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

    private static IReadOnlyList<LCardDraft> LCardScan(IReadOnlyList<LCardDraft> cards)
    {
        List<LCardDraft> filled = [];
        foreach (LCardDraft card in cards)
        {
            if (!LCardCheck(card))
            {
                filled.Add(card);
            }
        }

        return filled;
    }

    private static bool LCardCheck(LCardDraft card)
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

    private static bool LSentenceMatch(IReadOnlyList<LSentenceDraft> one, IReadOnlyList<LSentenceDraft> other)
    {
        one = [.. LCardClerkField.LSentenceRead(one)];
        other = [.. LCardClerkField.LSentenceRead(other)];
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LSentenceDraftId != other[index].LSentenceDraftId
                || one[index].LSentenceDraftParticle != other[index].LSentenceDraftParticle
                || one[index].LSentenceDraftDependence != other[index].LSentenceDraftDependence
                || !LExampleMatch(one[index].LSentenceDraftExample, other[index].LSentenceDraftExample))
            {
                return false;
            }
        }

        return true;
    }

    private static bool LExampleMatch(LExampleDraft? one, LExampleDraft? other)
    {
        if (one is null || other is null)
        {
            return one is null && other is null;
        }

        return one.LExampleDraftText == other.LExampleDraftText
            && one.LExampleDraftId == other.LExampleDraftId
            && one.LExampleDraftReference == other.LExampleDraftReference
            && one.LExampleDraftGloss.SequenceEqual(other.LExampleDraftGloss)
            && string.Equals(one.LExampleDraftLanguage, other.LExampleDraftLanguage, StringComparison.Ordinal)
            && one.LExampleDraftMention.SequenceEqual(other.LExampleDraftMention);
    }

    private static bool LSituationMatch(IReadOnlyList<LSituationDraft> one, IReadOnlyList<LSituationDraft> other)
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

    public static bool LImageMatch(IReadOnlyList<LImageDraft> one, IReadOnlyList<LImageDraft> other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        one = [.. LCardClerkField.LImageRead(one)];
        other = [.. LCardClerkField.LImageRead(other)];
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

    private static bool LLinkMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
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

    private static bool LTagMatch(IReadOnlyList<LTagDraft> one, IReadOnlyList<LTagDraft> other)
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
