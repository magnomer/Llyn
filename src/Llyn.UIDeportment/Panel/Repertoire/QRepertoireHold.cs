using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private CDesk QScenarioDesk => _lRepertoire.LRepertoireDesk;

    private void QScenarioDeskAttach()
    {
        QScenarioDesk.CDeskObserverAttach(
            LObserver.LObserverCreate<Action>(static run => run()), QScenarioDraftRestore);
        QScenarioDesk.CDeskFailed += _qRepertoireHost.PWindowFailureShow;
        QScenarioDesk.CDeskRefused += _qRepertoireHost.PWindowEnvoy.CEnvoyFailureShow;
    }

    private void QScenarioChangeDefer()
    {
        QScenarioDesk.CDeskQuill?.LQuillSituationSet(
            QScenarioTitle.Text, QScenarioDescription.Text, QScenarioKind.Text);
    }

    private void QScenarioDraftRestore()
    {
        if (_lRepertoire.LRepertoireScenarioRead() is CSituationDraft situation)
        {
            QScenarioShow(situation);
        }
    }

    public void QChronicleUndo()
    {
        QChronicle.QChronicleRun(_lRepertoire.LRepertoireSession.CSessionUndo);
    }

    public void QChronicleRedo()
    {
        QChronicle.QChronicleRun(_lRepertoire.LRepertoireSession.CSessionRedo);
    }

    public void QChronicleUpdate()
    {
        (bool undo, bool redo) = _lRepertoire.LRepertoireSession.CSessionChronicleRead();
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
