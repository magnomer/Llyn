using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private LDesk QTranscriptDesk => _lCorpus.LCorpusDesk;

    private QQuill QTranscriptQuill => _lCorpus.LCorpusDesk.LDeskQuill;

    private void QTranscriptDeskAttach()
    {
        UserControl surface = _qCorpusSurface;
        QTranscriptDesk.LDeskVigil.QVigilDraftAttach(
            CSubject.CSubjectDraft, LObserver.LObserverCreate<CBulletin>(surface, QTranscriptDraftRestore));
        QTranscriptDesk.LDeskVigil.QVigilDraftAttach(
            CSubject.CSubjectTenure, LObserver.LObserverCreate<CBulletin>(surface, QTranscriptDesk.LDeskStateUpdate));
        QTranscriptDesk.LDeskFailed += _qCorpusHost.PWindowFailureShow;
        QTranscriptDesk.LDeskRefused += _qCorpusHost.PWindowEnvoy.CEnvoyFailureShow;
    }

    private void QTranscriptDraftRestore()
    {
        QTranscriptShow(_lCorpus.LCorpusTranscriptRead());
    }

    public void QChronicleUndo()
    {
        QChronicle.QChronicleRun(_lCorpus.LCorpusSession.QSessionUndo);
    }

    public void QChronicleRedo()
    {
        QChronicle.QChronicleRun(_lCorpus.LCorpusSession.QSessionRedo);
    }

    public void QChronicleUpdate()
    {
        (bool undo, bool redo) = _lCorpus.LCorpusSession.QSessionChronicleRead();
        QCorpusBackward.IsEnabled = undo;
        QCorpusForward.IsEnabled = redo;
    }

    private void QCorpusUndoHandle(object sender, RoutedEventArgs e)
    {
        QChronicleUndo();
    }

    private void QCorpusRedoHandle(object sender, RoutedEventArgs e)
    {
        QChronicleRedo();
    }
}
