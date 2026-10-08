using System;
using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed class CSession
{
    private readonly CDesk _cSessionDesk;

    private readonly IReadOnlyList<Func<bool>> _cSessionPending;

    private readonly CEditor? _cSessionEditor;

    private readonly Func<bool> _cSessionShownSeam;

    private readonly Func<bool, bool> _cSessionFinishSeam;

    private readonly Func<bool> _cSessionReadySeam;

    private readonly Action<long> _cSessionStoredSeam;

    private readonly CEnvoy _cSessionEnvoy;

    internal CSession(
        CDesk desk,
        IReadOnlyList<Func<bool>> pending,
        CEditor? editor,
        Func<bool> shownSeam,
        Func<bool, bool> finishSeam,
        Func<bool> readySeam,
        Action<long> storedSeam,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(pending);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(finishSeam);
        ArgumentNullException.ThrowIfNull(readySeam);
        ArgumentNullException.ThrowIfNull(storedSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cSessionDesk = desk;
        _cSessionPending = pending;
        _cSessionEditor = editor;
        _cSessionShownSeam = shownSeam;
        _cSessionFinishSeam = finishSeam;
        _cSessionReadySeam = readySeam;
        _cSessionStoredSeam = storedSeam;
        _cSessionEnvoy = envoy;
        desk.CDeskStateChanged += LSessionStateUpdate;
        if (editor is not null)
        {
            editor.CEditorDesk.CDeskStateChanged += LSessionStateUpdate;
        }
    }

    public event Action? CSessionHeld;

    public event Action? CSessionChanged;

    private CDesk? CSessionEditorRead()
    {
        if (!_cSessionShownSeam())
        {
            return null;
        }

        return _cSessionEditor?.CEditorDesk;
    }

    public void CSessionStart(long? id)
    {
        _cSessionDesk.CDeskStart(id);
        CSessionHeld?.Invoke();
    }

    public void CSessionCancel()
    {
        _cSessionDesk.CDeskCancel();
        CSessionHeld?.Invoke();
    }

    internal bool LSessionFinish(bool store)
    {
        if (CSessionEditorRead() is not null)
        {
            return _cSessionFinishSeam(store);
        }

        if (!CSessionReadyCheck(store))
        {
            return false;
        }

        return _cSessionDesk.CDeskFinish(store, CSessionStoredShow);
    }

    public bool CSessionClose(bool store)
    {
        if (CSessionEditorRead() is not null)
        {
            return _cSessionFinishSeam(store);
        }

        return _cSessionDesk.CDeskFinish(store);
    }

    public bool CSessionSave()
    {
        if (CSessionEditorRead() is CDesk editor)
        {
            if (editor.LDeskChangeCheck())
            {
                _cSessionFinishSeam(true);
            }

            return true;
        }

        if (!_cSessionDesk.CDeskHeld)
        {
            return true;
        }

        if (!CSessionReadyCheck(true))
        {
            return false;
        }

        if (!_cSessionDesk.LDeskChangeCheck())
        {
            return true;
        }

        return _cSessionDesk.CDeskFinish(true, CSessionStoredShow);
    }

    private bool CSessionReadyCheck(bool store)
    {
        if (!store)
        {
            return true;
        }

        return _cSessionReadySeam();
    }

    private void CSessionStoredShow(long id)
    {
        _cSessionStoredSeam(id);
    }

    public (bool CDeskBackward, bool CDeskForward) CSessionChronicleRead()
    {
        return (CSessionEditorRead() ?? _cSessionDesk).CDeskChronicle.CDeskChronicleRead();
    }

    public void CSessionUndo()
    {
        if (CSessionEditorRead() is CDesk editor)
        {
            editor.CDeskChronicle.CDeskChronicleUndo();
            return;
        }

        _cSessionDesk.CDeskChronicle.CDeskChronicleUndo();
    }

    public void CSessionRedo()
    {
        if (CSessionEditorRead() is CDesk editor)
        {
            editor.CDeskChronicle.CDeskChronicleRedo();
            return;
        }

        _cSessionDesk.CDeskChronicle.CDeskChronicleRedo();
    }

    internal bool LSessionLeaveConfirm(bool shown)
    {
        if (!LSessionChangeCheck())
        {
            return true;
        }

        if (_cSessionEnvoy.CEnvoyLeaveConfirm() is not bool store)
        {
            return false;
        }

        if (!store)
        {
            return true;
        }

        return shown ? LSessionFinish(true) : CSessionClose(true);
    }

    internal void LSessionEditorClose()
    {
        _cSessionEditor?.CEditorClose();
        _cSessionEditor?.CEditorDisplay.CDisplayPlayback.CDisplayPlaybackCancel();
    }

    internal bool LSessionChangeCheck()
    {
        foreach (Func<bool> pending in _cSessionPending)
        {
            if (pending())
            {
                return true;
            }
        }

        return false;
    }

    private void LSessionStateUpdate()
    {
        CSessionChanged?.Invoke();
    }
}
