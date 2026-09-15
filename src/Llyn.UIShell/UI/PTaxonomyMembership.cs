using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PTaxonomy
{
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
            read = _lEngine.LEngineEntryFind(
                new LTag(_pDirectoryChoice, string.Empty),
                PScout.Text ?? string.Empty,
                _pLatticeChoice);
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
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                _lEngine.LEngineEpithetRead(entry.LEntryId)));
        }

        PTwin.PTwinNameApply(
            _pMembershipList,
            row => row.PMembershipItemHeadword,
            (row, name) => row.PMembershipItemName = name,
            row => row.PMembershipItemId);

        PMembershipEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PScout.Text) ? "Tag.Vacant" : "Tag.Unmatched");
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

    private void PMembershipEntryCreate()
    {
        long tag = _pDirectoryChoice;

        PTaxonomyClear();
        PTaxonomyMode.IsEnabled = true;
        PTaxonomyScribeShow(true);

        if (tag != 0)
        {
            PEditor.PEditorTagAdd(tag);
        }
    }
}
