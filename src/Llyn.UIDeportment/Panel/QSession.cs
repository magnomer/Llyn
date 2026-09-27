using System;
using System.Collections.Generic;

namespace Llyn.UIDeportment;

public sealed class QSession
{
    private readonly string _qSessionHold;

    private readonly Action<long?> _qSessionStartSeam;

    private readonly Func<bool> _qSessionReadySeam;

    private readonly Action<long> _qSessionStoredSeam;

    internal QSession(
        LDesk desk,
        IReadOnlyList<LPanel> panels,
        LEditor? editor,
        LPanel? scribe,
        string hold,
        Action<long?> startSeam,
        Func<bool> readySeam,
        Action<long> storedSeam)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(panels);
        ArgumentException.ThrowIfNullOrWhiteSpace(hold);
        ArgumentNullException.ThrowIfNull(startSeam);
        ArgumentNullException.ThrowIfNull(readySeam);
        ArgumentNullException.ThrowIfNull(storedSeam);

        QSessionDesk = desk;
        QSessionPanel = panels;
        QSessionEditor = editor;
        QSessionScribe = scribe;
        _qSessionHold = hold;
        _qSessionStartSeam = startSeam;
        _qSessionReadySeam = readySeam;
        _qSessionStoredSeam = storedSeam;
        desk.LDeskStateChanged += QSessionStateUpdate;
        if (editor is not null)
        {
            editor.LEditorStateChanged += QSessionStateUpdate;
        }
    }

    public event Action? QSessionHeld;

    public event Action? QSessionChanged;

    public event Action<string, Exception>? QSessionFailed;

    private LDesk QSessionDesk { get; }

    private IReadOnlyList<LPanel> QSessionPanel { get; }

    private LEditor? QSessionEditor { get; }

    private LPanel? QSessionScribe { get; }

    private bool QSessionEditorShown => QSessionScribe?.LPanelEditing ?? false;

    private LEditor? QSessionEditorRead()
    {
        if (!QSessionEditorShown)
        {
            return null;
        }

        return QSessionEditor;
    }

    public void QSessionStart(long? id)
    {
        _qSessionStartSeam(id);
        QSessionHeld?.Invoke();
    }

    public void QSessionCancel()
    {
        QSessionDesk.LDeskCancel();
        QSessionHeld?.Invoke();
    }

    public bool QSessionFinish(bool store)
    {
        if (QSessionEditorRead() is LEditor editor)
        {
            return editor.LEditorFinish(store);
        }

        if (!QSessionReadyCheck(store))
        {
            return false;
        }

        return QSessionDesk.LDeskFinish(store, QSessionStoredShow);
    }

    public bool QSessionClose(bool store)
    {
        if (QSessionEditorRead() is LEditor editor)
        {
            return editor.LEditorFinish(store);
        }

        return QSessionDesk.LDeskFinish(store);
    }

    public bool QSessionSave()
    {
        if (QSessionEditorRead() is LEditor editor)
        {
            editor.LEditorSave();
            return true;
        }

        if (!QSessionDesk.LDeskHeld)
        {
            return true;
        }

        if (!QSessionReadyCheck(true))
        {
            return false;
        }

        if (!QSessionDesk.LDeskChangeCheck())
        {
            return true;
        }

        return QSessionDesk.LDeskFinish(true, QSessionStoredShow);
    }

    private bool QSessionReadyCheck(bool store)
    {
        if (!store)
        {
            return true;
        }

        return _qSessionReadySeam();
    }

    private void QSessionStoredShow(long id)
    {
        _qSessionStoredSeam(id);
    }

    public (bool LDeskBackward, bool LDeskForward) QSessionChronicleRead()
    {
        if (QSessionEditorRead() is LEditor editor)
        {
            return editor.LEditorDesk.LDeskChronicleRead();
        }

        return QSessionDesk.LDeskChronicleRead();
    }

    public void QSessionUndo()
    {
        if (QSessionEditorRead() is LEditor editor)
        {
            editor.LEditorDesk.LDeskUndo();
            return;
        }

        QSessionChronicleRun(QSessionDesk.LDeskUndo);
    }

    public void QSessionRedo()
    {
        if (QSessionEditorRead() is LEditor editor)
        {
            editor.LEditorDesk.LDeskRedo();
            return;
        }

        QSessionChronicleRun(QSessionDesk.LDeskRedo);
    }

    private void QSessionChronicleRun(Action step)
    {
        try
        {
            step();
        }
        catch (Exception exception)
        {
            QSessionFailed?.Invoke(_qSessionHold, exception);
        }
    }

    public bool QSessionChangeCheck()
    {
        foreach (LPanel panel in QSessionPanel)
        {
            if (panel.LPanelChangeCheck())
            {
                return true;
            }
        }

        return false;
    }

    private void QSessionStateUpdate()
    {
        QSessionChanged?.Invoke();
    }
}
