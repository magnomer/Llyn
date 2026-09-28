using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CFavorite
{
    private readonly CAtelier _cFavoriteAtelier;

    private readonly LEntryPort _cFavoriteEntryPort;

    private readonly LPortraitPort _cFavoritePortraitPort;

    private readonly CEnvoy _cFavoriteEnvoy;

    private readonly LSettingsPort _cFavoriteSettingsPort;

    private LVista? _cFavoriteVista;

    private CFavorite(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cFavoriteAtelier = atelier;
        _cFavoriteEntryPort = atelier.CAtelierEntryPort;
        _cFavoritePortraitPort = atelier.CAtelierPortraitPort;
        _cFavoriteEnvoy = envoy;
        _cFavoriteSettingsPort = atelier.CAtelierSettingsPort;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CFavoriteEditor = editor;
        CFavoritePanel = new CPanel(
            envoy, "Favorite.LoadFailed", "Scribe", editor.CEditorDesk.CDeskChangeCheck, editor.CEditorFinish,
            shownSeam);
        CFavoritePanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CFavoritePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Favorite",
            CFavoritePanel.CPanelLeaveConfirm,
            CFavoritePanel.LPanelChosenRead,
            CFavoritePanel.LPanelScribeRestore,
            id => CFavoritePanel.CPanelRowOpen(id));
        CFavoritePanel.LPanelStationAttach(atelier.CAtelierNavigation.LNavigationStationAdd);
    }

    public static CFavorite CFavoriteCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CFavorite(atelier, shownSeam, envoy);
    }

    public CEditor CFavoriteEditor { get; }

    public CPanel CFavoritePanel { get; }

    public bool CFavoriteFiltered => _cFavoriteVista?.LVistaFiltered ?? false;

    public void CFavoriteVistaRestore()
    {
        LVista vista = _cFavoriteAtelier.CAtelierVistaStart(
            "favorite", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        _cFavoriteVista = vista;
        CFavoritePanel.CPanelVistaRestore(vista);
        CFavoriteEditor.LEditorVistaRestore(vista);
    }

    public void CFavoriteGraspResonate()
    {
        if (CFavoritePanel.CPanelOrder == CCatalogOrder.CCatalogOrderGrasp)
        {
            CFavoritePanel.CPanelRowsResonate();
        }
    }

    public void CFavoriteQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cFavoriteVista?.LVistaQuerySet(query);
    }

    public void CFavoriteOrderSet(CCatalogOrder? order)
    {
        _cFavoriteVista?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CFavoriteFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cFavoriteVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public IReadOnlyList<CVistaRow> CFavoriteRowsRead()
    {
        return _cFavoriteVista is LVista vista
            ? _cFavoriteEntryPort.LEngineFavoriteFind(vista).Select(CPanel.CPanelRowRead).ToList()
            : [];
    }

    public IReadOnlyList<string> CFavoriteLanguageRead()
    {
        return _cFavoriteSettingsPort.LEngineLanguageRead();
    }

    internal string LFavoriteFileRead()
    {
        return LVista.LVistaFileRead(_cFavoriteVista);
    }

    public Task CFavoritePortraitPrint()
    {
        return CPortrait.LPortraitTicketPrint(
            _cFavoriteEnvoy,
            chosen => _cFavoritePortraitPort.LEnginePortraitPrint(
                _cFavoriteVista, CPortrait.LPortraitLabelRead(_cFavoriteSettingsPort), chosen));
    }

    public Task CFavoritePortraitExport()
    {
        return CPortrait.LPortraitFileExport(
            _cFavoriteEnvoy,
            LFavoriteFileRead(),
            (file, medium) => _cFavoritePortraitPort.LEnginePortraitExport(
                _cFavoriteVista, file, medium, CPortrait.LPortraitLabelRead(_cFavoriteSettingsPort)));
    }
}
