using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly Dictionary<long, List<LRequest>> _lChronicleClerkDeferred = [];
    private readonly Dictionary<long, CancellationTokenSource> _lChronicleClerkPending = [];
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

    public async Task<CancellationTokenSource?> LChronicleClerkDefer(long id, LRequest request, int delay)
    {
        ArgumentNullException.ThrowIfNull(request);

        CancellationTokenSource? pending = null;
        CancellationTokenSource? replaced = null;
        CancellationToken cancellation = CancellationToken.None;
        lock (_lChronicleClerkDeferred)
        {
            if (!_lChronicleClerkDeferred.TryGetValue(id, out List<LRequest>? deferred))
            {
                deferred = [];
                _lChronicleClerkDeferred[id] = deferred;
            }

            int index = deferred.FindIndex(
                held => string.Equals(held.LRequestKey, request.LRequestKey, StringComparison.Ordinal));
            if (index < 0)
            {
                deferred.Add(request);
            }
            else
            {
                deferred[index] = request;
            }

            if (delay > 0)
            {
                _lChronicleClerkPending.Remove(id, out replaced);
                pending = new CancellationTokenSource();
                cancellation = pending.Token;
                _lChronicleClerkPending[id] = pending;
            }
        }

        if (replaced is not null)
        {
            replaced.Cancel();
            replaced.Dispose();
        }

        if (pending is null)
        {
            return null;
        }

        try
        {
            await _lChronicleClerkClock.LClockPause(TimeSpan.FromMilliseconds(delay), cancellation)
                .ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
        }
        catch (OperationCanceledException)
        {
            return null;
        }

        return pending;
    }

    public IReadOnlyList<LRequest> LChronicleClerkDispatch(long id, CancellationTokenSource? pending)
    {
        CancellationTokenSource? stopped;
        List<LRequest>? taken;
        lock (_lChronicleClerkDeferred)
        {
            _lChronicleClerkPending.TryGetValue(id, out CancellationTokenSource? current);
            if (pending is not null && !ReferenceEquals(current, pending))
            {
                return [];
            }

            _lChronicleClerkPending.Remove(id, out stopped);
            _lChronicleClerkDeferred.Remove(id, out taken);
        }

        if (stopped is not null)
        {
            stopped.Cancel();
            stopped.Dispose();
        }

        return taken ?? [];
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
