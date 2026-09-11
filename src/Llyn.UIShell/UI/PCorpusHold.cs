using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private const int PTranscriptChangeDelay = 250;

    private const string PTranscriptOrigin = "Corpus";

    private CancellationTokenSource? _pTranscriptPending;

    private long _pTranscriptDraft;

    private bool _pTranscriptHalted;

    internal bool PCorpusDraftFinish(bool store)
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

        _pTranscriptDraft = 0;

        try
        {
            _lEngine.LEngineExampleCommit(held);
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
        bool changed = PTranscriptDraftCheck();
        PTranscriptDiscard.IsEnabled = changed;
        PTranscriptStore.IsEnabled = changed;
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
            LDraft? held = _lEngine.LEngineDraftRead(_pTranscriptDraft);
            if (held?.LDraftExample is not LExample content)
            {
                return;
            }

            LExample sent = PTranscriptRead(content);
            LExample stored = _lEngine.LEngineExampleSave(held with { LDraftExample = sent });

            if (!ReferenceEquals(stored, sent))
            {
                PTranscriptApply(stored);
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

        return held?.LDraftEntry is null or 0 ? null : held.LDraftEntry;
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
