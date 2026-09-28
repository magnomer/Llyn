using System;
using System.Collections.Generic;
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

    private LVista? _cLibraryVista;

    private CLibrary(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);

        _cLibraryAtelier = atelier;
        _cLibraryEntryPort = atelier.CAtelierEntryPort;
        _cLibraryPortraitPort = atelier.CAtelierPortraitPort;
        _cLibrarySettingsPort = atelier.CAtelierSettingsPort;
        _cLibraryEnvoy = envoy;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CLibraryEditor = editor;
        CLibraryPanel = new CPanel(
            envoy, "List.LoadFailed", "Scribe", editor.CEditorDesk.CDeskChangeCheck, editor.CEditorFinish,
            shownSeam);
        CLibraryPanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CLibraryPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
    }

    public static CLibrary CLibraryCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CLibrary(atelier, shownSeam, envoy);
    }

    public CEditor CLibraryEditor { get; }

    public CPanel CLibraryPanel { get; }

    public bool CLibraryFiltered => _cLibraryVista?.LVistaFiltered ?? false;

    public void CLibraryVistaRestore()
    {
        LVista vista = _cLibraryAtelier.CAtelierVistaStart(
            "library", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        _cLibraryVista = vista;
        CLibraryPanel.CPanelVistaRestore(vista);
        CLibraryEditor.LEditorVistaRestore(vista);
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

    public IReadOnlyList<CVistaRow> CLibraryRowsRead()
    {
        return _cLibraryVista is LVista vista
            ? _cLibraryEntryPort.LEngineEntryFind(vista).Select(CPanel.CPanelRowRead).ToList()
            : [];
    }

    internal string LLibraryFileRead()
    {
        return LVista.LVistaFileRead(_cLibraryVista);
    }

    public Task CLibraryPortraitPrint()
    {
        return CPortrait.LPortraitTicketPrint(
            _cLibraryEnvoy,
            chosen => _cLibraryPortraitPort.LEnginePortraitPrint(
                _cLibraryVista, CPortrait.LPortraitLabelRead(_cLibrarySettingsPort), chosen));
    }

    public Task CLibraryPortraitExport()
    {
        return CPortrait.LPortraitFileExport(
            _cLibraryEnvoy,
            LLibraryFileRead(),
            (file, medium) => _cLibraryPortraitPort.LEnginePortraitExport(
                _cLibraryVista, file, medium, CPortrait.LPortraitLabelRead(_cLibrarySettingsPort)));
    }

    public async Task CLibraryMarkupImport(string? path)
    {
        if (path is null)
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
            _cLibraryEnvoy.CEnvoyOmissionShow(omissions.Select(LLibraryOmissionRead).ToList());
        }
        catch (Exception exception)
        {
            _cLibraryEnvoy.CEnvoyFailureShow("List.ImportFailed", exception);
        }
    }

    private IReadOnlyList<LMarkupIntake>? LLibraryIntakeRead(IReadOnlyList<LMarkupEntry> entries)
    {
        return _cLibraryEnvoy.CEnvoyCustomsRead(entries.Select(LLibraryEntryRead).ToList())?
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

    private static CMarkupEntry LLibraryEntryRead(LMarkupEntry entry)
    {
        return new CMarkupEntry(entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage, entry.LMarkupEntryName);
    }

    private static CMarkupOmission LLibraryOmissionRead(LMarkupOmission omission)
    {
        return new CMarkupOmission(omission.LMarkupOmissionLine, omission.LMarkupOmissionText);
    }
}
