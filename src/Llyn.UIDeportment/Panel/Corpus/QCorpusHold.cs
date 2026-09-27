using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private CDesk QTranscriptDesk => _qCorpusDesk;

    private LQuill? QTranscriptQuill => _lCorpus.LCorpusDesk.CDeskQuill;

    private void QTranscriptDeskAttach()
    {
        QTranscriptDesk.CDeskObserverAttach(
            LObserver.LObserverCreate<Action>(static run => run()), QTranscriptDraftRestore);
        QTranscriptDesk.CDeskFailed += _qCorpusHost.PWindowFailureShow;
        QTranscriptDesk.CDeskRefused += _qCorpusHost.PWindowEnvoy.CEnvoyFailureShow;
    }

    private void QTranscriptDraftRestore()
    {
        QTranscriptShow(_lCorpus.LCorpusTranscriptRead());
    }

    public void QChronicleUndo()
    {
        QChronicle.QChronicleRun(_lCorpus.LCorpusSession.CSessionUndo);
    }

    public void QChronicleRedo()
    {
        QChronicle.QChronicleRun(_lCorpus.LCorpusSession.CSessionRedo);
    }

    public void QChronicleUpdate()
    {
        (bool undo, bool redo) = _lCorpus.LCorpusSession.CSessionChronicleRead();
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
