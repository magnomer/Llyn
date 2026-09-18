using System;
using System.Windows;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private const string PTranscriptOrigin = "Corpus";

    private LTenure? _pTranscriptTenure;

    private long PTranscriptDraft => _pTranscriptTenure?.LTenureId ?? 0;

    private bool PTranscriptDraftFinish(bool store)
    {
        if (_pTranscriptTenure is not LTenure held)
        {
            return true;
        }

        try
        {
            _pCorpusHost.PWindowCommitRun(held, store);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.SaveFailed", exception);
            return false;
        }

        _pTranscriptTenure = null;
        return true;
    }

    private bool PTranscriptChangeCheck()
    {
        if (_pTranscriptTenure is not LTenure held)
        {
            return false;
        }

        held.LTenurePersist();
        return held.LTenureStateRead().LTenureStateChanged;
    }

    private void PTranscriptChangeDefer()
    {
        PTranscriptRequestDefer(PTranscriptRead(PTranscriptDraft));
    }

    private void PTranscriptRequestDefer(LRequest request)
    {
        if (_pTranscriptLoading || _pTranscriptTenure is not LTenure held)
        {
            return;
        }

        held.LTenureRequestDefer(request);
    }

    private void PTranscriptRequestSend(LRequest request)
    {
        if (_pTranscriptTenure is not LTenure held)
        {
            return;
        }

        held.LTenureRequestApply(request);
    }

    private void PTranscriptChangeUpdate()
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            return;
        }

        LTenureState? state = _pTranscriptTenure?.LTenureStateRead();
        PCorpusStore.IsEnabled = state is { LTenureStateChanged: true };
        if (state is not null)
        {
            PTranscriptHoldShow(!state.LTenureStateHalted);
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
        PTranscriptDraftCancel();

        try
        {
            LTenure started = _lEngine.LEngineTenureStart(PTranscriptOrigin, LSubject.LSubjectExample, example);
            _pTranscriptTenure = started;
            started.LTenureDraftAttach(LSubject.LSubjectDraft, new PObserver(this, PTranscriptDraftRestore));
            started.LTenureDraftAttach(LSubject.LSubjectTenure, new PObserver(this, PTranscriptChangeUpdate));
            PTranscript.IsEnabled = true;
            return started.LTenureRead();
        }
        catch (Exception exception)
        {
            PTranscriptDraftCancel();
            PTranscript.IsEnabled = false;
            _pCorpusHost.PWindowFailureShow("Example.HoldFailed", exception);
            return null;
        }
    }

    private void PTranscriptDraftShow(LDraft? started)
    {
        PTranscriptApply(started?.LDraftExample);
    }

    private void PTranscriptDraftRestore()
    {
        if (_pTranscriptLoading || _pTranscriptTenure is not LTenure held)
        {
            return;
        }

        try
        {
            held.LTenurePersist();
            if (held.LTenureRead()?.LDraftExample is LExample sentence)
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
        if (_pTranscriptTenure is not LTenure held)
        {
            return;
        }

        _pTranscriptTenure = null;
        held.LTenureCancel();
    }

    private long? PTranscriptExampleRead()
    {
        LDraft? held;
        try
        {
            held = _pTranscriptTenure?.LTenureRead();
        }
        catch (Exception)
        {
            return null;
        }

        return held?.LDraftEntryId is null or 0 ? null : held.LDraftEntryId;
    }

    public void PChronicleUndo()
    {
        PTranscriptChronicleRun(static held => held.LTenureUndo());
    }

    public void PChronicleRedo()
    {
        PTranscriptChronicleRun(static held => held.LTenureRedo());
    }

    private void PTranscriptChronicleRun(Func<LTenure, LDraft?> step)
    {
        if (_pTranscriptTenure is not LTenure held)
        {
            return;
        }

        try
        {
            PChronicle.PChronicleRun(() => step(held));
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
            : PTranscriptChronicleRead();
        PCorpusBackward.IsEnabled = undo;
        PCorpusForward.IsEnabled = redo;
    }

    private (bool PTranscriptBackward, bool PTranscriptForward) PTranscriptChronicleRead()
    {
        return _pTranscriptTenure?.LTenureStateRead() is LTenureState state
            ? (state.LTenureStateBackward, state.LTenureStateForward)
            : (false, false);
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
