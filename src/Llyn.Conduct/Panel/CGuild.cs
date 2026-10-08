using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CGuild
{
    private readonly CAtelier _cGuildAtelier;

    private readonly CEnvoy _cGuildEnvoy;

    private readonly LAuthorPort _cGuildAuthorPort;

    private readonly LPortraitPort _cGuildPortraitPort;

    private readonly LSettingsPort _cGuildSettingsPort;

    private readonly Action<Action> _cGuildMarshal;

    private LVista? _cGuildVista;

    private CGuild(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cGuildAtelier = atelier;
        _cGuildEnvoy = envoy;
        _cGuildAuthorPort = atelier.CAtelierEntryBundle.CEntryBundleAuthor;
        _cGuildPortraitPort = atelier.CAtelierPortraitPort;
        _cGuildSettingsPort = atelier.CAtelierSettingsPort;
        _cGuildMarshal = marshal;
        CGuildAutograph = new CDesk(atelier.CAtelierDraftPort, atelier.CAtelierSettingsPort, "Guild", envoy);
        CGuildPanel = new CPanel(
            envoy,
            _cGuildSettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            "Guild.LoadFailed", "Guild",
            CGuildAutograph.LDeskChangeCheck, store => CGuildSession!.LSessionFinish(store), shownSeam);
        CGuildOeuvre = new COeuvre(
            atelier.CAtelierEntryBundle.CEntryBundleEntry,
            atelier.CAtelierEntryBundle.CEntryBundleAuthor,
            atelier.CAtelierEntryBundle.CEntryBundleReference,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            envoy,
            shownSeam);
        CGuildPanel.CPanelRowsChanged += CGuildOeuvre.COeuvrePanel.CPanelRowsResonate;
        CGuildSession = new CSession(
            CGuildAutograph, [CGuildPanel.LPanelChangeCheck], null, static () => false, static _ => true,
            LGuildAutographCheck, LGuildStoredShow);
        CGuildSession.CSessionChanged += () => CGuildChanged?.Invoke();
        CGuildPanel.CPanelCleared += CGuildSession.CSessionCancel;
        CGuildPanel.CPanelEdited += id => CGuildSession.CSessionStart(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Guild", LGuildLeaveConfirm, CGuildPanel.LPanelChosenRead, LGuildScribeRestore, LGuildAuthorOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CGuildSession.LSessionChangeCheck, CGuildSession.LSessionFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LGuildVistaRestore);
        CGuildAutograph.CDeskStarted += () => CGuildUnionCleared?.Invoke();
        CGuildAutograph.CDeskObserverAttach(marshal);
        LGuildVistaRestore();
    }

    public static CGuild CGuildCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CGuild(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CGuildChanged;

    public event Action? CGuildUnionCleared;

    public CPanel CGuildPanel { get; }

    public COeuvre CGuildOeuvre { get; }

    public CDesk CGuildAutograph { get; }

    public CSession CGuildSession { get; }

    public bool CGuildColophonShown => LGuildSourceSide;

    public bool CGuildAutographShown => !LGuildSourceSide && CGuildPanel.CPanelEditing;

    public bool CGuildVitaShown => !LGuildSourceSide && !CGuildPanel.CPanelEditing;

    public bool CGuildVitaHeld => LGuildAuthorHeld && !CGuildPanel.CPanelEditing;

    public bool CGuildViewerChecked => !CGuildPanel.CPanelEditing;

    public bool CGuildScribeChecked => CGuildPanel.CPanelEditing;

    public bool CGuildModeEnabled => !LGuildSourceSide && LGuildAuthorShown;

    public bool CGuildBinEnabled => !LGuildSourceSide && LGuildAuthorHeld;

    public bool CGuildStoreEnabled =>
        CGuildAutographShown && CGuildAutograph.CDeskStorable;

    public bool CGuildPressAllowed => LGuildSourceSide;

    public bool CGuildFiltered => _cGuildVista?.LVistaFiltered ?? false;

    public bool CGuildUnionShown => CGuildAutograph.CDeskStored;

    private bool LGuildSourceSide => CGuildOeuvre.COeuvrePanel.CPanelBinEnabled;

    private long? LGuildAuthorStored => _cGuildVista?.LVistaStored;

    private bool LGuildAuthorHeld => LGuildAuthorStored is not null;

    private bool LGuildAuthorShown => LGuildAuthorHeld || CGuildPanel.CPanelEditing;

    private bool LGuildRowShown => CGuildPanel.CPanelBinEnabled && !CGuildPanel.CPanelEditing;

    internal void LGuildVistaRestore()
    {
        LVista vista = _cGuildAtelier.CAtelierVistaStart(
            "guild", CSubject.CSubjectAuthor, CCatalogOrder.CCatalogOrderName);
        LVista oeuvre = _cGuildAtelier.CAtelierVistaStart(
            "oeuvre", CSubject.CSubjectReference, CCatalogOrder.CCatalogOrderName);
        vista.LVistaQuerySet(_cGuildVista?.LVistaQuery ?? string.Empty);
        _cGuildVista = vista;
        CGuildPanel.CPanelVistaRestore(vista);
        CGuildOeuvre.LOeuvreVistaRestore(vista, oeuvre);
        CGuildAutograph.CDeskVistaRestore(vista);
        LGuildObserverAttach();
    }

    private void LGuildObserverAttach()
    {
        CPanel panel = CGuildPanel;
        Action<CBulletin> catalog = _ => _cGuildMarshal(LGuildCatalogResonate);
        panel.CPanelObserverAttach(CSubject.CSubjectVista, _ => _cGuildMarshal(panel.CPanelRowsResonate));
        CGuildOeuvre.LOeuvreObserverAttach(_cGuildMarshal);
        panel.CPanelObserverAttach(CSubject.CSubjectWorkspace, _ => _cGuildMarshal(LGuildAuthorClose));
        panel.CPanelObserverAttach(CSubject.CSubjectAuthor, catalog);
        panel.CPanelObserverAttach(CSubject.CSubjectReference, catalog);
        panel.CPanelObserverAttach(CSubject.CSubjectExample, catalog);
        panel.CPanelObserverAttach(CSubject.CSubjectEntry, catalog);
    }

    public static IReadOnlyList<CCatalogOrder> CGuildOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderName,
            CCatalogOrder.CCatalogOrderReverse,
            CCatalogOrder.CCatalogOrderWork,
            CCatalogOrder.CCatalogOrderUsage,
        ];
    }

    public CGuildRoll CGuildRollRead()
    {
        IReadOnlyList<CCatalogAuthor> rows =
            CGuildOeuvre.LOeuvreAuthorRead(_cGuildAuthorPort.LEngineRollFind(_cGuildVista));
        if (LGuildRowShown && !rows.Any(static row => row.CCatalogAuthorChosen))
        {
            LGuildAuthorClose();
        }

        return new CGuildRoll(
            rows,
            rows.Count == 0,
            COeuvre.LOeuvreVitaRead(_cGuildAuthorPort.LEngineVitaRead(_cGuildVista)));
    }

    public void CGuildQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cGuildVista?.LVistaQuerySet(query);
    }

    public void CGuildOrderSet(CCatalogOrder? order)
    {
        _cGuildVista?.LVistaOrderSet(CCatalog.LCatalogOrderRead(order));
    }

    public void CGuildFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cGuildVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    internal bool LGuildLeaveConfirm()
    {
        if (!CGuildSession.LSessionChangeCheck())
        {
            return true;
        }

        if (_cGuildEnvoy.CEnvoyLeaveConfirm() is not bool store)
        {
            return false;
        }

        return !store || CGuildSession.LSessionFinish(true);
    }

    private void LGuildAuthorClose()
    {
        CGuildOeuvre.COeuvrePanel.CPanelEntryClose();
        CGuildPanel.CPanelEntryClose();
    }

    private void LGuildCatalogResonate()
    {
        CGuildPanel.CPanelRowsResonate();
        if (LGuildSourceSide)
        {
            CGuildOeuvre.COeuvrePanel.CPanelDraftResonate();
        }

        CGuildChanged?.Invoke();
    }

    public void CGuildAuthorSelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!LGuildLeaveConfirm())
        {
            return;
        }

        _cGuildAtelier.CAtelierNavigation.LNavigationStationAdd();
        LGuildAuthorOpen(id, CGuildPanel.CPanelEditing);
    }

    internal void LGuildAuthorOpen(long id)
    {
        LGuildAuthorOpen(id, CGuildPanel.CPanelEditing);
    }

    private void LGuildAuthorOpen(long? id, bool editing)
    {
        CGuildSession.CSessionCancel();
        CGuildOeuvre.COeuvrePanel.CPanelEntryClose();
        CGuildPanel.CPanelScribeSet(editing && LVista.LVistaStoredCheck(id));
        CGuildPanel.CPanelRowOpen(id);
    }

    private void LGuildStoredShow(long id)
    {
        _cGuildVista?.LVistaSelect(id);
        CGuildPanel.CPanelRowsResonate();
        CGuildSession.CSessionStart(id);
        CGuildChanged?.Invoke();
    }

    public void CGuildSourceSelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!LGuildLeaveConfirm())
        {
            return;
        }

        CGuildSession.CSessionCancel();
        CGuildPanel.CPanelScribeSet(false);
        CGuildOeuvre.COeuvrePanel.CPanelRowOpen(id);
    }

    public void CGuildAuthorCreate()
    {
        if (!LGuildLeaveConfirm())
        {
            return;
        }

        CGuildOeuvre.COeuvrePanel.CPanelEntryClose();
        CGuildPanel.CPanelFreshOpen();
        CGuildSession.CSessionStart(null);
    }

    public void CGuildScribeToggle(bool editing)
    {
        if (LGuildSourceSide)
        {
            return;
        }

        if (!LGuildEditCheck(editing))
        {
            CGuildPanel.CPanelEntryClose();
            return;
        }

        CGuildPanel.CPanelScribeToggle(editing);
        if (!CGuildPanel.CPanelEditing)
        {
            CGuildSession.CSessionCancel();
        }
    }

    internal void LGuildScribeRestore(bool editing)
    {
        if (!LGuildEditCheck(editing))
        {
            return;
        }

        CGuildPanel.LPanelScribeRestore(editing);
        if (CGuildPanel.CPanelEditing)
        {
            CGuildSession.CSessionStart(LGuildAuthorStored);
        }
    }

    private bool LGuildEditCheck(bool editing)
    {
        return !editing || LGuildAuthorHeld;
    }

    private bool LGuildAutographCheck()
    {
        if (CGuildAutograph.LDeskReadyCheck())
        {
            return true;
        }

        _cGuildEnvoy.CEnvoyFailureShow("Guild.NameBlank");
        return false;
    }

    public void CGuildAuthorDelete()
    {
        if (LGuildSourceSide)
        {
            return;
        }

        CGuildPanel.CPanelEntryDelete();
    }

    public Task CGuildPortraitPrint()
    {
        if (!LGuildSourceSide)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitTicketPrint(
            _cGuildEnvoy,
            _cGuildSettingsPort,
            chosen => _cGuildPortraitPort.LEnginePortraitPrint(
                CGuildOeuvre.COeuvrePanel.CPanelVista,
                CPortrait.LPortraitLegendRead(_cGuildSettingsPort, "Source"),
                chosen));
    }

    public void CGuildNameSet(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        CGuildAutograph.CDeskAuthor?.LQuillAuthorSet(name);
    }

    public IReadOnlyList<CCatalogAuthor> CGuildUnionRead(string typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        return CGuildOeuvre.LOeuvreAuthorRead(_cGuildAuthorPort.LEngineUnionFind(_cGuildVista, typed));
    }

    public void CGuildUnionSelect(long? id)
    {
        if (id is not long kept)
        {
            return;
        }

        if (LGuildAuthorStored is not long author)
        {
            return;
        }

        if (!LGuildUnionConfirm(kept))
        {
            return;
        }

        try
        {
            _cGuildAuthorPort.LEngineAuthorAbsorb(kept, author);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cGuildEnvoy, _cGuildSettingsPort, "Guild.MergeFailed", exception);
            return;
        }

        LGuildAuthorOpen(kept, false);
    }

    private bool LGuildUnionConfirm(long kept)
    {
        (string dropped, string held) = _cGuildAuthorPort.LEngineUnionRead(CGuildAutograph.CDeskTenure, kept);
        return _cGuildEnvoy.CEnvoyUnionConfirm("Guild.MergeConfirm", dropped, held);
    }
}
