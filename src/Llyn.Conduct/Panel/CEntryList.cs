using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CEntryList
{
    private readonly CAtelier _cEntryListAtelier;

    private readonly CEnvoy _cEntryListEnvoy;

    private readonly LSettingsPort _cEntryListSettings;

    private readonly LPortraitPort _cEntryListPortrait;

    private readonly Action<Action> _cEntryListMarshal;

    private readonly string _cEntryListScope;

    private readonly string _cEntryListName;

    private readonly Func<LVista, IReadOnlyList<LVistaRow>> _cEntryListSeam;

    internal CEntryList(
        CAtelier atelier,
        Func<bool> shownSeam,
        CEnvoy envoy,
        Action<Action> marshal,
        string scope,
        string list,
        Func<bool> allowed,
        Func<LVista, IReadOnlyList<LVistaRow>> seam)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentException.ThrowIfNullOrWhiteSpace(list);
        ArgumentNullException.ThrowIfNull(allowed);
        ArgumentNullException.ThrowIfNull(seam);

        _cEntryListAtelier = atelier;
        _cEntryListEnvoy = envoy;
        _cEntryListSettings = atelier.CAtelierSettingsPort;
        _cEntryListPortrait = atelier.CAtelierPortraitPort;
        _cEntryListMarshal = marshal;
        _cEntryListScope = scope;
        _cEntryListName = list.ToLowerInvariant();
        _cEntryListSeam = seam;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CEntryListEditor = editor;
        CEntryListPanel = new CPanel(
            envoy,
            _cEntryListSettings,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            scope + ".LoadFailed",
            "Scribe",
            editor.CEditorDesk.LDeskChangeCheck,
            editor.LEditorFinish,
            shownSeam,
            scope + "." + list + "Vacant",
            scope + "." + list + "Unmatched");
        CEntryListPanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CEntryListPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        editor.CEditorDisplay.CDisplayPanelAttach(CEntryListPanel);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            scope,
            CEntryListPanel.CPanelLeaveConfirm,
            CEntryListPanel.LPanelChosenRead,
            CEntryListPanel.LPanelScribeRestore,
            id => CEntryListPanel.CPanelRowOpen(id),
            allowed);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CEntryListPanel.LPanelChangeCheck, editor.LEditorFinish);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LEntryListClose);
        CEntryListPanel.LPanelStationAttach(atelier.CAtelierNavigation.LNavigationStationAdd);
    }

    public event Action? CEntryListChanged;

    public CEditor CEntryListEditor { get; }

    public CPanel CEntryListPanel { get; }

    public bool CEntryListEditing => CEntryListPanel.CPanelEditing;

    internal void LEntryListRestore()
    {
        LVista vista = _cEntryListAtelier.CAtelierVistaStart(
            _cEntryListName, CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        CEntryListPanel.CPanelVistaRestore(vista);
        CEntryListEditor.LEditorVistaRestore(vista);
    }

    internal void LEntryListAttach()
    {
        CAperture aperture = CEntryListPanel.CPanelAperture;
        aperture.CApertureObserverAttach(
            CSubject.CSubjectVista, _ => _cEntryListMarshal(aperture.CApertureRowsResonate));
        aperture.CApertureObserverAttach(
            CSubject.CSubjectEntry, bulletin => _cEntryListMarshal(() => LEntryListResonate(bulletin)));
        aperture.CApertureChosenAttach(
            CSubject.CSubjectEntry, _ => _cEntryListMarshal(CEntryListPanel.CPanelDraftResonate));
    }

    public IReadOnlyList<CVistaRow> CEntryListRead()
    {
        CAperture aperture = CEntryListPanel.CPanelAperture;
        IReadOnlyList<CVistaRow> rows;
        try
        {
            rows = aperture.CApertureVista is LVista vista
                ? _cEntryListSeam(vista).Select(CCatalog.LCatalogRowRead).ToList()
                : [];
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(
                _cEntryListEnvoy, _cEntryListSettings, _cEntryListScope + ".LoadFailed", exception);
            rows = [];
        }

        aperture.CApertureCountSet(rows.Count);
        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CEntryListLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cEntryListEnvoy, _cEntryListSettings, _cEntryListScope + ".LoadFailed", store, CEntryListRead);

    internal void LEntryListResonate()
    {
        CEntryListChanged?.Invoke();
        CEntryListPanel.CPanelAperture.CApertureRowsResonate();
    }

    private void LEntryListResonate(CBulletin bulletin)
    {
        CEntryListChanged?.Invoke();
        CEntryListPanel.CPanelEntryResonate(bulletin);
    }

    private void LEntryListClose()
    {
        CEntryListEditor.CEditorClose();
        CEntryListEditor.CEditorDisplay.CDisplayPlayback.CDisplayPlaybackCancel();
    }

    internal string LEntryListResolve()
    {
        return _cEntryListAtelier.CAtelierEntryBundle.CEntryBundleVista.LEngineFileRead(
            CEntryListPanel.CPanelAperture.CApertureVista);
    }

    public Task CEntryListPrint()
    {
        if (!CEntryListPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitTicketPrint(
            _cEntryListEnvoy,
            _cEntryListSettings,
            chosen => _cEntryListPortrait.LEnginePortraitPrint(
                CEntryListPanel.CPanelAperture.CApertureVista,
                CPortrait.LPortraitLabelRead(_cEntryListSettings),
                chosen));
    }

    public Task CEntryListExport()
    {
        return CPortrait.LPortraitFileExport(
            _cEntryListEnvoy,
            _cEntryListSettings,
            LEntryListResolve,
            (file, medium) => _cEntryListPortrait.LEnginePortraitExport(
                CEntryListPanel.CPanelAperture.CApertureVista,
                file,
                medium,
                CPortrait.LPortraitLabelRead(_cEntryListSettings)));
    }
}
