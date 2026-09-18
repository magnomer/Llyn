using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PTaxonomy
{
    private LVista? _pMembershipVista;

    private void PMembershipChosenApply()
    {
        long? id = _pMembershipVista?.LVistaChosen;
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
                new LTag(_pTaxonomyVista?.LVistaChosen ?? 0, string.Empty),
                PScout.Text ?? string.Empty,
                _pTaxonomyVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty);
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

        LTwin.LTwinNameApply(
            _pMembershipList,
            row => row.PMembershipItemHeadword,
            (row, name) => row.PMembershipItemName = name,
            row => row.PMembershipItemId);

        PMembershipEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PScout.Text) ? "Tag.Vacant" : "Tag.Unmatched");
        PMembershipEmpty.Visibility = _pMembershipList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PMembershipChosenApply();
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
            PDirectoryFind();
            return;
        }

        _pMembershipVista?.LVistaSelect(id);
        PMembershipChosenApply();
        PTaxonomyBin.IsEnabled = true;
        PTaxonomyEntryShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PMembershipEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pMembershipVista?.LVistaSelect(bulletin.LBulletinId);
            PMembershipChosenApply();
            PTaxonomyBin.IsEnabled = true;
        }

        PDirectoryFind();
    }

    private void PTaxonomyEntryUpdate()
    {
        if (_pMembershipVista?.LVistaChosen is not long shown)
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

        PTaxonomyEntryShow(draft);
    }

    private void PMembershipEntryCreate()
    {
        long tag = _pTaxonomyVista?.LVistaChosen ?? 0;

        PTaxonomyClear();
        PTaxonomyMode.IsEnabled = true;
        PTaxonomyScribeShow(true);

        if (tag != 0)
        {
            PEditor.PEditorTagAdd(tag);
        }
    }
}
