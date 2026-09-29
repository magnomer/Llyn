using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private CDesk QTranscriptDesk => _qCorpusDesk;

    private LQuill? QTranscriptQuill => _cCorpus.CCorpusDesk.CDeskQuill;

    private void QTranscriptDeskAttach()
    {
        QTranscriptDesk.CDeskObserverAttach(
            LObserver.LObserverCreate<Action>(static run => run()), QTranscriptDraftRestore);
        QTranscriptDesk.CDeskFailed += _qCorpusHost.PWindowFailureRefine;
        QTranscriptDesk.CDeskRefused += _qCorpusHost.PWindowEnvoy.CEnvoyFailureShow;
    }

    private void QTranscriptDraftRestore()
    {
        QTranscriptShow(_cCorpus.CCorpusTranscriptRead());
    }

    public void QChronicleUndo()
    {
        QChronicle.QChronicleRun(_cCorpus.CCorpusSession.CSessionUndo);
    }

    public void QChronicleRedo()
    {
        QChronicle.QChronicleRun(_cCorpus.CCorpusSession.CSessionRedo);
    }

    public void QChronicleUpdate()
    {
        (bool undo, bool redo) = _cCorpus.CCorpusSession.CSessionChronicleRead();
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
