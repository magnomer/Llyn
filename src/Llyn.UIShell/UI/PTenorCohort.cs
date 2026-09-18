using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PTenor
{
    private LVista? _pCohortVista;

    private void PCohortChosenApply()
    {
        long? id = _pCohortVista?.LVistaChosen;
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
                new LRegister(_pTenorVista?.LVistaChosen ?? 0, LStateValue.LStateValueUnspecified),
                PQuest.Text ?? string.Empty,
                _pTenorVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty);
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
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                _lEngine.LEngineEpithetRead(entry.LEntryId)));
        }

        LTwin.LTwinNameApply(
            _pCohortList,
            row => row.PCohortItemHeadword,
            (row, name) => row.PCohortItemName = name,
            row => row.PCohortItemId);

        PCohortEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PQuest.Text) ? "Register.Vacant" : "Register.Unmatched");
        PCohortEmpty.Visibility = _pCohortList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PCohortChosenApply();
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
            PGamutFind();
            return;
        }

        _pCohortVista?.LVistaSelect(id);
        PCohortChosenApply();
        PTenorBin.IsEnabled = true;
        PTenorEntryShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PCohortEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pCohortVista?.LVistaSelect(bulletin.LBulletinId);
            PCohortChosenApply();
            PTenorBin.IsEnabled = true;
        }

        PGamutFind();
    }

    private void PTenorEntryUpdate()
    {
        if (_pCohortVista?.LVistaChosen is not long shown)
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

        PTenorEntryShow(draft);
    }

    private void PCohortEntryCreate()
    {
        long? register = _pTenorVista?.LVistaChosen;

        PTenorClear();
        PTenorMode.IsEnabled = true;
        PTenorScribeShow(true);

        if (register is long id)
        {
            PEditor.PEditorRegisterAdd(id);
        }
    }
}
