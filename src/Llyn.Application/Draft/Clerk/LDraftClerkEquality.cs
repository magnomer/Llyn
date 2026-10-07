using System;
using System.Collections.Generic;
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
            && one.LEntryDraftUnit == other.LEntryDraftUnit
            && LEntryClerkField.LFormMatch(one.LEntryDraftForms, other.LEntryDraftForms)
            && LInflectionClerk.LInflectionClerkMatch(one.LEntryDraftInflections, other.LEntryDraftInflections)
            && LCardEquality.LCardEqualityMatch(one.LEntryDraftMeanings, other.LEntryDraftMeanings)
            && LCardEquality.LCardEqualityMatch(one.LEntryDraftCollocations, other.LEntryDraftCollocations);
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
        List<LReflexDraft> left = [.. LReflexScan(other)];
        if (one.Count != left.Count)
        {
            return false;
        }

        foreach (LReflexDraft row in one)
        {
            int index = left.FindIndex(held =>
                LAnchor.LAnchorMatch(held.LReflexDraftAnchors, row.LReflexDraftAnchors)
                && held with { LReflexDraftAnchors = [] } == row with { LReflexDraftAnchors = [] });
            if (index < 0)
            {
                return false;
            }

            left.RemoveAt(index);
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
}
