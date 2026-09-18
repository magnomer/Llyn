using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PTaxonomy
{
    private readonly ObservableCollection<PDirectoryItem> _pDirectoryList = [];

    private readonly ObservableCollection<PMembershipItem> _pMembershipList = [];

    private LVista? _pTaxonomyVista;

    private async void PTaxonomyWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_lEngine);
        PTaxonomyReset();
    }

    private void PTaxonomyTagUpdate()
    {
        PDirectoryFind();
        PTaxonomyEntryUpdate();
    }

    private void PExplorationHandle(object sender, TextChangedEventArgs e)
    {
        _pTaxonomyVista?.LVistaQuerySet(PExploration.Text ?? string.Empty);
    }

    private void PScoutHandle(object sender, TextChangedEventArgs e)
    {
        PMembershipFind();
    }

    private void PFunnelHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pTaxonomyVista is null)
        {
            return;
        }

        PFunnelDropper.IsChecked = false;
        _pTaxonomyVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pTaxonomyVista.LVistaOrder));
    }

    private void PLatticeHandle(object sender, RoutedEventArgs e)
    {
        if (_pTaxonomyVista is null)
        {
            return;
        }

        _pTaxonomyVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PLatticeList));
        PLatticeRestore();
    }

    internal async void PTaxonomyVistaRestore(LVista vista, LVista membership)
    {
        _pTaxonomyVista = vista;
        _pMembershipVista = membership;
        vista.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PDirectoryFind));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PTaxonomyWorkspaceUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectTag, new PObserver(this, PTaxonomyTagUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PDirectoryFind));
        vista.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PDirectoryFind));
        membership.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, PMembershipEntryUpdate));
        membership.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PTaxonomyEntryUpdate));
        PDisplay.PDisplayVistaRestore(membership);
        PFunnelRestore();
        PLatticeRestore();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PLatticeList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PLatticeHandle);
        vista.LVistaQuerySet(PExploration.Text ?? string.Empty);
        PDirectoryFind();
    }

    private void PFunnelRestore()
    {
        if (_pTaxonomyVista is not null)
        {
            PChoice.PChoiceOrderApply(PFunnelDropdown, _pTaxonomyVista.LVistaOrder);
        }
    }

    private void PLatticeRestore()
    {
        bool active = _pTaxonomyVista?.LVistaFilter.LCatalogFilterActive == true;
        PLatticeMark.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PDirectoryReset()
    {
        _pTaxonomyVista?.LVistaSelect(null);
    }

    private void PDirectoryFind()
    {
        if (_pTaxonomyVista is null)
        {
            return;
        }

        IReadOnlyList<LTag> read;
        try
        {
            read = _lEngine.LEngineTagFind(_pTaxonomyVista);
        }
        catch (Exception exception)
        {
            _pTaxonomyHost.PWindowFailureShow("Tag.LoadFailed", exception);
            return;
        }

        long? chosen = _pTaxonomyVista.LVistaChosen;
        if (chosen is not null && !read.Any(tag => tag.LTagId == chosen))
        {
            _pTaxonomyVista.LVistaSelect(null);
            chosen = null;
        }

        _pDirectoryList.Clear();
        foreach (LTag tag in read)
        {
            _pDirectoryList.Add(new PDirectoryItem(tag.LTagId, tag.LTagText, tag.LTagId == chosen));
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

        PDirectorySelect(item.PDirectoryItemChosen ? null : item.PDirectoryItemId);
    }

    private void PDirectorySelect(long? id)
    {
        if (_pTaxonomyVista is null)
        {
            return;
        }

        _pTaxonomyVista.LVistaSelect(id);
        foreach (PDirectoryItem row in _pDirectoryList)
        {
            row.PDirectoryItemChosen = row.PDirectoryItemId == id;
        }

        PMembershipFind();
    }

    internal void PDirectoryTagShow(long id)
    {
        PExploration.Text = string.Empty;
        PScout.Text = string.Empty;
        _pTaxonomyVista?.LVistaSelect(id);
        PDirectoryFind();
    }

    private void PTaxonomyFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PTaxonomyLeaveConfirm())
        {
            return;
        }

        if (_pTaxonomyVista?.LVistaChosen is null && _pMembershipVista?.LVistaChosen is null)
        {
            PDirectoryTagCreate();
            return;
        }

        PMembershipEntryCreate();
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
            created = _lEngine.LEngineTagCreate(text);
        }
        catch (Exception exception)
        {
            _pTaxonomyHost.PWindowFailureShow("Tag.CreateFailed", exception);
            return;
        }

        PTaxonomyClear();
        PDirectoryTagShow(created.LTagId);
    }

    private void PTaxonomyScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PTaxonomyScribe);
        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PTaxonomyLeaveConfirm())
            {
                PTaxonomyScribeShow(true);
                return;
            }

            PTaxonomyScribeShow(false);

            if (_pMembershipVista?.LVistaChosen is long shown)
            {
                PMembershipEntryShow(shown);
                return;
            }

            PTaxonomyClear();
            return;
        }

        if (_pMembershipVista?.LVistaChosen is not long edited)
        {
            PTaxonomyClear();
            return;
        }

        PEditor.PEditorEntryShow(edited);
        PTaxonomyScribeShow(true);
    }

    private void PTaxonomyScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PTaxonomyViewer.IsChecked = !editing;
        PTaxonomyScribe.IsChecked = editing;
    }

    internal void PTaxonomyScribeRestore(bool editing)
    {
        if (editing && _pMembershipVista?.LVistaChosen is null)
        {
            return;
        }

        if (editing)
        {
            PTaxonomyMode.IsEnabled = true;
        }

        PTaxonomyScribeShow(editing);
    }

    internal bool PTaxonomyLeaveConfirm()
    {
        return _pTaxonomyHost.PWindowDiscardConfirm(PTaxonomyChangeCheck(), PTaxonomyDraftFinish);
    }

    internal long PTaxonomyVoyageRead()
    {
        return _pTaxonomyVista?.LVistaChosen ?? 0;
    }

    private void PTaxonomyEntryShow(LEntryDraft draft)
    {
        PDisplay.PDisplayShow(draft);
        PTaxonomyMode.IsEnabled = true;
    }

    private void PTaxonomyClear()
    {
        _pMembershipVista?.LVistaSelect(null);
        PMembershipChosenApply();
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PTaxonomyScribeShow(false);
        PTaxonomyMode.IsEnabled = false;
        PTaxonomyBin.IsEnabled = false;
    }

    private void PTaxonomyStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PTaxonomyBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pMembershipVista?.LVistaChosen is not long id)
        {
            return;
        }

        if (!_pTaxonomyHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lEngine.LEngineEntryDelete(id);
        }
        catch (Exception exception)
        {
            _pTaxonomyHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PTaxonomyClear();
    }
}
