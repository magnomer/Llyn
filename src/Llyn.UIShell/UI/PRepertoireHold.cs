using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private const int PScenarioChangeDelay = 250;

    private const string PScenarioOrigin = "Repertoire";

    private CancellationTokenSource? _pScenarioPending;

    private long _pScenarioDraft;

    private bool _pScenarioHalted;

    internal bool PRepertoireDraftFinish(bool store)
    {
        if (_pScenarioPending is not null)
        {
            PScenarioChangeSave();
        }

        if (!store || !PScenarioDraftCheck())
        {
            PScenarioDraftCancel();
            return true;
        }

        long held = _pScenarioDraft;
        if (held == 0)
        {
            return true;
        }

        _pScenarioDraft = 0;

        try
        {
            _pRepertoireHost.PWindowCommitRun(held, _lEngine.LEngineSituationCommit);
        }
        catch (Exception exception)
        {
            _pScenarioDraft = held;
            _pRepertoireHost.PWindowFailureShow("Situation.SaveFailed", exception);
            return false;
        }

        return true;
    }

    private bool PScenarioChangeCheck()
    {
        if (_pScenarioPending is not null)
        {
            PScenarioChangeSave();
        }

        return PScenarioDraftCheck();
    }

    private void PScenarioChangeDefer()
    {
        if (_pScenarioLoading || _pScenarioHalted || _pScenarioDraft == 0)
        {
            return;
        }

        PScenarioChangeStop();

        CancellationTokenSource pending = new();
        _pScenarioPending = pending;

        _ = PScenarioChangeRun(pending.Token);
    }

    private async Task PScenarioChangeRun(CancellationToken token)
    {
        try
        {
            await Task.Delay(PScenarioChangeDelay, token).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        try
        {
            PScenarioChangeSave();
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.HoldFailed", exception);
        }
    }

    private void PScenarioChangeSave()
    {
        PScenarioChangeStop();

        if (_pScenarioLoading || _pScenarioHalted || _pScenarioDraft == 0)
        {
            return;
        }

        PScenarioDraftSave();
        PScenarioChangeUpdate();
    }

    private void PScenarioChangeStop()
    {
        CancellationTokenSource? pending = _pScenarioPending;
        _pScenarioPending = null;

        if (pending is null)
        {
            return;
        }

        pending.Cancel();
        pending.Dispose();
    }

    private void PScenarioChangeUpdate()
    {
        bool changed = PScenarioDraftCheck();
        PScenarioDiscard.IsEnabled = changed;
        PScenarioStore.IsEnabled = changed;
    }

    private LDraft? PScenarioDraftStart(long? situation)
    {
        PScenarioChangeStop();
        PScenarioDraftCancel();

        try
        {
            LDraft started = _lEngine.LEngineSituationStart(PScenarioOrigin, situation);
            _pScenarioDraft = started.LDraftId;
            PScenarioHoldResume();
            return started;
        }
        catch (Exception exception)
        {
            _pScenarioDraft = 0;
            PScenarioHoldSuspend(exception);
            return null;
        }
    }

    private void PScenarioDraftShow(LDraft? started)
    {
        PScenarioApply(started?.LDraftSituation);
    }

    private void PScenarioDraftSave()
    {
        if (_pScenarioDraft == 0)
        {
            return;
        }

        try
        {
            LDraft? held = _lEngine.LEngineDraftRead(_pScenarioDraft);
            if (held?.LDraftSituation is not LSituation content)
            {
                return;
            }

            _lEngine.LEngineRequestApply(
                PScenarioRead(_pScenarioDraft, content.LSituationId));
        }
        catch (Exception exception)
        {
            PScenarioHoldSuspend(exception);
        }
    }

    private void PScenarioDraftRestore()
    {
        if (_pScenarioDraft == 0 || _pScenarioLoading)
        {
            return;
        }

        try
        {
            if (_lEngine.LEngineDraftRead(_pScenarioDraft)?.LDraftSituation is LSituation held)
            {
                PScenarioShow(held);
            }
        }
        catch (Exception exception)
        {
            PScenarioHoldSuspend(exception);
        }
    }

    private void PScenarioDraftCancel()
    {
        if (_pScenarioDraft == 0)
        {
            return;
        }

        long held = _pScenarioDraft;
        _pScenarioDraft = 0;

        try
        {
            _lEngine.LEngineDraftCancel(held);
        }
        catch (Exception)
        {
        }
    }

    private bool PScenarioDraftCheck()
    {
        if (_pScenarioDraft == 0)
        {
            return false;
        }

        try
        {
            return _lEngine.LEngineDraftCheck(_pScenarioDraft);
        }
        catch (Exception exception)
        {
            PScenarioHoldSuspend(exception);
            return false;
        }
    }

    private long? PScenarioSituationRead()
    {
        if (_pScenarioDraft == 0)
        {
            return null;
        }

        LDraft? held;
        try
        {
            held = _lEngine.LEngineDraftRead(_pScenarioDraft);
        }
        catch (Exception)
        {
            return null;
        }

        return held?.LDraftEntryId is null or 0 ? null : held.LDraftEntryId;
    }

    private void PScenarioHoldSuspend(Exception exception)
    {
        if (_pScenarioHalted)
        {
            return;
        }

        _pScenarioHalted = true;
        PScenario.IsEnabled = false;
        _pRepertoireHost.PWindowFailureShow("Situation.HoldFailed", exception);
    }

    private void PScenarioHoldResume()
    {
        if (!_pScenarioHalted)
        {
            return;
        }

        _pScenarioHalted = false;
        PScenario.IsEnabled = true;
    }
}
