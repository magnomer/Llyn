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

    private string _pImprintDraft = string.Empty;

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

        string held = _pImprintDraft;
        if (held.Length == 0)
        {
            return true;
        }

        _pImprintDraft = string.Empty;

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
        if (_pImprintLoading || _pImprintHalted || _pImprintDraft.Length == 0)
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

        if (_pImprintLoading || _pImprintHalted || _pImprintDraft.Length == 0)
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

    private LDraft? PImprintDraftStart(string? reference)
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
            _pImprintDraft = string.Empty;
            PImprintHoldSuspend(exception);
            return null;
        }
    }

    private void PImprintDraftShow(LDraft? started)
    {
        PImprintApply(started?.LDraftReference);
    }

    private void PImprintDraftSave()
    {
        if (_pImprintDraft.Length == 0)
        {
            return;
        }

        try
        {
            LDraft? held = _lEngine.LEngineDraftRead(_pImprintDraft);
            if (held?.LDraftReference is not LReference content)
            {
                return;
            }

            LReference sent = PImprintRead(content);
            LReference stored = _lEngine.LEngineReferenceSave(held with { LDraftReference = sent });

            if (!ReferenceEquals(stored, sent))
            {
                PImprintApply(stored);
            }
        }
        catch (Exception exception)
        {
            PImprintHoldSuspend(exception);
        }
    }

    internal void PImprintDraftCancel()
    {
        if (_pImprintDraft.Length == 0)
        {
            return;
        }

        string held = _pImprintDraft;
        _pImprintDraft = string.Empty;

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
        if (_pImprintDraft.Length == 0)
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

    private string? PImprintReferenceRead()
    {
        if (_pImprintDraft.Length == 0)
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

        return string.IsNullOrWhiteSpace(held?.LDraftEntry) ? null : held.LDraftEntry;
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
