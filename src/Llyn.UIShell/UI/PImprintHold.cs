using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PImprint
{
    private const string PImprintOrigin = "Reference";

    private LTenure? _pImprintTenure;

    internal Action? PImprintChronicleNotice;

    private long PImprintDraft => _pImprintTenure?.LTenureId ?? 0;

    internal bool PImprintDraftFinish(bool store)
    {
        if (_pImprintTenure is not LTenure held)
        {
            return true;
        }

        try
        {
            _pImprintHost.PWindowCommitRun(held, store);
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.SaveFailed", exception);
            return false;
        }

        _pImprintTenure = null;
        return true;
    }

    internal bool PImprintChangeCheck()
    {
        if (_pImprintTenure is not LTenure held)
        {
            return false;
        }

        held.LTenurePersist();
        return held.LTenureStateRead().LTenureStateChanged;
    }

    private void PImprintChangeDefer()
    {
        PImprintRequestDefer(PImprintRead(PImprintDraft));
    }

    private void PImprintRequestDefer(LRequest request)
    {
        if (_pImprintLoading || _pImprintTenure is not LTenure held)
        {
            return;
        }

        held.LTenureRequestDefer(request);
    }

    private bool PImprintRequestSend(LRequest request)
    {
        if (_pImprintLoading || _pImprintTenure is not LTenure held)
        {
            return false;
        }

        held.LTenureRequestApply(request);
        return !held.LTenureStateRead().LTenureStateHalted;
    }

    internal void PImprintChangeUpdate()
    {
        LTenureState? state = _pImprintTenure?.LTenureStateRead();
        PImprintChangeNotice?.Invoke(state is { LTenureStateChanged: true });
        if (state is not null)
        {
            PImprintHoldShow(!state.LTenureStateHalted);
        }

        PChronicleUpdate();
    }

    private void PImprintHoldShow(bool running)
    {
        if (running == IsEnabled)
        {
            return;
        }

        IsEnabled = running;
        if (!running)
        {
            _pImprintHost.PWindowFailureShow("Source.HoldFailed");
        }
    }

    private LDraft? PImprintDraftStart(long? reference)
    {
        PImprintDraftCancel();

        try
        {
            LTenure started = _lEngine.LEngineTenureStart(PImprintOrigin, LSubject.LSubjectReference, reference);
            _pImprintTenure = started;
            IsEnabled = true;
            return started.LTenureRead();
        }
        catch (Exception exception)
        {
            _pImprintTenure = null;
            IsEnabled = false;
            _pImprintHost.PWindowFailureShow("Source.HoldFailed", exception);
            return null;
        }
    }

    private void PImprintDraftShow(LDraft? started)
    {
        PImprintApply(started);
    }

    internal void PImprintDraftRestore(long id)
    {
        if (id == PImprintDraft)
        {
            PImprintDraftRestore();
        }
    }

    private void PImprintDraftRestore()
    {
        if (_pImprintLoading || _pImprintTenure is not LTenure held)
        {
            return;
        }

        try
        {
            held.LTenurePersist();
            if (held.LTenureRead() is LDraft draft)
            {
                PImprintShow(draft);
            }
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.HoldFailed", exception);
        }
    }

    internal void PImprintDraftCancel()
    {
        if (_pImprintTenure is not LTenure held)
        {
            return;
        }

        _pImprintTenure = null;
        held.LTenureCancel();
    }

    private long? PImprintReferenceRead()
    {
        LDraft? held;
        try
        {
            held = _pImprintTenure?.LTenureRead();
        }
        catch (Exception)
        {
            return null;
        }

        return held?.LDraftEntryId is null or 0 ? null : held.LDraftEntryId;
    }

    public void PChronicleUndo()
    {
        PImprintChronicleRun(static held => held.LTenureUndo());
    }

    public void PChronicleRedo()
    {
        PImprintChronicleRun(static held => held.LTenureRedo());
    }

    private void PImprintChronicleRun(Func<LTenure, LDraft?> step)
    {
        if (_pImprintTenure is not LTenure held)
        {
            return;
        }

        try
        {
            PChronicle.PChronicleRun(() => step(held));
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.HoldFailed", exception);
        }

        PChronicleUpdate();
    }

    public void PChronicleUpdate()
    {
        PImprintChronicleNotice?.Invoke();
    }

    internal (bool PImprintPast, bool PImprintFuture) PImprintChronicleRead()
    {
        return _pImprintTenure?.LTenureStateRead() is LTenureState state
            ? (state.LTenureStateBackward, state.LTenureStateForward)
            : (false, false);
    }
}
