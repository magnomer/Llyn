using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private long _pEditorDraft;

    private bool _pEditorHalted;

    internal Action? PEditorChronicleNotice;

    private LDraft? PEditorDraftStart(long? entry)
    {
        PEditorChangeStop();
        PEditorDraftCancel();
        _pMeaningList.Clear();
        _pCollocationList.Clear();
        _pRecordingFresh.Clear();

        try
        {
            LDraft started = _lEngine.LEngineDraftStart(_pEditorOrigin, entry);
            _pEditorDraft = started.LDraftId;
            PEditorHoldResume();
            return started;
        }
        catch (Exception exception)
        {
            _pEditorDraft = 0;
            PEditorHoldSuspend(exception);
            return null;
        }
    }

    public void PChronicleUndo()
    {
        PEditorChronicleRun(_lEngine.LEngineChronicleUndo);
    }

    public void PChronicleRedo()
    {
        PEditorChronicleRun(_lEngine.LEngineChronicleRedo);
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
        return (
            _pEditorDraft != 0 && _lEngine.LEngineUndoCheck(_pEditorDraft),
            _pEditorDraft != 0 && _lEngine.LEngineRedoCheck(_pEditorDraft));
    }

    private void PEditorChronicleRun(Func<long, LDraft?> step)
    {
        if (_pEditorDraft == 0)
        {
            return;
        }

        PEditorChangeSave();

        try
        {
            PChronicle.PChronicleRun(() => step(_pEditorDraft));
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Input.HoldFailed", exception);
        }

        PChronicleUpdate();
    }

    private void PEditorHoldSuspend(Exception exception)
    {
        if (_pEditorHalted)
        {
            return;
        }

        _pEditorHalted = true;
        IsEnabled = false;
        _pEditorHost.PWindowFailureShow("Input.HoldFailed", exception);
    }

    private void PEditorHoldResume()
    {
        if (!_pEditorHalted)
        {
            return;
        }

        _pEditorHalted = false;
        IsEnabled = true;
    }

    private void PEditorDraftCancel()
    {
        if (_pEditorDraft == 0)
        {
            return;
        }

        long held = _pEditorDraft;
        _pEditorDraft = 0;
        _pEditorRequestPending.Clear();

        try
        {
            _lEngine.LEngineDraftCancel(held);
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Input.HoldFailed", exception);
        }
    }

    private void PEditorDraftRestore()
    {
        if (_pEditorDraft == 0)
        {
            return;
        }

        try
        {
            if (_lEngine.LEngineDraftRead(_pEditorDraft) is LDraft held)
            {
                PEditorDraftShow(held.LDraftContent);
            }
        }
        catch (Exception exception)
        {
            PEditorHoldSuspend(exception);
        }
    }

    internal bool PEditorDraftFinish(bool store)
    {
        if (_pEditorPending is not null)
        {
            PEditorChangeSave();
        }

        if (!store || !PEditorDraftCheck())
        {
            PEditorDraftCancel();
            return true;
        }

        long held = _pEditorDraft;
        if (held == 0)
        {
            return true;
        }

        if (_pEditorHalted)
        {
            return false;
        }

        _pEditorDraft = 0;

        try
        {
            _pEditorHost.PWindowCommitRun(held, _lEngine.LEngineDraftCommit);
        }
        catch (Exception exception)
        {
            _pEditorDraft = held;
            long? entry = PEditorEntryRead();
            _pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);
            return false;
        }

        return true;
    }

    private bool PEditorDraftCheck()
    {
        return PEditorDraftCheck(out _);
    }

    private bool PEditorDraftCheck(out string? refusal)
    {
        refusal = null;
        if (_pEditorDraft == 0)
        {
            return false;
        }

        try
        {
            return _lEngine.LEngineDraftCheck(_pEditorDraft, out refusal);
        }
        catch (Exception exception)
        {
            PEditorHoldSuspend(exception);
            return false;
        }
    }
}
