using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LCitationClerk
{
    private readonly LClaimClerk _lCitationClerkClaims;
    private readonly LExampleClerk _lCitationClerkExamples;
    private readonly LReferenceClerk _lCitationClerkReferences;
    private readonly LSituationClerk _lCitationClerkSituations;
    private readonly LEntryClerk _lCitationClerkEntries;

    public LCitationClerk(
        LRig rig,
        LIdentity identity,
        LClaimClerk claims,
        LAuthorClerk authors,
        LExampleClerk examples,
        LReferenceClerk references,
        LSituationClerk situations,
        LEntryClerk entries)
    {
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(situations);
        ArgumentNullException.ThrowIfNull(entries);
        _lCitationClerkClaims = claims;
        _lCitationClerkExamples = examples;
        _lCitationClerkReferences = references;
        _lCitationClerkSituations = situations;
        _lCitationClerkEntries = entries;
        LCitationClerkAuthor = new LAuthorCitation(rig, claims, authors, entries);
        LCitationClerkExample = new LExampleCitation(rig, identity, claims, examples, entries);
        LCitationClerkReference = new LReferenceCitation(rig, identity, claims, authors, references, entries);
        LCitationClerkSituation = new LSituationCitation(rig, identity, claims, situations, entries);
    }

    public LAuthorCitation LCitationClerkAuthor { get; }

    public LExampleCitation LCitationClerkExample { get; }

    public LReferenceCitation LCitationClerkReference { get; }

    public LSituationCitation LCitationClerkSituation { get; }

    public LDraft LEntryStart(string origin, long? entryId, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);
        ArgumentNullException.ThrowIfNull(language);

        long entry = entryId is null or <= 0 ? 0 : entryId.Value;
        LEntryDraft content = entry == 0
            ? LClaimClerk.LDraftBlank with { LEntryDraftLanguage = language }
            : _lCitationClerkEntries.LEntryClerkLoad(entry) ?? throw new LRefusal(LRefusal.LRefusalEntry);

        return _lCitationClerkClaims.LClaimClerkStart(_lCitationClerkClaims.LDraftCreate(origin, entry, content));
    }

    public bool LCitationDraftCheck(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (draft.LDraftExample is LExample sentence)
        {
            return LCitationClerkExample.LExampleCitationCheck(draft, sentence);
        }

        if (draft.LDraftSituation is LSituation situation)
        {
            return LCitationClerkSituation.LSituationCitationCheck(draft, situation);
        }

        if (draft.LDraftReference is LReference reference)
        {
            return LCitationClerkReference.LReferenceCitationCheck(draft, reference);
        }

        if (draft.LDraftAuthorHeld is LAuthor author)
        {
            return LCitationClerkAuthor.LAuthorCitationCheck(draft, author);
        }

        LEntryDraft origin = draft.LDraftEntryId <= 0
            ? LClaimClerk.LDraftBlank with { LEntryDraftLanguage = draft.LDraftContent.LEntryDraftLanguage }
            : _lCitationClerkEntries.LEntryClerkLoad(draft.LDraftEntryId) ?? LClaimClerk.LDraftBlank;

        return !LDraftClerkEquality.LDraftMatch(origin, draft.LDraftContent);
    }

    public bool LCitationLeftoverCheck(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (draft.LDraftExample is LExample sentence)
        {
            return _lCitationClerkExamples.LExampleClerkRead(draft.LDraftEntryId) is LExample kept
                && LExampleClerk.LExampleClerkMatch(kept, sentence);
        }

        if (draft.LDraftSituation is LSituation situation)
        {
            return _lCitationClerkSituations.LSituationClerkRead(draft.LDraftEntryId) is LSituation standing
                && LSituationClerk.LSituationClerkMatch(standing, situation);
        }

        if (draft.LDraftReference is LReference reference)
        {
            return _lCitationClerkReferences.LReferenceClerkRead(draft.LDraftEntryId) is LReference cited
                && LReferenceClerk.LReferenceClerkMatch(cited, reference);
        }

        if (draft.LDraftAuthorHeld is LAuthor author)
        {
            return !LCitationClerkAuthor.LAuthorCitationCheck(draft, author);
        }

        LEntryDraft? stored = _lCitationClerkEntries.LEntryClerkLoad(draft.LDraftEntryId);
        return stored is not null && LDraftClerkEquality.LDraftMatch(stored, draft.LDraftContent);
    }

    internal static void LRevisionRecord(LEntryClerk entries, long target, string subject, bool fresh, string? summary)
    {
        entries.LRevisionRecord([new LRevisionDelta(target, subject, fresh ? "create" : "update", summary)]);
    }
}
