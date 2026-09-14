using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private const int PTranscriptChangeDelay = 250;

    private const string PTranscriptOrigin = "Corpus";

    private CancellationTokenSource? _pTranscriptPending;

    private long _pTranscriptDraft;

    private bool _pTranscriptHalted;

    private bool PTranscriptDraftFinish(bool store)
    {
        if (_pTranscriptPending is not null)
        {
            PTranscriptChangeSave();
        }

        if (!store || !PTranscriptDraftCheck())
        {
            PTranscriptDraftCancel();
            return true;
        }

        long held = _pTranscriptDraft;
        if (held == 0)
        {
            return true;
        }

        if (_pTranscriptHalted)
        {
            return false;
        }

        _pTranscriptDraft = 0;

        try
        {
            _pCorpusHost.PWindowCommitRun(held, _lEngine.LEngineExampleCommit);
        }
        catch (Exception exception)
        {
            _pTranscriptDraft = held;
            _pCorpusHost.PWindowFailureShow("Example.SaveFailed", exception);
            return false;
        }

        return true;
    }

    private bool PTranscriptChangeCheck()
    {
        if (_pTranscriptPending is not null)
        {
            PTranscriptChangeSave();
        }

        return PTranscriptDraftCheck();
    }

    private void PTranscriptChangeDefer()
    {
        if (_pTranscriptLoading || _pTranscriptHalted || _pTranscriptDraft == 0)
        {
            return;
        }

        PTranscriptChangeStop();

        CancellationTokenSource pending = new();
        _pTranscriptPending = pending;

        _ = PTranscriptChangeRun(pending.Token);
    }

    private async Task PTranscriptChangeRun(CancellationToken token)
    {
        try
        {
            await Task.Delay(PTranscriptChangeDelay, token).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        try
        {
            PTranscriptChangeSave();
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.HoldFailed", exception);
        }
    }

    private void PTranscriptChangeSave()
    {
        PTranscriptChangeStop();

        if (_pTranscriptLoading || _pTranscriptHalted || _pTranscriptDraft == 0)
        {
            return;
        }

        PTranscriptDraftSave();
        PTranscriptChangeUpdate();
    }

    private void PTranscriptChangeStop()
    {
        CancellationTokenSource? pending = _pTranscriptPending;
        _pTranscriptPending = null;

        if (pending is null)
        {
            return;
        }

        pending.Cancel();
        pending.Dispose();
    }

    private void PTranscriptChangeUpdate()
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            return;
        }

        PCorpusStore.IsEnabled = PTranscriptDraftCheck();
    }

    private LDraft? PTranscriptDraftStart(long? example)
    {
        PTranscriptChangeStop();
        PTranscriptDraftCancel();

        try
        {
            LDraft started = _lEngine.LEngineExampleStart(PTranscriptOrigin, example);
            _pTranscriptDraft = started.LDraftId;
            PTranscriptHoldResume();
            return started;
        }
        catch (Exception exception)
        {
            _pTranscriptDraft = 0;
            PTranscriptHoldSuspend(exception);
            return null;
        }
    }

    private void PTranscriptDraftShow(LDraft? started)
    {
        PTranscriptApply(started?.LDraftExample);
    }

    private void PTranscriptDraftSave()
    {
        if (_pTranscriptDraft == 0)
        {
            return;
        }

        try
        {
            _lEngine.LEngineRequestApply(PTranscriptRead(_pTranscriptDraft));
            foreach (LRequest request in PTranscriptGlossRead(_pTranscriptDraft))
            {
                _lEngine.LEngineRequestApply(request);
            }
        }
        catch (Exception exception)
        {
            PTranscriptHoldSuspend(exception);
        }
    }

    private void PTranscriptDraftRestore()
    {
        if (_pTranscriptDraft == 0 || _pTranscriptLoading)
        {
            return;
        }

        try
        {
            if (_lEngine.LEngineDraftRead(_pTranscriptDraft)?.LDraftExample is LExample held)
            {
                PTranscriptShow(held);
            }
        }
        catch (Exception exception)
        {
            PTranscriptHoldSuspend(exception);
        }
    }

    private void PTranscriptDraftCancel()
    {
        if (_pTranscriptDraft == 0)
        {
            return;
        }

        long held = _pTranscriptDraft;
        _pTranscriptDraft = 0;

        try
        {
            _lEngine.LEngineDraftCancel(held);
        }
        catch (Exception)
        {
        }
    }

    private bool PTranscriptDraftCheck()
    {
        if (_pTranscriptDraft == 0)
        {
            return false;
        }

        try
        {
            return _lEngine.LEngineDraftCheck(_pTranscriptDraft);
        }
        catch (Exception exception)
        {
            PTranscriptHoldSuspend(exception);
            return false;
        }
    }

    private long? PTranscriptExampleRead()
    {
        if (_pTranscriptDraft == 0)
        {
            return null;
        }

        LDraft? held;
        try
        {
            held = _lEngine.LEngineDraftRead(_pTranscriptDraft);
        }
        catch (Exception)
        {
            return null;
        }

        return held?.LDraftEntryId is null or 0 ? null : held.LDraftEntryId;
    }

    private void PTranscriptHoldSuspend(Exception exception)
    {
        if (_pTranscriptHalted)
        {
            return;
        }

        _pTranscriptHalted = true;
        PTranscript.IsEnabled = false;
        _pCorpusHost.PWindowFailureShow("Example.HoldFailed", exception);
    }

    private void PTranscriptHoldResume()
    {
        if (!_pTranscriptHalted)
        {
            return;
        }

        _pTranscriptHalted = false;
        PTranscript.IsEnabled = true;
    }
}
