using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LTenureQueue
{
    private readonly LEngine _lTenureQueueEngine;

    private readonly LChronicleClerk _lTenureQueueChronicle;

    private readonly long _lTenureQueueId;

    private readonly object _lTenureQueueGate;

    private readonly object _lTenureQueueTurn;

    private readonly Action _lTenureQueueObserver;

    private Exception? _lTenureQueueFault;

    internal LTenureQueue(LEngine engine, long id, object gate, object turn, Action observer)
    {
        _lTenureQueueEngine = engine;
        _lTenureQueueChronicle = engine.LEngineStaffHeld.LEngineStaffClaim.LClaimStaffChronicle;
        _lTenureQueueId = id;
        _lTenureQueueGate = gate;
        _lTenureQueueTurn = turn;
        _lTenureQueueObserver = observer;
    }

    internal Exception? LTenureQueueFault
    {
        get
        {
            lock (_lTenureQueueGate)
            {
                return _lTenureQueueFault;
            }
        }
    }

    internal bool LTenureQueueEnded { get; private set; }

    internal bool LTenureQueueLive => !LTenureQueueEnded && _lTenureQueueFault is null;

    internal async Task LTenureQueueStart(LRequest request, int delay)
    {
        CancellationTokenSource? pending = await _lTenureQueueChronicle
            .LChronicleClerkDefer(_lTenureQueueId, request, delay)
            .ConfigureAwait(false);
        if (pending is null)
        {
            return;
        }

        lock (_lTenureQueueTurn)
        {
            LTenureQueueDispatch(pending);
        }
    }

    internal bool LTenureQueueClose()
    {
        _lTenureQueueChronicle.LChronicleClerkDispatch(_lTenureQueueId, null);
        if (LTenureQueueEnded)
        {
            return false;
        }

        LTenureQueueEnded = true;
        return true;
    }

    internal void LTenureQueueDispatch(CancellationTokenSource? pending)
    {
        IReadOnlyList<LRequest> taken;
        lock (_lTenureQueueGate)
        {
            taken = _lTenureQueueChronicle.LChronicleClerkDispatch(_lTenureQueueId, pending);
            if (!LTenureQueueLive || taken.Count == 0)
            {
                return;
            }
        }

        LTenureQueueApply(taken);
    }

    internal void LTenureQueueApply(IReadOnlyList<LRequest> requests)
    {
        bool refused = false;
        foreach (LRequest request in requests)
        {
            lock (_lTenureQueueGate)
            {
                if (!LTenureQueueLive)
                {
                    return;
                }
            }

            try
            {
                _lTenureQueueEngine.LEngineRequest.LEngineRequestApply(request);
            }
            catch (Exception exception) when (LWorkspaceClerk.LWorkspaceRefusedCheck(exception))
            {
                refused = true;
            }
            catch (Exception exception)
            {
                LTenureQueueSuspend(exception);
                return;
            }
        }

        if (refused)
        {
            _lTenureQueueEngine.LEngineBulletinRaise(LSubject.LSubjectDraft, _lTenureQueueId);
        }

        _lTenureQueueObserver();
    }

    internal LDraft? LTenureQueueRestore(Func<long, LDraft?> step)
    {
        LDraft? restored;
        lock (_lTenureQueueTurn)
        {
            LTenureQueueDispatch(null);

            lock (_lTenureQueueGate)
            {
                if (!LTenureQueueLive)
                {
                    return null;
                }
            }

            restored = step(_lTenureQueueId);
        }

        _lTenureQueueObserver();
        return restored;
    }

    internal void LTenureQueueSuspend(Exception exception)
    {
        lock (_lTenureQueueGate)
        {
            if (_lTenureQueueFault is not null)
            {
                return;
            }

            _lTenureQueueFault = exception;
            _lTenureQueueChronicle.LChronicleClerkDispatch(_lTenureQueueId, null);
        }

        _lTenureQueueObserver();
    }
}
