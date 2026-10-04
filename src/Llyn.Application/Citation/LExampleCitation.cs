using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LExampleCitation
{
    private readonly LVault _lExampleCitationVault;
    private readonly LIdentity _lExampleCitationIdentity;
    private readonly LClaimClerk _lExampleCitationClaims;
    private readonly LExampleClerk _lExampleCitationExamples;
    private readonly LEntryClerk _lExampleCitationEntries;

    public LExampleCitation(
        LRig rig, LIdentity identity, LClaimClerk claims, LExampleClerk examples, LEntryClerk entries)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(entries);
        _lExampleCitationVault = rig.LRigVault;
        _lExampleCitationIdentity = identity;
        _lExampleCitationClaims = claims;
        _lExampleCitationExamples = examples;
        _lExampleCitationEntries = entries;
    }

    public LDraft LExampleCitationStart(string origin, long? exampleId)
    {
        long example = exampleId is null or <= 0 ? 0 : exampleId.Value;
        LExample content = example == 0
            ? LExampleClerk.LExampleClerkBlank with { LExampleId = _lExampleCitationIdentity.LIdentityCreate() }
            : _lExampleCitationExamples.LExampleClerkRead(example) ?? throw new LRefusal(LRefusal.LRefusalExample);

        LDraft draft = _lExampleCitationClaims.LDraftCreate(origin, example, LClaimClerk.LDraftBlank) with
        {
            LDraftExample = content,
        };

        return _lExampleCitationClaims.LClaimClerkStart(draft);
    }

    public LExample LExampleCitationCommit(long id)
    {
        LDraft draft = _lExampleCitationClaims.LDraftLoad(id);
        LExample sending = LExampleCitationNormalize(
            draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample));

        LExample stored;
        using (LVaultSession session = _lExampleCitationVault.LVaultSessionStart())
        {
            bool fresh = draft.LDraftEntryId == 0
                || _lExampleCitationExamples.LExampleClerkRead(draft.LDraftEntryId) is null;
            if (fresh)
            {
                stored = _lExampleCitationExamples.LExampleClerkCreate(sending);
            }
            else
            {
                LExample written = sending with { LExampleId = draft.LDraftEntryId };
                _lExampleCitationExamples.LExampleClerkUpdate(written);
                stored = _lExampleCitationExamples.LExampleClerkRead(draft.LDraftEntryId) ?? written;
            }

            LCitationClerk.LRevisionRecord(
                _lExampleCitationEntries, stored.LExampleId, "example", fresh, stored.LExampleText.LStateValueShow());
            session.LVaultSessionCommit();
        }

        _lExampleCitationClaims.LDraftSave(draft with { LDraftEntryId = stored.LExampleId, LDraftExample = stored });
        _lExampleCitationClaims.LClaimClerkFinish(id);
        return stored;
    }

    public bool LExampleCitationCheck(LDraft draft, LExample held)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(held);

        LExample blank = LExampleClerk.LExampleClerkBlank with { LExampleLanguage = held.LExampleLanguage };
        LExample origin = draft.LDraftEntryId == 0
            ? blank
            : _lExampleCitationExamples.LExampleClerkRead(draft.LDraftEntryId) ?? blank;

        return !LExampleClerk.LExampleClerkMatch(origin, held);
    }

    private LExample LExampleCitationNormalize(LExample content)
    {
        return content.LExampleId == 0
            ? content with { LExampleId = _lExampleCitationIdentity.LIdentityCreate() }
            : content;
    }
}
