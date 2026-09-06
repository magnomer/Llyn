using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PLibrary
{
    private readonly ObservableCollection<PIndexItem> _pIndexList = [];

    private string? _pDisplayEntry;

    private LCatalogOrder _pOrderChoice;

    private async void PLibraryHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PIndex.ItemsSource = _pIndexList;

        await PEnsign.PEnsignLoad(_lEngine);

        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    private void PInquiryHandle(object sender, TextChangedEventArgs e)
    {
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    private void POrderHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pOrderChoice = LCatalog.LCatalogOrderParse(choice, _pOrderChoice);
        _lEngine.LEngineOrderSave(_pOrderChoice);
        POrderDropper.IsChecked = false;
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    internal void POrderRestore(LCatalogOrder order)
    {
        _pOrderChoice = order;
        PChoice.PChoiceOrderApply(POrderDropdown, order);
    }

    private void PIndexFind(string query)
    {
        _pIndexList.Clear();
        foreach (LEntry entry in _lEngine.LEngineEntryFind(query, _pOrderChoice))
        {
            _pIndexList.Add(new PIndexItem(entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
        }

        PIndexEmpty.Visibility = _pIndexList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PIndexHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PIndexItem item)
        {
            return;
        }

        if (!PLibraryLeaveConfirm())
        {
            return;
        }

        PIndexEntryShow(item.PIndexItemId);
    }

    internal void PIndexEntryShow(string id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pLibraryHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PLibraryClear();
            PIndexFind(PInquiry.Text ?? string.Empty);
            return;
        }

        _pDisplayEntry = id;
        PLibraryEntryShow(id, draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PIndexEntryUpdate(string id)
    {
        _pDisplayEntry = id;
        PIndexFind(PInquiry.Text ?? string.Empty);

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is not null)
        {
            PLibraryEntryShow(id, draft);
        }
    }

    private void PEditorEntryRestore()
    {
        if (_pDisplayEntry is null)
        {
            PEditor.PEditorReset();
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry);
    }

    private void PLibraryFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PLibraryLeaveConfirm())
        {
            return;
        }

        PLibraryClear();
        PLibraryScribe.IsEnabled = true;
        PLibraryScribeShow(true);
    }

    private void PLibraryScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PLibraryLeaveConfirm())
            {
                return;
            }

            PLibraryScribeShow(false);

            if (_pDisplayEntry is not null)
            {
                PIndexEntryShow(_pDisplayEntry);
            }

            return;
        }

        if (_pDisplayEntry is null)
        {
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry);
        PLibraryScribeShow(true);
    }

    private void PLibraryScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PLibraryScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    internal void PLibraryScribeRestore(bool editing)
    {
        if (editing)
        {
            PLibraryScribe.IsEnabled = true;
        }

        PLibraryScribeShow(editing);
    }

    internal bool PLibraryLeaveConfirm()
    {
        return _pLibraryHost.PWindowDiscardConfirm(PLibraryChangeCheck());
    }

    private void PLibraryEntryShow(string id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PLibraryScribe.IsEnabled = true;
    }

    private void PLibraryClear()
    {
        _pDisplayEntry = null;
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PLibraryScribeShow(false);
        PLibraryScribe.IsEnabled = false;
    }
}
