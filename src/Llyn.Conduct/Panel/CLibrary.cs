using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CLibrary
{
    private readonly CAtelier _cLibraryAtelier;

    private readonly LEntryPort _cLibraryEntryPort;

    private readonly LPortraitPort _cLibraryPortraitPort;

    private readonly LSettingsPort _cLibrarySettingsPort;

    private readonly CEnvoy _cLibraryEnvoy;

    private readonly Action<Action> _cLibraryMarshal;

    private LVista? _cLibraryVista;

    private int _cLibraryCount;

    private CLibrary(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cLibraryAtelier = atelier;
        _cLibraryEntryPort = atelier.CAtelierEntryPort;
        _cLibraryPortraitPort = atelier.CAtelierPortraitPort;
        _cLibrarySettingsPort = atelier.CAtelierSettingsPort;
        _cLibraryEnvoy = envoy;
        _cLibraryMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CLibraryEditor = editor;
        CLibraryPanel = new CPanel(
            envoy,
            _cLibrarySettingsPort,
            "List.LoadFailed", "Scribe", editor.CEditorDesk.LDeskChangeCheck, editor.LEditorFinish,
            shownSeam);
        CLibraryPanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CLibraryPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Library",
            CLibraryPanel.CPanelLeaveConfirm,
            CLibraryPanel.LPanelChosenRead,
            CLibraryPanel.LPanelScribeRestore,
            id => CLibraryPanel.CPanelRowOpen(id));
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CLibraryPanel.LPanelChangeCheck, editor.LEditorFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LLibraryVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LLibraryClose);
        CLibraryPanel.LPanelStationAttach(atelier.CAtelierNavigation.LNavigationStationAdd);
        LLibraryVistaRestore();
    }

    public static CLibrary CLibraryCreate(
        CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CLibrary(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CLibraryWorkspaceChanged;

    public CEditor CLibraryEditor { get; }

    public CPanel CLibraryPanel { get; }

    public bool CLibraryFiltered => _cLibraryVista?.LVistaFiltered ?? false;

    public bool CLibraryEmpty => _cLibraryCount == 0;

    internal void LLibraryVistaRestore()
    {
        LVista vista = _cLibraryAtelier.CAtelierVistaStart(
            "library", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        vista.LVistaQuerySet(_cLibraryVista?.LVistaQuery ?? string.Empty);
        _cLibraryVista = vista;
        CLibraryPanel.CPanelVistaRestore(vista);
        CLibraryEditor.LEditorVistaRestore(vista);
        LLibraryObserverAttach();
    }

    private void LLibraryObserverAttach()
    {
        CPanel panel = CLibraryPanel;
        Action<CBulletin> rows = _ => _cLibraryMarshal(panel.CPanelRowsResonate);
        panel.CPanelObserverAttach(CSubject.CSubjectVista, rows);
        panel.CPanelObserverAttach(CSubject.CSubjectWorkspace, _ => _cLibraryMarshal(LLibraryWorkspaceResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, bulletin => _cLibraryMarshal(() => panel.CPanelEntryResonate(bulletin)));
        panel.CPanelObserverAttach(CSubject.CSubjectReflex, rows);
        panel.CPanelObserverAttach(CSubject.CSubjectSettings, rows);
        panel.CPanelChosenAttach(CSubject.CSubjectEntry, _ => _cLibraryMarshal(panel.CPanelDraftResonate));
    }

    private void LLibraryWorkspaceResonate()
    {
        CLibraryPanel.CPanelEntryClose();
        CLibraryWorkspaceChanged?.Invoke();
    }

    private void LLibraryClose()
    {
        CLibraryEditor.CEditorClose();
        CLibraryEditor.CEditorDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }

    public void CLibraryQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cLibraryVista?.LVistaQuerySet(query);
    }

    public void CLibraryOrderSet(CCatalogOrder? order)
    {
        _cLibraryVista?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CLibraryFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cLibraryVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public static IReadOnlyList<CCatalogOrder> CLibraryOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderHeadword,
            CCatalogOrder.CCatalogOrderReverse,
            CCatalogOrder.CCatalogOrderRecent,
            CCatalogOrder.CCatalogOrderEarliest,
        ];
    }

    public IReadOnlyList<CVistaRow> CLibraryRowsRead()
    {
        IReadOnlyList<CVistaRow> rows = _cLibraryVista is LVista vista
            ? _cLibraryEntryPort.LEngineEntryFind(vista).Select(CPanel.CPanelRowRead).ToList()
            : [];
        _cLibraryCount = rows.Count;
        return rows;
    }

    internal string LLibraryFileRead()
    {
        return LVista.LVistaFileRead(_cLibraryVista);
    }

    public Task CLibraryPortraitPrint()
    {
        return CPortrait.LPortraitTicketPrint(
            _cLibraryEnvoy,
            _cLibrarySettingsPort,
            chosen => _cLibraryPortraitPort.LEnginePortraitPrint(
                _cLibraryVista, CPortrait.LPortraitLabelRead(_cLibrarySettingsPort), chosen));
    }

    public Task CLibraryPortraitExport()
    {
        return CPortrait.LPortraitFileExport(
            _cLibraryEnvoy,
            _cLibrarySettingsPort,
            LLibraryFileRead(),
            (file, medium) => _cLibraryPortraitPort.LEnginePortraitExport(
                _cLibraryVista, file, medium, CPortrait.LPortraitLabelRead(_cLibrarySettingsPort)));
    }

    public async Task CLibraryMarkupImport()
    {
        if (_cLibraryEnvoy.CEnvoyMarkupRead() is not string path)
        {
            return;
        }

        try
        {
            if (await _cLibraryPortraitPort.LEngineMarkupStart(path, LLibraryIntakeRead)
                is not IReadOnlyList<LMarkupOmission> omissions)
            {
                return;
            }

            CLibraryPanel.CPanelRowsResonate();
            List<CMarkupOmission> report = omissions.Select(LLibraryOmissionRead).ToList();
            if (report.Count > 0)
            {
                _cLibraryEnvoy.CEnvoyOmissionShow(report);
            }
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cLibraryEnvoy, _cLibrarySettingsPort, "List.ImportFailed", exception);
        }
    }

    private IReadOnlyList<LMarkupIntake>? LLibraryIntakeRead(
        IReadOnlyList<LMarkupEntry> entries, IReadOnlyList<IReadOnlyList<LMarkupTarget>> targets)
    {
        CSCustoms customs = new(
            entries.Zip(targets, LLibraryEntryRead).ToList(),
            targets.SelectMany(static found => found).Select(LLibraryTargetRead).ToList());
        if (!_cLibraryEnvoy.CEnvoyCustomsRead(customs))
        {
            return null;
        }

        return customs.LSCustomsRowsRead()
            .Select(static (row, index) => LPortraitPort.LEngineIntakeRead(
                index, LLibraryModeRead(row.CSCustomsRowMode), row.CSCustomsRowTarget))
            .ToList();
    }

    private static LMarkupMode LLibraryModeRead(CSCustomsMode mode)
    {
        return mode switch
        {
            CSCustomsMode.CSCustomsModeFresh => LMarkupMode.LMarkupModeNew,
            CSCustomsMode.CSCustomsModeMerge => LMarkupMode.LMarkupModeMerge,
            CSCustomsMode.CSCustomsModeReplace => LMarkupMode.LMarkupModeReplace,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }

    private static CMarkupEntry LLibraryEntryRead(LMarkupEntry entry, IReadOnlyList<LMarkupTarget> targets)
    {
        return new CMarkupEntry(
            entry.LMarkupEntryLanguage,
            entry.LMarkupEntryName,
            targets.Select(static target => target.LMarkupTargetId).ToList());
    }

    private static CMarkupTarget LLibraryTargetRead(LMarkupTarget target)
    {
        return new CMarkupTarget(target.LMarkupTargetId, target.LMarkupTargetMeaning, target.LMarkupTargetCollocation);
    }

    private static CMarkupOmission LLibraryOmissionRead(LMarkupOmission omission)
    {
        return new CMarkupOmission(
            omission.LMarkupOmissionLine.ToString(CultureInfo.CurrentCulture), omission.LMarkupOmissionText);
    }
}
