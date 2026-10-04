using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LCardEquality
{
    public static bool LCardEqualityMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        IReadOnlyList<LCardDraft> first = LCardEqualityScan(one);
        IReadOnlyList<LCardDraft> second = LCardEqualityScan(other);

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
                || !LCardEqualityMatch(written.LCardDraftSentence, held.LCardDraftSentence)
                || !LCardEqualityMatch(written.LCardDraftSituation, held.LCardDraftSituation)
                || !LRegisterClerk.LRegisterClerkMatch(written.LCardDraftRegister, held.LCardDraftRegister)
                || !LCardEqualityMatch(written.LCardDraftTranslation, held.LCardDraftTranslation)
                || !LCardEqualityMatch(written.LCardDraftTag, held.LCardDraftTag)
                || !LCardEqualityMatch(written.LCardDraftImage, held.LCardDraftImage)
                || !LCardEqualityMatch(written.LCardDraftVideo, held.LCardDraftVideo)
                || !LCardEqualityMatch(written.LCardDraftChild, held.LCardDraftChild))
            {
                return false;
            }
        }

        return true;
    }

    public static bool LCardEqualityMatch(IReadOnlyList<LVideoDraft> one, IReadOnlyList<LVideoDraft> other)
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

    public static bool LCardEqualityMatch(IReadOnlyList<LImageDraft> one, IReadOnlyList<LImageDraft> other)
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

    private static IReadOnlyList<LCardDraft> LCardEqualityScan(IReadOnlyList<LCardDraft> cards)
    {
        List<LCardDraft> filled = [];
        foreach (LCardDraft card in cards)
        {
            if (!LCardEqualityCheck(card))
            {
                filled.Add(card);
            }
        }

        return filled;
    }

    private static bool LCardEqualityCheck(LCardDraft card)
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

    private static bool LCardEqualityMatch(IReadOnlyList<LSentenceDraft> one, IReadOnlyList<LSentenceDraft> other)
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
                || !LCardEqualityMatch(one[index].LSentenceDraftExample, other[index].LSentenceDraftExample))
            {
                return false;
            }
        }

        return true;
    }

    private static bool LCardEqualityMatch(LExampleDraft? one, LExampleDraft? other)
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

    private static bool LCardEqualityMatch(IReadOnlyList<LSituationDraft> one, IReadOnlyList<LSituationDraft> other)
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

    private static bool LCardEqualityMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
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

    private static bool LCardEqualityMatch(IReadOnlyList<LTagDraft> one, IReadOnlyList<LTagDraft> other)
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
