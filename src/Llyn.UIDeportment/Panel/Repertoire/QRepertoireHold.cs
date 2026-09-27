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
        QScenarioDesk.LDeskDraftAttach(
            CSubject.CSubjectDraft, LObserver.LObserverCreate<CBulletin>(surface, QScenarioDraftRestore));
        QScenarioDesk.LDeskDraftAttach(
            CSubject.CSubjectTenure, LObserver.LObserverCreate<CBulletin>(surface, QScenarioDesk.LDeskStateUpdate));
        QScenarioDesk.LDeskFailed += _qRepertoireHost.PWindowFailureShow;
        QScenarioDesk.LDeskRefused += _qRepertoireHost.PWindowFailureShow;
    }

    private void QScenarioChangeDefer()
    {
        QScenarioDesk.LDeskQuill.QQuillSituationChange(
            QScenarioTitle.Text, QScenarioDescription.Text, QScenarioKind.Text);
    }

    private void QScenarioDraftRestore()
    {
        if (_lRepertoire.LRepertoireScenarioRead() is CSituationDraft situation)
        {
            QScenarioShow(situation);
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
        QRepertoireBackward.IsEnabled = undo;
        QRepertoireForward.IsEnabled = redo;
    }

    private void QRepertoireUndoHandle(object sender, RoutedEventArgs e)
    {
        PChronicleUndo();
    }

    private void QRepertoireRedoHandle(object sender, RoutedEventArgs e)
    {
        PChronicleRedo();
    }
}
