using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LClaimClerk
{
    private readonly LDraftVault _lClaimClerkDrafts;
    private readonly LClaimVault _lClaimClerkClaims;
    private readonly LAuthorVault _lClaimClerkAuthors;
    private readonly LClock _lClaimClerkClock;
    private readonly int _lClaimClerkProcess;
    private readonly LIdentity _lClaimClerkIdentity;
    private readonly LChronicleClerk _lClaimClerkChronicle;
    private readonly LCourtClerk _lClaimClerkCourts;
    private readonly HashSet<long> _lClaimClerkHeld = [];

    public LClaimClerk(LRig rig, LIdentity identity, LChronicleClerk chronicle, LCourtClerk courts)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(chronicle);
        ArgumentNullException.ThrowIfNull(courts);
        _lClaimClerkDrafts = rig.LRigDrafts;
        _lClaimClerkClaims = rig.LRigClaims;
        _lClaimClerkAuthors = rig.LRigAuthors;
        _lClaimClerkClock = rig.LRigClock;
        _lClaimClerkProcess = rig.LRigProcess;
        _lClaimClerkIdentity = identity;
        _lClaimClerkChronicle = chronicle;
        _lClaimClerkCourts = courts;
    }

    public IReadOnlySet<long> LClaimClerkHeld => _lClaimClerkHeld;

    public static LEntryDraft LDraftBlank =>
        new(string.Empty, string.Empty, null, string.Empty, [], []);

    public LDraft LDraftCreate(string origin, long entryId, LEntryDraft content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);
        ArgumentNullException.ThrowIfNull(content);

        return new LDraft(
            _lClaimClerkIdentity.LIdentityCreate(),
            origin,
            entryId,
            content,
            _lClaimClerkClock.LClockRead());
    }

    public LDraft LClaimClerkStart(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _lClaimClerkDrafts.LDraftSave(draft);
        _lClaimClerkClaims.LClaimSave(_lClaimClerkClaims.LClaimCreate(draft.LDraftId));
        _lClaimClerkHeld.Add(draft.LDraftId);
        return draft;
    }

    public LDraft? LDraftRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);
        return LDraftAuthorUpdate(_lClaimClerkDrafts.LDraftRead(id));
    }

    public LDraft LDraftLoad(long id)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);

        return _lClaimClerkDrafts.LDraftRead(id)
            ?? throw new LRefusal(LRefusal.LRefusalDraft);
    }

    public LDraft LClaimClerkLoad(long id)
    {
        if (!_lClaimClerkHeld.Contains(id))
        {
            throw new LRefusal(LRefusal.LRefusalDraft);
        }

        return LDraftLoad(id);
    }

    public static void LClaimStaleValidate(IReadOnlySet<long> stale, long id)
    {
        ArgumentNullException.ThrowIfNull(stale);

        if (stale.Contains(id))
        {
            throw new LRefusal(LRefusal.LRefusalStale);
        }
    }

    public static string? LDraftRefusalRead(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (draft.LDraftExample is not null
            || draft.LDraftSituation is not null
            || draft.LDraftReference is not null
            || draft.LDraftAuthorHeld is not null)
        {
            return null;
        }

        return string.IsNullOrWhiteSpace(draft.LDraftContent.LEntryDraftHeadword)
            ? LRefusal.LRefusalHeadword
            : null;
    }

    public IReadOnlyList<LDraft> LDraftScan()
    {
        return _lClaimClerkDrafts.LDraftScan();
    }

    public void LDraftSave(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        _lClaimClerkDrafts.LDraftSave(draft);
    }

    public bool LClaimClerkCheck(long id)
    {
        LClaim? claim = _lClaimClerkClaims.LClaimCheck(id)
            ? _lClaimClerkClaims.LClaimRead(id)
            : null;

        return claim is not null
            && claim.LClaimProcess == _lClaimClerkProcess
            && _lClaimClerkHeld.Contains(id);
    }

    public bool LClaimForeignCheck(long id)
    {
        if (!_lClaimClerkClaims.LClaimCheck(id))
        {
            return false;
        }

        LClaim? claim = _lClaimClerkClaims.LClaimRead(id);
        return claim is not null && claim.LClaimProcess != _lClaimClerkProcess;
    }

    public void LClaimClerkFinish(long id)
    {
        _lClaimClerkHeld.Remove(id);
        _lClaimClerkChronicle.LChronicleClerkClear(id);
        _lClaimClerkClaims.LClaimDelete(id);
        _lClaimClerkDrafts.LDraftDelete(id);
    }

    public void LClaimClerkDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);
        LCourtRemove(id);
        LClaimClerkFinish(id);
    }

    public void LClaimClerkCancel(long id)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);
        LCourtRemove(id);
        _lClaimClerkCourts.LCourtClerkSettle(id, 0);
        LClaimClerkFinish(id);
    }

    public void LClaimClerkSweep()
    {
        IReadOnlyList<long> dropped = _lClaimClerkDrafts.LDraftSweep();
        _lClaimClerkCourts.LCourtClerkSweep();

        foreach (long id in dropped)
        {
            LClaimClerkCancel(id);
        }

        foreach (LClaim claim in _lClaimClerkClaims.LClaimScan())
        {
            if (_lClaimClerkDrafts.LDraftRead(claim.LClaimDraft) is null)
            {
                _lClaimClerkClaims.LClaimDelete(claim.LClaimDraft);
            }
        }
    }

    private LDraft? LDraftAuthorUpdate(LDraft? draft)
    {
        if (draft is null || draft.LDraftAuthor.Count == 0)
        {
            return draft;
        }

        List<LAuthor> named = new(draft.LDraftAuthor.Count);
        foreach (LAuthor author in draft.LDraftAuthor)
        {
            named.Add(author.LAuthorStored ? _lClaimClerkAuthors.LAuthorRead(author.LAuthorId) ?? author : author);
        }

        return draft with { LDraftAuthor = named };
    }

    private void LCourtRemove(long id)
    {
        IReadOnlyList<LCourt> court = _lClaimClerkCourts.LCourtClerkScan();

        foreach (LCourt link in court)
        {
            if (link.LCourtOwnerId != id)
            {
                if (_lClaimClerkDrafts.LDraftRead(link.LCourtOwnerId) is null)
                {
                    _lClaimClerkCourts.LCourtClerkDelete(link.LCourtId);
                }

                continue;
            }

            _lClaimClerkCourts.LCourtClerkDelete(link.LCourtId);

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

            LDraft? target = _lClaimClerkDrafts.LDraftRead(link.LCourtTargetId);

            if (claimed || target is null || !LClaimClerkCheck(link.LCourtTargetId))
            {
                continue;
            }

            _lClaimClerkHeld.Remove(link.LCourtTargetId);
            _lClaimClerkClaims.LClaimDelete(link.LCourtTargetId);
            _lClaimClerkDrafts.LDraftDelete(link.LCourtTargetId);
        }
    }
}
