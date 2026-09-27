using System;
using System.Windows;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PRepertoire
{
    private LDesk PScenarioDesk => _lRepertoire.LRepertoireDesk;

    private long PScenarioDraft => PScenarioDesk.LDeskId;

    private void PScenarioDeskAttach()
    {
        PScenarioDesk.LDeskDraftAttach(
            CSubject.CSubjectDraft, LObserver.LObserverCreate<CBulletin>(this, PScenarioDraftRestore));
        PScenarioDesk.LDeskDraftAttach(
            CSubject.CSubjectTenure, LObserver.LObserverCreate<CBulletin>(this, PScenarioDesk.LDeskStateUpdate));
        PScenarioDesk.LDeskFailed += _pRepertoireHost.PWindowFailureShow;
        PScenarioDesk.LDeskRefused += _pRepertoireHost.PWindowFailureShow;
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

    private void PScenarioDraftRestore()
    {
        if (_pScenarioLoading)
        {
            return;
        }

        if (_lRepertoire.LRepertoireScenarioRead() is LSituation situation)
        {
            PScenarioShow(situation);
        }
    }

    public void PChronicleUndo()
    {
        PChronicle.PChronicleRun(_lRepertoire.LRepertoireUndo);
    }

    public void PChronicleRedo()
    {
        PChronicle.PChronicleRun(_lRepertoire.LRepertoireRedo);
    }

    public void PChronicleUpdate()
    {
        (bool undo, bool redo) = _lRepertoire.LRepertoireChronicleRead();
        PRepertoireBackward.IsEnabled = undo;
        PRepertoireForward.IsEnabled = redo;
    }

    private void PRepertoireUndoHandle(object sender, RoutedEventArgs e)
    {
        PChronicleUndo();
    }

    private void PRepertoireRedoHandle(object sender, RoutedEventArgs e)
    {
        PChronicleRedo();
    }
}
