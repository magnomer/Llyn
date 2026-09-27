using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LLibrary
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private LVista? _lLibraryVista;

    internal LLibrary(
        LEntryPort entries,
        LPortraitPort portraits,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        LLibraryEditor = editor;
        LLibraryPanel = new LPanel(
            "List.LoadFailed", "Scribe.DeleteFailed",
            editor.LEditorDesk.LDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LLibraryPanel.LPanelCleared += LLibraryEditorClear;
        LLibraryPanel.LPanelEdited += LLibraryEditorOpen;
        LLibraryPanel.LPanelDraftChanged += lectern.LLecternDraftShow;
        LLibraryPanel.LPanelCleared += lectern.LLecternClear;
    }

    public event Action<string, Exception>? LLibraryFailed;

    public LEditor LLibraryEditor { get; }

    public LPanel LLibraryPanel { get; }

    private void LLibraryEditorClear()
    {
        LLibraryEditor.LEditorOpen(null);
    }

    private void LLibraryEditorOpen(long id)
    {
        LLibraryEditor.LEditorOpen(id);
    }

    public bool LLibraryFiltered => _lLibraryVista?.LVistaFiltered ?? false;

    internal void LLibraryVistaRestore(LVista vista)
    {
        _lLibraryVista = vista;
        LLibraryPanel.LPanelVistaRestore(vista);
        LLibraryEditor.LEditorVistaRestore(vista);
    }

    public IReadOnlyList<CVistaRow> LLibraryRowsRead()
    {
        return _lLibraryVista is LVista vista
            ? LSplice.LSpliceBuild(_lEntryPort.LEngineEntryFind(vista), LPanel.LPanelRowRead)
            : [];
    }

    public long LLibraryVoyageRead()
    {
        return _lLibraryVista?.LVistaChosen ?? 0;
    }

    public void LLibraryInquirySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lLibraryVista?.LVistaQuerySet(query);
    }

    public void LLibraryOrderSet(CCatalogOrder? order)
    {
        if (_lLibraryVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LPanel.LPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LLibrarySieveSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lLibraryVista?.LVistaFilterSet(LPanel.LPanelFilterRead(filter));
    }

    public Task LLibraryPortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        if (_lLibraryVista is not LVista vista)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(
            vista, QPortrait.QPortraitLabelRead(label), QPortrait.QPortraitTicketRead(ticket));
    }

    public async Task LLibraryMarkupStart(
        Func<string?> pathSeam,
        Func<IReadOnlyList<CMarkupEntry>, IReadOnlyList<CSCustomsRow>?> customsSeam,
        Action<IReadOnlyList<CMarkupOmission>> omissionSeam)
    {
        ArgumentNullException.ThrowIfNull(pathSeam);
        ArgumentNullException.ThrowIfNull(customsSeam);
        ArgumentNullException.ThrowIfNull(omissionSeam);

        if (pathSeam() is not string path)
        {
            return;
        }

        try
        {
            await LLibraryMarkupImport(await _lPortraitPort.LEngineMarkupStart(path), customsSeam, omissionSeam);
        }
        catch (Exception exception)
        {
            LLibraryFailed?.Invoke("List.ImportFailed", exception);
        }
    }

    private async Task LLibraryMarkupImport(
        LMarkupCargo cargo,
        Func<IReadOnlyList<CMarkupEntry>, IReadOnlyList<CSCustomsRow>?> customsSeam,
        Action<IReadOnlyList<CMarkupOmission>> omissionSeam)
    {
        IReadOnlyList<CMarkupEntry> entries = LLibraryEntryRead(cargo.LMarkupCargoEntry);
        if (customsSeam(entries) is not IReadOnlyList<CSCustomsRow> rows)
        {
            return;
        }

        LMarkupOutcome outcome = await _lPortraitPort.LEngineMarkupStart(cargo, LLibraryIntakeRead(rows));

        LLibraryPanel.LPanelRowsUpdate();
        omissionSeam(LLibraryOmissionRead(outcome.LMarkupOutcomeOmission));
    }

    internal static IReadOnlyList<CMarkupEntry> LLibraryEntryRead(IReadOnlyList<LMarkupEntry> entries)
    {
        return LSplice.LSpliceBuild(
            entries,
            static entry => new CMarkupEntry(
                entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage, entry.LMarkupEntryName));
    }

    internal static IReadOnlyList<LMarkupIntake> LLibraryIntakeRead(IReadOnlyList<CSCustomsRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<LMarkupIntake> intakes = new(rows.Count);
        for (int index = 0; index < rows.Count; index++)
        {
            intakes.Add(LMarkupIntake.LMarkupIntakeCreate(
                index,
                rows[index].CSCustomsRowMode switch
                {
                    CSCustomsMode.CSCustomsModeMerge => LMarkupMode.LMarkupModeMerge,
                    CSCustomsMode.CSCustomsModeReplace => LMarkupMode.LMarkupModeReplace,
                    _ => LMarkupMode.LMarkupModeNew,
                },
                rows[index].CSCustomsRowTarget));
        }

        return intakes;
    }

    internal static IReadOnlyList<CMarkupOmission> LLibraryOmissionRead(IReadOnlyList<LMarkupOmission> omissions)
    {
        return LSplice.LSpliceBuild(
            omissions,
            static omission => new CMarkupOmission(omission.LMarkupOmissionLine, omission.LMarkupOmissionText));
    }

    internal void LLibraryVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LLibraryVistaRestore(
            window.LWindowVistaStart("library", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public string LLibraryFileRead()
    {
        return LVista.LVistaFileRead(LLibraryPanel.LPanelVista);
    }

    public Task LLibraryPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            LLibraryPanel.LPanelVista,
            path,
            QPortrait.QPortraitMediumRead(format),
            QPortrait.QPortraitLabelRead(label));
    }
}
