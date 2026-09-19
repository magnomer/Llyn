using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LDesk
{
    private readonly LEngine _lEngine;

    private readonly string _lDeskScope;

    private readonly Func<bool> _lDeskUnreadableSeam;

    private LVista? _lDeskVista;

    private LTenure? _lDeskTenure;

    private bool _lDeskFilling;

    public LDesk(LEngine engine, string scope, Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(engine);
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentNullException.ThrowIfNull(unreadableSeam);

        _lEngine = engine;
        _lDeskScope = scope;
        _lDeskUnreadableSeam = unreadableSeam;
    }

    public event Action<LTenure>? LDeskStarted;

    public event Action<LDraft>? LDeskDraftChanged;

    public event Action? LDeskStateChanged;

    public event Action<long>? LDeskFinished;

    public event Action<string, Exception>? LDeskFailed;

    public bool LDeskHeld => _lDeskTenure is not null;

    public bool LDeskFilling => _lDeskFilling;

    public long LDeskId => _lDeskTenure?.LTenureId ?? 0;

    public bool LDeskStored => LDeskRead()?.LDraftStored is not null;

    public void LDeskVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lDeskVista = vista;
    }

    public void LDeskStart(long? id)
    {
        LDeskCancel();
        if (_lDeskVista is not LVista vista)
        {
            return;
        }

        try
        {
            LTenure started = _lEngine.LEngineTenureStart(vista, id);
            _lDeskTenure = started;
            LDeskStarted?.Invoke(started);
        }
        catch (Exception exception)
        {
            LDeskCancel();
            LDeskFailed?.Invoke(_lDeskScope + ".LoadFailed", exception);
            return;
        }

        LDeskDraftUpdate();
        LDeskStateChanged?.Invoke();
    }

    public LDraft? LDeskRead()
    {
        if (_lDeskTenure is not LTenure held)
        {
            return null;
        }

        held.LTenurePersist();
        return held.LTenureRead();
    }

    public void LDeskDraftUpdate()
    {
        if (LDeskFilling)
        {
            return;
        }

        try
        {
            LDeskDraftShow(LDeskRead());
        }
        catch (Exception exception)
        {
            LDeskFailed?.Invoke(_lDeskScope + ".LoadFailed", exception);
        }
    }

    private void LDeskDraftShow(LDraft? draft)
    {
        if (draft is null)
        {
            return;
        }

        _lDeskFilling = true;
        try
        {
            LDeskDraftChanged?.Invoke(draft);
        }
        finally
        {
            _lDeskFilling = false;
        }
    }

    public void LDeskStateUpdate()
    {
        LDeskStateChanged?.Invoke();
    }

    public void LDeskDefer(LRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (LDeskFilling)
        {
            return;
        }

        _lDeskTenure?.LTenureRequestDefer(request);
    }

    public void LDeskSend(LRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (LDeskFilling)
        {
            return;
        }

        _lDeskTenure?.LTenureRequestApply(request);
    }

    public bool LDeskChangeCheck()
    {
        if (_lDeskTenure is not LTenure held)
        {
            return false;
        }

        held.LTenurePersist();
        return held.LTenureStateRead().LTenureStateChanged;
    }

    public (bool LDeskBackward, bool LDeskForward) LDeskChronicleRead()
    {
        if (_lDeskTenure is not LTenure held)
        {
            return (false, false);
        }

        return LDeskChronicleRead(held.LTenureStateRead());
    }

    private static (bool LDeskBackward, bool LDeskForward) LDeskChronicleRead(LTenureState state)
    {
        return (state.LTenureStateBackward, state.LTenureStateForward);
    }

    public void LDeskUndo()
    {
        _lDeskTenure?.LTenureUndo();
        LDeskStateChanged?.Invoke();
    }

    public void LDeskRedo()
    {
        _lDeskTenure?.LTenureRedo();
        LDeskStateChanged?.Invoke();
    }

    public bool LDeskFinish(bool store)
    {
        if (_lDeskTenure is not LTenure held)
        {
            return true;
        }

        long? stored;
        try
        {
            stored = LDeskCommitRun(held, store);
        }
        catch (Exception exception)
        {
            LDeskFailed?.Invoke(_lDeskScope + ".SaveFailed", exception);
            return false;
        }

        _lDeskTenure = null;
        LDeskStateChanged?.Invoke();
        if (stored is long id)
        {
            LDeskFinished?.Invoke(id);
        }

        return true;
    }

    private long? LDeskCommitRun(LTenure held, bool store)
    {
        try
        {
            return held.LTenureFinish(store);
        }
        catch (LRefusal refusal) when (refusal.LRefusalIllegible)
        {
            if (!LDeskUnreadableConfirm())
            {
                throw;
            }

            held.LTenureSweep();
            return held.LTenureFinish(store);
        }
    }

    private bool LDeskUnreadableConfirm()
    {
        return _lDeskUnreadableSeam();
    }

    public void LDeskCancel()
    {
        if (_lDeskTenure is not LTenure held)
        {
            return;
        }

        _lDeskTenure = null;
        held.LTenureCancel();
        LDeskStateChanged?.Invoke();
    }
}
