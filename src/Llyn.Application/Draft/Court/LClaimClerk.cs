using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LClaimClerk
{
    private readonly LDraftVault _lClaimClerkDrafts;
    private readonly LAuthorVault _lClaimClerkAuthors;
    private readonly LClock _lClaimClerkClock;
    private readonly LIdentity _lClaimClerkIdentity;
    private readonly LChronicleClerk _lClaimClerkChronicle;
    private readonly HashSet<long> _lClaimClerkHeld = [];

    public LClaimClerk(LRig rig, LIdentity identity, LChronicleClerk chronicle)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(chronicle);
        _lClaimClerkDrafts = rig.LRigDraft;
        _lClaimClerkAuthors = rig.LRigCitation.LRigCitationAuthors;
        _lClaimClerkClock = rig.LRigClock;
        _lClaimClerkIdentity = identity;
        _lClaimClerkChronicle = chronicle;
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

        LDraft started = _lClaimClerkDrafts.LDraftStart(draft);
        _lClaimClerkHeld.Add(started.LDraftId);
        return started;
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

        if (draft.LDraftAuthorHeld is LAuthor author)
        {
            return author.LAuthorNamed ? null : LRefusal.LRefusalName;
        }

        if (draft.LDraftExample is not null
            || draft.LDraftSituation is not null
            || draft.LDraftReference is not null)
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

    public LDraft LDraftSweep(long id)
    {
        LDraft swept = LDraftLoad(id).LDraftNormalize();
        LDraftSave(swept);
        return swept;
    }

    public bool LClaimClerkCheck(long id)
    {
        return _lClaimClerkDrafts.LDraftClaimCheck(id) && _lClaimClerkHeld.Contains(id);
    }

    public bool LClaimForeignCheck(long id)
    {
        return _lClaimClerkDrafts.LDraftForeignCheck(id);
    }

    public void LClaimClerkFinish(long id)
    {
        _lClaimClerkHeld.Remove(id);
        _lClaimClerkChronicle.LChronicleClerkClear(id);
        _lClaimClerkDrafts.LDraftFinish(id);
    }

    public void LClaimClerkDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);
        try
        {
            LDraftReleaseApply(_lClaimClerkDrafts.LDraftDelete(id, _lClaimClerkHeld));
        }
        catch
        {
            LClaimClerkSync();
            throw;
        }
    }

    public void LClaimClerkCancel(long id)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);
        try
        {
            LDraftReleaseApply(_lClaimClerkDrafts.LDraftCancel(id, _lClaimClerkHeld));
        }
        catch
        {
            LClaimClerkSync();
            throw;
        }
    }

    public void LClaimClerkSweep()
    {
        try
        {
            LDraftReleaseApply(_lClaimClerkDrafts.LDraftSweep(_lClaimClerkHeld));
        }
        catch
        {
            LClaimClerkSync();
            throw;
        }
    }

    private void LClaimClerkSync()
    {
        foreach (long id in _lClaimClerkHeld.ToArray())
        {
            if (!_lClaimClerkDrafts.LDraftClaimCheck(id))
            {
                _lClaimClerkHeld.Remove(id);
                _lClaimClerkChronicle.LChronicleClerkClear(id);
            }
        }
    }

    private void LDraftReleaseApply(LDraftRelease release)
    {
        foreach (long id in release.LDraftReleaseDrafts)
        {
            _lClaimClerkHeld.Remove(id);
            _lClaimClerkChronicle.LChronicleClerkClear(id);
        }

        foreach (long id in release.LDraftReleaseOwners)
        {
            _lClaimClerkChronicle.LChronicleClerkClear(id);
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
}
