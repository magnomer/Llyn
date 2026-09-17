using System;
using System.Windows;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PEditor
{
    private LTenure? _pEditorTenure;

    internal Action? PEditorChronicleNotice;

    private long PEditorDraft => _pEditorTenure?.LTenureId ?? 0;

    private LDraft? PEditorDraftStart(long? entry)
    {
        PEditorDraftCancel();
        _pMeaningList.Clear();
        _pCollocationList.Clear();

        try
        {
            LTenure started = _lEngine.LEngineTenureStart(_pEditorOrigin, LSubject.LSubjectEntry, entry);
            _pEditorTenure = started;
            IsEnabled = true;
            return started.LTenureRead();
        }
        catch (Exception exception)
        {
            _pEditorTenure = null;
            IsEnabled = false;
            _pEditorHost.PWindowFailureShow("Input.HoldFailed", exception);
            return null;
        }
    }

    public void PChronicleUndo()
    {
        PEditorChronicleRun(static held => held.LTenureUndo());
    }

    public void PChronicleRedo()
    {
        PEditorChronicleRun(static held => held.LTenureRedo());
    }

    private void PEditorUndoHandle(object sender, RoutedEventArgs e)
    {
        PChronicleUndo();
    }

    private void PEditorRedoHandle(object sender, RoutedEventArgs e)
    {
        PChronicleRedo();
    }

    public void PChronicleUpdate()
    {
        (bool undo, bool redo) = PEditorChronicleRead();
        PEditorBackward.IsEnabled = undo;
        PEditorForward.IsEnabled = redo;
        PEditorChronicleNotice?.Invoke();
    }

    internal (bool PEditorPast, bool PEditorFuture) PEditorChronicleRead()
    {
        return _pEditorTenure?.LTenureStateRead() is LTenureState state
            ? (state.LTenureStateBackward, state.LTenureStateForward)
            : (false, false);
    }

    private void PEditorChronicleRun(Func<LTenure, LDraft?> step)
    {
        if (_pEditorTenure is not LTenure held)
        {
            return;
        }

        try
        {
            PChronicle.PChronicleRun(() => step(held));
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Input.HoldFailed", exception);
        }

        PChronicleUpdate();
    }

    private void PEditorHoldShow(bool running)
    {
        if (running == IsEnabled)
        {
            return;
        }

        IsEnabled = running;
        if (!running)
        {
            _pEditorHost.PWindowFailureShow("Input.HoldFailed");
        }
    }

    private void PEditorDraftCancel()
    {
        if (_pEditorTenure is not LTenure held)
        {
            return;
        }

        _pEditorTenure = null;
        held.LTenureCancel();
    }

    private void PEditorDraftRestore()
    {
        if (_pEditorTenure is not LTenure held)
        {
            return;
        }

        try
        {
            held.LTenurePersist();
            if (held.LTenureRead() is LDraft draft)
            {
                PEditorDraftShow(draft.LDraftContent);
            }
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Input.HoldFailed", exception);
        }
    }

    internal bool PEditorDraftFinish(bool store)
    {
        if (_pEditorTenure is not LTenure held)
        {
            return true;
        }

        try
        {
            _pEditorHost.PWindowCommitRun(held, store);
        }
        catch (Exception exception)
        {
            long? entry = PEditorEntryRead();
            _pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);
            return false;
        }

        _pEditorTenure = null;
        return true;
    }
}
