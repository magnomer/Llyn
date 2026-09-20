using System;
using System.Windows;
using Llyn.Application;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PRepertoire
{
    private LDesk PScenarioDesk => _lRepertoire.LRepertoireDesk;

    private long PScenarioDraft => PScenarioDesk.LDeskId;

    private void PScenarioDeskAttach()
    {
        PScenarioDesk.LDeskStarted += PScenarioStartUpdate;
        PScenarioDesk.LDeskDraftAttach(LSubject.LSubjectDraft, PObserver.PObserverCreate(this, PScenarioDraftRestore));
        PScenarioDesk.LDeskDraftAttach(LSubject.LSubjectTenure, PObserver.PObserverCreate(this, PScenarioChangeUpdate));
        PScenarioDesk.LDeskFailed += PScenarioFailureShow;
        PScenarioDesk.LDeskFinished += PScenarioStoredShow;
    }

    private void PScenarioStartUpdate()
    {
        PScenario.IsEnabled = true;
    }

    private void PScenarioFailureShow(string key, Exception exception)
    {
        _pRepertoireHost.PWindowFailureShow(key, exception);
    }

    private bool PScenarioDraftFinish(bool store)
    {
        return PScenarioDesk.LDeskFinish(store);
    }

    private bool PScenarioChangeCheck()
    {
        return PScenarioDesk.LDeskChangeCheck();
    }

    private void PScenarioChangeDefer()
    {
        PScenarioRequestDefer(PScenarioRead(PScenarioDraft));
    }

    private void PScenarioRequestDefer(LRequest request)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        PScenarioDesk.LDeskDefer(request);
    }

    private void PScenarioRequestSend(LRequest request)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        PScenarioDesk.LDeskSend(request);
    }

    private void PScenarioChangeUpdate()
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            return;
        }

        PRepertoireStore.IsEnabled = PScenarioDesk.LDeskChanged;
        if (PScenarioDesk.LDeskHeld)
        {
            PScenarioHoldShow(!PScenarioDesk.LDeskHalted);
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
        _lRepertoire.LRepertoireStart(situation);
        if (!PScenarioDesk.LDeskHeld)
        {
            PScenario.IsEnabled = false;
            return null;
        }

        return PScenarioDesk.LDeskRead();
    }

    private void PScenarioDraftShow(LDraft? started)
    {
        PScenarioApply(started?.LDraftSituation);
    }

    private void PScenarioDraftRestore()
    {
        if (_pScenarioLoading)
        {
            return;
        }

        try
        {
            if (PScenarioDesk.LDeskRead()?.LDraftSituation is LSituation situation)
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
        PScenarioDesk.LDeskCancel();
    }

    private long? PScenarioSituationRead()
    {
        LDraft? held;
        try
        {
            held = PScenarioDesk.LDeskRead();
        }
        catch (Exception)
        {
            return null;
        }

        return held?.LDraftStored;
    }

    public void PChronicleUndo()
    {
        PScenarioChronicleRun(PScenarioDesk.LDeskUndo);
    }

    public void PChronicleRedo()
    {
        PScenarioChronicleRun(PScenarioDesk.LDeskRedo);
    }

    private void PScenarioChronicleRun(Action step)
    {
        try
        {
            PChronicle.PChronicleRun(step);
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
            : PScenarioDesk.LDeskChronicleRead();
        PRepertoireBackward.IsEnabled = undo;
        PRepertoireForward.IsEnabled = redo;
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
