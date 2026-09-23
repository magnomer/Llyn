using System;
using System.Windows;
using Llyn.Application;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PCorpus
{
    private LDesk PTranscriptDesk => _lCorpus.LCorpusDesk;

    private long PTranscriptDraft => PTranscriptDesk.LDeskId;

    private void PTranscriptDeskAttach()
    {
        PTranscriptDesk.LDeskStarted += PTranscriptStartUpdate;
        PTranscriptDesk.LDeskDraftAttach(
            LSubject.LSubjectDraft, PObserver.PObserverCreate(this, PTranscriptDraftRestore));
        PTranscriptDesk.LDeskDraftAttach(
            LSubject.LSubjectTenure, PObserver.PObserverCreate(this, PTranscriptChangeUpdate));
        PTranscriptDesk.LDeskFailed += PTranscriptFailureShow;
        PTranscriptDesk.LDeskFinished += PTranscriptStoredShow;
    }

    private void PTranscriptStartUpdate()
    {
        PTranscript.IsEnabled = true;
    }

    private void PTranscriptFailureShow(string key, Exception exception)
    {
        _pCorpusHost.PWindowFailureShow(key, exception);
    }

    private bool PTranscriptDraftFinish(bool store)
    {
        return PTranscriptDesk.LDeskFinish(store);
    }

    private bool PTranscriptChangeCheck()
    {
        return PTranscriptDesk.LDeskChangeCheck();
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

    private void PTranscriptChangeUpdate()
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            return;
        }

        PCorpusStore.IsEnabled = PTranscriptDesk.LDeskChanged;
        if (PTranscriptDesk.LDeskHeld)
        {
            PTranscriptHoldShow(!PTranscriptDesk.LDeskHalted);
        }

        PChronicleUpdate();
    }

    private void PTranscriptHoldShow(bool running)
    {
        if (running == PTranscript.IsEnabled)
        {
            return;
        }

        PTranscript.IsEnabled = running;
        if (!running)
        {
            _pCorpusHost.PWindowFailureShow("Example.HoldFailed");
        }
    }

    private LDraft? PTranscriptDraftStart(long? example)
    {
        _lCorpus.LCorpusStart(example);
        if (!PTranscriptDesk.LDeskHeld)
        {
            PTranscript.IsEnabled = false;
            return null;
        }

        return PTranscriptDesk.LDeskRead();
    }

    private void PTranscriptDraftShow(LDraft? started)
    {
        PTranscriptApply(started?.LDraftExample);
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

    private void PTranscriptDraftCancel()
    {
        PTranscriptDesk.LDeskCancel();
    }

    private long? PTranscriptExampleRead()
    {
        LDraft? held;
        try
        {
            held = PTranscriptDesk.LDeskRead();
        }
        catch (Exception)
        {
            return null;
        }

        return held?.LDraftStored;
    }

    public void PChronicleUndo()
    {
        PTranscriptChronicleRun(PTranscriptDesk.LDeskUndo);
    }

    public void PChronicleRedo()
    {
        PTranscriptChronicleRun(PTranscriptDesk.LDeskRedo);
    }

    private void PTranscriptChronicleRun(Action step)
    {
        try
        {
            PChronicle.PChronicleRun(step);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.HoldFailed", exception);
        }

        PChronicleUpdate();
    }

    public void PChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorChronicleRead()
            : PTranscriptDesk.LDeskChronicleRead();
        PCorpusBackward.IsEnabled = undo;
        PCorpusForward.IsEnabled = redo;
    }

    private void PCorpusUndoHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PChronicleUndo();
            return;
        }

        PChronicleUndo();
    }

    private void PCorpusRedoHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PChronicleRedo();
            return;
        }

        PChronicleRedo();
    }
}
