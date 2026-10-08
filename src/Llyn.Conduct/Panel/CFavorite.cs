using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CFavorite
{
    private readonly CAtelier _cFavoriteAtelier;

    private readonly LFavoritePort _cFavoritePort;

    private readonly LPortraitPort _cFavoritePortraitPort;

    private readonly CEnvoy _cFavoriteEnvoy;

    private readonly LSettingsPort _cFavoriteSettingsPort;

    private readonly Action<Action> _cFavoriteMarshal;

    private CFavorite(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(marshal);

        _cFavoriteAtelier = atelier;
        _cFavoritePort = atelier.CAtelierEntryBundle.CEntryBundleFavorite;
        _cFavoritePortraitPort = atelier.CAtelierPortraitPort;
        _cFavoriteEnvoy = envoy;
        _cFavoriteSettingsPort = atelier.CAtelierSettingsPort;
        _cFavoriteMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CFavoriteEditor = editor;
        CFavoritePanel = new CPanel(
            envoy,
            _cFavoriteSettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            "Favorite.LoadFailed", "Scribe", editor.CEditorDesk.LDeskChangeCheck, editor.LEditorFinish,
            shownSeam);
        CFavoritePanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CFavoritePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        editor.CEditorDisplay.CDisplayPanelAttach(CFavoritePanel);
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

    internal void LFavoriteVistaRestore()
    {
        LVista vista = _cFavoriteAtelier.CAtelierVistaStart(
            "favorite", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        CFavoritePanel.CPanelVistaRestore(vista);
        CFavoriteEditor.LEditorVistaRestore(vista);
        LFavoriteObserverAttach();
    }

    private void LFavoriteObserverAttach()
    {
        CPanel panel = CFavoritePanel;
        Action<CBulletin> rows = _ => _cFavoriteMarshal(panel.CPanelAperture.CApertureRowsResonate);
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectVista, rows);
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectWorkspace, _ => _cFavoriteMarshal(LFavoriteWorkspaceResonate));
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectGrasp, _ => _cFavoriteMarshal(LFavoriteGraspResonate));
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectEntry, bulletin => _cFavoriteMarshal(() => panel.CPanelEntryResonate(bulletin)));
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectFavorite, rows);
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectReflex, rows);
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectSettings, rows);
        panel.CPanelAperture.CApertureChosenAttach(
            CSubject.CSubjectEntry, _ => _cFavoriteMarshal(panel.CPanelDraftResonate));
    }

    private void LFavoriteWorkspaceResonate()
    {
        CFavoritePanel.CPanelEntryClose();
        CFavoriteWorkspaceChanged?.Invoke();
    }

    private void LFavoriteClose()
    {
        CFavoriteEditor.CEditorClose();
        CFavoriteEditor.CEditorDisplay.CDisplayPlayback.CDisplayPlaybackCancel();
    }

    private void LFavoriteGraspResonate()
    {
        if (CFavoritePanel.CPanelAperture.CApertureOrder == CCatalogOrder.CCatalogOrderGrasp)
        {
            CFavoritePanel.CPanelAperture.CApertureRowsResonate();
        }
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
        if (CFavoritePanel.CPanelAperture.CApertureVista is not LVista vista)
        {
            return [];
        }

        try
        {
            return _cFavoritePort.LEngineFavoriteFind(vista).Select(CCatalog.LCatalogRowRead).ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cFavoriteEnvoy, _cFavoriteSettingsPort, "Favorite.LoadFailed", exception);
            return [];
        }
    }

    public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CFavoriteRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cFavoriteEnvoy, _cFavoriteSettingsPort, "Favorite.LoadFailed", store, CFavoriteRowsRead);

    internal string LFavoriteFileRead()
    {
        return _cFavoriteAtelier.CAtelierEntryBundle.CEntryBundleVista.LEngineFileRead(
            CFavoritePanel.CPanelAperture.CApertureVista);
    }

    public Task CFavoritePortraitPrint()
    {
        return CPortrait.LPortraitTicketPrint(
            _cFavoriteEnvoy,
            _cFavoriteSettingsPort,
            chosen => _cFavoritePortraitPort.LEnginePortraitPrint(
                CFavoritePanel.CPanelAperture.CApertureVista,
                CPortrait.LPortraitLabelRead(_cFavoriteSettingsPort),
                chosen));
    }

    public Task CFavoritePortraitExport()
    {
        return CPortrait.LPortraitFileExport(
            _cFavoriteEnvoy,
            _cFavoriteSettingsPort,
            LFavoriteFileRead,
            (file, medium) => _cFavoritePortraitPort.LEnginePortraitExport(
                CFavoritePanel.CPanelAperture.CApertureVista,
                file,
                medium,
                CPortrait.LPortraitLabelRead(_cFavoriteSettingsPort)));
    }
}
