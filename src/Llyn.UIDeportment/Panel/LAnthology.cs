using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LAnthology
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private readonly Func<int, bool> _lAnthologyRemovalSeam;

    private readonly LDesk _lAnthologyDesk;

    private LVista? _lAnthologyVista;

    public LAnthology(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        LDesk desk,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(removalSeam);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        _lAnthologyDesk = desk;
        _lAnthologyRemovalSeam = removalSeam;
        LAnthologyPanel = new LPanel(
            "Example.LoadFailed", "Example.DeleteFailed",
            desk.LDeskChangeCheck, shownSeam, leaveSeam, LAnthologyDeleteConfirm);
    }

    public LPanel LAnthologyPanel { get; }

    public long? LAnthologyChosen => _lAnthologyVista?.LVistaChosen;

    public bool LAnthologyFiltered => _lAnthologyVista?.LVistaFiltered ?? false;

    public bool LAnthologyNarrowed => _lAnthologyVista?.LVistaNarrowed ?? false;

    public void LAnthologyVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lAnthologyVista = vista;
        LAnthologyPanel.LPanelVistaRestore(vista);
    }

    public void LAnthologyQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lAnthologyVista?.LVistaQuerySet(query);
    }

    public void LAnthologyRankSet(LCatalogOrder? order)
    {
        if (_lAnthologyVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LAnthologyGauzeSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lAnthologyVista?.LVistaFilterSet(filter);
    }

    public IReadOnlyList<LCatalogExample> LAnthologyRowsRead(string unknown, string unwritten)
    {
        return _lAnthologyVista is LVista vista ? _lEntryPort.LEngineExampleFind(vista, unknown, unwritten) : [];
    }

    public IReadOnlyDictionary<long, int> LAnthologyUsageRead()
    {
        return _lEntryPort.LEngineUsageRead(LOwner.LOwnerExample);
    }

    private int LAnthologyUsageRead(long? id)
    {
        return id is long stored ? LAnthologyUsageRead().GetValueOrDefault(stored) : 0;
    }

    private bool LAnthologyDeleteConfirm()
    {
        return _lAnthologyRemovalSeam(LAnthologyUsageRead(LAnthologyChosen));
    }

    public LMentionResult LAnthologyMentionFind(long id, int offset)
    {
        return _lEntryPort.LEngineMentionFind(id, offset);
    }

    public IReadOnlyList<string> LAnthologyLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LAnthologyPortraitPrint(LPortraitLegend legend, LPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(_lAnthologyVista, legend, ticket);
    }

    public IReadOnlyList<LCatalogReference> LAnthologyReferenceFind()
    {
        return _lEntryPort.LEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderAuthor);
    }

    public IReadOnlyList<LCatalogReference> LAnthologyReferenceFind(string word)
    {
        return _lEntryPort.LEngineReferenceFind(word, LCatalogOrder.LCatalogOrderUsage);
    }

    public void LAnthologyCitationSet(string title)
    {
        _lAnthologyDesk.LDeskSend(new LRequestExampleReference(
            _lAnthologyDesk.LDeskId, _lEntryPort.LEngineCitationResolve(_lAnthologyDesk.LDeskId, 0, 0, title)));
    }
}
