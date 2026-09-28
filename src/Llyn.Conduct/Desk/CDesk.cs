using System;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDesk
{
    private readonly LDraftPort _cDeskPort;

    private readonly string _cDeskScope;

    private readonly CEnvoy _cDeskEnvoy;

    private readonly string? _cDeskOrigin;

    private readonly CSubject _cDeskSubject;

    private LVista? _cDeskVista;

    private LTenure? _cDeskTenure;

    private LQuill? _cDeskQuill;

    private LEasel? _cDeskEasel;

    private bool _cDeskFilling;

    private bool _cDeskHalted;

    internal CDesk(LDraftPort drafts, string scope, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDeskPort = drafts;
        _cDeskScope = scope;
        _cDeskEnvoy = envoy;
        CDeskErrand = new CErrand(this);
        CDeskVigil = new LVigil(this);
    }

    internal CDesk(LDraftPort drafts, string scope, CEnvoy envoy, string origin, CSubject subject)
        : this(drafts, scope, envoy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);

        _cDeskOrigin = origin;
        _cDeskSubject = subject;
    }

    public event Action? CDeskStarted;

    internal event Action<LDraft>? CDeskDraftPrepared;

    public event Action<CDraft>? CDeskDraftChanged;

    public event Action? CDeskStateChanged;

    public event Action<long>? CDeskFinished;

    public event Action<string, Exception>? CDeskFailed;

    public event Action<string>? CDeskRefused;

    internal LQuill? CDeskQuill => CDeskFilling ? null : _cDeskQuill;

    internal LEasel? CDeskEasel => CDeskFilling ? null : _cDeskEasel;

    public CErrand CDeskErrand { get; }

    internal LVigil CDeskVigil { get; }

    public bool CDeskHeld => _cDeskTenure is not null;

    public bool CDeskFilling => _cDeskFilling;

    public long CDeskId => _cDeskTenure?.LTenureId ?? 0;

    public bool CDeskStored => CDeskStoredRead() is not null;

    public bool CDeskChanged => _cDeskTenure?.LTenureStateRead() is { LTenureStateChanged: true };

    public bool CDeskStorable => _cDeskTenure?.LTenureStorable ?? false;

    public bool CDeskHalted => _cDeskTenure?.LTenureStateRead() is { LTenureStateHalted: true };

    public bool CDeskRunning => CDeskHeld && !CDeskHalted;

    private bool CDeskStalling => CDeskHalted && !_cDeskHalted;

    internal string CDeskScope => _cDeskScope;

    internal LTenure? CDeskTenure => _cDeskTenure;

    internal LDraft? CDeskDraft => _cDeskTenure is LTenure held ? held.LTenureRead() : null;

    public void CDeskObserverAttach(Action<Action> marshal)
    {
        CDeskObserverAttach(marshal, CDeskDraftResonate);
    }

    public void CDeskObserverAttach(Action<Action> marshal, Action drafted)
    {
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentNullException.ThrowIfNull(drafted);

        CDeskVigil.LVigilDraftAttach(CSubject.CSubjectTenure, _ => marshal(CDeskStateResonate));
        CDeskVigil.LVigilDraftAttach(CSubject.CSubjectDraft, _ => marshal(drafted));
    }

    internal void CDeskVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _cDeskVista = vista;
    }

    public void CDeskStart(long? id)
    {
        CDeskCancel();
        if (_cDeskVista is LVista vista)
        {
            CDeskStartRun(() => _cDeskPort.LEngineTenureStart(vista, id));
            return;
        }

        if (_cDeskOrigin is string origin)
        {
            CDeskStartRun(() => _cDeskPort.LEngineTenureStart(origin, CPanel.CPanelSubjectRead(_cDeskSubject), id));
        }
    }

    internal void LDeskOccurrenceStart(long? situation)
    {
        CDeskCancel();
        if (_cDeskVista is LVista vista)
        {
            CDeskStartRun(() => _cDeskPort.LEngineOccurrenceStart(vista, situation));
        }
    }

    internal void LDeskQuotationStart(long? example)
    {
        CDeskCancel();
        if (_cDeskVista is LVista vista)
        {
            CDeskStartRun(() => _cDeskPort.LEngineQuotationStart(vista, example));
        }
    }

    internal void LDeskFootnoteStart(long? reference)
    {
        CDeskCancel();
        if (_cDeskVista is LVista vista)
        {
            CDeskStartRun(() => _cDeskPort.LEngineFootnoteStart(vista, reference));
        }
    }

    private void CDeskStartRun(Func<LTenure> start)
    {
        try
        {
            LTenure started = start();
            _cDeskTenure = started;
            _cDeskQuill = new LQuill(started);
            _cDeskEasel = new LEasel(started);
            _cDeskHalted = false;
            CDeskVigil.LVigilApply(started);
            CDeskStarted?.Invoke();
        }
        catch (Exception exception)
        {
            CDeskCancel();
            CDeskFailed?.Invoke(_cDeskScope + ".LoadFailed", exception);
            return;
        }

        CDeskDraftResonate();
        CDeskStateChanged?.Invoke();
    }

    internal LDraft? CDeskRead()
    {
        if (_cDeskTenure is not LTenure held)
        {
            return null;
        }

        held.LTenurePersist();
        return held.LTenureRead();
    }

    public long? CDeskStoredRead()
    {
        try
        {
            return CDeskRead()?.LDraftStored;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void CDeskDraftResonate()
    {
        if (CDeskFilling)
        {
            return;
        }

        try
        {
            CDeskDraftShow(CDeskPrepare());
        }
        catch (Exception exception)
        {
            CDeskFailed?.Invoke(_cDeskScope + ".LoadFailed", exception);
        }
    }

    private LDraft? CDeskPrepare()
    {
        if (_cDeskTenure is not LTenure held)
        {
            return null;
        }

        held.LTenurePersist();
        return held.LTenurePrepare();
    }

    private void CDeskDraftShow(LDraft? draft)
    {
        if (draft is null)
        {
            return;
        }

        _cDeskFilling = true;
        try
        {
            CDeskDraftPrepared?.Invoke(draft);
            CDeskDraftChanged?.Invoke(new CDraft(draft.LDraftAuthorName));
        }
        finally
        {
            _cDeskFilling = false;
        }
    }

    public void CDeskStateResonate()
    {
        if (CDeskStalling)
        {
            CDeskRefused?.Invoke(_cDeskScope + ".HoldFailed");
        }

        _cDeskHalted = CDeskHalted;
        CDeskStateChanged?.Invoke();
    }

    public void CDeskPersist()
    {
        if (CDeskFilling)
        {
            return;
        }

        _cDeskTenure?.LTenurePersist();
    }

    internal void CDeskDefer(LRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (CDeskFilling)
        {
            return;
        }

        _cDeskTenure?.LTenureRequestDefer(request);
    }

    internal void CDeskSend(LRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (CDeskFilling)
        {
            return;
        }

        _cDeskTenure?.LTenureRequestApply(request);
    }

    public bool CDeskChangeCheck()
    {
        return _cDeskTenure?.LTenureChangeCheck() ?? false;
    }

    public (bool CDeskBackward, bool CDeskForward) CDeskChronicleRead()
    {
        if (_cDeskTenure is not LTenure held)
        {
            return (false, false);
        }

        LTenureState state = held.LTenureStateRead();
        return (state.LTenureStateBackward, state.LTenureStateForward);
    }

    public void CDeskUndo()
    {
        _cDeskTenure?.LTenureUndo();
        CDeskStateChanged?.Invoke();
    }

    public void CDeskRedo()
    {
        _cDeskTenure?.LTenureRedo();
        CDeskStateChanged?.Invoke();
    }

    public bool CDeskFinish(bool store)
    {
        return CDeskFinish(store, id => CDeskFinished?.Invoke(id));
    }

    public bool CDeskFinish(bool store, Action<long> stored)
    {
        ArgumentNullException.ThrowIfNull(stored);

        if (_cDeskTenure is not LTenure held)
        {
            return true;
        }

        long? kept;
        try
        {
            kept = held.LTenureFinish(store, CDeskUnreadableConfirm);
        }
        catch (Exception exception)
        {
            CDeskFailed?.Invoke(_cDeskScope + ".SaveFailed", exception);
            return false;
        }

        _cDeskTenure = null;
        _cDeskQuill = null;
        _cDeskEasel = null;
        CDeskStateChanged?.Invoke();
        if (kept is long id)
        {
            stored(id);
        }

        return true;
    }

    private bool CDeskUnreadableConfirm()
    {
        return _cDeskEnvoy.CEnvoyConfirm("Notice.UnreadableDrop");
    }

    public void CDeskCancel()
    {
        if (_cDeskTenure is not LTenure held)
        {
            return;
        }

        _cDeskTenure = null;
        _cDeskQuill = null;
        _cDeskEasel = null;
        held.LTenureCancel();
        CDeskStateChanged?.Invoke();
    }
}
