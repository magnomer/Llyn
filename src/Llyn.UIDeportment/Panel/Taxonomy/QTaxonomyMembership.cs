using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QTaxonomy
{
    private void QMembershipFind()
    {
        IReadOnlyList<CVistaRow> read;
        try
        {
            read = _lTaxonomy.LTaxonomyMembershipRead();
        }
        catch (Exception exception)
        {
            _qTaxonomyHost.PWindowFailureShow("Tag.LoadFailed", exception);
            return;
        }

        List<QMembershipItem> fresh = [];
        foreach (CVistaRow entry in read)
        {
            fresh.Add(new QMembershipItem(
                entry.CVistaRowId,
                entry.CVistaRowHeadword,
                entry.CVistaRowLanguage,
                entry.CVistaRowEpithet,
                entry.CVistaRowChosen)
            {
                QMembershipItemName = entry.CVistaRowName,
            });
        }

        LSplice.LSpliceApply(
            _qMembershipList, fresh, QMembershipItem.QMembershipItemMatch, QMembershipItem.QMembershipItemSync);

        QMembershipEmpty.SetResourceReference(TextBlock.TextProperty, _lTaxonomy.LTaxonomyEmptyRead(QScout.Text));
        QMembershipEmpty.Visibility = _qMembershipList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QMembershipHandle(object sender, RoutedEventArgs e)
    {
        QMembershipRowShow((sender as FrameworkElement)?.DataContext as QMembershipItem);
    }

    private void QMembershipRowShow(QMembershipItem? item)
    {
        if (item is null)
        {
            return;
        }

        if (!QTaxonomyLeaveConfirm())
        {
            return;
        }

        _lTaxonomy.LTaxonomyPanel.CPanelRowOpen(item.QMembershipItemId);
    }

    private void QMembershipApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QMembershipItem membership)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PMembershipRow") is Button row)
        {
            if (membership.QMembershipItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QMembershipHandle;
            row.Click += QMembershipHandle;
        }

        if (QLook.QLookPartFind<Image>(container, "PMembershipFlag") is Image flag)
        {
            flag.Source = membership.QMembershipItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PMembershipName") is Run name)
        {
            name.Text = membership.QMembershipItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PMembershipEpithet") is Run epithet)
        {
            epithet.Text = " " + membership.QMembershipItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMembershipLanguage") is TextBlock language)
        {
            language.Text = membership.QMembershipItemLanguage;
        }
    }
}
