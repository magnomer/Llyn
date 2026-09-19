using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PTaxonomy
{
    private LVista? _pMembershipVista;

    private void PMembershipFind()
    {
        IReadOnlyList<LVistaRow> read;
        try
        {
            read = _lEngine.LEngineEntryFind(_pTaxonomyVista, _pMembershipVista);
        }
        catch (Exception exception)
        {
            _pTaxonomyHost.PWindowFailureShow("Tag.LoadFailed", exception);
            return;
        }

        List<PMembershipItem> fresh = [];
        foreach (LVistaRow entry in read)
        {
            fresh.Add(new PMembershipItem(
                entry.LVistaRowId,
                entry.LVistaRowHeadword,
                entry.LVistaRowLanguage,
                entry.LVistaRowEpithet ?? string.Empty,
                entry.LVistaRowChosen)
            {
                PMembershipItemName = entry.LVistaRowName,
            });
        }

        PSplice.PSpliceApply(
            _pMembershipList, fresh, PMembershipItem.PMembershipItemMatch, PMembershipItem.PMembershipItemSync);

        PMembershipEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PScout.Text) ? "Tag.Vacant" : "Tag.Unmatched");
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

    private void PMembershipEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            _pMembershipVista?.LVistaSelect(id);
            draft = _pMembershipVista?.LVistaLoad()?.LDraftContent;
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
        PMembershipFind();
        PTaxonomyBin.IsEnabled = true;
        PTaxonomyEntryShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PMembershipEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinStored)
        {
            if (IsVisible)
            {
                if (PEditor.Visibility == Visibility.Visible)
                {
                    _pMembershipVista?.LVistaSelect(bulletin.LBulletinId);
                    PMembershipFind();
                    PTaxonomyBin.IsEnabled = true;
                }
            }
        }

        PDirectoryFind();
    }

    private void PTaxonomyEntryUpdate()
    {
        LEntryDraft? draft;
        try
        {
            draft = _pMembershipVista?.LVistaLoad()?.LDraftContent;
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
            _lEditor.LEditorTagAdd(tag);
        }
    }
}
