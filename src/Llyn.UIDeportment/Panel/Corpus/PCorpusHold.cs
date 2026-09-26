using System;
using System.Windows;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PCorpus
{
    private LDesk PTranscriptDesk => _lCorpus.LCorpusDesk;

    private long PTranscriptDraft => PTranscriptDesk.LDeskId;

    private void PTranscriptDeskAttach()
    {
        PTranscriptDesk.LDeskDraftAttach(
            LSubject.LSubjectDraft, LObserver.LObserverCreate(this, PTranscriptDraftRestore));
        PTranscriptDesk.LDeskDraftAttach(
            LSubject.LSubjectTenure, LObserver.LObserverCreate(this, PTranscriptDesk.LDeskStateUpdate));
        PTranscriptDesk.LDeskFailed += _pCorpusHost.PWindowFailureShow;
        PTranscriptDesk.LDeskRefused += _pCorpusHost.PWindowFailureShow;
    }

    private void PTranscriptRequestDefer(LRequest request)
    {
        if (_pTranscriptLoading)
        {
            return;
        }

        PTranscriptDesk.LDeskDefer(request);
    }

    private void PTranscriptRequestSend(LRequest request)
    {
        PTranscriptDesk.LDeskSend(request);
    }

    private void PTranscriptDraftRestore()
    {
        if (_pTranscriptLoading)
        {
            return;
        }

        try
        {
            if (PTranscriptDesk.LDeskRead()?.LDraftExample is LExample sentence)
            {
                PTranscriptShow(sentence);
            }
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.HoldFailed", exception);
        }
    }

    public void PChronicleUndo()
    {
        PChronicle.PChronicleRun(_lCorpus.LCorpusUndo);
    }

    public void PChronicleRedo()
    {
        PChronicle.PChronicleRun(_lCorpus.LCorpusRedo);
    }

    public void PChronicleUpdate()
    {
        (bool undo, bool redo) = _lCorpus.LCorpusChronicleRead();
        PCorpusBackward.IsEnabled = undo;
        PCorpusForward.IsEnabled = redo;
    }

    private void PCorpusUndoHandle(object sender, RoutedEventArgs e)
    {
        PChronicleUndo();
    }

    private void PCorpusRedoHandle(object sender, RoutedEventArgs e)
    {
        PChronicleRedo();
    }
}
