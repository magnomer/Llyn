using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeDraft : LDraftVault
{
    private readonly Dictionary<long, LDraft> _tVaultFakeRows = [];
    private readonly HashSet<long> _tVaultFakeStale = [];
    private readonly Dictionary<long, int> _tVaultFakeClaims = [];
    private readonly Dictionary<long, LCourt> _tVaultFakeCourts = [];
    private int _tVaultFakeProcess;
    private long _tVaultFakeDoomed;

    internal TVaultFakeDraft(int process)
    {
        _tVaultFakeProcess = process;
    }

    public LDraft LDraftStart(LDraft draft)
    {
        _tVaultFakeRows[draft.LDraftId] = draft;
        _tVaultFakeClaims[draft.LDraftId] = _tVaultFakeProcess;
        return draft;
    }

    public LDraft? LDraftRead(long id) => _tVaultFakeRows.GetValueOrDefault(id);

    public IReadOnlyList<LDraft> LDraftScan() => [.. _tVaultFakeRows.Values.OrderBy(draft => draft.LDraftId)];

    public void LDraftSave(LDraft draft)
    {
        _tVaultFakeRows[draft.LDraftId] = draft;
    }

    public bool LDraftClaimCheck(long id) =>
        _tVaultFakeClaims.TryGetValue(id, out int process) && process == _tVaultFakeProcess;

    public bool LDraftForeignCheck(long id) =>
        _tVaultFakeClaims.TryGetValue(id, out int process) && process != _tVaultFakeProcess;

    public void LDraftFinish(long id)
    {
        _tVaultFakeClaims.Remove(id);
        _tVaultFakeRows.Remove(id);
    }

    public LDraftRelease LDraftDelete(long id, IReadOnlySet<long> held)
    {
        if (_tVaultFakeDoomed != 0)
        {
            LDraftFinish(_tVaultFakeDoomed);
            throw new InvalidOperationException("cascade failed");
        }

        List<long> drafts = TCourtRemove(id, held);
        LDraftFinish(id);
        drafts.Add(id);
        return new LDraftRelease(drafts, []);
    }

    public LDraftRelease LDraftCancel(long id, IReadOnlySet<long> held)
    {
        List<long> drafts = TCourtRemove(id, held);
        IReadOnlyList<long> owners = LDraftCourtSettle(id, 0);
        LDraftFinish(id);
        drafts.Add(id);
        return new LDraftRelease(drafts, owners);
    }

    public LDraftRelease LDraftSweep(IReadOnlySet<long> held)
    {
        List<long> dropped = [.. _tVaultFakeStale.Where(_tVaultFakeRows.ContainsKey)];
        _tVaultFakeStale.Clear();

        List<long> drafts = [];
        List<long> owners = [];
        foreach (long id in dropped)
        {
            LDraftRelease release = LDraftCancel(id, held);
            drafts.AddRange(release.LDraftReleaseDrafts);
            owners.AddRange(release.LDraftReleaseOwners);
        }

        foreach (long id in _tVaultFakeClaims.Keys.Where(id => !_tVaultFakeRows.ContainsKey(id)).ToList())
        {
            _tVaultFakeClaims.Remove(id);
        }

        return new LDraftRelease(drafts, owners);
    }

    public void LDraftCourtSave(LCourt link)
    {
        _tVaultFakeCourts[link.LCourtId] = link;
    }

    public IReadOnlyList<LCourt> LDraftCourtScan() => [.. _tVaultFakeCourts.Values.OrderBy(link => link.LCourtId)];

    public void LDraftCourtDelete(long linkId)
    {
        _tVaultFakeCourts.Remove(linkId);
    }

    public IReadOnlyList<long> LDraftCourtSettle(long draftId, long realId)
    {
        List<long> owners = [];
        foreach (LCourt link in _tVaultFakeCourts.Values.Where(link => link.LCourtTargetId == draftId).ToList())
        {
            _tVaultFakeCourts.Remove(link.LCourtId);
            if (_tVaultFakeRows.GetValueOrDefault(link.LCourtOwnerId) is LDraft owner)
            {
                _tVaultFakeRows[owner.LDraftId] = owner with
                {
                    LDraftContent = owner.LDraftContent.LEntryDraftSettle(draftId, realId),
                };
                owners.Add(owner.LDraftId);
            }
        }

        return owners;
    }

    internal void TDraftDoomedSet(long id)
    {
        _tVaultFakeDoomed = id;
    }

    internal void TDraftStaleSet(long id)
    {
        _tVaultFakeStale.Add(id);
    }

    internal void TClaimProcessSet(int process)
    {
        _tVaultFakeProcess = process;
    }

    private List<long> TCourtRemove(long id, IReadOnlySet<long> held)
    {
        List<long> released = [];
        foreach (LCourt link in _tVaultFakeCourts.Values.Where(link => link.LCourtOwnerId == id).ToList())
        {
            _tVaultFakeCourts.Remove(link.LCourtId);
            bool claimed = _tVaultFakeCourts.Values.Any(other => other.LCourtTargetId == link.LCourtTargetId);
            if (link.LCourtTargetId != id
                && !claimed
                && held.Contains(link.LCourtTargetId)
                && LDraftClaimCheck(link.LCourtTargetId))
            {
                LDraftFinish(link.LCourtTargetId);
                released.Add(link.LCourtTargetId);
            }
        }

        return released;
    }
}
