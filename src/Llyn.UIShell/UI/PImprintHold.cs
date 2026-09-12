using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PImprint
{
    private const int PImprintChangeDelay = 250;

    private const string PImprintOrigin = "Reference";

    private CancellationTokenSource? _pImprintPending;

    private long _pImprintDraft;

    private bool _pImprintHalted;

    internal bool PImprintDraftFinish(bool store)
    {
        if (_pImprintPending is not null)
        {
            PImprintChangeSave();
        }

        if (!store || !PImprintDraftCheck())
        {
            PImprintDraftCancel();
            return true;
        }

        long held = _pImprintDraft;
        if (held == 0)
        {
            return true;
        }

        _pImprintDraft = 0;

        try
        {
            _lEngine.LEngineReferenceCommit(held);
        }
        catch (Exception exception)
        {
            _pImprintDraft = held;
            _pImprintHost.PWindowFailureShow("Source.SaveFailed", exception);
            return false;
        }

        return true;
    }

    internal bool PImprintChangeCheck()
    {
        if (_pImprintPending is not null)
        {
            PImprintChangeSave();
        }

        return PImprintDraftCheck();
    }

    private void PImprintChangeDefer()
    {
        if (_pImprintLoading || _pImprintHalted || _pImprintDraft == 0)
        {
            return;
        }

        PImprintChangeStop();

        CancellationTokenSource pending = new();
        _pImprintPending = pending;

        _ = PImprintChangeRun(pending.Token);
    }

    private async Task PImprintChangeRun(CancellationToken token)
    {
        try
        {
            await Task.Delay(PImprintChangeDelay, token).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        try
        {
            PImprintChangeSave();
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.HoldFailed", exception);
        }
    }

    private void PImprintChangeSave()
    {
        PImprintChangeStop();

        if (_pImprintLoading || _pImprintHalted || _pImprintDraft == 0)
        {
            return;
        }

        PImprintDraftSave();
        PImprintChangeUpdate();
    }

    private void PImprintChangeStop()
    {
        CancellationTokenSource? pending = _pImprintPending;
        _pImprintPending = null;

        if (pending is null)
        {
            return;
        }

        pending.Cancel();
        pending.Dispose();
    }

    private void PImprintChangeUpdate()
    {
        bool changed = PImprintDraftCheck();
        PImprintDiscard.IsEnabled = changed;
        PImprintStore.IsEnabled = changed;
    }

    private LDraft? PImprintDraftStart(long? reference)
    {
        PImprintChangeStop();
        PImprintDraftCancel();

        try
        {
            LDraft started = _lEngine.LEngineReferenceStart(PImprintOrigin, reference);
            _pImprintDraft = started.LDraftId;
            PImprintHoldResume();
            return started;
        }
        catch (Exception exception)
        {
            _pImprintDraft = 0;
            PImprintHoldSuspend(exception);
            return null;
        }
    }

    private void PImprintDraftShow(LDraft? started)
    {
        PImprintApply(started);
    }

    private void PImprintDraftSave()
    {
        if (_pImprintDraft == 0)
        {
            return;
        }

        try
        {
            _lEngine.LEngineRequestApply(new LRequestReferenceBody(_pImprintDraft, PImprintRead()));
        }
        catch (Exception exception)
        {
            PImprintHoldSuspend(exception);
        }
    }

    internal void PImprintDraftRestore(long id)
    {
        if (id == _pImprintDraft)
        {
            PImprintDraftRestore();
        }
    }

    private void PImprintDraftRestore()
    {
        if (_pImprintDraft == 0 || _pImprintLoading)
        {
            return;
        }

        try
        {
            if (_lEngine.LEngineDraftRead(_pImprintDraft) is LDraft held)
            {
                PImprintShow(held);
            }
        }
        catch (Exception exception)
        {
            PImprintHoldSuspend(exception);
        }
    }

    internal void PImprintDraftCancel()
    {
        if (_pImprintDraft == 0)
        {
            return;
        }

        long held = _pImprintDraft;
        _pImprintDraft = 0;

        try
        {
            _lEngine.LEngineDraftCancel(held);
        }
        catch (Exception)
        {
        }
    }

    private bool PImprintDraftCheck()
    {
        if (_pImprintDraft == 0)
        {
            return false;
        }

        try
        {
            return _lEngine.LEngineDraftCheck(_pImprintDraft);
        }
        catch (Exception exception)
        {
            PImprintHoldSuspend(exception);
            return false;
        }
    }

    private long? PImprintReferenceRead()
    {
        if (_pImprintDraft == 0)
        {
            return null;
        }

        LDraft? held;
        try
        {
            held = _lEngine.LEngineDraftRead(_pImprintDraft);
        }
        catch (Exception)
        {
            return null;
        }

        return held?.LDraftEntryId is null or 0 ? null : held.LDraftEntryId;
    }

    private void PImprintHoldSuspend(Exception exception)
    {
        if (_pImprintHalted)
        {
            return;
        }

        _pImprintHalted = true;
        IsEnabled = false;
        _pImprintHost.PWindowFailureShow("Source.HoldFailed", exception);
    }

    private void PImprintHoldResume()
    {
        if (!_pImprintHalted)
        {
            return;
        }

        _pImprintHalted = false;
        IsEnabled = true;
    }
}
