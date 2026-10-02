using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private void QScenarioDeskIntroduce()
    {
        _cRepertoire.CRepertoireDraftChanged += QScenarioFieldsRefine;
    }

    public void QChronicleUndoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cRepertoire.CRepertoireSession.CSessionUndo);
    }

    public void QChronicleRedoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cRepertoire.CRepertoireSession.CSessionRedo);
    }

    private void QRepertoireChronicleRefine()
    {
        (bool undo, bool redo) = _cRepertoire.CRepertoireSession.CSessionChronicleRead();
        QRepertoireBackward.IsEnabled = undo;
        QRepertoireForward.IsEnabled = redo;
    }

    private void QRepertoireUndoObserve(object sender, RoutedEventArgs e)
    {
        QChronicleUndoObserve();
    }

    private void QRepertoireRedoObserve(object sender, RoutedEventArgs e)
    {
        QChronicleRedoObserve();
    }
}
