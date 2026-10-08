using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LSituationCitation
{
    private readonly LVault _lSituationCitationVault;
    private readonly LIdentity _lSituationCitationIdentity;
    private readonly LClaimClerk _lSituationCitationClaims;
    private readonly LSituationClerk _lSituationCitationSituations;
    private readonly LRevisionClerk _lSituationCitationRevisions;

    public LSituationCitation(
        LRig rig, LIdentity identity, LClaimClerk claims, LSituationClerk situations, LRevisionClerk revisions)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(situations);
        ArgumentNullException.ThrowIfNull(revisions);
        _lSituationCitationVault = rig.LRigVault;
        _lSituationCitationIdentity = identity;
        _lSituationCitationClaims = claims;
        _lSituationCitationSituations = situations;
        _lSituationCitationRevisions = revisions;
    }

    public LDraft LSituationCitationStart(string origin, long? situationId)
    {
        long situation = situationId is null or <= 0 ? 0 : situationId.Value;
        LSituation content = situation == 0
            ? LSituationClerk.LSituationClerkBlank with { LSituationId = _lSituationCitationIdentity.LIdentityCreate() }
            : _lSituationCitationSituations.LSituationClerkRead(situation)
                ?? throw new LRefusal(LRefusal.LRefusalSituation);

        LDraft draft = _lSituationCitationClaims.LDraftCreate(origin, situation, LClaimClerk.LDraftBlank) with
        {
            LDraftSituation = content,
        };

        return _lSituationCitationClaims.LClaimClerkStart(draft);
    }

    public LSituation LSituationCitationCommit(long id)
    {
        LDraft draft = _lSituationCitationClaims.LDraftLoad(id);
        LSituation sending = LSituationCitationNormalize(
            draft.LDraftSituation ?? throw new LRefusal(LRefusal.LRefusalSituation));

        LSituation stored;
        using (LVaultSession session = _lSituationCitationVault.LVaultSessionStart())
        {
            bool fresh = draft.LDraftEntryId == 0
                || _lSituationCitationSituations.LSituationClerkRead(draft.LDraftEntryId) is null;
            if (fresh)
            {
                stored = _lSituationCitationSituations.LSituationClerkCreate(sending);
            }
            else
            {
                LSituation written = sending with { LSituationId = draft.LDraftEntryId };
                _lSituationCitationSituations.LSituationClerkUpdate(written);
                stored = _lSituationCitationSituations.LSituationClerkRead(draft.LDraftEntryId) ?? written;
            }

            _lSituationCitationRevisions.LRevisionClerkRecord(
                stored.LSituationId,
                "situation",
                fresh,
                stored.LSituationTitle.LStateValueShow());
            session.LVaultSessionCommit();
        }

        _lSituationCitationClaims.LDraftSave(
            draft with { LDraftEntryId = stored.LSituationId, LDraftSituation = stored });
        _lSituationCitationClaims.LClaimClerkFinish(id);
        return stored;
    }

    public bool LSituationCitationCheck(LDraft draft, LSituation held)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(held);

        LSituation blank = LSituationClerk.LSituationClerkBlank;
        LSituation origin = draft.LDraftEntryId == 0
            ? blank
            : _lSituationCitationSituations.LSituationClerkRead(draft.LDraftEntryId) ?? blank;

        return !LSituationClerk.LSituationClerkMatch(origin, held);
    }

    private LSituation LSituationCitationNormalize(LSituation content)
    {
        IReadOnlyList<LImageDraft> images = LDraftClerkList.LDraftListNormalize(
            content.LSituationImage,
            static row => row.LImageDraftEmpty,
            row => row.LImageDraftId == 0
                ? row with { LImageDraftId = _lSituationCitationIdentity.LIdentityCreate() }
                : row);
        IReadOnlyList<LVideoDraft> videos = LDraftClerkList.LDraftListNormalize(
            content.LSituationVideo,
            static row => row.LVideoDraftEmpty,
            row => row.LVideoDraftId == 0
                ? row with { LVideoDraftId = _lSituationCitationIdentity.LIdentityCreate() }
                : row);

        if (content.LSituationId != 0
            && ReferenceEquals(images, content.LSituationImage)
            && ReferenceEquals(videos, content.LSituationVideo))
        {
            return content;
        }

        return content with
        {
            LSituationId = content.LSituationId == 0
                ? _lSituationCitationIdentity.LIdentityCreate()
                : content.LSituationId,
            LSituationImage = images,
            LSituationVideo = videos,
        };
    }
}
