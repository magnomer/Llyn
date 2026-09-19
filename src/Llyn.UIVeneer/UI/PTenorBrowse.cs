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

    private LVista? _pTenorVista;

    private async void PTenorWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_lEngine);
        PTenorReset();
    }

    private void PTenorRegisterUpdate()
    {
        PGamutFind();
        PTenorEntryUpdate();
    }

    private void PSoundingHandle(object sender, TextChangedEventArgs e)
    {
        _pTenorVista?.LVistaQuerySet(PSounding.Text ?? string.Empty);
    }

    private void PDegreeHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pTenorVista is null)
        {
            return;
        }

        PDegreeDropper.IsChecked = false;
        _pTenorVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pTenorVista.LVistaOrder));
    }

    private void PQuestHandle(object sender, TextChangedEventArgs e)
    {
        _pCohortVista?.LVistaQuerySet(PQuest.Text);
    }

    private void PGrilleHandle(object sender, RoutedEventArgs e)
    {
        if (_pTenorVista is null)
        {
            return;
        }

        _pTenorVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PGrilleList));
        PGrilleRestore();
    }

    internal async void PTenorVistaRestore(LVista vista, LVista cohort)
    {
        _pTenorVista = vista;
        _pCohortVista = cohort;
        vista.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PGamutFind));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PTenorWorkspaceUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectRegister, new PObserver(this, PTenorRegisterUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PGamutFind));
        vista.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PGamutFind));
        cohort.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, PCohortEntryUpdate));
        cohort.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PTenorEntryUpdate));
        cohort.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PCohortFind));
        PDisplay.PDisplayVistaRestore(cohort);
        PEditor.PEditorVistaRestore(cohort);
        PDegreeRestore();
        PGrilleRestore();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PGrilleList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PGrilleHandle);
        vista.LVistaQuerySet(PSounding.Text ?? string.Empty);
        cohort.LVistaQuerySet(PQuest.Text);
        PGamutFind();
    }

    private void PDegreeRestore()
    {
        if (_pTenorVista is not null)
        {
            PChoice.PChoiceOrderApply(PDegreeDropdown, _pTenorVista.LVistaOrder);
        }
    }

    private void PGrilleRestore()
    {
        if (_pTenorVista is not LVista vista)
        {
            PGrilleMark.Visibility = Visibility.Collapsed;
            return;
        }

        PGrilleMark.Visibility = vista.LVistaFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PGamutReset()
    {
        _pTenorVista?.LVistaSelect(null);
    }

    private void PGamutFind()
    {
        if (_pTenorVista is null)
        {
            return;
        }

        IReadOnlyList<LCatalogRegister> read;
        try
        {
            read = _lEngine.LEngineRegisterFind(_pTenorVista);
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
        if (_pTenorVista is null)
        {
            return;
        }

        _pTenorVista.LVistaSelect(id);
        PGamutFind();
    }

    internal void PGamutRegisterShow(long id)
    {
        PSounding.Text = string.Empty;
        PQuest.Text = string.Empty;
        _pTenorVista?.LVistaSelect(id);
        PGamutFind();
    }

    private void PTenorFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PTenorLeaveConfirm())
        {
            return;
        }

        if (_pTenorVista?.LVistaChosen is null)
        {
            if (_pCohortVista?.LVistaChosen is null)
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

            if (_pCohortVista?.LVistaChosen is long shown)
            {
                PCohortEntryShow(shown);
                return;
            }

            PTenorClear();
            return;
        }

        if (_pCohortVista?.LVistaChosen is not long edited)
        {
            PTenorClear();
            return;
        }

        PEditor.PEditorEntryShow(edited);
        PTenorScribeShow(true);
    }

    private void PTenorScribeShow(bool editing)
    {
        _pCohortVista?.LVistaEditingSet(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PTenorViewer.IsChecked = !editing;
        PTenorScribe.IsChecked = editing;
    }

    internal void PTenorScribeRestore(bool editing)
    {
        if (editing)
        {
            if (_pCohortVista?.LVistaChosen is null)
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
        return _pTenorVista?.LVistaChosen ?? 0;
    }

    private void PTenorEntryShow(LEntryDraft draft)
    {
        PDisplay.PDisplayShow(draft);
        PTenorMode.IsEnabled = true;
    }

    private void PTenorClear()
    {
        _pCohortVista?.LVistaSelect(null);
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
            _pCohortVista?.LVistaDelete();
        }
        catch (Exception exception)
        {
            _pTenorHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PTenorClear();
    }
}
