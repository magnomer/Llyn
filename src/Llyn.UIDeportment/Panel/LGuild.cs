using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LGuild
{
    private const int LGuildUnionLimit = 8;

    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private readonly Func<bool> _lGuildLeaveSeam;

    private readonly Func<int, bool> _lGuildRemovalSeam;

    private readonly Func<string, string, bool> _lGuildUnionSeam;

    private LVista? _lGuildVista;

    private int _lGuildCount;

    public LGuild(
        LDraftPort drafts, LEntryPort entries, LPortraitPort portraits, LSettingsPort settings,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam,
        Func<string, string, bool> unionSeam,
        Func<bool> unreadableSeam)
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
        _lGuildUnionSeam = unionSeam;
        LGuildAutograph = new LDesk(drafts, "Guild", unreadableSeam);
        LGuildPanel = new LPanel("Guild.LoadFailed", LAutographChangeCheck, shownSeam, leaveSeam, LGuildDeleteConfirm);
        LGuildOeuvre = new LOeuvre(entries, settings, shownSeam);
        LGuildPanel.LPanelRowsChanged += LGuildOeuvre.LOeuvrePanel.LPanelRowsUpdate;
        LGuildPanel.LPanelCleared += LGuildAutograph.LDeskCancel;
        LGuildPanel.LPanelEdited += LGuildDraftStart;
        LGuildAutograph.LDeskFinished += LGuildStoredShow;
        LGuildAutograph.LDeskStateChanged += LGuildStateUpdate;
    }

    public event Action? LGuildChanged;

    public event Action<string>? LGuildRefused;

    public event Action<string, Exception>? LGuildFailed;

    public LPanel LGuildPanel { get; }

    public LOeuvre LGuildOeuvre { get; }

    public LDesk LGuildAutograph { get; }

    public bool LGuildSourceSide => LGuildOeuvre.LOeuvrePanel.LPanelBinEnabled;

    public bool LGuildColophonShown => LGuildSourceSide;

    public bool LGuildAutographShown => !LGuildSourceSide && LGuildPanel.LPanelEditing;

    public bool LGuildVitaShown => !LGuildSourceSide && !LGuildPanel.LPanelEditing;

    public bool LGuildVitaHeld => LGuildAuthorHeld && !LGuildPanel.LPanelEditing;

    public bool LGuildViewerChecked => !LGuildPanel.LPanelEditing;

    public bool LGuildScribeChecked => LGuildPanel.LPanelEditing;

    public bool LGuildModeEnabled => !LGuildSourceSide && LGuildAuthorShown;

    public bool LGuildBinEnabled => !LGuildSourceSide && LGuildAuthorHeld;

    public bool LGuildStoreEnabled =>
        LGuildAutographShown && LGuildAutographNamed && LGuildAutograph.LDeskChangeCheck();

    public bool LGuildPressAllowed => LGuildSourceSide;

    public bool LGuildEmpty => _lGuildCount == 0;

    public bool LGuildLouverActive => _lGuildVista?.LVistaFiltered ?? false;

    public bool LGuildUnionShown => LGuildAutograph.LDeskStored;

    private bool LGuildAuthorHeld => _lGuildVista?.LVistaStored is not null;

    private bool LGuildAuthorShown => LGuildAuthorHeld || LGuildPanel.LPanelEditing;

    private bool LGuildAutographNamed => LGuildAutograph.LDeskRead()?.LDraftAuthorHeld?.LAuthorNamed ?? false;

    private long LGuildAuthorId => _lGuildVista?.LVistaStored ?? 0;

    public void LGuildVistaRestore(LVista vista, LVista oeuvre)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(oeuvre);

        _lGuildVista = vista;
        LGuildPanel.LPanelVistaRestore(vista);
        LGuildOeuvre.LOeuvreVistaRestore(vista, oeuvre);
        LGuildAutograph.LDeskVistaRestore(vista);
    }

    public IReadOnlyList<LCatalogAuthor> LGuildRollRead()
    {
        return LGuildRollApply(LGuildRollFind());
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

    public LVita LGuildVitaRead()
    {
        if (!LGuildAuthorHeld)
        {
            return LVita.LVitaCreate(null, [], [], _lSettingsPort.LEngineTextRead);
        }

        return LVita.LVitaCreate(
            _lEntryPort.LEngineAuthorFind(LGuildAuthorId),
            _lEntryPort.LEngineFellowFind(LGuildAuthorId),
            _lEntryPort.LEngineUsageRead(LGuildAuthorId, LOwner.LOwnerAuthor),
            _lSettingsPort.LEngineTextRead);
    }

    public IReadOnlyList<LCatalogAuthor> LGuildUnionRead(string typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        if (string.IsNullOrWhiteSpace(typed))
        {
            return [];
        }

        if (!LGuildAuthorHeld)
        {
            return [];
        }

        return _lEntryPort.LEngineAuthorFind(typed, LGuildAuthorId, LGuildUnionLimit);
    }

    public void LGuildQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lGuildVista?.LVistaQuerySet(query);
    }

    public void LGuildOrderSet(LCatalogOrder? order)
    {
        if (_lGuildVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public LCatalogFilter LGuildLouverRead()
    {
        return _lGuildVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty;
    }

    public void LGuildLouverSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lGuildVista?.LVistaFilterSet(filter);
    }

    public bool LGuildChangeCheck()
    {
        return LGuildPanel.LPanelChangeCheck();
    }

    private bool LAutographChangeCheck()
    {
        return LGuildAutograph.LDeskChangeCheck();
    }

    public bool LGuildLeaveConfirm()
    {
        if (!LGuildChangeCheck())
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

    public void LGuildClear()
    {
        LGuildOeuvre.LOeuvrePanel.LPanelClear();
        LGuildPanel.LPanelClear();
    }

    public void LGuildReset()
    {
        LGuildOeuvre.LOeuvrePanel.LPanelClear();
        LGuildPanel.LPanelReset();
    }

    public void LGuildCatalogUpdate()
    {
        LGuildPanel.LPanelRowsUpdate();
        if (LGuildSourceSide)
        {
            LGuildOeuvre.LOeuvrePanel.LPanelDraftUpdate();
        }

        LGuildChanged?.Invoke();
    }

    private void LGuildStateUpdate()
    {
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

    private void LGuildAuthorOpen(long? id, bool editing)
    {
        LGuildAutograph.LDeskCancel();
        LGuildOeuvre.LOeuvrePanel.LPanelClear();
        LGuildPanel.LPanelScribeShow(editing && id is > 0);
        LGuildPanel.LPanelRowShow(id);
    }

    private void LGuildDraftStart(long id)
    {
        LGuildAutograph.LDeskStart(id);
    }

    private void LGuildStoredShow(long id)
    {
        _lGuildVista?.LVistaSelect(id);
        LGuildPanel.LPanelRowsUpdate();
        LGuildAutograph.LDeskStart(id);
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

        LGuildAutograph.LDeskCancel();
        LGuildPanel.LPanelScribeShow(false);
        LGuildOeuvre.LOeuvrePanel.LPanelRowShow(id);
    }

    public void LGuildFreshStart()
    {
        if (!LGuildLeaveConfirm())
        {
            return;
        }

        LGuildOeuvre.LOeuvrePanel.LPanelClear();
        LGuildPanel.LPanelFreshOpen();
        LGuildAutograph.LDeskStart(null);
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
            LGuildAutograph.LDeskCancel();
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
            LGuildAutograph.LDeskStart(LGuildAuthorId);
        }
    }

    private bool LGuildEditCheck(bool editing)
    {
        return !editing || LGuildAuthorHeld;
    }

    public bool LGuildSave()
    {
        if (!LGuildAutographShown)
        {
            return true;
        }

        if (!LGuildAutographNamed)
        {
            LGuildRefused?.Invoke("Guild.NameBlank");
            return false;
        }

        return LGuildAutograph.LDeskFinish(true);
    }

    public bool LGuildDraftFinish(bool store)
    {
        if (!LGuildAutographShown)
        {
            return true;
        }

        if (store)
        {
            return LGuildSave();
        }

        LGuildAutograph.LDeskCancel();
        return true;
    }

    public void LGuildUnionSelect(long? id)
    {
        if (id is not long kept)
        {
            return;
        }

        if (!LGuildAuthorHeld)
        {
            return;
        }

        if (!LGuildUnionConfirm(kept))
        {
            return;
        }

        try
        {
            _lEntryPort.LEngineAuthorAbsorb(kept, LGuildAuthorId);
        }
        catch (Exception exception)
        {
            LGuildFailed?.Invoke("Guild.MergeFailed", exception);
            return;
        }

        LGuildAuthorOpen(kept, false);
    }

    private bool LGuildUnionConfirm(long kept)
    {
        return _lGuildUnionSeam(
            LGuildAutograph.LDeskRead()?.LDraftAuthorName ?? string.Empty,
            _lEntryPort.LEngineAuthorFind(kept)?.LCatalogAuthorName ?? string.Empty);
    }

    public void LGuildDelete()
    {
        if (LGuildSourceSide)
        {
            return;
        }

        LGuildPanel.LPanelDelete();
    }

    public Task LGuildPortraitPrint(LPortraitLegend legend, LPressTicket ticket)
    {
        if (!LGuildSourceSide)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(LGuildOeuvre.LOeuvrePanel.LPanelVista, legend, ticket);
    }

    public void LGuildVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LGuildVistaRestore(
            window.LWindowVistaStart("guild", LSubject.LSubjectAuthor, LCatalogOrder.LCatalogOrderName),
            window.LWindowVistaStart("oeuvre", LSubject.LSubjectReference, LCatalogOrder.LCatalogOrderName));
    }
}
