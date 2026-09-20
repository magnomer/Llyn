using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PTenor
{
    private const string PTenorFailure = "Register.LoadFailed";

    private readonly ObservableCollection<PGamutItem> _pGamutList = [];

    private readonly ObservableCollection<PCohortItem> _pCohortList = [];

    private async void PTenorWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_pTenorHost.PWindowDeportment);
        PTenorReset();
    }

    private void PTenorRegisterUpdate()
    {
        PGamutFind();
        PTenorEntryUpdate();
    }

    private void PSoundingHandle(object sender, TextChangedEventArgs e)
    {
        _lTenor.LTenorSoundingSet(PSounding.Text ?? string.Empty);
    }

    private void PDegreeHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        PDegreeDropper.IsChecked = false;
        _lTenor.LTenorDegreeSet(choice);
    }

    private void PQuestHandle(object sender, TextChangedEventArgs e)
    {
        _lTenor.LTenorQuestSet(PQuest.Text);
    }

    private void PGrilleHandle(object sender, RoutedEventArgs e)
    {
        _lTenor.LTenorGrilleSet(PChoice.PChoiceFilterRead(PGrilleList));
        PGrilleRestore();
    }

    internal async void PTenorVistaRestore()
    {
        _lTenor.LTenorVistaRestore(_pTenorHost.PWindowPosture);
        _lTenor.LTenorObserverAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PGamutFind));
        _lTenor.LTenorObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PTenorWorkspaceUpdate));
        _lTenor.LTenorObserverAttach(LSubject.LSubjectRegister, PObserver.PObserverCreate(this, PTenorRegisterUpdate));
        _lTenor.LTenorObserverAttach(LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PGamutFind));
        _lTenor.LTenorObserverAttach(LSubject.LSubjectSettings, PObserver.PObserverCreate(this, PGamutFind));
        _lTenor.LTenorCohortAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PCohortEntryUpdate));
        _lTenor.LTenorEntryAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PTenorEntryUpdate));
        _lTenor.LTenorCohortAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PCohortFind));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderApply(PDegreeDropdown, _lTenor.LTenorOrder);
        PGrilleRestore();

        await PEnsign.PEnsignLoad(_pTenorHost.PWindowDeportment);

        PChoice.PChoiceFilterBuild(PGrilleList, _lTenor.LTenorLanguageRead(), _lTenor.LTenorFilter, PGrilleHandle);
        _lTenor.LTenorSoundingSet(PSounding.Text ?? string.Empty);
        _lTenor.LTenorQuestSet(PQuest.Text);
        PGamutFind();
    }

    private void PGrilleRestore()
    {
        PGrilleMark.Visibility = _lTenor.LTenorFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PGamutReset()
    {
        _lTenor.LTenorSelect(null);
    }

    private void PGamutFind()
    {
        IReadOnlyList<LCatalogRegister> read;
        try
        {
            read = _lTenor.LTenorRowsRead();
        }
        catch (Exception exception)
        {
            _pTenorHost.PWindowFailureShow(PTenorFailure, exception);
            return;
        }

        _pGamutList.Clear();
        foreach (LCatalogRegister row in read)
        {
            LRegister stored = row.LCatalogRegisterStored;
            _pGamutList.Add(new PGamutItem(
                stored.LRegisterId,
                stored.LRegisterName.LStateValueShow(),
                row.LCatalogRegisterUsage,
                row.LCatalogRegisterChosen));
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

        PGamutSelect(item.PGamutItemChosen ? null : item.PGamutItemId);
    }

    private void PGamutSelect(long? id)
    {
        _lTenor.LTenorSelect(id);
        PGamutFind();
    }

    internal void PGamutRegisterShow(long id)
    {
        PSounding.Text = string.Empty;
        PQuest.Text = string.Empty;
        _lTenor.LTenorSelect(id);
        PGamutFind();
    }

    private void PTenorFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PTenorLeaveConfirm())
        {
            return;
        }

        if (_lTenor.LTenorChosen is null)
        {
            if (_lTenor.LTenorCohortChosen is null)
            {
                PGamutRegisterCreate();
                return;
            }
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
            created = _lTenor.LTenorRegisterCreate(name);
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

            if (_lTenor.LTenorCohortChosen is long shown)
            {
                PCohortEntryShow(shown);
                return;
            }

            PTenorClear();
            return;
        }

        if (_lTenor.LTenorCohortChosen is not long edited)
        {
            PTenorClear();
            return;
        }

        PEditor.PEditorEntryShow(edited);
        PTenorScribeShow(true);
    }

    private void PTenorScribeShow(bool editing)
    {
        _lTenor.LTenorScribeSet(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PTenorViewer.IsChecked = !editing;
        PTenorScribe.IsChecked = editing;
    }

    internal void PTenorScribeRestore(bool editing)
    {
        if (editing)
        {
            if (_lTenor.LTenorCohortChosen is null)
            {
                return;
            }
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
        return _lTenor.LTenorChosen ?? 0;
    }

    private void PTenorEntryShow(LEntryDraft draft)
    {
        PDisplay.PDisplayShow(draft);
        PTenorMode.IsEnabled = true;
    }

    private void PTenorClear()
    {
        _lTenor.LTenorCohortSelect(null);
        PCohortFind();
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
        if (!_pTenorHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lTenor.LTenorCohortDelete();
        }
        catch (Exception exception)
        {
            _pTenorHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PTenorClear();
    }
}
