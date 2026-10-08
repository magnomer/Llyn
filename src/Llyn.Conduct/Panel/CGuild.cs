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
        CGuildPanel.CPanelAperture.CApertureRowsChanged +=
            CGuildOeuvre.COeuvrePanel.CPanelAperture.CApertureRowsResonate;
        CGuildSession = new CSession(
            CGuildAutograph, [CGuildPanel.LPanelChangeCheck], null, static () => false, static _ => true,
            LGuildAutographCheck, LGuildStoredShow, envoy);
        CGuildDiptych = new CDiptych(
            CGuildPanel,
            CGuildOeuvre.COeuvrePanel,
            CGuildSession,
            atelier.CAtelierNavigation,
            CGuildSession.CSessionStart,
            CGuildSession.CSessionCancel,
            null);
        CGuildSession.CSessionChanged += () => CGuildChanged?.Invoke();
        CGuildPanel.CPanelCleared += CGuildSession.CSessionCancel;
        CGuildPanel.CPanelEdited += id => CGuildSession.CSessionStart(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Guild",
            () => CGuildSession.LSessionLeaveConfirm(true),
            CGuildPanel.LPanelChosenRead,
            LGuildScribeRestore,
            LGuildAuthorOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CGuildSession.LSessionChangeCheck, CGuildSession.LSessionFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LGuildVistaRestore);
        CGuildUnion = new CGuildUnion(
            _cGuildAuthorPort,
            envoy,
            _cGuildSettingsPort,
            CGuildAutograph,
            CGuildOeuvre,
            CGuildPanel,
            kept => LGuildAuthorOpen(kept, false));
        CGuildAutograph.CDeskObserverAttach(marshal);
        LGuildVistaRestore();
    }

    public static CGuild CGuildCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CGuild(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CGuildChanged;

    public CPanel CGuildPanel { get; }

    public COeuvre CGuildOeuvre { get; }

    public CDesk CGuildAutograph { get; }

    public CSession CGuildSession { get; }

    public CDiptych CGuildDiptych { get; }

    public CGuildUnion CGuildUnion { get; }

    public bool CGuildVitaHeld => LGuildAuthorHeld && !CGuildPanel.CPanelEditing;

    public bool CGuildModeEnabled => !CGuildDiptych.CDiptychChildSide && LGuildAuthorShown;

    public bool CGuildBinEnabled => !CGuildDiptych.CDiptychChildSide && LGuildAuthorHeld;

    public bool CGuildStoreEnabled =>
        CGuildDiptych.CDiptychParentEditing && CGuildAutograph.CDeskDraft.CDeskDraftStorable;

    private long? LGuildAuthorStored => CGuildPanel.CPanelAperture.CApertureVista?.LVistaStored;

    private bool LGuildAuthorHeld => LGuildAuthorStored is not null;

    private bool LGuildAuthorShown => LGuildAuthorHeld || CGuildPanel.CPanelEditing;

    private bool LGuildRowShown => CGuildPanel.CPanelBinEnabled && !CGuildPanel.CPanelEditing;

    internal void LGuildVistaRestore()
    {
        LVista vista = _cGuildAtelier.CAtelierVistaStart(
            "guild", CSubject.CSubjectAuthor, CCatalogOrder.CCatalogOrderName);
        LVista oeuvre = _cGuildAtelier.CAtelierVistaStart(
            "oeuvre", CSubject.CSubjectReference, CCatalogOrder.CCatalogOrderName);
        CGuildPanel.CPanelVistaRestore(vista);
        CGuildOeuvre.LOeuvreVistaRestore(vista, oeuvre);
        CGuildAutograph.CDeskVistaRestore(vista);
        LGuildObserverAttach();
    }

    private void LGuildObserverAttach()
    {
        CPanel panel = CGuildPanel;
        Action<CBulletin> catalog = _ => _cGuildMarshal(LGuildCatalogResonate);
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectVista, _ => _cGuildMarshal(panel.CPanelAperture.CApertureRowsResonate));
        CGuildOeuvre.LOeuvreObserverAttach(_cGuildMarshal);
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectWorkspace, _ => _cGuildMarshal(CGuildDiptych.LDiptychEntryClose));
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectAuthor, catalog);
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectReference, catalog);
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectExample, catalog);
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectEntry, catalog);
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
            CGuildOeuvre.LOeuvreAuthorRead(
                _cGuildAuthorPort.LEngineRollFind(CGuildPanel.CPanelAperture.CApertureVista));
        if (LGuildRowShown && !rows.Any(static row => row.CCatalogAuthorChosen))
        {
            CGuildDiptych.LDiptychEntryClose();
        }

        return new CGuildRoll(
            rows,
            rows.Count == 0,
            COeuvre.LOeuvreVitaRead(_cGuildAuthorPort.LEngineVitaRead(CGuildPanel.CPanelAperture.CApertureVista)),
            CGuildOeuvre.LOeuvreKindRead());
    }

    private void LGuildCatalogResonate()
    {
        CGuildPanel.CPanelAperture.CApertureRowsResonate();
        if (CGuildDiptych.CDiptychChildSide)
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

        if (!CGuildSession.LSessionLeaveConfirm(true))
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
        CGuildPanel.CPanelAperture.CApertureVista?.LVistaSelect(id);
        CGuildPanel.CPanelAperture.CApertureRowsResonate();
        CGuildSession.CSessionStart(id);
        CGuildChanged?.Invoke();
    }

    public void CGuildSourceSelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!CGuildSession.LSessionLeaveConfirm(true))
        {
            return;
        }

        CGuildSession.CSessionCancel();
        CGuildPanel.CPanelScribeSet(false);
        CGuildOeuvre.COeuvrePanel.CPanelRowOpen(id);
    }

    public void CGuildScribeToggle(bool editing)
    {
        if (CGuildDiptych.CDiptychChildSide)
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

    public Task CGuildPortraitPrint()
    {
        if (!CGuildDiptych.CDiptychChildSide)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitTicketPrint(
            _cGuildEnvoy,
            _cGuildSettingsPort,
            chosen => _cGuildPortraitPort.LEnginePortraitPrint(
                CGuildOeuvre.COeuvrePanel.CPanelAperture.CApertureVista,
                CPortrait.LPortraitLegendRead(_cGuildSettingsPort, "Source"),
                chosen));
    }

    public void CGuildNameSet(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        CGuildAutograph.CDeskDraft.CDeskDraftAuthor?.LQuillAuthorSet(name);
    }
}
