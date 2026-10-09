using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LMarkupClerkExample
{
    private readonly LEntryVault _lMarkupExampleEntries;
    private readonly LMeaningVault _lMarkupExampleMeanings;
    private readonly LReferenceVault _lMarkupExampleReferences;
    private readonly LAuthorVault _lMarkupExampleAuthors;

    public LMarkupClerkExample(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lMarkupExampleEntries = rig.LRigEntries;
        _lMarkupExampleMeanings = rig.LRigLexicon.LRigLexiconMeanings;
        _lMarkupExampleReferences = rig.LRigCitation.LRigCitationReferences;
        _lMarkupExampleAuthors = rig.LRigCitation.LRigCitationAuthors;
    }

    public LMarkupExample LMarkupExampleCreate(LExampleDraft example)
    {
        List<LGlossDraft> glosses = new(example.LExampleDraftGloss.Count);
        foreach (LGlossDraft gloss in example.LExampleDraftGloss)
        {
            glosses.Add(gloss with { LGlossDraftId = 0 });
        }

        List<LMarkupMention> mentions = new(example.LExampleDraftMention.Count);
        foreach (LMentionDraft mention in example.LExampleDraftMention)
        {
            mentions.Add(LMarkupMentionCreate(mention));
        }

        return new LMarkupExample(
            example.LExampleDraftText,
            example.LExampleDraftLanguage,
            glosses,
            mentions,
            LMarkupReferenceCreate(example.LExampleDraftReference));
    }

    public LMarkupMention LMarkupMentionCreate(LMentionDraft mention)
    {
        if (mention.LMentionDraftEntry <= 0
            || _lMarkupExampleEntries.LEntryRead(mention.LMentionDraftEntry) is not LEntry entry)
        {
            return new LMarkupMention(
                mention.LMentionDraftOffset, mention.LMentionDraftLength, string.Empty, string.Empty);
        }

        return new LMarkupMention(
            mention.LMentionDraftOffset,
            mention.LMentionDraftLength,
            entry.LEntryHeadword,
            entry.LEntryLanguage,
            LMarkupMeaningResolve(entry.LEntryId, mention.LMentionDraftSense));
    }

    private string LMarkupMeaningResolve(long entryId, long meaningId)
    {
        if (meaningId <= 0)
        {
            return string.Empty;
        }

        Dictionary<long, LMeaning> meanings = [];
        foreach (LMeaning meaning in _lMarkupExampleMeanings.LMeaningRead(entryId))
        {
            meanings[meaning.LMeaningId] = meaning;
        }

        List<string> steps = [];
        long? current = meaningId;
        while (current is long step && meanings.TryGetValue(step, out LMeaning? found))
        {
            steps.Insert(0, (found.LMeaningPosition + 1).ToString(CultureInfo.InvariantCulture));
            current = found.LMeaningParentId;
        }

        return string.Join('.', steps);
    }

    private LMarkupReference? LMarkupReferenceCreate(LStateAnchor anchor)
    {
        if (anchor.LStateAnchorState != LState.LStateSpecified || anchor.LStateAnchorId is not long referenceId)
        {
            return null;
        }

        if (_lMarkupExampleReferences.LReferenceRead(referenceId) is not LReference reference)
        {
            return null;
        }

        IReadOnlyList<LAuthor> credited = _lMarkupExampleAuthors.LAuthorReferenceRead(referenceId);
        List<string> authors = new(credited.Count);
        foreach (LAuthor author in credited)
        {
            authors.Add(author.LAuthorName);
        }

        return new LMarkupReference(
            reference.LReferenceTitle,
            reference.LReferenceYear,
            reference.LReferenceKind,
            reference.LReferenceUrl,
            reference.LReferenceNote,
            authors);
    }
}
