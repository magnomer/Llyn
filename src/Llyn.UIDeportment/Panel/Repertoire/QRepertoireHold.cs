using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private CDesk QScenarioDesk => _cRepertoire.CRepertoireDesk;

    private void QScenarioDeskAttach()
    {
        QScenarioDesk.CDeskObserverAttach(
            LObserver.LObserverCreate<Action>(static run => run()), QScenarioDraftRestore);
        QScenarioDesk.CDeskFailed += _qRepertoireHost.PWindowFailureRefine;
        QScenarioDesk.CDeskRefused += _qRepertoireHost.PWindowEnvoy.CEnvoyFailureShow;
    }

    private void QScenarioChangeDefer()
    {
        QScenarioDesk.CDeskQuill?.LQuillSituationSet(
            QScenarioTitle.Text, QScenarioDescription.Text, QScenarioKind.Text);
    }

    private void QScenarioDraftRestore()
    {
        if (_cRepertoire.CRepertoireScenarioRead() is CSituationDraft situation)
        {
            QScenarioShow(situation);
        }
    }

    public void QChronicleUndo()
    {
        QChronicle.QChronicleRun(_cRepertoire.CRepertoireSession.CSessionUndo);
    }

    public void QChronicleRedo()
    {
        QChronicle.QChronicleRun(_cRepertoire.CRepertoireSession.CSessionRedo);
    }

    public void QChronicleUpdate()
    {
        (bool undo, bool redo) = _cRepertoire.CRepertoireSession.CSessionChronicleRead();
        QRepertoireBackward.IsEnabled = undo;
        QRepertoireForward.IsEnabled = redo;
    }

    private void QRepertoireUndoHandle(object sender, RoutedEventArgs e)
    {
        QChronicleUndo();
    }

    private void QRepertoireRedoHandle(object sender, RoutedEventArgs e)
    {
        QChronicleRedo();
    }
}
