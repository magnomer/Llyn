using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

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

        PSplice.PSpliceApply(
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

        PCohortEntryShow(item.PCohortItemId);
    }

    private void PCohortEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            _lTenor.LTenorCohortSelect(id);
            draft = _lTenor.LTenorCohortLoad();
        }
        catch (Exception exception)
        {
            _pTenorHost.PWindowFailureShow(PTenorFailure, exception);
            return;
        }

        if (draft is null)
        {
            PTenorClear();
            PGamutFind();
            return;
        }

        _lTenor.LTenorCohortSelect(id);
        PCohortFind();
        PTenorBin.IsEnabled = true;
        PTenorEntryShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PCohortEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinStored)
        {
            if (IsVisible)
            {
                if (PEditor.Visibility == Visibility.Visible)
                {
                    _lTenor.LTenorCohortSelect(bulletin.LBulletinId);
                    PCohortFind();
                    PTenorBin.IsEnabled = true;
                }
            }
        }

        PGamutFind();
    }

    private void PTenorEntryUpdate()
    {
        LEntryDraft? draft;
        try
        {
            draft = _lTenor.LTenorCohortLoad();
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            PTenorClear();
            return;
        }

        PTenorEntryShow(draft);
    }

    private void PCohortEntryCreate()
    {
        long? register = _lTenor.LTenorChosen;

        PTenorClear();
        PTenorMode.IsEnabled = true;
        PTenorScribeShow(true);

        if (register is long id)
        {
            _lEditor.LEditorRegisterAdd(id);
        }
    }
}
