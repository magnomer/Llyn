using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QTenor
{
    private readonly ObservableCollection<QGamutItem> _qGamutList = [];

    private readonly ObservableCollection<QCohortItem> _qCohortList = [];

    private async void QTenorWorkspaceRefine()
    {
        await LEnsignImage.LEnsignLoad(_qTenorHost.PWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad);
    }

    private void QSoundingObserve(object sender, TextChangedEventArgs e)
    {
        _cTenor.CTenorQuerySet(QSounding.Text ?? string.Empty);
    }

    private void QQuestObserve(object sender, TextChangedEventArgs e)
    {
        _cTenor.CTenorCohort.CCohortQuerySet(QQuest.Text);
    }

    private void QDegreeObserve(object sender, RoutedEventArgs e)
    {
        _cTenor.CTenorOrderSet(QChoice.QChoiceOrderRead(sender));
        QDegreeDropperRefine();
    }

    private void QDegreeDropperRefine()
    {
        QDegreeDropper.IsChecked = false;
    }

    private void QGrilleObserve(object sender, RoutedEventArgs e)
    {
        _cTenor.CTenorFilterSet(QChoice.QChoiceFilterRead(sender));
        QGrilleRefine();
    }

    internal async void QTenorVistaRefine()
    {
        QChoice.QChoiceOrderBuild(
            QDegreeList,
            "Degree",
            QDegreeObserve,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(QDegreeDropdown, _cTenor.CTenorOrder);
        QGrilleRefine();
        IReadOnlyList<string> languages =
            await LEnsignImage.LEnsignLoad(_qTenorHost.PWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad);
        QGrilleListRefine(languages);
        QGamutRefine();
    }

    private void QGrilleListRefine(IReadOnlyList<string> languages)
    {
        QChoice.QChoiceFilterBuild(QGrilleList, languages, _cTenor.CTenorFilter, QGrilleObserve);
    }

    private void QGrilleRefine()
    {
        QGrilleMark.Visibility = _cTenor.CTenorFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QGamutRefine()
    {
        _qGamutList.Clear();
        foreach (CCatalogRegister row in _cTenor.CTenorRowsRead())
        {
            _qGamutList.Add(new QGamutItem(row));
        }

        QGamutEmpty.Visibility = _qGamutList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QGamutObserve(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is QGamutItem item)
        {
            _cTenor.CTenorRegisterToggle(item.QGamutItemId);
        }
    }

    private void QGamutItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QGamutItem gamut)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PGamutRow") is Button row)
        {
            if (gamut.QGamutItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QGamutObserve;
            row.Click += QGamutObserve;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PGamutIcon") is QIconImage icon)
        {
            icon.QIconSource = gamut.QGamutItemIcon;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGamutName") is TextBlock name)
        {
            name.Text = gamut.QGamutItemName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGamutCount") is TextBlock count)
        {
            count.Text = gamut.QGamutItemCount;
        }
    }

    private void QGamutRegisterRefine()
    {
        QSounding.Text = string.Empty;
        QQuest.Text = string.Empty;
    }

    private void QTenorFreshObserve(object sender, RoutedEventArgs e)
    {
        _cTenor.CTenorEntryCreate();
    }

    private void QTenorViewerObserve(object sender, RoutedEventArgs e)
    {
        _cTenor.CTenorCohort.CCohortPanel.CPanelScribeToggle(false);
    }

    private void QTenorScribeObserve(object sender, RoutedEventArgs e)
    {
        _cTenor.CTenorCohort.CCohortPanel.CPanelScribeToggle(true);
    }

    private void QTenorStoreObserve(object sender, RoutedEventArgs e)
    {
        _cTenor.CTenorEditor.CEditorEntrySave();
    }

    private void QTenorBinObserve(object sender, RoutedEventArgs e)
    {
        _cTenor.CTenorCohort.CCohortPanel.CPanelEntryDelete();
    }
}
