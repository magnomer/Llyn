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

    private readonly Action<Action> _cFavoriteMarshal;

    private LVista? _cFavoriteVista;

    private CFavorite(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(marshal);

        _cFavoriteAtelier = atelier;
        _cFavoriteEntryPort = atelier.CAtelierEntryPort;
        _cFavoritePortraitPort = atelier.CAtelierPortraitPort;
        _cFavoriteEnvoy = envoy;
        _cFavoriteSettingsPort = atelier.CAtelierSettingsPort;
        _cFavoriteMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CFavoriteEditor = editor;
        CFavoritePanel = new CPanel(
            envoy,
            _cFavoriteSettingsPort,
            "Favorite.LoadFailed", "Scribe", editor.CEditorDesk.LDeskChangeCheck, editor.LEditorFinish,
            shownSeam);
        CFavoritePanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CFavoritePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Favorite",
            CFavoritePanel.CPanelLeaveConfirm,
            CFavoritePanel.LPanelChosenRead,
            CFavoritePanel.LPanelScribeRestore,
            id => CFavoritePanel.CPanelRowOpen(id));
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CFavoritePanel.LPanelChangeCheck, editor.LEditorFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LFavoriteVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LFavoriteClose);
        CFavoritePanel.LPanelStationAttach(atelier.CAtelierNavigation.LNavigationStationAdd);
        LFavoriteVistaRestore();
    }

    public static CFavorite CFavoriteCreate(
        CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CFavorite(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CFavoriteWorkspaceChanged;

    public CEditor CFavoriteEditor { get; }

    public CPanel CFavoritePanel { get; }

    public bool CFavoriteFiltered => _cFavoriteVista?.LVistaFiltered ?? false;

    internal void LFavoriteVistaRestore()
    {
        LVista vista = _cFavoriteAtelier.CAtelierVistaStart(
            "favorite", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        vista.LVistaQuerySet(_cFavoriteVista?.LVistaQuery ?? string.Empty);
        _cFavoriteVista = vista;
        CFavoritePanel.CPanelVistaRestore(vista);
        CFavoriteEditor.LEditorVistaRestore(vista);
        LFavoriteObserverAttach();
    }

    private void LFavoriteObserverAttach()
    {
        CPanel panel = CFavoritePanel;
        Action<CBulletin> rows = _ => _cFavoriteMarshal(panel.CPanelRowsResonate);
        panel.CPanelObserverAttach(CSubject.CSubjectVista, rows);
        panel.CPanelObserverAttach(CSubject.CSubjectWorkspace, _ => _cFavoriteMarshal(LFavoriteWorkspaceResonate));
        panel.CPanelObserverAttach(CSubject.CSubjectGrasp, _ => _cFavoriteMarshal(LFavoriteGraspResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, bulletin => _cFavoriteMarshal(() => panel.CPanelEntryResonate(bulletin)));
        panel.CPanelObserverAttach(CSubject.CSubjectFavorite, rows);
        panel.CPanelObserverAttach(CSubject.CSubjectReflex, rows);
        panel.CPanelObserverAttach(CSubject.CSubjectSettings, rows);
        panel.CPanelChosenAttach(CSubject.CSubjectEntry, _ => _cFavoriteMarshal(panel.CPanelDraftResonate));
    }

    private void LFavoriteWorkspaceResonate()
    {
        CFavoritePanel.CPanelEntryClose();
        CFavoriteWorkspaceChanged?.Invoke();
    }

    private void LFavoriteClose()
    {
        CFavoriteEditor.CEditorClose();
        CFavoriteEditor.CEditorDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }

    private void LFavoriteGraspResonate()
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

    public static IReadOnlyList<CCatalogOrder> CFavoriteOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderHeadword,
            CCatalogOrder.CCatalogOrderReverse,
            CCatalogOrder.CCatalogOrderLanguage,
            CCatalogOrder.CCatalogOrderMarked,
            CCatalogOrder.CCatalogOrderGrasp,
        ];
    }

    public IReadOnlyList<CVistaRow> CFavoriteRowsRead()
    {
        if (_cFavoriteVista is not LVista vista)
        {
            return [];
        }

        try
        {
            return _cFavoriteEntryPort.LEngineFavoriteFind(vista).Select(CPanel.CPanelRowRead).ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cFavoriteEnvoy, _cFavoriteSettingsPort, "Favorite.LoadFailed", exception);
            return [];
        }
    }

    public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CFavoriteRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(_cFavoriteSettingsPort, store, CFavoriteRowsRead);

    internal string LFavoriteFileRead()
    {
        return LVista.LVistaFileRead(_cFavoriteVista);
    }

    public Task CFavoritePortraitPrint()
    {
        return CPortrait.LPortraitTicketPrint(
            _cFavoriteEnvoy,
            _cFavoriteSettingsPort,
            chosen => _cFavoritePortraitPort.LEnginePortraitPrint(
                _cFavoriteVista, CPortrait.LPortraitLabelRead(_cFavoriteSettingsPort), chosen));
    }

    public Task CFavoritePortraitExport()
    {
        return CPortrait.LPortraitFileExport(
            _cFavoriteEnvoy,
            _cFavoriteSettingsPort,
            LFavoriteFileRead(),
            (file, medium) => _cFavoritePortraitPort.LEnginePortraitExport(
                _cFavoriteVista, file, medium, CPortrait.LPortraitLabelRead(_cFavoriteSettingsPort)));
    }
}
