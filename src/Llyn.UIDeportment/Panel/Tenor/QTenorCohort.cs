using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QTenor
{
    private void QCohortRefine()
    {
        List<QCohortItem> fresh = [];
        foreach (CVistaRow entry in _cTenor.CTenorCohort.CCohortRowsRead())
        {
            fresh.Add(new QCohortItem(
                entry.CVistaRowId,
                entry.CVistaRowHeadword,
                entry.CVistaRowLanguage,
                entry.CVistaRowEpithet,
                entry.CVistaRowChosen)
            {
                QCohortItemName = entry.CVistaRowName,
            });
        }

        QSplice.QSpliceRefine(
            _qCohortList, fresh, QCohortItem.QCohortItemMatch, QCohortItem.QCohortItemSync);

        QCohortEmpty.SetResourceReference(
            TextBlock.TextProperty, _cTenor.CTenorCohort.CCohortPanel.CPanelAperture.CApertureKey);
        QCohortEmpty.Visibility = _qCohortList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QCohortObserve(object sender, RoutedEventArgs e)
    {
        _cTenor.CTenorCohort.CCohortPanel.CPanelRowSelect(
            ((sender as FrameworkElement)?.DataContext as QCohortItem)?.QCohortItemId);
    }

    private void QCohortItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QCohortItem cohort)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PCohortRow") is Button row)
        {
            if (cohort.QCohortItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QCohortObserve;
            row.Click += QCohortObserve;
        }

        if (QLook.QLookPartFind<Image>(container, "PCohortFlag") is Image flag)
        {
            flag.Source = cohort.QCohortItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PCohortName") is Run name)
        {
            name.Text = cohort.QCohortItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PCohortEpithet") is Run epithet)
        {
            epithet.Text = QLook.QLookEpithetRead(cohort.QCohortItemEpithet);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCohortLanguage") is TextBlock language)
        {
            language.Text = cohort.QCohortItemLanguage;
        }
    }
}
