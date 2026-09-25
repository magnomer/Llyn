using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

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

    private void PTenorEntryUpdate(LDraft draft)
    {
        PDisplay.PDisplayShow(draft.LDraftContent);
    }
}
