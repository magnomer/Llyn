using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private void QTranscriptDeskIntroduce()
    {
        _cCorpus.CCorpusDraftChanged += QTranscriptDraftRefine;
    }

    public void QChronicleUndoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cCorpus.CCorpusSession.CSessionUndo);
    }

    public void QChronicleRedoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cCorpus.CCorpusSession.CSessionRedo);
    }

    private void QCorpusChronicleRefine()
    {
        (bool undo, bool redo) = _cCorpus.CCorpusSession.CSessionChronicleRead();
        QCorpusBackward.IsEnabled = undo;
        QCorpusForward.IsEnabled = redo;
    }

    private void QCorpusUndoObserve(object sender, RoutedEventArgs e)
    {
        QChronicleUndoObserve();
    }

    private void QCorpusRedoObserve(object sender, RoutedEventArgs e)
    {
        QChronicleRedoObserve();
    }
}
