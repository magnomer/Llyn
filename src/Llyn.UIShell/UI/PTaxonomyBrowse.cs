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

    private long _pDirectoryChoice;

    private long? _pDisplayEntry;

    private LCatalogOrder _pFunnelChoice;

    private LCatalogFilter _pLatticeChoice = LCatalogFilter.LCatalogFilterEmpty;

    private async void PTaxonomyBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PTaxonomyReset();
            return;
        }

        if (!PBulletin.PBulletinEntryCheck(bulletin.LBulletinSubject))
        {
            return;
        }

        PMembershipEntryUpdate(
            bulletin.LBulletinSubject == LSubject.LSubjectTag ? 0 : bulletin.LBulletinId);
    }

    private void PExplorationHandle(object sender, TextChangedEventArgs e)
    {
        PDirectoryFind(PExploration.Text ?? string.Empty);
    }

    private void PScoutHandle(object sender, TextChangedEventArgs e)
    {
        PMembershipFind();
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

    private void PLatticeHandle(object sender, RoutedEventArgs e)
    {
        _pLatticeChoice = PChoice.PChoiceFilterRead(PLatticeList);
        _lEngine.LEngineLatticeSave(_pLatticeChoice);
        PLatticeMark.Visibility = _pLatticeChoice.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;
        PMembershipFind();
    }

    internal async void PLatticeRestore(LCatalogFilter filter)
    {
        _pLatticeChoice = filter;
        PLatticeMark.Visibility = filter.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PLatticeList, _lEngine.LEngineLanguageRead(), filter, PLatticeHandle);
    }

    private void PDirectoryReset()
    {
        _pDirectoryChoice = 0;
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
            bool chosen = tag.LTagId == _pDirectoryChoice;
            kept |= chosen;
            _pDirectoryList.Add(new PDirectoryItem(tag.LTagId, tag.LTagText, chosen));
        }

        if (!kept)
        {
            _pDirectoryChoice = 0;
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

        _pDirectoryChoice = item.PDirectoryItemChosen ? 0 : item.PDirectoryItemId;
        PDirectoryFind(PExploration.Text ?? string.Empty);
    }

    internal void PDirectoryTagShow(long id)
    {
        _pDirectoryChoice = id;
        PExploration.Text = string.Empty;
        PScout.Text = string.Empty;
        PDirectoryFind(string.Empty);
    }

    private void PTaxonomyFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PTaxonomyLeaveConfirm())
        {
            return;
        }

        if (_pDirectoryChoice == 0 && _pDisplayEntry is null)
        {
            PDirectoryTagCreate();
            return;
        }

        PMembershipEntryCreate();
    }

    private void PDirectoryTagCreate()
    {
        if (PSCoinage.PSCoinageShow(_pTaxonomyHost, "Coinage.Tag") is not string text)
        {
            return;
        }

        LTag created;
        try
        {
            created = _lEngine.LEngineTagCreate(text);
        }
        catch (Exception exception)
        {
            _pTaxonomyHost.PWindowFailureShow("Tag.CreateFailed", exception);
            return;
        }

        PTaxonomyClear();
        PDirectoryTagShow(created.LTagId);
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
        return _pTaxonomyHost.PWindowDiscardConfirm(PTaxonomyChangeCheck(), PTaxonomyDraftFinish);
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
