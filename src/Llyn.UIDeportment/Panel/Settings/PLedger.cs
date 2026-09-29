using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private readonly List<PLedgerItem> _pLedgerList = [];

    private void PLedgerRefine(CLedgerState state)
    {
        foreach (CLedgerPage page in state.CLedgerStatePages)
        {
            PLedgerPageRefine(page);
        }

        PLedgerFindRefine(state.CLedgerStateShown);
        PLedgerMetaRefine();
    }

    private void PLedgerPageRefine(CLedgerPage page)
    {
        PLedgerItem? item = _pLedgerList.Find(
            item => string.Equals(item.PLedgerItemChild, page.CLedgerPageChild, StringComparison.Ordinal));
        if (item is null)
        {
            item = new PLedgerItem(page.CLedgerPageChild, page.CLedgerPageTitle);
            _pLedgerList.Add(item);
        }

        item.PLedgerItemTitle = page.CLedgerPageTitle;
        item.PLedgerItemMeta = page.CLedgerPageMeta;
    }

    private void PLedgerMetaRefine()
    {
        PLedgerPageRefine(PSettingsAtelier.CAtelierLedger.CLedgerMetaRead(
            PSettingsPosture.QPostureRead().LCapsuleContentLinked));
    }

    private void PLedgerFindRefine(CLedgerShown shown)
    {
        PLedger.ItemsSource = _pLedgerList
            .Where(item => shown.CLedgerShownChildren.Contains(item.PLedgerItemChild, StringComparer.Ordinal))
            .ToList();
        PLedgerEmpty.Visibility = shown.CLedgerShownEmpty ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PLedgerRowRefine(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PLedgerItem item)
        {
            return;
        }

        PDialRefine(item.PLedgerItemChild);
    }

    private void PLedgerItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not PLedgerItem ledger)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PLedgerRow") is Button row)
        {
            if (ledger.PLedgerItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= PLedgerRowRefine;
            row.Click += PLedgerRowRefine;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLedgerTitle") is TextBlock title)
        {
            title.Text = ledger.PLedgerItemTitle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLedgerMeta") is TextBlock meta)
        {
            meta.Text = ledger.PLedgerItemMeta;
        }
    }

    private void PWinnowObserve(object sender, TextChangedEventArgs e)
    {
        PLedgerFindRefine(PSettingsAtelier.CAtelierLedger.CLedgerFind(PWinnow.Text));
    }
}
