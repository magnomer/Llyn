using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QMembership
{
    private readonly UserControl _qMembershipSurface;

    private readonly ObservableCollection<QMembershipItem> _qMembershipList = [];

    private CTaxonomy _cTaxonomy = null!;

    internal QMembership(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qMembershipSurface = surface;

        QScout.SetResourceReference(QField.QFieldHintProperty, "Scout.Search");
        QScout.TextChanged += QScoutObserve;
    }

    private ItemsControl QMembershipView =>
        QContract.QContractFind<ItemsControl>(_qMembershipSurface, "PMembership");

    private TextBlock QMembershipEmpty =>
        QContract.QContractFind<TextBlock>(_qMembershipSurface, "PMembershipEmpty");

    private TextBox QScout => QContract.QContractFind<TextBox>(_qMembershipSurface, "PScout");

    internal void QMembershipIntroduce(CTaxonomy taxonomy)
    {
        ArgumentNullException.ThrowIfNull(taxonomy);

        _cTaxonomy = taxonomy;
        _cTaxonomy.CTaxonomyTagOpened += QScoutRefine;
        _cTaxonomy.CTaxonomyMembership.CMembershipPanel.CPanelAperture.CApertureRowsChanged += QMembershipRefine;

        QMembershipView.ItemsSource = _qMembershipList;
        QLookItem.QLookItemAttach(QMembershipView, QMembershipItemRefine);
    }

    private void QScoutObserve(object sender, TextChangedEventArgs e)
    {
        _cTaxonomy.CTaxonomyMembership.CMembershipPanel.CPanelAperture.CApertureQuerySet(QScout.Text);
    }

    private void QScoutRefine()
    {
        QScout.Text = string.Empty;
    }

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

        QSplice.QSpliceRefine(
            _qMembershipList, fresh, QMembershipItem.QMembershipItemMatch, QMembershipItem.QMembershipItemSync);

        QMembershipEmpty.SetResourceReference(
            TextBlock.TextProperty, _cTaxonomy.CTaxonomyMembership.CMembershipPanel.CPanelAperture.CApertureKey);
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
            epithet.Text = QLook.QLookEpithetRead(membership.QMembershipItemEpithet);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMembershipLanguage") is TextBlock language)
        {
            language.Text = membership.QMembershipItemLanguage;
        }
    }
}
