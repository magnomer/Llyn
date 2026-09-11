using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PTaxonomy
{
    private readonly ObservableCollection<PDirectoryItem> _pDirectoryList = [];

    private readonly ObservableCollection<PMembershipItem> _pMembershipList = [];

    private string? _pDirectoryChoice;

    private long? _pDisplayEntry;

    private LCatalogOrder _pFunnelChoice;

    private async void PTaxonomyBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PTaxonomyReset();
            return;
        }

        PMembershipEntryUpdate(bulletin.LBulletinId);
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

        _pFunnelChoice = LCatalog.LCatalogOrderParse(choice, _pFunnelChoice);
        _lEngine.LEngineFunnelSave(_pFunnelChoice);
        PFunnelDropper.IsChecked = false;
        PDirectoryFind(PExploration.Text ?? string.Empty);
    }

    internal async void PFunnelRestore(LCatalogOrder order)
    {
        _pFunnelChoice = order;
        PChoice.PChoiceOrderApply(PFunnelDropdown, order);

        await PEnsign.PEnsignLoad(_lEngine);

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

    internal void PDirectoryTagShow(string text)
    {
        _pDirectoryChoice = text;
        PExploration.Text = string.Empty;
        PDirectoryFind(string.Empty);
    }

    private void PMembershipSelect(long? id)
    {
        foreach (PMembershipItem item in _pMembershipList)
        {
            item.PMembershipItemChosen = id is not null
                && item.PMembershipItemId == id;
        }
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

        PTwin.PTwinNameApply(
            _pMembershipList, row => row.PMembershipItemHeadword, (row, name) => row.PMembershipItemName = name);

        PMembershipEmpty.Visibility = _pMembershipList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PMembershipSelect(_pDisplayEntry);
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

    private void PMembershipEntryShow(long id)
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
        PMembershipSelect(id);
        PTaxonomyBin.IsEnabled = true;
        PTaxonomyEntryShow(id, draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PMembershipEntryUpdate(long id)
    {
        if (id > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pDisplayEntry = id;
            PMembershipSelect(id);
            PTaxonomyBin.IsEnabled = true;
        }

        PDirectoryFind(PExploration.Text ?? string.Empty);

        if (_pDisplayEntry is not long shown
            || (id > 0 && shown != id))
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(shown);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            PTaxonomyClear();
            return;
        }

        PTaxonomyEntryShow(shown, draft);
    }

    private void PTaxonomyFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PTaxonomyLeaveConfirm())
        {
            return;
        }

        PTaxonomyClear();
        PTaxonomyMode.IsEnabled = true;
        PTaxonomyScribeShow(true);
    }

    private void PTaxonomyScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PTaxonomyScribe);
        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PTaxonomyLeaveConfirm())
            {
                PTaxonomyScribeShow(true);
                return;
            }

            PTaxonomyScribeShow(false);

            if (_pDisplayEntry is not null)
            {
                PMembershipEntryShow(_pDisplayEntry.Value);
                return;
            }

            PTaxonomyClear();
            return;
        }

        if (_pDisplayEntry is null)
        {
            PTaxonomyClear();
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry.Value);
        PTaxonomyScribeShow(true);
    }

    private void PTaxonomyScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PTaxonomyViewer.IsChecked = !editing;
        PTaxonomyScribe.IsChecked = editing;
    }

    internal void PTaxonomyScribeRestore(bool editing)
    {
        if (editing && _pDisplayEntry is null)
        {
            return;
        }

        if (editing)
        {
            PTaxonomyMode.IsEnabled = true;
        }

        PTaxonomyScribeShow(editing);
    }

    internal bool PTaxonomyLeaveConfirm()
    {
        return _pTaxonomyHost.PWindowDiscardConfirm(PTaxonomyChangeCheck());
    }

    private void PTaxonomyEntryShow(long id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PTaxonomyMode.IsEnabled = true;
    }

    private void PTaxonomyClear()
    {
        _pDisplayEntry = null;
        PMembershipSelect(null);
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PTaxonomyScribeShow(false);
        PTaxonomyMode.IsEnabled = false;
        PTaxonomyBin.IsEnabled = false;
    }

    private void PTaxonomyStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PTaxonomyBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayEntry is not long id)
        {
            return;
        }

        if (!_pTaxonomyHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lEngine.LEngineEntryDelete(id);
        }
        catch (Exception exception)
        {
            _pTaxonomyHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PTaxonomyClear();
    }
}
