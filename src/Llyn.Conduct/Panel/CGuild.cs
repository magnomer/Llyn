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

    private readonly LEntryPort _cGuildEntryPort;

    private readonly LPortraitPort _cGuildPortraitPort;

    private readonly LSettingsPort _cGuildSettingsPort;

    private LVista? _cGuildVista;

    private int _cGuildCount;

    private CGuild(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cGuildAtelier = atelier;
        _cGuildEnvoy = envoy;
        _cGuildEntryPort = atelier.CAtelierEntryPort;
        _cGuildPortraitPort = atelier.CAtelierPortraitPort;
        _cGuildSettingsPort = atelier.CAtelierSettingsPort;
        CGuildAutograph = new CDesk(atelier.CAtelierDraftPort, "Guild", envoy);
        CGuildPanel = new CPanel(
            envoy, "Guild.LoadFailed", "Guild",
            CGuildAutograph.CDeskChangeCheck, store => CGuildSession!.CSessionFinish(store), shownSeam);
        CGuildOeuvre = new COeuvre(atelier.CAtelierEntryPort, envoy, shownSeam);
        CGuildPanel.CPanelRowsChanged += CGuildOeuvre.COeuvrePanel.CPanelRowsResonate;
        CGuildSession = new CSession(
            CGuildAutograph, [CGuildPanel.CPanelChangeCheck], null, static () => false, static _ => true,
            LGuildAutographCheck, LGuildStoredShow);
        CGuildSession.CSessionChanged += () => CGuildChanged?.Invoke();
        CGuildSession.CSessionFailed += envoy.CEnvoyFailureShow;
        CGuildPanel.CPanelCleared += CGuildSession.CSessionCancel;
        CGuildPanel.CPanelEdited += id => CGuildSession.CSessionStart(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Guild", LGuildLeaveConfirm, CGuildPanel.LPanelChosenRead, LGuildScribeRestore, LGuildAuthorOpen);
    }

    public static CGuild CGuildCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CGuild(atelier, shownSeam, envoy);
    }

    public event Action? CGuildChanged;

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
        CGuildAutographShown
        && CGuildAutograph.CDeskChangeCheck()
        && CGuildAutograph.CDeskTenure?.LTenureReadyCheck() is true;

    public bool CGuildPressAllowed => LGuildSourceSide;

    public bool CGuildEmpty => _cGuildCount == 0;

    public bool CGuildFiltered => _cGuildVista?.LVistaFiltered ?? false;

    public bool CGuildUnionShown => CGuildAutograph.CDeskStored;

    private bool LGuildSourceSide => CGuildOeuvre.COeuvrePanel.CPanelBinEnabled;

    private long? LGuildAuthorStored => _cGuildVista?.LVistaStored;

    private bool LGuildAuthorHeld => LGuildAuthorStored is not null;

    private bool LGuildAuthorShown => LGuildAuthorHeld || CGuildPanel.CPanelEditing;

    private bool LGuildRowShown => CGuildPanel.CPanelBinEnabled && !CGuildPanel.CPanelEditing;

    public void CGuildVistaRestore()
    {
        LVista vista = _cGuildAtelier.CAtelierVistaStart(
            "guild", CSubject.CSubjectAuthor, CCatalogOrder.CCatalogOrderName);
        LVista oeuvre = _cGuildAtelier.CAtelierVistaStart(
            "oeuvre", CSubject.CSubjectReference, CCatalogOrder.CCatalogOrderName);
        _cGuildVista = vista;
        CGuildPanel.CPanelVistaRestore(vista);
        CGuildOeuvre.LOeuvreVistaRestore(vista, oeuvre);
        CGuildAutograph.CDeskVistaRestore(vista);
    }

    public IReadOnlyList<CCatalogAuthor> CGuildRollRead()
    {
        IReadOnlyList<CCatalogAuthor> rows =
            CGuildOeuvre.LOeuvreAuthorRead(_cGuildEntryPort.LEngineRollFind(_cGuildVista));
        _cGuildCount = rows.Count;
        if (LGuildRowShown && !rows.Any(static row => row.CCatalogAuthorChosen))
        {
            CGuildAuthorClose();
        }

        return rows;
    }

    public CVita CGuildVitaRead()
    {
        return COeuvre.LOeuvreVitaRead(_cGuildEntryPort.LEngineVitaRead(_cGuildVista));
    }

    public void CGuildQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cGuildVista?.LVistaQuerySet(query);
    }

    public void CGuildOrderSet(CCatalogOrder? order)
    {
        _cGuildVista?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CGuildFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cGuildVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    internal bool LGuildLeaveConfirm()
    {
        if (!CGuildSession.CSessionChangeCheck())
        {
            return true;
        }

        if (_cGuildEnvoy.CEnvoyLeaveConfirm() is not bool store)
        {
            return false;
        }

        return !store || CGuildSession.CSessionFinish(true);
    }

    public void CGuildAuthorClose()
    {
        CGuildOeuvre.COeuvrePanel.CPanelEntryClose();
        CGuildPanel.CPanelEntryClose();
    }

    public void CGuildCatalogResonate()
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
        if (CGuildAutograph.CDeskTenure?.LTenureReadyCheck() is true)
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
            chosen => _cGuildPortraitPort.LEnginePortraitPrint(
                CGuildOeuvre.COeuvrePanel.CPanelVista,
                CPortrait.LPortraitLegendRead(_cGuildSettingsPort, "Source"),
                chosen));
    }

    public IReadOnlyList<CCatalogAuthor> CGuildUnionRead(string typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        return CGuildOeuvre.LOeuvreAuthorRead(_cGuildEntryPort.LEngineUnionFind(_cGuildVista, typed));
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
            _cGuildEntryPort.LEngineAuthorAbsorb(kept, author);
        }
        catch (Exception exception)
        {
            _cGuildEnvoy.CEnvoyFailureShow("Guild.MergeFailed", exception);
            return;
        }

        LGuildAuthorOpen(kept, false);
    }

    private bool LGuildUnionConfirm(long kept)
    {
        (string dropped, string held) = _cGuildEntryPort.LEngineUnionRead(CGuildAutograph.CDeskTenure, kept);
        return _cGuildEnvoy.CEnvoyUnionConfirm("Guild.MergeConfirm", dropped, held);
    }
}
