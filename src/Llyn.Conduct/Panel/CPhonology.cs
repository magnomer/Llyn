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

    private LVista? _cPhonologyVista;

    private int _cPhonologyCount;

    private CPhonology(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cPhonologyAtelier = atelier;
        _cPhonologyPort = atelier.CAtelierPhonologyPort;
        _cPhonologyPortraitPort = atelier.CAtelierPortraitPort;
        _cPhonologySettingsPort = atelier.CAtelierSettingsPort;
        _cPhonologyEnvoy = envoy;
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
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(CPhonologyVistaRestore);
        CPhonologyPanel.LPanelStationAttach(atelier.CAtelierNavigation.LNavigationStationAdd);
    }

    public static CPhonology CPhonologyCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CPhonology(atelier, shownSeam, envoy);
    }

    public CEditor CPhonologyEditor { get; }

    public CPanel CPhonologyPanel { get; }

    public bool CPhonologyEmpty => _cPhonologyCount == 0;

    public bool CPhonologyFiltered => _cPhonologyVista?.LVistaFiltered ?? false;

    public void CPhonologyVistaRestore()
    {
        LVista vista = _cPhonologyAtelier.CAtelierVistaStart(
            "phonology", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        _cPhonologyVista = vista;
        CPhonologyPanel.CPanelVistaRestore(vista);
        CPhonologyEditor.LEditorVistaRestore(vista);
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
                    row.LCatalogPronunciationSound))
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
