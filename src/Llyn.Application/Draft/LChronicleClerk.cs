using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LChronicleClerk
{
    private const int LChronicleClerkCap = 100;

    private const int LChronicleClerkWindow = 1500;

    private readonly LDraftVault _lChronicleClerkDrafts;
    private readonly LClock _lChronicleClerkClock;
    private readonly Dictionary<long, List<LDraft>> _lChronicleClerkPast = [];
    private readonly Dictionary<long, List<LDraft>> _lChronicleClerkFuture = [];
    private (long, Type, DateTimeOffset)? _lChronicleClerkLast;

    public LChronicleClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lChronicleClerkDrafts = rig.LRigDrafts;
        _lChronicleClerkClock = rig.LRigClock;
    }

    public void LChronicleClerkRecord(LDraft held, LDraft saved, LRequest? request)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(saved);

        DateTimeOffset now = _lChronicleClerkClock.LClockRead();
        if (request is not null
            && _lChronicleClerkLast is (long draft, Type type, DateTimeOffset moment)
            && draft == held.LDraftId
            && type == request.GetType()
            && (now - moment).TotalMilliseconds < LChronicleClerkWindow)
        {
            _lChronicleClerkLast = (draft, type, now);
            return;
        }

        _lChronicleClerkLast = request is null ? null : (held.LDraftId, request.GetType(), now);
        if (!_lChronicleClerkPast.TryGetValue(held.LDraftId, out List<LDraft>? past))
        {
            past = [];
            _lChronicleClerkPast[held.LDraftId] = past;
        }

        past.Add(held);
        _lChronicleClerkFuture.Remove(held.LDraftId);

        if (past.Count > LChronicleClerkCap)
        {
            past.RemoveRange(0, past.Count - LChronicleClerkCap);
        }
    }

    public void LChronicleClerkClear(long id)
    {
        _lChronicleClerkLast = null;
        _lChronicleClerkPast.Remove(id);
        _lChronicleClerkFuture.Remove(id);
    }

    public LDraft? LChronicleClerkUndo(long id)
    {
        _lChronicleClerkLast = null;
        return LChronicleClerkRestore(id, _lChronicleClerkPast, _lChronicleClerkFuture);
    }

    public LDraft? LChronicleClerkRedo(long id)
    {
        _lChronicleClerkLast = null;
        return LChronicleClerkRestore(id, _lChronicleClerkFuture, _lChronicleClerkPast);
    }

    public bool LChronicleUndoCheck(long id)
    {
        return _lChronicleClerkPast.TryGetValue(id, out List<LDraft>? past) && past.Count > 0;
    }

    public bool LChronicleRedoCheck(long id)
    {
        return _lChronicleClerkFuture.TryGetValue(id, out List<LDraft>? future) && future.Count > 0;
    }

    private LDraft? LChronicleClerkRestore(
        long id, Dictionary<long, List<LDraft>> source, Dictionary<long, List<LDraft>> target)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);

        if (!source.TryGetValue(id, out List<LDraft>? taken) || taken.Count == 0)
        {
            return null;
        }

        LDraft current = _lChronicleClerkDrafts.LDraftRead(id) ?? throw new LRefusal(LRefusal.LRefusalDraft);
        LDraft restored = taken[^1];
        taken.RemoveAt(taken.Count - 1);

        if (!target.TryGetValue(id, out List<LDraft>? kept))
        {
            kept = [];
            target[id] = kept;
        }

        kept.Add(current);
        _lChronicleClerkDrafts.LDraftSave(restored);
        return restored;
    }
}
