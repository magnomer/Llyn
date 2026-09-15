using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PTenor
{
    private void PCohortSelect(long? id)
    {
        foreach (PCohortItem item in _pCohortList)
        {
            item.PCohortItemChosen = id is not null
                && item.PCohortItemId == id;
        }
    }

    private void PCohortFind()
    {
        IReadOnlyList<LEntry> read;
        try
        {
            read = _lEngine.LEngineEntryFind(
                new LRegister(_pGamutChoice ?? 0, LStateValue.LStateValueUnspecified),
                PQuest.Text ?? string.Empty,
                _pGrilleChoice);
        }
        catch (Exception exception)
        {
            _pTenorHost.PWindowFailureShow(PTenorFailure, exception);
            return;
        }

        _pCohortList.Clear();
        foreach (LEntry entry in read)
        {
            _pCohortList.Add(new PCohortItem(
                entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
        }

        PTwin.PTwinNameApply(
            _pCohortList,
            row => row.PCohortItemHeadword,
            (row, name) => row.PCohortItemName = name,
            row => row.PCohortItemId);

        PCohortEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PQuest.Text) ? "Register.Vacant" : "Register.Unmatched");
        PCohortEmpty.Visibility = _pCohortList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PCohortSelect(_pDisplayEntry);
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
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pTenorHost.PWindowFailureShow(PTenorFailure, exception);
            return;
        }

        if (draft is null)
        {
            PTenorClear();
            PGamutFind(PSounding.Text ?? string.Empty);
            return;
        }

        _pDisplayEntry = id;
        PCohortSelect(id);
        PTenorBin.IsEnabled = true;
        PTenorEntryShow(id, draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PCohortEntryUpdate(long id)
    {
        if (id > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pDisplayEntry = id;
            PCohortSelect(id);
            PTenorBin.IsEnabled = true;
        }

        PGamutFind(PSounding.Text ?? string.Empty);

        if (_pDisplayEntry is not long shown
            || (id > 0 && shown != id))
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(shown);
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

        PTenorEntryShow(shown, draft);
    }

    private void PCohortEntryCreate()
    {
        long? register = _pGamutChoice;

        PTenorClear();
        PTenorMode.IsEnabled = true;
        PTenorScribeShow(true);

        if (register is long id)
        {
            PEditor.PEditorRegisterAdd(id);
        }
    }
}
