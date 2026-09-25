using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LAtlas
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private readonly Func<int, bool> _lAtlasRemovalSeam;

    private LVista? _lAtlasVista;

    public LAtlas(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        Func<bool> changeSeam,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(removalSeam);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        _lAtlasRemovalSeam = removalSeam;
        LAtlasPanel = new LPanel(
            "Situation.LoadFailed", "Situation.DeleteFailed",
            changeSeam, shownSeam, leaveSeam, LAtlasDeleteConfirm);
    }

    public LPanel LAtlasPanel { get; }

    public long? LAtlasChosen => _lAtlasVista?.LVistaChosen;

    public bool LAtlasFiltered => _lAtlasVista?.LVistaFiltered ?? false;

    public bool LAtlasNarrowed => _lAtlasVista?.LVistaNarrowed ?? false;

    public void LAtlasVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lAtlasVista = vista;
        LAtlasPanel.LPanelVistaRestore(vista);
    }

    public void LAtlasInquestSet(string inquest)
    {
        ArgumentNullException.ThrowIfNull(inquest);

        _lAtlasVista?.LVistaQuerySet(inquest);
    }

    public void LAtlasTierSet(LCatalogOrder? order)
    {
        if (_lAtlasVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LAtlasMeshSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lAtlasVista?.LVistaFilterSet(filter);
    }

    public IReadOnlyList<LCatalogSituation> LAtlasRowsRead(string unknown, string untitled)
    {
        return _lAtlasVista is LVista vista ? _lEntryPort.LEngineSituationFind(vista, unknown, untitled) : [];
    }

    public IReadOnlyDictionary<long, int> LAtlasUsageRead()
    {
        return _lEntryPort.LEngineUsageRead(LOwner.LOwnerSituation);
    }

    private int LAtlasUsageRead(long? id)
    {
        return id is long stored ? LAtlasUsageRead().GetValueOrDefault(stored) : 0;
    }

    private bool LAtlasDeleteConfirm()
    {
        return _lAtlasRemovalSeam(LAtlasUsageRead(LAtlasChosen));
    }

    public IReadOnlyList<string> LAtlasLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LAtlasPortraitPrint(LPortraitLegend legend, LPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(_lAtlasVista, legend, ticket);
    }
}
