using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDesk
{
    private readonly LDraftPort _cDeskPort;

    private readonly string _cDeskScope;

    private readonly CEnvoy _cDeskEnvoy;

    private readonly LSettingsPort _cDeskSettings;

    private readonly string? _cDeskOrigin;

    private readonly CSubject _cDeskSubject;

    private LVista? _cDeskVista;

    internal CDesk(LDraftPort drafts, LSettingsPort settings, string scope, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDeskPort = drafts;
        _cDeskSettings = settings;
        _cDeskScope = scope;
        _cDeskEnvoy = envoy;
        CDeskDraft = new CDeskDraft(drafts, settings, scope, envoy);
        CDeskChronicle = new CDeskChronicle(CDeskDraft, settings, scope, envoy);
        CDeskChronicle.CDeskChronicleChanged += () => CDeskStateChanged?.Invoke();
        CDeskErrand = new CErrand(this);
        CDeskVigil = new LVigil(this);
    }

    internal CDesk(
        LDraftPort drafts, LSettingsPort settings, string scope, CEnvoy envoy, string origin, CSubject subject)
        : this(drafts, settings, scope, envoy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);

        _cDeskOrigin = origin;
        _cDeskSubject = subject;
    }

    public event Action? CDeskStarted;

    public event Action? CDeskStateChanged;

    public event Action<long>? CDeskFinished;

    public CDeskDraft CDeskDraft { get; }

    public CDeskChronicle CDeskChronicle { get; }

    public CErrand CDeskErrand { get; }

    internal LVigil CDeskVigil { get; }

    public bool CDeskHeld => CDeskDraft.CDeskDraftTenure is not null;

    public long CDeskId => CDeskDraft.CDeskDraftTenure?.LTenureId ?? 0;

    public bool CDeskStored => CDeskStoredRead() is not null;

    public void CDeskObserverAttach(Action<Action> marshal)
    {
        CDeskObserverAttach(marshal, CDeskDraft.CDeskDraftResonate);
    }

    public void CDeskObserverAttach(Action<Action> marshal, Action drafted)
    {
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentNullException.ThrowIfNull(drafted);

        CDeskVigil.LVigilDraftAttach(CSubject.CSubjectTenure, _ => marshal(CDeskChronicle.CDeskChronicleResonate));
        CDeskVigil.LVigilDraftAttach(CSubject.CSubjectDraft, _ => marshal(drafted));
        CDeskErrand.LErrandObserverAttach(marshal);
    }

    internal void CDeskVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _cDeskVista = vista;
    }

    public void CDeskStart(long? id)
    {
        bool dropped = LDeskTenureClear();
        if (_cDeskVista is LVista vista)
        {
            CDeskStartRun(() => _cDeskPort.LEngineTenureStart(vista, id));
            return;
        }

        if (_cDeskOrigin is string origin)
        {
            CDeskStartRun(() => _cDeskPort.LEngineTenureStart(origin, CCatalog.LCatalogSubjectRead(_cDeskSubject), id));
            return;
        }

        if (dropped)
        {
            CDeskStateChanged?.Invoke();
        }
    }

    internal void LDeskRun(Func<LDraftPort, LVista, LTenure> start)
    {
        ArgumentNullException.ThrowIfNull(start);

        bool dropped = LDeskTenureClear();
        if (_cDeskVista is LVista vista)
        {
            CDeskStartRun(() => start(_cDeskPort, vista));
            return;
        }

        if (dropped)
        {
            CDeskStateChanged?.Invoke();
        }
    }

    private void CDeskStartRun(Func<LTenure> start)
    {
        try
        {
            LTenure started = start();
            CDeskDraft.LDeskDraftSet(started);
            CDeskChronicle.LDeskChronicleClear();
            CDeskVigil.LVigilApply(started);
            CDeskStarted?.Invoke();
        }
        catch (Exception exception)
        {
            LDeskTenureClear();
            CDeskStateChanged?.Invoke();
            CLedger.LLedgerFailureShow(_cDeskEnvoy, _cDeskSettings, _cDeskScope + ".LoadFailed", exception);
            return;
        }

        CDeskDraft.CDeskDraftResonate();
        CDeskStateChanged?.Invoke();
    }

    public long? CDeskStoredRead()
    {
        return CDeskDraft.CDeskDraftTenure?.LTenureStoredRead();
    }

    internal bool LDeskChangeCheck()
    {
        return CDeskDraft.CDeskDraftTenure?.LTenureChangeCheck() ?? false;
    }

    internal bool LDeskReadyCheck()
    {
        return CDeskDraft.CDeskDraftTenure?.LTenureReadyCheck() ?? false;
    }

    public bool CDeskFinish(bool store)
    {
        return CDeskFinish(store, id => CDeskFinished?.Invoke(id));
    }

    public bool CDeskFinish(bool store, Action<long> stored)
    {
        ArgumentNullException.ThrowIfNull(stored);

        if (CDeskDraft.CDeskDraftTenure is not LTenure held)
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
            CLedger.LLedgerFailureShow(_cDeskEnvoy, _cDeskSettings, _cDeskScope + ".SaveFailed", exception);
            return false;
        }

        CDeskDraft.LDeskDraftClear();
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

    internal void LDeskFailureShow(string key, Exception exception)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(exception);

        CLedger.LLedgerFailureShow(_cDeskEnvoy, _cDeskSettings, _cDeskScope + key, exception);
    }

    internal void LDeskRepaintShow(CLedgerNoticed noticed, string key, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(noticed);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(exception);

        noticed.LLedgerRepaintShow(_cDeskEnvoy, _cDeskSettings, _cDeskScope + key, exception);
    }

    public void CDeskCancel()
    {
        if (LDeskTenureClear())
        {
            CDeskStateChanged?.Invoke();
        }
    }

    private bool LDeskTenureClear()
    {
        if (CDeskDraft.CDeskDraftTenure is not LTenure held)
        {
            return false;
        }

        CDeskDraft.LDeskDraftClear();
        held.LTenureCancel();
        return true;
    }
}
