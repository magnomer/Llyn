using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PTenor
{
    private void PCohortFind()
    {
        IReadOnlyList<LVistaRow> read;
        try
        {
            read = _lTenor.LTenorCohortRead();
        }
        catch (Exception exception)
        {
            _pTenorHost.PWindowFailureShow(PTenorFailure, exception);
            return;
        }

        List<PCohortItem> fresh = [];
        foreach (LVistaRow entry in read)
        {
            fresh.Add(new PCohortItem(
                entry.LVistaRowId,
                entry.LVistaRowHeadword,
                entry.LVistaRowLanguage,
                entry.LVistaRowEpithet ?? string.Empty,
                entry.LVistaRowChosen)
            {
                PCohortItemName = entry.LVistaRowName,
            });
        }

        LSplice.LSpliceApply(
            _pCohortList, fresh, PCohortItem.PCohortItemMatch, PCohortItem.PCohortItemSync);

        PCohortEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PQuest.Text) ? "Register.Vacant" : "Register.Unmatched");
        PCohortEmpty.Visibility = _pCohortList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    }

    private void PCohortHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PCohortItem item)
        {
            return;
        }

        if (!PTenorLeaveConfirm())
        {
            return;
        }

        _lTenor.LTenorPanel.LPanelRowShow(item.PCohortItemId);
    }

    private void PCohortApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PCohortItem cohort)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PCohortRow") is Button row)
        {
            if (cohort.PCohortItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= PCohortHandle;
            row.Click += PCohortHandle;
        }

        if (QLook.QLookPartFind<Image>(container, "PCohortFlag") is Image flag)
        {
            flag.Source = cohort.PCohortItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PCohortName") is Run name)
        {
            name.Text = cohort.PCohortItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PCohortEpithet") is Run epithet)
        {
            epithet.Text = " " + cohort.PCohortItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCohortLanguage") is TextBlock language)
        {
            language.Text = cohort.PCohortItemLanguage;
        }
    }
}
