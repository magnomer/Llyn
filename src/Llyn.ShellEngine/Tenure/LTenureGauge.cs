using System;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LTenureGauge
{
    private static readonly LTenureState LTenureGaugeHalted = new(false, null, false, false, true);

    private static readonly LTenureState LTenureGaugeEnded = new(false, null, false, false, false);

    private readonly LEngine _lEngine;

    private readonly long _lTenureGaugeId;

    private readonly object _lTenureGaugeGate;

    private readonly LTenureQueue _lTenureGaugeQueue;

    private LTenureState _lTenureGaugeLast;

    private LTenureState? _lTenureGaugeState;

    private long _lTenureGaugeRevision;

    internal LTenureGauge(LEngine engine, long id, object gate, LTenureQueue queue)
    {
        _lEngine = engine;
        _lTenureGaugeId = id;
        _lTenureGaugeGate = gate;
        _lTenureGaugeQueue = queue;
        _lTenureGaugeLast = LTenureGaugeRead();
    }

    public LTenureState LTenureGaugeRead()
    {
        long revision;
        lock (_lEngine.LEngineGate)
        {
            revision = _lEngine.LEngineRevision;
        }

        bool halted;
        lock (_lTenureGaugeGate)
        {
            if (_lTenureGaugeQueue.LTenureQueueEnded)
            {
                return LTenureGaugeEnded;
            }

            halted = _lTenureGaugeQueue.LTenureQueueFault is not null;
            if (!halted && _lTenureGaugeState is LTenureState kept && _lTenureGaugeRevision == revision)
            {
                return kept;
            }
        }

        try
        {
            bool changed = _lEngine.LEngineDraft.LEngineDraftCheck(_lTenureGaugeId, out string? refusal);
            LTenureState state = new(
                changed,
                refusal,
                !halted && _lEngine.LEngineRequest.LEngineUndoCheck(_lTenureGaugeId),
                !halted && _lEngine.LEngineRequest.LEngineRedoCheck(_lTenureGaugeId),
                halted);
            lock (_lTenureGaugeGate)
            {
                if (!halted && _lTenureGaugeQueue.LTenureQueueLive)
                {
                    _lTenureGaugeState = state;
                    _lTenureGaugeRevision = revision;
                }
            }

            return state;
        }
        catch (Exception exception)
        {
            _lTenureGaugeQueue.LTenureQueueSuspend(exception);
            return LTenureGaugeHalted;
        }
    }

    public bool LTenureGaugeStorable =>
        LTenureGaugeRead() is { LTenureStateChanged: true } state && LTenureGaugeCheck(state);

    internal static bool LTenureGaugeCheck(LTenureState state) => state.LTenureStateRefusal is null;

    internal void LTenureGaugeRaise()
    {
        LTenureState state = LTenureGaugeRead();
        lock (_lTenureGaugeGate)
        {
            if (state == _lTenureGaugeLast)
            {
                return;
            }

            _lTenureGaugeLast = state;
        }

        _lEngine.LEngineBulletinRaise(LSubject.LSubjectTenure, _lTenureGaugeId);
    }
}
