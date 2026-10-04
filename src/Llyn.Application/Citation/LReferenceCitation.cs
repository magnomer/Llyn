using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LReferenceCitation
{
    private readonly LVault _lReferenceCitationVault;
    private readonly LIdentity _lReferenceCitationIdentity;
    private readonly LClaimClerk _lReferenceCitationClaims;
    private readonly LAuthorClerk _lReferenceCitationAuthors;
    private readonly LReferenceClerk _lReferenceCitationReferences;
    private readonly LEntryClerk _lReferenceCitationEntries;

    public LReferenceCitation(
        LRig rig,
        LIdentity identity,
        LClaimClerk claims,
        LAuthorClerk authors,
        LReferenceClerk references,
        LEntryClerk entries)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(authors);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(entries);
        _lReferenceCitationVault = rig.LRigVault;
        _lReferenceCitationIdentity = identity;
        _lReferenceCitationClaims = claims;
        _lReferenceCitationAuthors = authors;
        _lReferenceCitationReferences = references;
        _lReferenceCitationEntries = entries;
    }

    public LDraft LReferenceCitationStart(string origin, long? referenceId)
    {
        long reference = referenceId is null or <= 0 ? 0 : referenceId.Value;
        LReference content = reference == 0
            ? LReferenceClerk.LReferenceClerkBlank with { LReferenceId = _lReferenceCitationIdentity.LIdentityCreate() }
            : _lReferenceCitationReferences.LReferenceClerkRead(reference)
                ?? throw new LRefusal(LRefusal.LRefusalReference);

        LDraft draft = _lReferenceCitationClaims.LDraftCreate(origin, reference, LClaimClerk.LDraftBlank) with
        {
            LDraftReference = content,
            LDraftAuthor = reference == 0 ? [] : _lReferenceCitationAuthors.LAuthorReferenceRead(reference),
        };

        return _lReferenceCitationClaims.LClaimClerkStart(draft);
    }

    public LReference LReferenceCitationCommit(long id)
    {
        LDraft draft = _lReferenceCitationClaims.LDraftLoad(id);
        LReference sending = LReferenceCitationNormalize(
            draft.LDraftReference ?? throw new LRefusal(LRefusal.LRefusalReference));

        LReference stored;
        using (LVaultSession session = _lReferenceCitationVault.LVaultSessionStart())
        {
            bool fresh = draft.LDraftEntryId == 0
                || _lReferenceCitationReferences.LReferenceClerkRead(draft.LDraftEntryId) is null;
            if (fresh)
            {
                stored = _lReferenceCitationReferences.LReferenceClerkCreate(sending);
            }
            else
            {
                LReference written = sending with { LReferenceId = draft.LDraftEntryId };
                _lReferenceCitationReferences.LReferenceClerkUpdate(written);
                stored = _lReferenceCitationReferences.LReferenceClerkRead(draft.LDraftEntryId) ?? written;
            }

            _lReferenceCitationAuthors.LAuthorReferenceSave(stored.LReferenceId, draft.LDraftAuthor);
            LCitationClerk.LRevisionRecord(
                _lReferenceCitationEntries,
                stored.LReferenceId,
                "reference",
                fresh,
                stored.LReferenceTitle.LStateValueShow());
            session.LVaultSessionCommit();
        }

        _lReferenceCitationClaims.LDraftSave(
            draft with { LDraftEntryId = stored.LReferenceId, LDraftReference = stored });
        _lReferenceCitationClaims.LClaimClerkFinish(id);
        return stored;
    }

    public bool LReferenceCitationCheck(LDraft draft, LReference held)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(held);

        LReference blank = LReferenceClerk.LReferenceClerkBlank;
        LReference origin = draft.LDraftEntryId == 0
            ? blank
            : _lReferenceCitationReferences.LReferenceClerkRead(draft.LDraftEntryId) ?? blank;

        IReadOnlyList<LAuthor> credited = draft.LDraftEntryId == 0
            ? []
            : _lReferenceCitationAuthors.LAuthorReferenceRead(draft.LDraftEntryId);

        return !LReferenceClerk.LReferenceClerkMatch(origin, held)
            || !LAuthorClerk.LAuthorClerkMatch(credited, draft.LDraftAuthor);
    }

    private LReference LReferenceCitationNormalize(LReference content)
    {
        return content.LReferenceId == 0
            ? content with { LReferenceId = _lReferenceCitationIdentity.LIdentityCreate() }
            : content;
    }
}
