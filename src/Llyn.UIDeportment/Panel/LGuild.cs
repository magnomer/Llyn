using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LGuild
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private readonly Func<bool> _lGuildLeaveSeam;

    private readonly Func<int, bool> _lGuildRemovalSeam;

    private LVista? _lGuildVista;

    private int _lGuildCount;

    internal LGuild(
        LDraftPort drafts, LEntryPort entries, LPortraitPort portraits, LSettingsPort settings,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam,
        Func<string, string, bool> unionSeam,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(leaveSeam);
        ArgumentNullException.ThrowIfNull(removalSeam);
        ArgumentNullException.ThrowIfNull(unionSeam);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        _lGuildLeaveSeam = leaveSeam;
        _lGuildRemovalSeam = removalSeam;
        LGuildAutograph = new CDesk(drafts, "Guild", envoy);
        LGuildPanel = new LPanel(
            "Guild.LoadFailed", "Guild.DeleteFailed",
            LGuildAutograph.CDeskChangeCheck, shownSeam, leaveSeam, LGuildDeleteConfirm);
        LGuildOeuvre = new COeuvre(entries);
        LGuildOeuvrePanel = new LPanel(
            "Source.LoadFailed", "Source.DeleteFailed",
            static () => false, shownSeam, static () => true, static () => false);
        LGuildOeuvrePanel.LPanelDraftChanged += LGuildOeuvre.COeuvreColophonUpdate;
        LGuildOeuvre.COeuvreStrayed += LGuildOeuvrePanel.LPanelClear;
        LGuildPanel.LPanelRowsChanged += LGuildOeuvrePanel.LPanelRowsUpdate;
        LGuildSession = new CSession(
            LGuildAutograph, [LGuildPanel.LPanelChangeCheck], null, static () => false, static _ => true,
            LGuildAutographCheck, LGuildStoredShow);
        LGuildUnion = new QUnion(
            LGuildAutograph,
            () => LGuildAuthorStored,
            (typed, author, limit) => LGuildOeuvre.COeuvreAuthorRead(entries.LEngineAuthorFind(typed, author, limit)),
            kept => entries.LEngineAuthorFind(kept)?.LCatalogAuthorName ?? string.Empty,
            entries.LEngineAuthorAbsorb,
            unionSeam,
            kept => LGuildAuthorOpen(kept, false));
        LGuildSession.CSessionChanged += () => LGuildChanged?.Invoke();
        LGuildSession.CSessionFailed += (key, exception) => LGuildFailed?.Invoke(key, exception);
        LGuildUnion.QUnionFailed += (key, exception) => LGuildFailed?.Invoke(key, exception);
        LGuildPanel.LPanelCleared += LGuildSession.CSessionCancel;
        LGuildPanel.LPanelEdited += id => LGuildSession.CSessionStart(id);
    }

    public event Action? LGuildChanged;

    public event Action<string>? LGuildRefused;

    public event Action<string, Exception>? LGuildFailed;

    public LPanel LGuildPanel { get; }

    public COeuvre LGuildOeuvre { get; }

    public LPanel LGuildOeuvrePanel { get; }

    public CDesk LGuildAutograph { get; }

    public CSession LGuildSession { get; }

    public QUnion LGuildUnion { get; }

    private bool LGuildSourceSide => LGuildOeuvrePanel.LPanelBinEnabled;

    public bool LGuildColophonShown => LGuildSourceSide;

    public bool LGuildAutographShown => !LGuildSourceSide && LGuildPanel.LPanelEditing;

    public bool LGuildVitaShown => !LGuildSourceSide && !LGuildPanel.LPanelEditing;

    public bool LGuildVitaHeld => LGuildAuthorHeld && !LGuildPanel.LPanelEditing;

    public bool LGuildViewerChecked => !LGuildPanel.LPanelEditing;

    public bool LGuildScribeChecked => LGuildPanel.LPanelEditing;

    public bool LGuildModeEnabled => !LGuildSourceSide && LGuildAuthorShown;

    public bool LGuildBinEnabled => !LGuildSourceSide && LGuildAuthorHeld;

    public bool LGuildStoreEnabled =>
        LGuildAutographShown && LGuildAutographNamed && LGuildAutograph.CDeskChangeCheck();

    public bool LGuildPressAllowed => LGuildSourceSide;

    public bool LGuildEmpty => _lGuildCount == 0;

    public bool LGuildLouverActive => _lGuildVista?.LVistaFiltered ?? false;

    private long? LGuildAuthorStored => _lGuildVista?.LVistaStored;

    private bool LGuildAuthorHeld => LGuildAuthorStored is not null;

    private bool LGuildAuthorShown => LGuildAuthorHeld || LGuildPanel.LPanelEditing;

    private bool LGuildAutographNamed => LGuildAutograph.CDeskRead()?.LDraftAuthorHeld?.LAuthorNamed ?? false;

    private long LGuildAuthorId => LGuildAuthorStored ?? 0;

    internal void LGuildVistaRestore(LVista vista, LVista oeuvre)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(oeuvre);

        _lGuildVista = vista;
        LGuildPanel.LPanelVistaRestore(vista);
        LGuildOeuvrePanel.LPanelVistaRestore(oeuvre);
        LGuildOeuvre.COeuvreVistaRestore(vista, oeuvre);
        LGuildAutograph.CDeskVistaRestore(vista);
    }

    public IReadOnlyList<CCatalogAuthor> LGuildRollRead()
    {
        return LGuildOeuvre.COeuvreAuthorRead(LGuildRollApply(LGuildRollFind()));
    }

    private IReadOnlyList<LCatalogAuthor> LGuildRollFind()
    {
        if (_lGuildVista is not LVista vista)
        {
            return [];
        }

        return _lEntryPort.LEngineAuthorFind(vista, _lSettingsPort.LEngineTextRead("Guild.Uncredited"));
    }

    private IReadOnlyList<LCatalogAuthor> LGuildRollApply(IReadOnlyList<LCatalogAuthor> rows)
    {
        _lGuildCount = rows.Count;
        if (LGuildRowShown)
        {
            if (!LGuildChosenCheck(rows))
            {
                LGuildClear();
            }
        }

        return rows;
    }

    private bool LGuildRowShown => LGuildPanel.LPanelBinEnabled && !LGuildPanel.LPanelEditing;

    private static bool LGuildChosenCheck(IReadOnlyList<LCatalogAuthor> rows)
    {
        foreach (LCatalogAuthor row in rows)
        {
            if (row.LCatalogAuthorChosen)
            {
                return true;
            }
        }

        return false;
    }

    public CVita LGuildVitaRead()
    {
        if (!LGuildAuthorHeld)
        {
            return COeuvre.COeuvreVitaRead(LVita.LVitaCreate(null, [], [], _lSettingsPort.LEngineTextRead));
        }

        return COeuvre.COeuvreVitaRead(LVita.LVitaCreate(
            _lEntryPort.LEngineAuthorFind(LGuildAuthorId),
            _lEntryPort.LEngineFellowFind(LGuildAuthorId),
            _lEntryPort.LEngineUsageRead(LGuildAuthorId, LOwner.LOwnerAuthor),
            _lSettingsPort.LEngineTextRead));
    }

    public void LGuildQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lGuildVista?.LVistaQuerySet(query);
    }

    public void LGuildOrderSet(CCatalogOrder? order)
    {
        if (_lGuildVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LPanel.LPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public CCatalogFilter LGuildLouverRead()
    {
        return LPanel.LPanelFilterRead(_lGuildVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty);
    }

    public void LGuildLouverSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lGuildVista?.LVistaFilterSet(LPanel.LPanelFilterRead(filter));
    }

    public bool LGuildLeaveConfirm()
    {
        if (!LGuildSession.CSessionChangeCheck())
        {
            return true;
        }

        return _lGuildLeaveSeam();
    }

    private bool LGuildDeleteConfirm()
    {
        return _lGuildRemovalSeam(LGuildWorkRead(_lEntryPort.LEngineAuthorFind(LGuildAuthorId)));
    }

    private static int LGuildWorkRead(LCatalogAuthor? row)
    {
        return row?.LCatalogAuthorWork ?? 0;
    }

    private void LGuildClear()
    {
        LGuildOeuvrePanel.LPanelClear();
        LGuildPanel.LPanelClear();
    }

    public void LGuildReset()
    {
        LGuildOeuvrePanel.LPanelClear();
        LGuildPanel.LPanelClear();
    }

    public void LGuildCatalogUpdate()
    {
        LGuildPanel.LPanelRowsUpdate();
        if (LGuildSourceSide)
        {
            LGuildOeuvrePanel.LPanelDraftUpdate();
        }

        LGuildChanged?.Invoke();
    }

    public void LGuildRowSelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!LGuildLeaveConfirm())
        {
            return;
        }

        LGuildAuthorOpen(id, LGuildPanel.LPanelEditing);
    }

    public void LGuildRowShow(long id)
    {
        LGuildAuthorOpen(id, LGuildPanel.LPanelEditing);
    }

    private void LGuildAuthorOpen(long? id, bool editing)
    {
        LGuildSession.CSessionCancel();
        LGuildOeuvrePanel.LPanelClear();
        LGuildPanel.LPanelScribeShow(editing && id is > 0);
        LGuildPanel.LPanelRowShow(id);
    }

    private void LGuildStoredShow(long id)
    {
        _lGuildVista?.LVistaSelect(id);
        LGuildPanel.LPanelRowsUpdate();
        LGuildSession.CSessionStart(id);
        LGuildChanged?.Invoke();
    }

    public void LGuildSourceSelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!LGuildLeaveConfirm())
        {
            return;
        }

        LGuildSession.CSessionCancel();
        LGuildPanel.LPanelScribeShow(false);
        LGuildOeuvrePanel.LPanelRowShow(id);
    }

    public void LGuildFreshStart()
    {
        if (!LGuildLeaveConfirm())
        {
            return;
        }

        LGuildOeuvrePanel.LPanelClear();
        LGuildPanel.LPanelFreshOpen();
        LGuildSession.CSessionStart(null);
    }

    public void LGuildScribeSet(bool editing)
    {
        if (LGuildSourceSide)
        {
            return;
        }

        if (!LGuildEditCheck(editing))
        {
            LGuildPanel.LPanelClear();
            return;
        }

        LGuildPanel.LPanelScribeSet(editing);
        if (!LGuildPanel.LPanelEditing)
        {
            LGuildSession.CSessionCancel();
        }
    }

    public void LGuildScribeRestore(bool editing)
    {
        if (!LGuildEditCheck(editing))
        {
            return;
        }

        LGuildPanel.LPanelScribeRestore(editing);
        if (LGuildPanel.LPanelEditing)
        {
            LGuildSession.CSessionStart(LGuildAuthorId);
        }
    }

    private bool LGuildEditCheck(bool editing)
    {
        return !editing || LGuildAuthorHeld;
    }

    private bool LGuildAutographCheck()
    {
        if (LGuildAutographNamed)
        {
            return true;
        }

        LGuildRefused?.Invoke("Guild.NameBlank");
        return false;
    }

    public void LGuildDelete()
    {
        if (LGuildSourceSide)
        {
            return;
        }

        LGuildPanel.LPanelDelete();
    }

    public Task LGuildPortraitPrint(CPortraitLegend legend, CPressTicket ticket)
    {
        if (!LGuildSourceSide)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(
            LGuildOeuvrePanel.LPanelVista,
            LAtlas.LAtlasLegendRead(legend),
            QPortrait.QPortraitTicketRead(ticket));
    }

    internal void LGuildVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LGuildVistaRestore(
            atelier.CAtelierVistaStart("guild", CSubject.CSubjectAuthor, CCatalogOrder.CCatalogOrderName),
            atelier.CAtelierVistaStart(
                "oeuvre", CSubject.CSubjectReference, CCatalogOrder.CCatalogOrderName));
    }
}
