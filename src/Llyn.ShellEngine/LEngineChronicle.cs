using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private const int LEngineChronicleCap = 100;

    private const int LEngineChronicleWindow = 1500;

    private readonly Dictionary<long, List<LDraft>> _lEngineChroniclePast = [];

    private readonly Dictionary<long, List<LDraft>> _lEngineChronicleFuture = [];

    private (long, Type, DateTimeOffset)? _lEngineChronicleLast;

    private Func<DateTimeOffset> _lEngineChronicleClock = static () => DateTimeOffset.UtcNow;

    internal Func<DateTimeOffset> LEngineChronicleClock
    {
        set => _lEngineChronicleClock = value;
    }

    internal LDraft? LEngineChronicleUndo(long id)
    {
        LDraft? restored;
        lock (_lEngineGate)
        {
            _lEngineChronicleLast = null;
            restored = LEngineChronicleRestore(id, _lEngineChroniclePast, _lEngineChronicleFuture);
        }

        if (restored is not null)
        {
            LEngineBulletinRaise(LSubject.LSubjectDraft, restored.LDraftId);
        }

        return restored;
    }

    internal LDraft? LEngineChronicleRedo(long id)
    {
        LDraft? restored;
        lock (_lEngineGate)
        {
            _lEngineChronicleLast = null;
            restored = LEngineChronicleRestore(id, _lEngineChronicleFuture, _lEngineChroniclePast);
        }

        if (restored is not null)
        {
            LEngineBulletinRaise(LSubject.LSubjectDraft, restored.LDraftId);
        }

        return restored;
    }

    internal bool LEngineUndoCheck(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineChroniclePast.TryGetValue(id, out List<LDraft>? past) && past.Count > 0;
        }
    }

    internal bool LEngineRedoCheck(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineChronicleFuture.TryGetValue(id, out List<LDraft>? future) && future.Count > 0;
        }
    }

    private void LEngineChronicleRecord(LDraft held, LDraft saved, LRequest? request)
    {
        if (LEngineDraftMatch(held, saved) || (!LEngineDraftCheck(held) && !LEngineDraftCheck(saved)))
        {
            return;
        }

        DateTimeOffset now = _lEngineChronicleClock();
        if (request is not null
            && _lEngineChronicleLast is (long draft, Type type, DateTimeOffset moment)
            && draft == held.LDraftId
            && type == request.GetType()
            && (now - moment).TotalMilliseconds < LEngineChronicleWindow)
        {
            _lEngineChronicleLast = (draft, type, now);
            return;
        }

        _lEngineChronicleLast = request is null ? null : (held.LDraftId, request.GetType(), now);
        if (!_lEngineChroniclePast.TryGetValue(held.LDraftId, out List<LDraft>? past))
        {
            past = [];
            _lEngineChroniclePast[held.LDraftId] = past;
        }

        past.Add(held);
        _lEngineChronicleFuture.Remove(held.LDraftId);

        if (past.Count > LEngineChronicleCap)
        {
            past.RemoveRange(0, past.Count - LEngineChronicleCap);
        }
    }

    private void LEngineChronicleClear(long id)
    {
        _lEngineChronicleLast = null;
        _lEngineChroniclePast.Remove(id);
        _lEngineChronicleFuture.Remove(id);
    }

    private LDraft? LEngineChronicleRestore(
        long id, Dictionary<long, List<LDraft>> source, Dictionary<long, List<LDraft>> target)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);
        LEngineDraftValidate(id);

        if (!source.TryGetValue(id, out List<LDraft>? taken) || taken.Count == 0)
        {
            return null;
        }

        LDraft current = LEngineDraftLoad(id);
        LDraft restored = taken[^1];
        taken.RemoveAt(taken.Count - 1);

        if (!target.TryGetValue(id, out List<LDraft>? kept))
        {
            kept = [];
            target[id] = kept;
        }

        kept.Add(current);
        _lEngineDrafts.LDraftSave(restored);
        return restored;
    }
}
