using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QTaxonomy
{
    private void QMembershipRefine()
    {
        List<QMembershipItem> fresh = [];
        foreach (CVistaRow entry in _cTaxonomy.CTaxonomyMembership.CMembershipRowsRead())
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

        QMembershipEmpty.SetResourceReference(
            TextBlock.TextProperty, _cTaxonomy.CTaxonomyMembership.CMembershipEmptyKey);
        QMembershipEmpty.Visibility = _qMembershipList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QMembershipObserve(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowSelect(
            ((sender as FrameworkElement)?.DataContext as QMembershipItem)?.QMembershipItemId);
    }

    private void QMembershipItemRefine(FrameworkElement container, object item, string? _)
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

            row.Click -= QMembershipObserve;
            row.Click += QMembershipObserve;
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
