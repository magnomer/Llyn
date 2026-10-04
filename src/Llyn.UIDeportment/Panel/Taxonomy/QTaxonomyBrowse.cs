using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QTaxonomy
{
    private readonly ObservableCollection<QDirectoryItem> _qDirectoryList = [];

    private readonly ObservableCollection<QMembershipItem> _qMembershipList = [];

    private async void QTaxonomyWorkspaceRefine()
    {
        await _qTaxonomyHost.QWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad(
            _qTaxonomyHost.QWindowEnvoy, QEnsignImage.QEnsignDraw);
    }

    private void QExplorationObserve(object sender, TextChangedEventArgs e)
    {
        _cTaxonomy.CTaxonomyQuerySet(QExploration.Text ?? string.Empty);
    }

    private void QScoutObserve(object sender, TextChangedEventArgs e)
    {
        _cTaxonomy.CTaxonomyMembership.CMembershipQuerySet(QScout.Text);
    }

    private void QFunnelObserve(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyOrderSet(QChoice.QChoiceOrderRead(sender));
        QFunnelDropperRefine();
    }

    private void QFunnelDropperRefine()
    {
        QFunnelDropper.IsChecked = false;
    }

    private void QLatticeObserve(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyFilterSet(QChoice.QChoiceFilterRead(sender));
        QLatticeRefine();
    }

    internal async void QTaxonomyVistaRefine()
    {
        QChoice.QChoiceOrderBuild(
            QFunnelList,
            "Funnel",
            QFunnelObserve,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
            ]);
        QChoice.QChoiceOrderApply(QFunnelDropdown, _cTaxonomy.CTaxonomyOrder);
        QLatticeRefine();
        CEnsignSheet<IReadOnlyList<CCatalogTag>> sheet =
            await _cTaxonomy.CTaxonomyRowsLoad(QEnsignImage.QEnsignDraw);
        QLatticeListRefine(sheet.CEnsignSheetLanguages);
        QDirectoryRefine(sheet.CEnsignSheetRows);
    }

    private void QLatticeListRefine(IReadOnlyList<string> languages)
    {
        QChoice.QChoiceFilterBuild(QLatticeList, languages, _cTaxonomy.CTaxonomyFilter, QLatticeObserve);
    }

    private void QLatticeRefine()
    {
        QLatticeMark.Visibility = _cTaxonomy.CTaxonomyFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QDirectoryRefine()
    {
        QDirectoryRefine(_cTaxonomy.CTaxonomyRowsRead());
    }

    private void QDirectoryRefine(IReadOnlyList<CCatalogTag> rows)
    {
        _qDirectoryList.Clear();
        foreach (CCatalogTag row in rows)
        {
            _qDirectoryList.Add(new QDirectoryItem(
                row.CCatalogTagStored.CTagId, row.CCatalogTagStored.CTagText, row.CCatalogTagChosen));
        }

        QDirectoryEmpty.Visibility = _qDirectoryList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QDirectoryObserve(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is QDirectoryItem item)
        {
            _cTaxonomy.CTaxonomyTagToggle(item.QDirectoryItemId);
        }
    }

    private void QDirectoryItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QDirectoryItem directory)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PDirectoryRow") is Button row)
        {
            if (directory.QDirectoryItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QDirectoryObserve;
            row.Click += QDirectoryObserve;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PDirectoryIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("tag", 16);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PDirectoryText") is TextBlock text)
        {
            text.Text = directory.QDirectoryItemText;
        }
    }

    private void QDirectoryTagRefine()
    {
        QExploration.Text = string.Empty;
        QScout.Text = string.Empty;
    }

    private void QTaxonomyFreshObserve(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyEntryCreate();
    }

    private void QTaxonomyViewerObserve(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyMembership.CMembershipPanel.CPanelScribeToggle(false);
    }

    private void QTaxonomyScribeObserve(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyMembership.CMembershipPanel.CPanelScribeToggle(true);
    }

    private void QTaxonomyStoreObserve(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyEditor.CEditorEntrySave();
    }

    private void QTaxonomyBinObserve(object sender, RoutedEventArgs e)
    {
        _cTaxonomy.CTaxonomyMembership.CMembershipPanel.CPanelEntryDelete();
    }
}
