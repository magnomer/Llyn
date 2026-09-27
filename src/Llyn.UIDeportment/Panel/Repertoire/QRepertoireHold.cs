using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private LDesk QScenarioDesk => _lRepertoire.LRepertoireDesk;

    private void QScenarioDeskAttach()
    {
        UserControl surface = _qRepertoireSurface;
        QScenarioDesk.LDeskVigil.QVigilDraftAttach(
            CSubject.CSubjectDraft, LObserver.LObserverCreate<CBulletin>(surface, QScenarioDraftRestore));
        QScenarioDesk.LDeskVigil.QVigilDraftAttach(
            CSubject.CSubjectTenure, LObserver.LObserverCreate<CBulletin>(surface, QScenarioDesk.LDeskStateUpdate));
        QScenarioDesk.LDeskFailed += _qRepertoireHost.PWindowFailureShow;
        QScenarioDesk.LDeskRefused += _qRepertoireHost.PWindowEnvoy.CEnvoyFailureShow;
    }

    private void QScenarioChangeDefer()
    {
        QScenarioDesk.LDeskQuill?.LQuillSituationSet(
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
        QChronicle.QChronicleRun(_lRepertoire.LRepertoireSession.QSessionUndo);
    }

    public void QChronicleRedo()
    {
        QChronicle.QChronicleRun(_lRepertoire.LRepertoireSession.QSessionRedo);
    }

    public void QChronicleUpdate()
    {
        (bool undo, bool redo) = _lRepertoire.LRepertoireSession.QSessionChronicleRead();
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
