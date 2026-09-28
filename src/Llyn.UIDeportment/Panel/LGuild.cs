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

    private LVista? _lGuildVista;

    private int _lGuildCount;

    internal LGuild(
        LDraftPort drafts, LEntryPort entries, LPortraitPort portraits, LSettingsPort settings,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<string, string, bool> unionSeam,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(leaveSeam);
        ArgumentNullException.ThrowIfNull(unionSeam);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        _lGuildLeaveSeam = leaveSeam;
        LGuildAutograph = new CDesk(drafts, "Guild", envoy);
        LGuildPanel = new CPanel(
            envoy, "Guild.LoadFailed", "Guild",
            LGuildAutograph.CDeskChangeCheck, store => LGuildSession!.CSessionFinish(store), shownSeam);
        LGuildOeuvre = new COeuvre(entries, envoy, shownSeam);
        LGuildPanel.CPanelRowsChanged += LGuildOeuvre.COeuvrePanel.CPanelRowsUpdate;
        LGuildSession = new CSession(
            LGuildAutograph, [LGuildPanel.CPanelChangeCheck], null, static () => false, static _ => true,
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
        LGuildPanel.CPanelCleared += LGuildSession.CSessionCancel;
        LGuildPanel.CPanelEdited += id => LGuildSession.CSessionStart(id);
    }

    public event Action? LGuildChanged;

    public event Action<string>? LGuildRefused;

    public event Action<string, Exception>? LGuildFailed;

    public CPanel LGuildPanel { get; }

    public COeuvre LGuildOeuvre { get; }

    public CDesk LGuildAutograph { get; }

    public CSession LGuildSession { get; }

    public QUnion LGuildUnion { get; }

    private bool LGuildSourceSide => LGuildOeuvre.COeuvrePanel.CPanelBinEnabled;

    public bool LGuildColophonShown => LGuildSourceSide;

    public bool LGuildAutographShown => !LGuildSourceSide && LGuildPanel.CPanelEditing;

    public bool LGuildVitaShown => !LGuildSourceSide && !LGuildPanel.CPanelEditing;

    public bool LGuildVitaHeld => LGuildAuthorHeld && !LGuildPanel.CPanelEditing;

    public bool LGuildViewerChecked => !LGuildPanel.CPanelEditing;

    public bool LGuildScribeChecked => LGuildPanel.CPanelEditing;

    public bool LGuildModeEnabled => !LGuildSourceSide && LGuildAuthorShown;

    public bool LGuildBinEnabled => !LGuildSourceSide && LGuildAuthorHeld;

    public bool LGuildStoreEnabled =>
        LGuildAutographShown && LGuildAutographNamed && LGuildAutograph.CDeskChangeCheck();

    public bool LGuildPressAllowed => LGuildSourceSide;

    public bool LGuildEmpty => _lGuildCount == 0;

    public bool LGuildLouverActive => _lGuildVista?.LVistaFiltered ?? false;

    private long? LGuildAuthorStored => _lGuildVista?.LVistaStored;

    private bool LGuildAuthorHeld => LGuildAuthorStored is not null;

    private bool LGuildAuthorShown => LGuildAuthorHeld || LGuildPanel.CPanelEditing;

    private bool LGuildAutographNamed => LGuildAutograph.CDeskRead()?.LDraftAuthorHeld?.LAuthorNamed ?? false;

    private long LGuildAuthorId => LGuildAuthorStored ?? 0;

    internal void LGuildVistaRestore(LVista vista, LVista oeuvre)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(oeuvre);

        _lGuildVista = vista;
        LGuildPanel.CPanelVistaRestore(vista);
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

    private bool LGuildRowShown => LGuildPanel.CPanelBinEnabled && !LGuildPanel.CPanelEditing;

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

        vista.LVistaOrderSet(CPanel.CPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public CCatalogFilter LGuildLouverRead()
    {
        return CPanel.CPanelFilterRead(_lGuildVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty);
    }

    public void LGuildLouverSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lGuildVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public bool LGuildLeaveConfirm()
    {
        if (!LGuildSession.CSessionChangeCheck())
        {
            return true;
        }

        return _lGuildLeaveSeam();
    }

    private void LGuildClear()
    {
        LGuildOeuvre.COeuvrePanel.CPanelEntryClose();
        LGuildPanel.CPanelEntryClose();
    }

    public void LGuildReset()
    {
        LGuildOeuvre.COeuvrePanel.CPanelEntryClose();
        LGuildPanel.CPanelEntryClose();
    }

    public void LGuildCatalogUpdate()
    {
        LGuildPanel.CPanelRowsUpdate();
        if (LGuildSourceSide)
        {
            LGuildOeuvre.COeuvrePanel.CPanelDraftUpdate();
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

        LGuildAuthorOpen(id, LGuildPanel.CPanelEditing);
    }

    public void LGuildRowShow(long id)
    {
        LGuildAuthorOpen(id, LGuildPanel.CPanelEditing);
    }

    private void LGuildAuthorOpen(long? id, bool editing)
    {
        LGuildSession.CSessionCancel();
        LGuildOeuvre.COeuvrePanel.CPanelEntryClose();
        LGuildPanel.CPanelScribeSet(editing && id is > 0);
        LGuildPanel.CPanelRowOpen(id);
    }

    private void LGuildStoredShow(long id)
    {
        _lGuildVista?.LVistaSelect(id);
        LGuildPanel.CPanelRowsUpdate();
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
        LGuildPanel.CPanelScribeSet(false);
        LGuildOeuvre.COeuvrePanel.CPanelRowOpen(id);
    }

    public void LGuildFreshStart()
    {
        if (!LGuildLeaveConfirm())
        {
            return;
        }

        LGuildOeuvre.COeuvrePanel.CPanelEntryClose();
        LGuildPanel.CPanelFreshOpen();
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
            LGuildPanel.CPanelEntryClose();
            return;
        }

        LGuildPanel.CPanelScribeToggle(editing);
        if (!LGuildPanel.CPanelEditing)
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

        LGuildPanel.CPanelScribeRestore(editing);
        if (LGuildPanel.CPanelEditing)
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

        LGuildPanel.CPanelEntryDelete();
    }

    public Task LGuildPortraitPrint(CPortraitLegend legend, CPressTicket ticket)
    {
        if (!LGuildSourceSide)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(
            LGuildOeuvre.COeuvrePanel.CPanelVista,
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
