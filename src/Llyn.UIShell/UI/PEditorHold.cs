using System;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private long _pEditorDraft;

    private bool _pEditorHalted;

    private LDraft? PEditorDraftStart(long? entry)
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
            _pEditorDraft = 0;
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
        if (_pEditorDraft == 0)
        {
            return;
        }

        long held = _pEditorDraft;
        _pEditorDraft = 0;

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
        if (_pEditorDraft == 0)
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
            PEditorIdentityApply(stored);
        }
        catch (Exception exception)
        {
            PEditorHoldSuspend(exception);
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

        _pEditorDraft = 0;

        try
        {
            _lEngine.LEngineDraftCommit(held);
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
        if (_pEditorDraft == 0)
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
