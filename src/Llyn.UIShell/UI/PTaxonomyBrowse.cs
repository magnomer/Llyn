using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PTaxonomy
{
    private readonly ObservableCollection<PDirectoryItem> _pDirectoryList = [];

    private readonly ObservableCollection<PMembershipItem> _pMembershipList = [];

    private string? _pDirectoryChoice;

    private string? _pDisplayEntry;

    private LCatalogOrder _pFunnelChoice = LCatalogOrder.LCatalogOrderName;

    private async void PTaxonomyHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PDirectory.ItemsSource = _pDirectoryList;
        PMembership.ItemsSource = _pMembershipList;

        await PEnsign.PEnsignLoad(_lEngine);

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

        _pFunnelChoice = LCatalog.LCatalogOrderParse(choice, LCatalogOrder.LCatalogOrderName);
        PFunnelDropper.IsChecked = false;
        PDirectoryFind(PExploration.Text ?? string.Empty);
    }

    private void PDirectoryReset()
    {
        _pDirectoryChoice = null;
    }

    private void PDirectoryFind(string query)
    {
        IReadOnlyList<LTag> read;
        try
        {
            read = _lEngine.LEngineTagFind(query, _pFunnelChoice);
        }
        catch (Exception exception)
        {
            _pTaxonomyHost.PWindowFailureShow("Tag.LoadFailed", exception);
            return;
        }

        _pDirectoryList.Clear();
        bool kept = false;
        foreach (LTag tag in read)
        {
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
            _pTaxonomyHost.PWindowFailureShow("Tag.LoadFailed", exception);
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

        if (!PTaxonomyLeaveConfirm())
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
            _pTaxonomyHost.PWindowFailureShow("Tag.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PTaxonomyClear();
            PDirectoryFind(PExploration.Text ?? string.Empty);
            return;
        }

        _pDisplayEntry = id;
        PTaxonomyEntryShow(id, draft);

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
            PTaxonomyEntryShow(id, draft);
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

    private void PTaxonomyFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PTaxonomyLeaveConfirm())
        {
            return;
        }

        PTaxonomyClear();
        PTaxonomyScribe.IsEnabled = true;
        PTaxonomyScribeShow(true);
    }

    private void PTaxonomyScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PTaxonomyLeaveConfirm())
            {
                return;
            }

            PTaxonomyScribeShow(false);

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
        PTaxonomyScribeShow(true);
    }

    private void PTaxonomyScribeShow(bool editing)
    {
        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PTaxonomyScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    private bool PTaxonomyLeaveConfirm()
    {
        return _pTaxonomyHost.PWindowDiscardConfirm(PTaxonomyChangeCheck());
    }

    private void PTaxonomyEntryShow(string id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PTaxonomyScribe.IsEnabled = true;
    }

    private void PTaxonomyClear()
    {
        _pDisplayEntry = null;
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PTaxonomyScribeShow(false);
        PTaxonomyScribe.IsEnabled = false;
    }
}
