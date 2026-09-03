using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PList
{
    private readonly ObservableCollection<PIndexItem> _pIndexList = [];

    private string? _pDisplayEntry;

    private string _pOrderChoice = "Headword";

    private async void PListHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PIndex.ItemsSource = _pIndexList;

        await PLangcodeIndicator.PLangcodeIndicatorLoad(_lEngine);

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

        _pOrderChoice = choice;
        POrderBase.IsChecked = false;
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    private IEnumerable<LEntry> POrderSort(IReadOnlyList<LEntry> entries)
    {
        return _pOrderChoice switch
        {
            "Reverse" => entries.OrderByDescending(
                entry => entry.LEntryHeadword, StringComparer.CurrentCultureIgnoreCase),
            "Recent" => entries.OrderByDescending(
                entry => entry.LEntryAddedUtc ?? string.Empty, StringComparer.Ordinal),
            "Earliest" => entries.OrderBy(
                entry => entry.LEntryAddedUtc ?? string.Empty, StringComparer.Ordinal),
            _ => entries
        };
    }

    private void PIndexFind(string query)
    {
        _pIndexList.Clear();
        foreach (LEntry entry in POrderSort(_lEngine.LEngineEntryFind(query)))
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

        if (!PListLeaveConfirm())
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
            _pListHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PListClear();
            PIndexFind(PInquiry.Text ?? string.Empty);
            return;
        }

        _pDisplayEntry = id;
        PListEntryShow(id, draft);

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
            PListEntryShow(id, draft);
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

    private void PFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PListLeaveConfirm())
        {
            return;
        }

        PListClear();
        PScribe.IsEnabled = true;
        PScribeShow(true);
    }

    private void PScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PListLeaveConfirm())
            {
                return;
            }

            PScribeShow(false);

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
        PScribeShow(true);
    }

    private void PScribeShow(bool editing)
    {
        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    private bool PListLeaveConfirm()
    {
        return _pListHost.PWindowDiscardConfirm(PListChangeCheck());
    }

    private void PListEntryShow(string id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PScribe.IsEnabled = true;
    }

    private void PListClear()
    {
        _pDisplayEntry = null;
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PScribeShow(false);
        PScribe.IsEnabled = false;
    }
}
