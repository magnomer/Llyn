using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PTag
{
    private readonly ObservableCollection<PDirectoryItem> _pDirectoryList = [];

    private readonly ObservableCollection<PMembershipItem> _pMembershipList = [];

    private string? _pDirectoryChoice;

    private string? _pDisplayEntry;

    private string _pFunnelChoice = "Name";

    private async void PTagHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PDirectory.ItemsSource = _pDirectoryList;
        PMembership.ItemsSource = _pMembershipList;

        await PLangcodeIndicator.PLangcodeIndicatorLoad(_lEngine);

        PDirectoryFind(PExploration.Text ?? string.Empty);
    }

    private void PExplorationHandle(object sender, TextChangedEventArgs e)
    {
        PDirectoryFind(PExploration.Text ?? string.Empty);
    }

    private void PFunnelHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pFunnelChoice = choice;
        PFunnelBase.IsChecked = false;
        PDirectoryFind(PExploration.Text ?? string.Empty);
    }

    private IEnumerable<LTag> PFunnelSort(IReadOnlyList<LTag> tags)
    {
        return _pFunnelChoice switch
        {
            "Reverse" => tags.OrderByDescending(
                tag => tag.LTagText, StringComparer.CurrentCultureIgnoreCase),
            _ => tags.OrderBy(
                tag => tag.LTagText, StringComparer.CurrentCultureIgnoreCase)
        };
    }

    private void PDirectoryReset()
    {
        _pDirectoryChoice = null;
    }

    private void PDirectoryFind(string query)
    {
        query = query.Trim();

        IReadOnlyList<LTag> read;
        try
        {
            read = _lEngine.LEngineTagRead();
        }
        catch (Exception exception)
        {
            _pTagHost.PWindowFailureShow("Tag.LoadFailed", exception);
            return;
        }

        _pDirectoryList.Clear();
        bool kept = false;
        foreach (LTag tag in PFunnelSort(read))
        {
            if (query.Length > 0 &&
                tag.LTagText.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) < 0)
            {
                continue;
            }

            bool chosen = string.Equals(tag.LTagText, _pDirectoryChoice, StringComparison.Ordinal);
            kept |= chosen;
            _pDirectoryList.Add(new PDirectoryItem(tag.LTagText, chosen));
        }

        if (!kept)
        {
            _pDirectoryChoice = null;
        }

        PDirectoryEmpty.Visibility = _pDirectoryList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PMembershipFind();
    }

    private void PDirectoryHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PDirectoryItem item)
        {
            return;
        }

        _pDirectoryChoice = item.PDirectoryItemChosen ? null : item.PDirectoryItemText;
        PDirectoryFind(PExploration.Text ?? string.Empty);
    }

    private void PMembershipFind()
    {
        IReadOnlyList<LEntry> read;
        try
        {
            read = _lEngine.LEngineEntryFind(new LTag(_pDirectoryChoice ?? string.Empty));
        }
        catch (Exception exception)
        {
            _pTagHost.PWindowFailureShow("Tag.LoadFailed", exception);
            return;
        }

        _pMembershipList.Clear();
        foreach (LEntry entry in read)
        {
            _pMembershipList.Add(new PMembershipItem(
                entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
        }

        PMembershipEmpty.Visibility = _pMembershipList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PMembershipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PMembershipItem item)
        {
            return;
        }

        if (!PTagLeaveConfirm())
        {
            return;
        }

        PMembershipEntryShow(item.PMembershipItemId);
    }

    private void PMembershipEntryShow(string id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pTagHost.PWindowFailureShow("Tag.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PTagClear();
            PDirectoryFind(PExploration.Text ?? string.Empty);
            return;
        }

        _pDisplayEntry = id;
        PTagEntryShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PMembershipEntryUpdate(string id)
    {
        _pDisplayEntry = id;
        PDirectoryFind(PExploration.Text ?? string.Empty);

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
            PTagEntryShow(draft);
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
        if (!PTagLeaveConfirm())
        {
            return;
        }

        PTagClear();
        PScribe.IsEnabled = true;
        PScribeShow(true);
    }

    private void PScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PTagLeaveConfirm())
            {
                return;
            }

            PScribeShow(false);

            if (_pDisplayEntry is not null)
            {
                PMembershipEntryShow(_pDisplayEntry);
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

    private bool PTagLeaveConfirm()
    {
        return _pTagHost.PWindowDiscardConfirm(PTagChangeCheck());
    }

    private void PTagEntryShow(LEntryDraft draft)
    {
        PDisplay.PDisplayShow(draft);
        PScribe.IsEnabled = true;
    }

    private void PTagClear()
    {
        _pDisplayEntry = null;
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PScribeShow(false);
        PScribe.IsEnabled = false;
    }
}
