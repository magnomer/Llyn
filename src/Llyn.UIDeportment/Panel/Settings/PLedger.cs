using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private readonly ObservableCollection<PLedgerItem> _pLedgerList = [];

    private void PLedgerShow(IReadOnlyList<CLedgerPage> pages)
    {
        if (_pLedgerList.Count == 0)
        {
            foreach (CLedgerPage page in pages)
            {
                _pLedgerList.Add(new PLedgerItem(page.CLedgerPageChild, page.CLedgerPageTitle));
            }

            PLedger.ItemsSource = _pLedgerList;
            PLedgerEmpty.Visibility = _pLedgerList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        foreach (CLedgerPage page in pages)
        {
            foreach (PLedgerItem item in _pLedgerList)
            {
                if (string.Equals(item.PLedgerItemChild, page.CLedgerPageChild, StringComparison.Ordinal))
                {
                    item.PLedgerItemTitle = page.CLedgerPageTitle;
                    item.PLedgerItemMeta = page.CLedgerPageMeta;
                }
            }
        }

        PLedgerMetaApply();
    }

    private void PLedgerMetaApply()
    {
        foreach (PLedgerItem item in _pLedgerList)
        {
            if (string.Equals(item.PLedgerItemChild, "Layout", StringComparison.Ordinal))
            {
                item.PLedgerItemMeta = PSettingsAtelier.CAtelierLedger.CLedgerMetaRead(
                    item.PLedgerItemChild, PSettingsPosture.QPostureRead().LCapsuleContentLinked);
            }
        }
    }

    private void PLedgerFind(string text)
    {
        HashSet<string> found = PSettingsAtelier.CAtelierLedger.CLedgerFind(text).ToHashSet(StringComparer.Ordinal);
        List<PLedgerItem> shown = _pLedgerList.Where(item => found.Contains(item.PLedgerItemChild)).ToList();

        PLedger.ItemsSource = shown;
        PLedgerEmpty.Visibility = shown.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PLedgerHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PLedgerItem item)
        {
            return;
        }

        PDialShow(item.PLedgerItemChild);
    }

    private void PLedgerApply(FrameworkElement container, object item, string? _)
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

            row.Click -= PLedgerHandle;
            row.Click += PLedgerHandle;
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

    private void PWinnowHandle(object sender, TextChangedEventArgs e)
    {
        PLedgerFind(PWinnow.Text ?? string.Empty);
    }
}
