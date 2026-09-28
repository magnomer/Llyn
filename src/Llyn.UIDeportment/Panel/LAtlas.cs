using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
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

    internal LAtlas(
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

    internal void LAtlasVistaRestore(LVista vista)
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

    public void LAtlasTierSet(CCatalogOrder? order)
    {
        if (_lAtlasVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LPanel.LPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LAtlasMeshSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lAtlasVista?.LVistaFilterSet(LPanel.LPanelFilterRead(filter));
    }

    public IReadOnlyList<CCatalogSituation> LAtlasRowsRead(string unknown, string untitled)
    {
        return _lAtlasVista is LVista vista
            ? LSplice.LSpliceBuild(_lEntryPort.LEngineSituationFind(vista, unknown, untitled), CAtlas.CAtlasRowRead)
            : [];
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

    public Task LAtlasPortraitPrint(CPortraitLegend legend, CPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(
            _lAtlasVista, LAtlasLegendRead(legend), QPortrait.QPortraitTicketRead(ticket));
    }

    internal static LPortraitLegend LAtlasLegendRead(CPortraitLegend legend)
    {
        ArgumentNullException.ThrowIfNull(legend);

        Dictionary<LReferenceKind, string> kinds = [];
        foreach (LReferenceKind kind in Enum.GetValues<LReferenceKind>())
        {
            kinds[kind] = legend.CPortraitLegendKind.GetValueOrDefault(
                LReference.LReferenceKindResolve(kind), LReference.LReferenceKindFormat(kind));
        }

        return new LPortraitLegend(
            legend.CPortraitLegendUnknown,
            legend.CPortraitLegendUntitled,
            legend.CPortraitLegendUnwritten,
            legend.CPortraitLegendUnused,
            legend.CPortraitLegendOnce,
            legend.CPortraitLegendUses,
            legend.CPortraitLegendTranslation,
            legend.CPortraitLegendSource,
            legend.CPortraitLegendAuthor,
            legend.CPortraitLegendYear,
            legend.CPortraitLegendUrl,
            legend.CPortraitLegendNote,
            legend.CPortraitLegendDescription,
            kinds);
    }

    internal static CSituationDraft? LAtlasSituationRead(LSituation? situation)
    {
        return situation is null
            ? null
            : new CSituationDraft(
                situation.LSituationId,
                CFolio.CFolioStateRead(situation.LSituationTitle),
                CFolio.CFolioStateRead(situation.LSituationKind),
                CFolio.CFolioStateRead(situation.LSituationDescription),
                CFolio.CFolioImageRead(situation.LSituationImage),
                CFolio.CFolioVideoRead(situation.LSituationVideo));
    }
}
