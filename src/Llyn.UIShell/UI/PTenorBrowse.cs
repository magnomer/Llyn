using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PTenor
{
    private const string PTenorFailure = "Register.LoadFailed";

    private readonly ObservableCollection<PGamutItem> _pGamutList = [];

    private readonly ObservableCollection<PCohortItem> _pCohortList = [];

    private long? _pGamutChoice;

    private long? _pDisplayEntry;

    private LCatalogOrder _pDegreeChoice;

    private LCatalogFilter _pGrilleChoice = LCatalogFilter.LCatalogFilterEmpty;

    private async void PTenorBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PTenorReset();
            return;
        }

        if (!PBulletin.PBulletinEntryCheck(bulletin.LBulletinSubject))
        {
            return;
        }

        PCohortEntryUpdate(
            bulletin.LBulletinSubject == LSubject.LSubjectRegister ? 0 : bulletin.LBulletinId);
    }

    private void PSoundingHandle(object sender, TextChangedEventArgs e)
    {
        PGamutFind(PSounding.Text ?? string.Empty);
    }

    private void PDegreeHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pDegreeChoice = LCatalog.LCatalogOrderParse(choice, _pDegreeChoice);
        _lEngine.LEngineDegreeSave(_pDegreeChoice);
        PDegreeDropper.IsChecked = false;
        PGamutFind(PSounding.Text ?? string.Empty);
    }

    internal async void PDegreeRestore(LCatalogOrder order)
    {
        _pDegreeChoice = order;
        PChoice.PChoiceOrderApply(PDegreeDropdown, order);

        await PEnsign.PEnsignLoad(_lEngine);

        PGamutFind(PSounding.Text ?? string.Empty);
    }

    private void PQuestHandle(object sender, TextChangedEventArgs e)
    {
        PCohortFind();
    }

    private void PGrilleHandle(object sender, RoutedEventArgs e)
    {
        _pGrilleChoice = PChoice.PChoiceFilterRead(PGrilleList);
        _lEngine.LEngineGrilleSave(_pGrilleChoice);
        PGrilleMark.Visibility = _pGrilleChoice.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;
        PCohortFind();
    }

    internal async void PGrilleRestore(LCatalogFilter filter)
    {
        _pGrilleChoice = filter;
        PGrilleMark.Visibility = filter.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PGrilleList, _lEngine.LEngineLanguageRead(), filter, PGrilleHandle);
    }

    private void PGamutReset()
    {
        _pGamutChoice = null;
    }

    private void PGamutFind(string query)
    {
        IReadOnlyList<LCatalogRegister> read;
        try
        {
            read = _lEngine.LEngineRegisterFind(query, _pDegreeChoice);
        }
        catch (Exception exception)
        {
            _pTenorHost.PWindowFailureShow(PTenorFailure, exception);
            return;
        }

        _pGamutList.Clear();
        bool kept = false;
        foreach (LCatalogRegister row in read)
        {
            LRegister stored = row.LCatalogRegisterStored;
            bool chosen = stored.LRegisterId == _pGamutChoice;
            kept |= chosen;
            _pGamutList.Add(new PGamutItem(
                stored.LRegisterId,
                stored.LRegisterName.LStateValueShow(),
                row.LCatalogRegisterUsage,
                chosen));
        }

        if (!kept)
        {
            _pGamutChoice = null;
        }

        PGamutEmpty.Visibility = _pGamutList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PCohortFind();
    }

    private void PGamutHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PGamutItem item)
        {
            return;
        }

        _pGamutChoice = item.PGamutItemChosen ? null : item.PGamutItemId;
        PGamutFind(PSounding.Text ?? string.Empty);
    }

    internal void PGamutRegisterShow(long id)
    {
        _pGamutChoice = id;
        PSounding.Text = string.Empty;
        PQuest.Text = string.Empty;
        PGamutFind(string.Empty);
    }

    private void PTenorFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PTenorLeaveConfirm())
        {
            return;
        }

        if (_pGamutChoice is null && _pDisplayEntry is null)
        {
            PGamutRegisterCreate();
            return;
        }

        PCohortEntryCreate();
    }

    private void PGamutRegisterCreate()
    {
        if (PSCoinage.PSCoinageShow(_pTenorHost, "Coinage.Register") is not string name)
        {
            return;
        }

        LRegister created;
        try
        {
            created = _lEngine.LEngineRegisterCreate(name);
        }
        catch (Exception exception)
        {
            _pTenorHost.PWindowFailureShow("Register.CreateFailed", exception);
            return;
        }

        PTenorClear();
        PGamutRegisterShow(created.LRegisterId);
    }

    private void PTenorScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PTenorScribe);
        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PTenorLeaveConfirm())
            {
                PTenorScribeShow(true);
                return;
            }

            PTenorScribeShow(false);

            if (_pDisplayEntry is not null)
            {
                PCohortEntryShow(_pDisplayEntry.Value);
                return;
            }

            PTenorClear();
            return;
        }

        if (_pDisplayEntry is null)
        {
            PTenorClear();
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry.Value);
        PTenorScribeShow(true);
    }

    private void PTenorScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PTenorViewer.IsChecked = !editing;
        PTenorScribe.IsChecked = editing;
    }

    internal void PTenorScribeRestore(bool editing)
    {
        if (editing && _pDisplayEntry is null)
        {
            return;
        }

        if (editing)
        {
            PTenorMode.IsEnabled = true;
        }

        PTenorScribeShow(editing);
    }

    internal bool PTenorLeaveConfirm()
    {
        return _pTenorHost.PWindowDiscardConfirm(PTenorChangeCheck(), PTenorDraftFinish);
    }

    internal long PTenorVoyageRead()
    {
        return _pGamutChoice ?? 0;
    }

    private void PTenorEntryShow(long id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PTenorMode.IsEnabled = true;
    }

    private void PTenorClear()
    {
        _pDisplayEntry = null;
        PCohortSelect(null);
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PTenorScribeShow(false);
        PTenorMode.IsEnabled = false;
        PTenorBin.IsEnabled = false;
    }

    private void PTenorStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PTenorBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayEntry is not long id)
        {
            return;
        }

        if (!_pTenorHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lEngine.LEngineEntryDelete(id);
        }
        catch (Exception exception)
        {
            _pTenorHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PTenorClear();
    }
}
