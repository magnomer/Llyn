using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

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

    private void PMembershipApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PMembershipItem membership)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PMembershipRow") is Button row)
        {
            if (membership.PMembershipItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= PMembershipHandle;
            row.Click += PMembershipHandle;
        }

        if (QLook.QLookPartFind<Image>(container, "PMembershipFlag") is Image flag)
        {
            flag.Source = membership.PMembershipItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PMembershipName") is Run name)
        {
            name.Text = membership.PMembershipItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PMembershipEpithet") is Run epithet)
        {
            epithet.Text = " " + membership.PMembershipItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMembershipLanguage") is TextBlock language)
        {
            language.Text = membership.PMembershipItemLanguage;
        }
    }
}
