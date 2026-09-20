using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

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
        PTaxonomyEntryUpdate();
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
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        PFunnelDropper.IsChecked = false;
        _lTaxonomy.LTaxonomyFunnelSet(choice);
    }

    private void PLatticeHandle(object sender, RoutedEventArgs e)
    {
        _lTaxonomy.LTaxonomyLatticeSet(PChoice.PChoiceFilterRead(PLatticeList));
        PLatticeRestore();
    }

    internal async void PTaxonomyVistaRestore()
    {
        _lTaxonomy.LTaxonomyVistaRestore(_pTaxonomyHost.PWindowPosture);
        _lTaxonomy.LTaxonomyObserverAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PDirectoryFind));
        _lTaxonomy.LTaxonomyObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PTaxonomyWorkspaceUpdate));
        _lTaxonomy.LTaxonomyObserverAttach(LSubject.LSubjectTag, PObserver.PObserverCreate(this, PTaxonomyTagUpdate));
        _lTaxonomy.LTaxonomyObserverAttach(LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PDirectoryFind));
        _lTaxonomy.LTaxonomyObserverAttach(LSubject.LSubjectSettings, PObserver.PObserverCreate(this, PDirectoryFind));
        _lTaxonomy.LTaxonomyMembershipAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PMembershipEntryUpdate));
        _lTaxonomy.LTaxonomyEntryAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PTaxonomyEntryUpdate));
        _lTaxonomy.LTaxonomyMembershipAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PMembershipFind));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
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
            if (_lTaxonomy.LTaxonomyMembershipChosen is null)
            {
                PDirectoryTagCreate();
                return;
            }
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
            created = _lTaxonomy.LTaxonomyTagCreate(text);
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

            if (_lTaxonomy.LTaxonomyMembershipChosen is long shown)
            {
                PMembershipEntryShow(shown);
                return;
            }

            PTaxonomyClear();
            return;
        }

        if (_lTaxonomy.LTaxonomyMembershipChosen is not long edited)
        {
            PTaxonomyClear();
            return;
        }

        PEditor.PEditorEntryShow(edited);
        PTaxonomyScribeShow(true);
    }

    private void PTaxonomyScribeShow(bool editing)
    {
        _lTaxonomy.LTaxonomyScribeSet(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PTaxonomyViewer.IsChecked = !editing;
        PTaxonomyScribe.IsChecked = editing;
    }

    internal void PTaxonomyScribeRestore(bool editing)
    {
        if (editing)
        {
            if (_lTaxonomy.LTaxonomyMembershipChosen is null)
            {
                return;
            }
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
        return _lTaxonomy.LTaxonomyChosen ?? 0;
    }

    private void PTaxonomyEntryShow(LEntryDraft draft)
    {
        PDisplay.PDisplayShow(draft);
        PTaxonomyMode.IsEnabled = true;
    }

    private void PTaxonomyClear()
    {
        _lTaxonomy.LTaxonomyMembershipSelect(null);
        PMembershipFind();
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
        if (!_pTaxonomyHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lTaxonomy.LTaxonomyMembershipDelete();
        }
        catch (Exception exception)
        {
            _pTaxonomyHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PTaxonomyClear();
    }
}
