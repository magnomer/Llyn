using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LDossierEntrance : LDraftVault
{
    private readonly LDraftArchive _lDossierEntranceDrafts;
    private readonly LClaimArchive _lDossierEntranceClaims;
    private readonly LCourtArchive _lDossierEntranceCourts;

    public LDossierEntrance(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lDossierEntranceDrafts = new LDraftArchive(root);
        _lDossierEntranceClaims = new LClaimArchive(root);
        _lDossierEntranceCourts = new LCourtArchive(root);
    }

    LDraft LDraftVault.LDraftStart(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _lDossierEntranceDrafts.LDraftSave(draft);
        _lDossierEntranceClaims.LClaimSave(_lDossierEntranceClaims.LClaimCreate(draft.LDraftId));
        return draft;
    }

    LDraft? LDraftVault.LDraftRead(long id) => _lDossierEntranceDrafts.LDraftRead(id);

    IReadOnlyList<LDraft> LDraftVault.LDraftScan() => _lDossierEntranceDrafts.LDraftScan();

    void LDraftVault.LDraftSave(LDraft draft) => _lDossierEntranceDrafts.LDraftSave(draft);

    bool LDraftVault.LDraftClaimCheck(long id)
    {
        LClaim? claim = _lDossierEntranceClaims.LClaimCheck(id)
            ? _lDossierEntranceClaims.LClaimRead(id)
            : null;

        return claim is not null && claim.LClaimProcess == Environment.ProcessId;
    }

    bool LDraftVault.LDraftForeignCheck(long id)
    {
        if (!_lDossierEntranceClaims.LClaimCheck(id))
        {
            return false;
        }

        LClaim? claim = _lDossierEntranceClaims.LClaimRead(id);
        return claim is not null && claim.LClaimProcess != Environment.ProcessId;
    }

    void LDraftVault.LDraftFinish(long id)
    {
        _lDossierEntranceClaims.LClaimDelete(id);
        _lDossierEntranceDrafts.LDraftDelete(id);
    }

    LDraftRelease LDraftVault.LDraftDelete(long id, IReadOnlySet<long> held)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);
        ArgumentNullException.ThrowIfNull(held);

        LDraftVault vault = this;
        List<long> drafts = LCourtRemove(id, held);
        vault.LDraftFinish(id);
        drafts.Add(id);
        return new LDraftRelease(drafts, []);
    }

    LDraftRelease LDraftVault.LDraftCancel(long id, IReadOnlySet<long> held)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);
        ArgumentNullException.ThrowIfNull(held);

        LDraftVault vault = this;
        List<long> drafts = LCourtRemove(id, held);
        IReadOnlyList<long> owners = vault.LDraftCourtSettle(id, 0);
        vault.LDraftFinish(id);
        drafts.Add(id);
        return new LDraftRelease(drafts, owners);
    }

    LDraftRelease LDraftVault.LDraftSweep(IReadOnlySet<long> held)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraftVault vault = this;
        IReadOnlyList<long> dropped = _lDossierEntranceDrafts.LDraftSweep();
        _lDossierEntranceCourts.LCourtSweep();

        List<long> drafts = [];
        List<long> owners = [];
        foreach (long id in dropped)
        {
            LDraftRelease release = vault.LDraftCancel(id, held);
            drafts.AddRange(release.LDraftReleaseDrafts);
            owners.AddRange(release.LDraftReleaseOwners);
        }

        foreach (LClaim claim in _lDossierEntranceClaims.LClaimScan())
        {
            if (_lDossierEntranceDrafts.LDraftRead(claim.LClaimDraft) is null)
            {
                _lDossierEntranceClaims.LClaimDelete(claim.LClaimDraft);
            }
        }

        return new LDraftRelease(drafts, owners);
    }

    void LDraftVault.LDraftCourtSave(LCourt link) => _lDossierEntranceCourts.LCourtSave(link);

    IReadOnlyList<LCourt> LDraftVault.LDraftCourtScan() => _lDossierEntranceCourts.LCourtScan();

    void LDraftVault.LDraftCourtDelete(long linkId) => _lDossierEntranceCourts.LCourtDelete(linkId);

    IReadOnlyList<long> LDraftVault.LDraftCourtSettle(long draftId, long realId)
    {
        List<long> owners = [];
        foreach (LCourt link in _lDossierEntranceCourts.LCourtSettle(draftId))
        {
            LDraft? owner = _lDossierEntranceDrafts.LDraftRead(link.LCourtOwnerId);
            if (owner is null)
            {
                continue;
            }

            _lDossierEntranceDrafts.LDraftSave(owner with
            {
                LDraftContent = owner.LDraftContent.LEntryDraftSettle(link.LCourtTargetId, realId),
            });
            owners.Add(owner.LDraftId);
        }

        return owners;
    }

    private List<long> LCourtRemove(long id, IReadOnlySet<long> held)
    {
        LDraftVault vault = this;
        IReadOnlyList<LCourt> court = _lDossierEntranceCourts.LCourtScan();
        List<long> released = [];

        foreach (LCourt link in court)
        {
            if (link.LCourtOwnerId != id)
            {
                if (_lDossierEntranceDrafts.LDraftRead(link.LCourtOwnerId) is null)
                {
                    _lDossierEntranceCourts.LCourtDelete(link.LCourtId);
                }

                continue;
            }

            _lDossierEntranceCourts.LCourtDelete(link.LCourtId);

            if (link.LCourtTargetId == id)
            {
                continue;
            }

            bool claimed = false;
            foreach (LCourt other in court)
            {
                if (other.LCourtOwnerId != id
                    && other.LCourtTargetId == link.LCourtTargetId)
                {
                    claimed = true;
                    break;
                }
            }

            if (claimed
                || _lDossierEntranceDrafts.LDraftRead(link.LCourtTargetId) is null
                || !vault.LDraftClaimCheck(link.LCourtTargetId)
                || !held.Contains(link.LCourtTargetId))
            {
                continue;
            }

            vault.LDraftFinish(link.LCourtTargetId);
            released.Add(link.LCourtTargetId);
        }

        return released;
    }
}
