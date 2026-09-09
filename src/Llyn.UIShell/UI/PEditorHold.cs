using System;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private string _pEditorDraft = string.Empty;

    private bool _pEditorHalted;

    private LDraft? PEditorDraftStart(string? entry)
    {
        PEditorChangeStop();
        PEditorDraftCancel();

        try
        {
            LDraft started = _lEngine.LEngineDraftStart(_pEditorOrigin, entry);
            _pEditorDraft = started.LDraftId;
            PEditorHoldResume();
            return started;
        }
        catch (Exception exception)
        {
            _pEditorDraft = string.Empty;
            PEditorHoldSuspend(exception);
            return null;
        }
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
        if (_pEditorDraft.Length == 0)
        {
            return;
        }

        string held = _pEditorDraft;
        _pEditorDraft = string.Empty;

        try
        {
            _lEngine.LEngineDraftCancel(held);
        }
        catch (Exception)
        {
        }
    }

    private void PEditorDraftSave()
    {
        if (_pEditorDraft.Length == 0)
        {
            return;
        }

        try
        {
            LDraft? held = _lEngine.LEngineDraftRead(_pEditorDraft);
            if (held is null)
            {
                return;
            }

            LEntryDraft sent = PEditorDraftRead();
            LEntryDraft stored = _lEngine.LEngineDraftSave(held with { LDraftContent = sent });

            if (!ReferenceEquals(stored, sent))
            {
                PEditorDraftShow(stored);
            }
        }
        catch (Exception exception)
        {
            PEditorHoldSuspend(exception);
        }
    }

    private void PEditorDraftRestore()
    {
        if (_pEditorDraft.Length == 0)
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

        string held = _pEditorDraft;
        if (held.Length == 0)
        {
            return true;
        }

        _pEditorDraft = string.Empty;

        try
        {
            _lEngine.LEngineDraftCommit(held);
        }
        catch (Exception exception)
        {
            _pEditorDraft = held;
            string? entry = PEditorEntryRead();
            _pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);
            return false;
        }

        return true;
    }

    private bool PEditorDraftCheck()
    {
        if (_pEditorDraft.Length == 0)
        {
            return false;
        }

        try
        {
            return _lEngine.LEngineDraftCheck(_pEditorDraft);
        }
        catch (Exception exception)
        {
            PEditorHoldSuspend(exception);
            return false;
        }
    }
}
