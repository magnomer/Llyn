using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

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
        _lTenor.LTenorPanel.LPanelDraftUpdate();
    }

    private void PSoundingHandle(object sender, TextChangedEventArgs e)
    {
        _lTenor.LTenorSoundingSet(PSounding.Text ?? string.Empty);
    }

    private void PDegreeHandle(object sender, RoutedEventArgs e)
    {
        PDegreeDropper.IsChecked = false;
        _lTenor.LTenorDegreeSet(LChoice.LChoiceOrderRead(sender));
    }

    private void PQuestHandle(object sender, TextChangedEventArgs e)
    {
        _lTenor.LTenorQuestSet(PQuest.Text);
    }

    private void PGrilleHandle(object sender, RoutedEventArgs e)
    {
        _lTenor.LTenorGrilleSet(LChoice.LChoiceFilterRead(PGrilleList));
        PGrilleRestore();
    }

    internal async void PTenorVistaRestore()
    {
        _lTenor.LTenorVistaRestore(_pTenorHost.PWindowDeportment);
        _lTenor.LTenorObserverAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PGamutFind));
        _lTenor.LTenorObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PTenorWorkspaceUpdate));
        _lTenor.LTenorObserverAttach(LSubject.LSubjectRegister, PObserver.PObserverCreate(this, PTenorRegisterUpdate));
        _lTenor.LTenorObserverAttach(LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PGamutFind));
        _lTenor.LTenorObserverAttach(LSubject.LSubjectSettings, PObserver.PObserverCreate(this, PGamutFind));
        LPanel panel = _lTenor.LTenorPanel;
        panel.LPanelObserverAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, panel.LPanelEntryHandle));
        panel.LPanelObserverAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PCohortFind));
        panel.LPanelChosenAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, panel.LPanelDraftUpdate));
        _lTenor.LTenorObserverAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PGamutFind));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(
            PDegreeList,
            "Degree",
            PDegreeHandle,
            [
                LCatalogOrder.LCatalogOrderName,
                LCatalogOrder.LCatalogOrderReverse,
                LCatalogOrder.LCatalogOrderUsage,
            ]);
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

        _pTenorHost.PVoyageRecord();
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
            if (!_lTenor.LTenorPanel.LPanelBinEnabled)
            {
                PGamutRegisterCreate();
                return;
            }
        }

        _lTenor.LTenorEntryCreate();
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

        _lTenor.LTenorPanel.LPanelClear();
        PGamutRegisterShow(created.LRegisterId);
    }

    private void PTenorScribeHandle(object sender, RoutedEventArgs e)
    {
        _lTenor.LTenorPanel.LPanelScribeSet(ReferenceEquals(sender, PTenorScribe));
    }

    internal void PTenorScribeRestore(bool editing)
    {
        _lTenor.LTenorPanel.LPanelScribeRestore(editing);
    }

    internal bool PTenorLeaveConfirm()
    {
        return _lTenor.LTenorPanel.LPanelLeaveConfirm();
    }

    internal long PTenorVoyageRead()
    {
        return _lTenor.LTenorChosen ?? 0;
    }

    private void PTenorStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PTenorBinHandle(object sender, RoutedEventArgs e)
    {
        _lTenor.LTenorPanel.LPanelDelete();
    }
}
