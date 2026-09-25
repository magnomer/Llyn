using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LLibrary
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private LVista? _lLibraryVista;

    private LIndex? _lLibraryIndex;

    public LLibrary(
        LEntryPort entries,
        LPortraitPort portraits,
        LEditor editor,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        LLibraryEditor = editor;
        LLibraryPanel = new LPanel(
            "List.LoadFailed", "Scribe.DeleteFailed",
            editor.LEditorDesk.LDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LLibraryPanel.LPanelCleared += LLibraryEditorClear;
        LLibraryPanel.LPanelEdited += LLibraryEditorOpen;
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

    private bool LLibrarySieveActive => _lLibraryVista?.LVistaFiltered ?? false;

    public void LLibraryVistaRestore(LVista vista)
    {
        _lLibraryVista = vista;
        LLibraryPanel.LPanelVistaRestore(vista);
        LLibraryEditor.LEditorVistaRestore(vista);
    }

    public IReadOnlyList<LVistaRow> LLibraryRowsRead()
    {
        return _lLibraryVista is LVista vista ? _lEntryPort.LEngineEntryFind(vista) : [];
    }

    public void LLibraryIndexAttach(ItemsControl view, FrameworkElement empty, Func<string, ImageSource?> flagSeam)
    {
        _lLibraryIndex = new LIndex(view, empty, flagSeam);
        LLibraryPanel.LPanelRowsChanged += LLibraryIndexShow;
    }

    private void LLibraryIndexShow()
    {
        _lLibraryIndex?.LIndexShow(LLibraryRowsRead(), true);
    }

    public void LLibraryIndexSelect(object sender)
    {
        LLibraryPanel.LPanelRowSelect(LIndex.LIndexEntryRead(sender));
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

    private void LLibraryOrderSet(LCatalogOrder? order)
    {
        if (_lLibraryVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    private void LLibrarySieveSet(LCatalogFilter filter)
    {
        _lLibraryVista?.LVistaFilterSet(filter);
    }

    public void LLibraryOrderHandle(object sender, ToggleButton dropper)
    {
        ArgumentNullException.ThrowIfNull(dropper);

        dropper.IsChecked = false;
        LLibraryOrderSet(LChoice.LChoiceOrderRead(sender));
    }

    public void LLibrarySieveHandle(Panel list, UIElement mark)
    {
        LLibrarySieveSet(LChoice.LChoiceFilterRead(list));
        LLibrarySieveShow(mark);
    }

    public void LLibrarySieveShow(UIElement mark)
    {
        ArgumentNullException.ThrowIfNull(mark);

        mark.Visibility = LLibrarySieveActive ? Visibility.Visible : Visibility.Collapsed;
    }

    public Task LLibraryPortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        if (_lLibraryVista is not LVista vista)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(vista, label, ticket);
    }

    public async Task LLibraryMarkupStart(
        Func<string?> pathSeam,
        Func<LMarkupCargo, IReadOnlyList<LMarkupIntake>?> customsSeam,
        Action<IReadOnlyList<LMarkupOmission>> omissionSeam)
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
        Func<LMarkupCargo, IReadOnlyList<LMarkupIntake>?> customsSeam,
        Action<IReadOnlyList<LMarkupOmission>> omissionSeam)
    {
        if (customsSeam(cargo) is not IReadOnlyList<LMarkupIntake> intakes)
        {
            return;
        }

        LMarkupOutcome outcome = await _lPortraitPort.LEngineMarkupStart(cargo, intakes);

        LLibraryPanel.LPanelRowsUpdate();
        omissionSeam(outcome.LMarkupOutcomeOmission);
    }

    public void LLibraryVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LLibraryVistaRestore(
            window.LWindowVistaStart("library", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public string LLibraryFileRead()
    {
        return LVista.LVistaFileRead(LLibraryPanel.LPanelVista);
    }

    public Task LLibraryPortraitExport(string path, LPortraitFormat format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(LLibraryPanel.LPanelVista, path, format, label);
    }
}
