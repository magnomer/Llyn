using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CPhonology
{
    private readonly CAtelier _cPhonologyAtelier;

    private readonly LPronunciationPort _cPhonologyPort;

    private readonly LPortraitPort _cPhonologyPortraitPort;

    private readonly LSettingsPort _cPhonologySettingsPort;

    private readonly CEnvoy _cPhonologyEnvoy;

    private readonly Action<Action> _cPhonologyMarshal;

    private CPhonology(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cPhonologyAtelier = atelier;
        _cPhonologyPort = atelier.CAtelierEntryBundle.CEntryBundlePronunciation;
        _cPhonologyPortraitPort = atelier.CAtelierPortraitPort;
        _cPhonologySettingsPort = atelier.CAtelierSettingsPort;
        _cPhonologyEnvoy = envoy;
        _cPhonologyMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CPhonologyEditor = editor;
        CPhonologyPanel = new CPanel(
            envoy,
            _cPhonologySettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
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

    internal void LPhonologyVistaRestore()
    {
        LVista vista = _cPhonologyAtelier.CAtelierVistaStart(
            "phonology", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        CPhonologyPanel.CPanelVistaRestore(vista);
        CPhonologyEditor.LEditorVistaRestore(vista);
        LPhonologyObserverAttach();
    }

    private void LPhonologyObserverAttach()
    {
        CPanel panel = CPhonologyPanel;
        Action<CBulletin> rows = _ => _cPhonologyMarshal(panel.CPanelAperture.CApertureRowsResonate);
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectVista, rows);
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectWorkspace, _ => _cPhonologyMarshal(LPhonologyWorkspaceResonate));
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectEntry, bulletin => _cPhonologyMarshal(() => panel.CPanelEntryResonate(bulletin)));
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectReflex, rows);
        panel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectSettings, rows);
        panel.CPanelAperture.CApertureChosenAttach(
            CSubject.CSubjectEntry, _ => _cPhonologyMarshal(panel.CPanelDraftResonate));
    }

    private void LPhonologyWorkspaceResonate()
    {
        CPhonologyPanel.CPanelEntryClose();
        CPhonologyWorkspaceChanged?.Invoke();
    }

    private void LPhonologyClose()
    {
        CPhonologyEditor.CEditorClose();
        CPhonologyEditor.CEditorDisplay.CDisplayPlayback.CDisplayPlaybackCancel();
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
        IReadOnlyList<CCatalogPronunciation> rows;
        try
        {
            rows = CPhonologyPanel.CPanelAperture.CApertureVista is LVista vista
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
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cPhonologyEnvoy, _cPhonologySettingsPort, "Sound.LoadFailed", exception);
            rows = [];
        }

        CPhonologyPanel.CPanelAperture.CApertureCountSet(rows.Count);
        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CCatalogPronunciation>>> CPhonologyRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cPhonologyEnvoy, _cPhonologySettingsPort, "Sound.LoadFailed", store, CPhonologyRowsRead);

    internal string LPhonologyFileRead()
    {
        return _cPhonologyAtelier.CAtelierEntryBundle.CEntryBundleVista.LEngineFileRead(
            CPhonologyPanel.CPanelAperture.CApertureVista);
    }

    public Task CPhonologyPortraitPrint()
    {
        return CPortrait.LPortraitTicketPrint(
            _cPhonologyEnvoy,
            _cPhonologySettingsPort,
            chosen => _cPhonologyPortraitPort.LEnginePortraitPrint(
                CPhonologyPanel.CPanelAperture.CApertureVista,
                CPortrait.LPortraitLabelRead(_cPhonologySettingsPort),
                chosen));
    }

    public Task CPhonologyPortraitExport()
    {
        return CPortrait.LPortraitFileExport(
            _cPhonologyEnvoy,
            _cPhonologySettingsPort,
            LPhonologyFileRead,
            (file, medium) => _cPhonologyPortraitPort.LEnginePortraitExport(
                CPhonologyPanel.CPanelAperture.CApertureVista,
                file,
                medium,
                CPortrait.LPortraitLabelRead(_cPhonologySettingsPort)));
    }
}
