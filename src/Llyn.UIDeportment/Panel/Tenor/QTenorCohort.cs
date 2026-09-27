using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QTenor
{
    private void QCohortFind()
    {
        IReadOnlyList<CVistaRow> read;
        try
        {
            read = _lTenor.LTenorCohortRead();
        }
        catch (Exception exception)
        {
            _qTenorHost.PWindowFailureShow(QTenorFailure, exception);
            return;
        }

        List<QCohortItem> fresh = [];
        foreach (CVistaRow entry in read)
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

        LSplice.LSpliceApply(
            _qCohortList, fresh, QCohortItem.QCohortItemMatch, QCohortItem.QCohortItemSync);

        QCohortEmpty.SetResourceReference(TextBlock.TextProperty, _lTenor.LTenorEmptyRead(QQuest.Text));
        QCohortEmpty.Visibility = _qCohortList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QCohortHandle(object sender, RoutedEventArgs e)
    {
        QCohortRowShow((sender as FrameworkElement)?.DataContext as QCohortItem);
    }

    private void QCohortRowShow(QCohortItem? item)
    {
        if (item is null)
        {
            return;
        }

        if (!QTenorLeaveConfirm())
        {
            return;
        }

        _lTenor.LTenorPanel.LPanelRowShow(item.QCohortItemId);
    }

    private void QCohortApply(FrameworkElement container, object item, string? _)
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

            row.Click -= QCohortHandle;
            row.Click += QCohortHandle;
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
            epithet.Text = " " + cohort.QCohortItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCohortLanguage") is TextBlock language)
        {
            language.Text = cohort.QCohortItemLanguage;
        }
    }
}
