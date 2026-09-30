using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CPhonology
{
    private readonly CAtelier _cPhonologyAtelier;

    private readonly LPhonologyPort _cPhonologyPort;

    private readonly LPortraitPort _cPhonologyPortraitPort;

    private readonly LSettingsPort _cPhonologySettingsPort;

    private readonly CEnvoy _cPhonologyEnvoy;

    private readonly Action<Action> _cPhonologyMarshal;

    private LVista? _cPhonologyVista;

    private int _cPhonologyCount;

    private CPhonology(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cPhonologyAtelier = atelier;
        _cPhonologyPort = atelier.CAtelierPhonologyPort;
        _cPhonologyPortraitPort = atelier.CAtelierPortraitPort;
        _cPhonologySettingsPort = atelier.CAtelierSettingsPort;
        _cPhonologyEnvoy = envoy;
        _cPhonologyMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CPhonologyEditor = editor;
        CPhonologyPanel = new CPanel(
            envoy,
            _cPhonologySettingsPort,
            "Sound.LoadFailed",
            "Scribe",
            editor.CEditorDesk.LDeskChangeCheck,
            editor.LEditorFinish,
            shownSeam);
        CPhonologyPanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CPhonologyPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Phonology",
            CPhonologyPanel.CPanelLeaveConfirm,
            CPhonologyPanel.LPanelChosenRead,
            CPhonologyPanel.LPanelScribeRestore,
            id => CPhonologyPanel.CPanelRowOpen(id));
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CPhonologyPanel.LPanelChangeCheck, editor.LEditorFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LPhonologyVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LPhonologyClose);
        CPhonologyPanel.LPanelStationAttach(atelier.CAtelierNavigation.LNavigationStationAdd);
        LPhonologyVistaRestore();
    }

    public static CPhonology CPhonologyCreate(
        CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CPhonology(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CPhonologyWorkspaceChanged;

    public CEditor CPhonologyEditor { get; }

    public CPanel CPhonologyPanel { get; }

    public bool CPhonologyEmpty => _cPhonologyCount == 0;

    public bool CPhonologyFiltered => _cPhonologyVista?.LVistaFiltered ?? false;

    internal void LPhonologyVistaRestore()
    {
        LVista vista = _cPhonologyAtelier.CAtelierVistaStart(
            "phonology", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        vista.LVistaQuerySet(_cPhonologyVista?.LVistaQuery ?? string.Empty);
        _cPhonologyVista = vista;
        CPhonologyPanel.CPanelVistaRestore(vista);
        CPhonologyEditor.LEditorVistaRestore(vista);
        LPhonologyObserverAttach();
    }

    private void LPhonologyObserverAttach()
    {
        CPanel panel = CPhonologyPanel;
        Action<CBulletin> rows = _ => _cPhonologyMarshal(panel.CPanelRowsResonate);
        panel.CPanelObserverAttach(CSubject.CSubjectVista, rows);
        panel.CPanelObserverAttach(CSubject.CSubjectWorkspace, _ => _cPhonologyMarshal(LPhonologyWorkspaceResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, bulletin => _cPhonologyMarshal(() => panel.CPanelEntryResonate(bulletin)));
        panel.CPanelObserverAttach(CSubject.CSubjectReflex, rows);
        panel.CPanelObserverAttach(CSubject.CSubjectSettings, rows);
        panel.CPanelChosenAttach(CSubject.CSubjectEntry, _ => _cPhonologyMarshal(panel.CPanelDraftResonate));
    }

    private void LPhonologyWorkspaceResonate()
    {
        CPhonologyPanel.CPanelEntryClose();
        CPhonologyWorkspaceChanged?.Invoke();
    }

    private void LPhonologyClose()
    {
        CPhonologyEditor.CEditorClose();
        CPhonologyEditor.CEditorDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }

    public static IReadOnlyList<CCatalogOrder> CPhonologyOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderHeadword,
            CCatalogOrder.CCatalogOrderReverse,
            CCatalogOrder.CCatalogOrderSound,
            CCatalogOrder.CCatalogOrderPending,
        ];
    }

    public IReadOnlyList<CCatalogPronunciation> CPhonologyRowsRead()
    {
        IReadOnlyList<CCatalogPronunciation> rows = _cPhonologyVista is LVista vista
            ? _cPhonologyPort.LEnginePronunciationFind(vista)
                .Select(static row => new CCatalogPronunciation(
                    new CVistaRow(
                        row.LCatalogPronunciationEntry.LEntryId,
                        row.LCatalogPronunciationEntry.LEntryHeadword,
                        row.LCatalogPronunciationEntry.LEntryLanguage,
                        row.LCatalogPronunciationEpithet,
                        row.LCatalogPronunciationName,
                        row.LCatalogPronunciationChosen),
                    row.LCatalogPronunciationSound,
                    row.LCatalogPronunciationText))
                .ToList()
            : [];
        _cPhonologyCount = rows.Count;
        return rows;
    }

    public void CPhonologyQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cPhonologyVista?.LVistaQuerySet(query);
    }

    public void CPhonologyOrderSet(CCatalogOrder? order)
    {
        _cPhonologyVista?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CPhonologyFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cPhonologyVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    internal string LPhonologyFileRead()
    {
        return LVista.LVistaFileRead(_cPhonologyVista);
    }

    public Task CPhonologyPortraitPrint()
    {
        return CPortrait.LPortraitTicketPrint(
            _cPhonologyEnvoy,
            _cPhonologySettingsPort,
            chosen => _cPhonologyPortraitPort.LEnginePortraitPrint(
                _cPhonologyVista, CPortrait.LPortraitLabelRead(_cPhonologySettingsPort), chosen));
    }

    public Task CPhonologyPortraitExport()
    {
        return CPortrait.LPortraitFileExport(
            _cPhonologyEnvoy,
            _cPhonologySettingsPort,
            LPhonologyFileRead(),
            (file, medium) => _cPhonologyPortraitPort.LEnginePortraitExport(
                _cPhonologyVista, file, medium, CPortrait.LPortraitLabelRead(_cPhonologySettingsPort)));
    }
}
