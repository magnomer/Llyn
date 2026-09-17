using System;
using System.Windows;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private const string PScenarioOrigin = "Repertoire";

    private LTenure? _pScenarioTenure;

    private long PScenarioDraft => _pScenarioTenure?.LTenureId ?? 0;

    private bool PScenarioDraftFinish(bool store)
    {
        if (_pScenarioTenure is not LTenure held)
        {
            return true;
        }

        try
        {
            _pRepertoireHost.PWindowCommitRun(held, store);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.SaveFailed", exception);
            return false;
        }

        _pScenarioTenure = null;
        return true;
    }

    private bool PScenarioChangeCheck()
    {
        if (_pScenarioTenure is not LTenure held)
        {
            return false;
        }

        held.LTenurePersist();
        return held.LTenureStateRead().LTenureStateChanged;
    }

    private void PScenarioChangeDefer()
    {
        PScenarioRequestDefer(PScenarioRead(PScenarioDraft));
    }

    private void PScenarioRequestDefer(LRequest request)
    {
        if (_pScenarioLoading || _pScenarioTenure is not LTenure held)
        {
            return;
        }

        held.LTenureRequestDefer(request);
    }

    private void PScenarioRequestSend(LRequest request)
    {
        if (_pScenarioLoading || _pScenarioTenure is not LTenure held)
        {
            return;
        }

        held.LTenureRequestApply(request);
    }

    private void PScenarioChangeUpdate()
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            return;
        }

        LTenureState? state = _pScenarioTenure?.LTenureStateRead();
        PRepertoireStore.IsEnabled = state is { LTenureStateChanged: true };
        if (state is not null)
        {
            PScenarioHoldShow(!state.LTenureStateHalted);
        }

        PChronicleUpdate();
    }

    private void PScenarioHoldShow(bool running)
    {
        if (running == PScenario.IsEnabled)
        {
            return;
        }

        PScenario.IsEnabled = running;
        if (!running)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.HoldFailed");
        }
    }

    private LDraft? PScenarioDraftStart(long? situation)
    {
        PScenarioDraftCancel();

        try
        {
            LTenure started = _lEngine.LEngineTenureStart(PScenarioOrigin, LSubject.LSubjectSituation, situation);
            _pScenarioTenure = started;
            PScenario.IsEnabled = true;
            return started.LTenureRead();
        }
        catch (Exception exception)
        {
            _pScenarioTenure = null;
            PScenario.IsEnabled = false;
            _pRepertoireHost.PWindowFailureShow("Situation.HoldFailed", exception);
            return null;
        }
    }

    private void PScenarioDraftShow(LDraft? started)
    {
        PScenarioApply(started?.LDraftSituation);
    }

    private void PScenarioDraftRestore()
    {
        if (_pScenarioLoading || _pScenarioTenure is not LTenure held)
        {
            return;
        }

        try
        {
            held.LTenurePersist();
            if (held.LTenureRead()?.LDraftSituation is LSituation situation)
            {
                PScenarioShow(situation);
            }
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.HoldFailed", exception);
        }
    }

    private void PScenarioDraftCancel()
    {
        if (_pScenarioTenure is not LTenure held)
        {
            return;
        }

        _pScenarioTenure = null;
        held.LTenureCancel();
    }

    private long? PScenarioSituationRead()
    {
        LDraft? held;
        try
        {
            held = _pScenarioTenure?.LTenureRead();
        }
        catch (Exception)
        {
            return null;
        }

        return held?.LDraftEntryId is null or 0 ? null : held.LDraftEntryId;
    }

    public void PChronicleUndo()
    {
        PScenarioChronicleRun(static held => held.LTenureUndo());
    }

    public void PChronicleRedo()
    {
        PScenarioChronicleRun(static held => held.LTenureRedo());
    }

    private void PScenarioChronicleRun(Func<LTenure, LDraft?> step)
    {
        if (_pScenarioTenure is not LTenure held)
        {
            return;
        }

        try
        {
            PChronicle.PChronicleRun(() => step(held));
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.HoldFailed", exception);
        }

        PChronicleUpdate();
    }

    public void PChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorChronicleRead()
            : PScenarioChronicleRead();
        PRepertoireBackward.IsEnabled = undo;
        PRepertoireForward.IsEnabled = redo;
    }

    private (bool PScenarioBackward, bool PScenarioForward) PScenarioChronicleRead()
    {
        return _pScenarioTenure?.LTenureStateRead() is LTenureState state
            ? (state.LTenureStateBackward, state.LTenureStateForward)
            : (false, false);
    }

    private void PRepertoireUndoHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PChronicleUndo();
            return;
        }

        PChronicleUndo();
    }

    private void PRepertoireRedoHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PChronicleRedo();
            return;
        }

        PChronicleRedo();
    }
}
