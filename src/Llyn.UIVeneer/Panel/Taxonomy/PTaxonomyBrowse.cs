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

public partial class PTaxonomy
{
    private readonly ObservableCollection<PDirectoryItem> _pDirectoryList = [];

    private readonly ObservableCollection<PMembershipItem> _pMembershipList = [];

    private async void PTaxonomyWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_pTaxonomyHost.PWindowDeportment);
        PTaxonomyReset();
    }

    private void PTaxonomyTagUpdate()
    {
        PDirectoryFind();
        _lTaxonomy.LTaxonomyPanel.LPanelDraftUpdate();
    }

    private void PExplorationHandle(object sender, TextChangedEventArgs e)
    {
        _lTaxonomy.LTaxonomyExplorationSet(PExploration.Text ?? string.Empty);
    }

    private void PScoutHandle(object sender, TextChangedEventArgs e)
    {
        _lTaxonomy.LTaxonomyScoutSet(PScout.Text);
    }

    private void PFunnelHandle(object sender, RoutedEventArgs e)
    {
        PFunnelDropper.IsChecked = false;
        _lTaxonomy.LTaxonomyFunnelSet(PSender.PSenderOrderRead(sender));
    }

    private void PLatticeHandle(object sender, RoutedEventArgs e)
    {
        _lTaxonomy.LTaxonomyLatticeSet(PChoice.PChoiceFilterRead(PLatticeList));
        PLatticeRestore();
    }

    internal async void PTaxonomyVistaRestore()
    {
        _lTaxonomy.LTaxonomyVistaRestore(_pTaxonomyHost.PWindowDeportment);
        _lTaxonomy.LTaxonomyObserverAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PDirectoryFind));
        _lTaxonomy.LTaxonomyObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PTaxonomyWorkspaceUpdate));
        _lTaxonomy.LTaxonomyObserverAttach(LSubject.LSubjectTag, PObserver.PObserverCreate(this, PTaxonomyTagUpdate));
        _lTaxonomy.LTaxonomyObserverAttach(LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PDirectoryFind));
        _lTaxonomy.LTaxonomyObserverAttach(LSubject.LSubjectSettings, PObserver.PObserverCreate(this, PDirectoryFind));
        LPanel panel = _lTaxonomy.LTaxonomyPanel;
        panel.LPanelObserverAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, panel.LPanelEntryHandle));
        panel.LPanelObserverAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PMembershipFind));
        panel.LPanelChosenAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, panel.LPanelDraftUpdate));
        _lTaxonomy.LTaxonomyObserverAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PDirectoryFind));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(
            PFunnelList,
            "Funnel",
            PFunnelHandle,
            [
                LCatalogOrder.LCatalogOrderName,
                LCatalogOrder.LCatalogOrderReverse,
            ]);
        PChoice.PChoiceOrderApply(PFunnelDropdown, _lTaxonomy.LTaxonomyOrder);
        PLatticeRestore();

        await PEnsign.PEnsignLoad(_pTaxonomyHost.PWindowDeportment);

        PChoice.PChoiceFilterBuild(
            PLatticeList, _lTaxonomy.LTaxonomyLanguageRead(), _lTaxonomy.LTaxonomyFilter, PLatticeHandle);
        _lTaxonomy.LTaxonomyExplorationSet(PExploration.Text ?? string.Empty);
        _lTaxonomy.LTaxonomyScoutSet(PScout.Text);
        PDirectoryFind();
    }

    private void PLatticeRestore()
    {
        PLatticeMark.Visibility = _lTaxonomy.LTaxonomyFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PDirectoryReset()
    {
        _lTaxonomy.LTaxonomySelect(null);
    }

    private void PDirectoryFind()
    {
        IReadOnlyList<LCatalogTag> read;
        try
        {
            read = _lTaxonomy.LTaxonomyRowsRead();
        }
        catch (Exception exception)
        {
            _pTaxonomyHost.PWindowFailureShow("Tag.LoadFailed", exception);
            return;
        }

        _pDirectoryList.Clear();
        foreach (LCatalogTag row in read)
        {
            _pDirectoryList.Add(new PDirectoryItem(
                row.LCatalogTagStored.LTagId, row.LCatalogTagStored.LTagText, row.LCatalogTagChosen));
        }

        PDirectoryEmpty.Visibility = _pDirectoryList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PMembershipFind();
    }

    private void PDirectoryHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PDirectoryItem item)
        {
            return;
        }

        _pTaxonomyHost.PVoyageRecord();
        PDirectorySelect(item.PDirectoryItemChosen ? null : item.PDirectoryItemId);
    }

    private void PDirectorySelect(long? id)
    {
        _lTaxonomy.LTaxonomySelect(id);
        PDirectoryFind();
    }

    internal void PDirectoryTagShow(long id)
    {
        PExploration.Text = string.Empty;
        PScout.Text = string.Empty;
        _lTaxonomy.LTaxonomySelect(id);
        PDirectoryFind();
    }

    private void PTaxonomyFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PTaxonomyLeaveConfirm())
        {
            return;
        }

        if (_lTaxonomy.LTaxonomyChosen is null)
        {
            if (!_lTaxonomy.LTaxonomyPanel.LPanelBinEnabled)
            {
                PDirectoryTagCreate();
                return;
            }
        }

        _lTaxonomy.LTaxonomyEntryCreate();
    }

    private void PDirectoryTagCreate()
    {
        if (PSCoinage.PSCoinageShow(_pTaxonomyHost, "Coinage.Tag") is not string text)
        {
            return;
        }

        LTag created;
        try
        {
            created = _lTaxonomy.LTaxonomyTagCreate(text);
        }
        catch (Exception exception)
        {
            _pTaxonomyHost.PWindowFailureShow("Tag.CreateFailed", exception);
            return;
        }

        _lTaxonomy.LTaxonomyPanel.LPanelClear();
        PDirectoryTagShow(created.LTagId);
    }

    private void PTaxonomyScribeHandle(object sender, RoutedEventArgs e)
    {
        _lTaxonomy.LTaxonomyPanel.LPanelScribeSet(ReferenceEquals(sender, PTaxonomyScribe));
    }

    internal void PTaxonomyScribeRestore(bool editing)
    {
        _lTaxonomy.LTaxonomyPanel.LPanelScribeRestore(editing);
    }

    internal bool PTaxonomyLeaveConfirm()
    {
        return _lTaxonomy.LTaxonomyPanel.LPanelLeaveConfirm();
    }

    internal long PTaxonomyVoyageRead()
    {
        return _lTaxonomy.LTaxonomyChosen ?? 0;
    }

    private void PTaxonomyStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PTaxonomyBinHandle(object sender, RoutedEventArgs e)
    {
        _lTaxonomy.LTaxonomyPanel.LPanelDelete();
    }
}
