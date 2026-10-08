using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CAtlas
{
    private readonly LSituationPort _cAtlasSituationPort;

    private readonly LPortraitPort _cAtlasPortraitPort;

    private readonly LSettingsPort _cAtlasSettingsPort;

    private readonly CEnvoy _cAtlasEnvoy;

    private LVista? _cAtlasVista;

    internal CAtlas(
        LSituationPort situations,
        LPortraitPort portraits,
        LSettingsPort settings,
        LVistaPort vistas,
        CDesk desk,
        Func<bool> shownSeam,
        CEnvoy envoy,
        Func<bool, bool> finishSeam)
    {
        ArgumentNullException.ThrowIfNull(situations);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(envoy);

        _cAtlasEnvoy = envoy;
        _cAtlasSituationPort = situations;
        _cAtlasPortraitPort = portraits;
        _cAtlasSettingsPort = settings;
        CAtlasPanel = new CPanel(
            envoy,
            settings,
            vistas,
            "Situation.LoadFailed", "Situation", desk.LDeskChangeCheck, finishSeam, shownSeam);
    }

    public CPanel CAtlasPanel { get; }

    public long? CAtlasChosen => _cAtlasVista?.LVistaChosen;

    public bool CAtlasFiltered => _cAtlasVista?.LVistaFiltered ?? false;

    internal bool LAtlasNarrowed => _cAtlasVista?.LVistaNarrowed ?? false;

    internal void LAtlasVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        vista.LVistaQuerySet(_cAtlasVista?.LVistaQuery ?? string.Empty);
        _cAtlasVista = vista;
        CAtlasPanel.CPanelVistaRestore(vista);
    }

    internal void LAtlasObserverAttach(Action<Action> marshal, Action workspace)
    {
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentNullException.ThrowIfNull(workspace);

        Action<CBulletin> rows = _ => marshal(CAtlasPanel.CPanelRowsResonate);
        CAtlasPanel.CPanelObserverAttach(CSubject.CSubjectVista, rows);
        CAtlasPanel.CPanelObserverAttach(CSubject.CSubjectWorkspace, _ => marshal(workspace));
        CAtlasPanel.CPanelObserverAttach(CSubject.CSubjectSituation, rows);
        CAtlasPanel.CPanelObserverAttach(CSubject.CSubjectReflex, rows);
        CAtlasPanel.CPanelObserverAttach(CSubject.CSubjectSettings, rows);
    }

    public void CAtlasQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cAtlasVista?.LVistaQuerySet(query);
    }

    public void CAtlasOrderSet(CCatalogOrder? order)
    {
        _cAtlasVista?.LVistaOrderSet(CCatalog.LCatalogOrderRead(order));
    }

    public static IReadOnlyList<CCatalogOrder> CAtlasOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderName,
            CCatalogOrder.CCatalogOrderKind,
            CCatalogOrder.CCatalogOrderUsage,
        ];
    }

    public void CAtlasFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cAtlasVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    internal IReadOnlyList<CCatalogSituation>? LAtlasRowsRead()
    {
        if (_cAtlasVista is not LVista vista)
        {
            return [];
        }

        try
        {
            return _cAtlasSituationPort
                .LEngineSituationFind(
                    vista,
                    _cAtlasSettingsPort.LEngineTextRead("Display.Unknown"),
                    _cAtlasSettingsPort.LEngineTextRead("Situation.Untitled"))
                .Select(LAtlasRowRead)
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cAtlasEnvoy, _cAtlasSettingsPort, "Situation.LoadFailed", exception);
            return null;
        }
    }

    public IReadOnlyList<string> CAtlasLanguageRead()
    {
        return _cAtlasSettingsPort.LEngineLanguageRead();
    }

    internal Task LAtlasPortraitPrint(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitTicketPrint(
            envoy,
            settings,
            chosen => _cAtlasPortraitPort.LEnginePortraitPrint(
                _cAtlasVista, CPortrait.LPortraitLegendRead(settings, "Situation"), chosen));
    }

    internal static CCatalogSituation LAtlasRowRead(LCatalogSituation row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new CCatalogSituation(
            row.LCatalogSituationStored.LSituationId,
            row.LCatalogSituationName,
            row.LCatalogSituationCount,
            row.LCatalogSituationKind,
            row.LCatalogSituationChosen);
    }

    internal static CSituationDraft? LAtlasDraftRead(LDraft? draft, LMediaPort media)
    {
        return draft?.LDraftSituation is LSituation situation
            ? new CSituationDraft(
                situation.LSituationId,
                CFolio.CFolioStateRead(situation.LSituationTitle),
                CFolio.CFolioStateRead(situation.LSituationKind),
                CFolio.CFolioStateRead(situation.LSituationDescription),
                CFolio.CFolioImageRead(situation.LSituationImage, media),
                CFolio.CFolioVideoRead(situation.LSituationVideo, media))
            : null;
    }

    internal static CSituation? LAtlasSituationRead(LDraft? draft, LMediaPort media, LMarkdownPort markdown)
    {
        if (draft?.LDraftSituation is not LSituation situation)
        {
            return null;
        }

        CStateValue description = CFolio.CFolioStateRead(situation.LSituationDescription);
        return new CSituation(
            CStateWording.LStateWordingRead(CFolio.CFolioStateRead(situation.LSituationTitle), "Situation.Untitled"),
            CStateWording.LStateWordingRead(CFolio.CFolioStateRead(situation.LSituationKind), null),
            CStateWording.LStateWordingRead(description, null),
            CMarkdown.LMarkdownParse(markdown, description.CStateValueText),
            CFolio.CFolioImageRead(situation.LSituationImageFilled, media),
            CFolio.CFolioVideoRead(situation.LSituationVideoFilled, media));
    }
}
