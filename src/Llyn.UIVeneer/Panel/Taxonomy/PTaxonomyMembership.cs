using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PTaxonomy
{
    private void PMembershipFind()
    {
        IReadOnlyList<LVistaRow> read;
        try
        {
            read = _lTaxonomy.LTaxonomyMembershipRead();
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

        LSplice.LSpliceApply(
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

        _lTaxonomy.LTaxonomyPanel.LPanelRowShow(item.PMembershipItemId);
    }
}
