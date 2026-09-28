using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CAtlas
{
    private readonly LEntryPort _cAtlasEntryPort;

    private readonly LPortraitPort _cAtlasPortraitPort;

    private readonly LSettingsPort _cAtlasSettingsPort;

    private LVista? _cAtlasVista;

    internal CAtlas(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        CDesk desk,
        Func<bool> shownSeam,
        CEnvoy envoy,
        Func<bool, bool> finishSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(desk);

        _cAtlasEntryPort = entries;
        _cAtlasPortraitPort = portraits;
        _cAtlasSettingsPort = settings;
        CAtlasPanel = new CPanel(
            envoy, "Situation.LoadFailed", "Situation", desk.CDeskChangeCheck, finishSeam, shownSeam);
    }

    public static CAtlas CAtlasCreate(
        CAtelier atelier, CDesk desk, Func<bool> shownSeam, CEnvoy envoy, Func<bool, bool> finishSeam)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        return new CAtlas(
            atelier.CAtelierEntryPort,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            desk,
            shownSeam,
            envoy,
            finishSeam);
    }

    public CPanel CAtlasPanel { get; }

    public long? CAtlasChosen => _cAtlasVista?.LVistaChosen;

    public bool CAtlasFiltered => _cAtlasVista?.LVistaFiltered ?? false;

    public bool CAtlasNarrowed => _cAtlasVista?.LVistaNarrowed ?? false;

    internal void CAtlasVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _cAtlasVista = vista;
        CAtlasPanel.CPanelVistaRestore(vista);
    }

    public void CAtlasQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cAtlasVista?.LVistaQuerySet(query);
    }

    public void CAtlasOrderSet(CCatalogOrder? order)
    {
        _cAtlasVista?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CAtlasFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cAtlasVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public IReadOnlyList<CCatalogSituation> CAtlasRowsRead(string unknown, string untitled)
    {
        return _cAtlasVista is LVista vista
            ? _cAtlasEntryPort.LEngineSituationFind(vista, unknown, untitled).Select(CAtlasRowRead).ToList()
            : [];
    }

    public IReadOnlyDictionary<long, int> CAtlasUsageRead()
    {
        return _cAtlasEntryPort.LEngineUsageRead(LOwner.LOwnerSituation);
    }

    public IReadOnlyList<string> CAtlasLanguageRead()
    {
        return _cAtlasSettingsPort.LEngineLanguageRead();
    }

    public Task CAtlasPortraitPrint(CPortraitLegend legend, CPressTicket ticket)
    {
        return _cAtlasPortraitPort.LEnginePortraitPrint(
            _cAtlasVista, CPortrait.CPortraitLegendRead(legend), CPortrait.CPortraitTicketRead(ticket));
    }

    internal static CCatalogSituation CAtlasRowRead(LCatalogSituation row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new CCatalogSituation(
            row.LCatalogSituationStored.LSituationId,
            row.LCatalogSituationName,
            row.LCatalogSituationUsage,
            CFolio.CFolioStateRead(row.LCatalogSituationStored.LSituationKind),
            row.LCatalogSituationChosen);
    }

    internal static CSituationDraft? CAtlasSituationRead(LSituation? situation)
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
