using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PLibrary
{
    private readonly ObservableCollection<PIndexItem> _pIndexList = [];

    private long? _pDisplayEntry;

    private LCatalogOrder _pOrderChoice;

    private LCatalogFilter _pSieveChoice = LCatalogFilter.LCatalogFilterEmpty;

    private async void PLibraryBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PLibraryReset();
            return;
        }

        if (PBulletin.PBulletinEntryCheck(bulletin.LBulletinSubject))
        {
            PIndexEntryUpdate(bulletin);
        }
    }

    private void PInquiryHandle(object sender, TextChangedEventArgs e)
    {
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    private void POrderHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pOrderChoice = LCatalog.LCatalogOrderParse(choice, _pOrderChoice);
        _lEngine.LEngineOrderSave(_pOrderChoice);
        POrderDropper.IsChecked = false;
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    internal async void POrderRestore(LCatalogOrder order)
    {
        _pOrderChoice = order;
        PChoice.PChoiceOrderApply(POrderDropdown, order);

        await PEnsign.PEnsignLoad(_lEngine);

        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    private void PSieveHandle(object sender, RoutedEventArgs e)
    {
        _pSieveChoice = PChoice.PChoiceFilterRead(PSieveList);
        _lEngine.LEngineSieveSave(_pSieveChoice);
        PSieveMark.Visibility = _pSieveChoice.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    internal async void PSieveRestore(LCatalogFilter filter)
    {
        _pSieveChoice = filter;
        PSieveMark.Visibility = filter.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PSieveList, _lEngine.LEngineLanguageRead(), filter, PSieveHandle);
    }

    private void PIndexSelect(long? id)
    {
        foreach (PIndexItem item in _pIndexList)
        {
            item.PIndexItemChosen = id is not null
                && item.PIndexItemId == id;
        }
    }

    private void PIndexFind(string query)
    {
        _pIndexList.Clear();
        foreach (LEntry entry in _lEngine.LEngineEntryFind(query, _pOrderChoice, _pSieveChoice))
        {
            _pIndexList.Add(new PIndexItem(
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                _lEngine.LEngineEpithetRead(entry.LEntryId)));
        }

        PTwin.PTwinNameApply(
            _pIndexList,
            row => row.PIndexItemHeadword,
            (row, name) => row.PIndexItemName = name,
            row => row.PIndexItemId);

        PIndexEmpty.Visibility = _pIndexList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PIndexSelect(_pDisplayEntry);
    }

    private void PIndexHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PIndexItem item)
        {
            return;
        }

        if (!PLibraryLeaveConfirm())
        {
            return;
        }

        PIndexEntryShow(item.PIndexItemId);
    }

    internal void PIndexEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pLibraryHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PLibraryClear();
            PIndexFind(PInquiry.Text ?? string.Empty);
            return;
        }

        _pDisplayEntry = id;
        PIndexSelect(id);
        PLibraryEntryShow(id, draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PIndexEntryUpdate(LBulletin bulletin)
    {
        long id = bulletin.LBulletinId;
        bool stored = bulletin.LBulletinSubject == LSubject.LSubjectEntry;
        if (stored && id > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pDisplayEntry = id;
            PIndexSelect(id);
        }

        PLibraryCommandApply();

        PIndexFind(PInquiry.Text ?? string.Empty);

        if (!stored
            || _pDisplayEntry is not long shown
            || (id > 0 && shown != id))
        {
            return;
        }

        LEntry? entry;
        try
        {
            entry = _lEngine.LEngineEntryRead(shown);
        }
        catch (Exception)
        {
            return;
        }

        if (entry is null)
        {
            PLibraryClear();
            return;
        }

        PLibraryMode.IsEnabled = true;
        PLibraryCommandApply();
    }

    private void PLibraryFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PLibraryLeaveConfirm())
        {
            return;
        }

        PLibraryClear();
        PLibraryMode.IsEnabled = true;
        PLibraryScribeShow(true);
    }

    private void PLibraryScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PLibraryScribe);
        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PLibraryLeaveConfirm())
            {
                PLibraryScribeShow(true);
                return;
            }

            PLibraryScribeShow(false);

            if (_pDisplayEntry is not null)
            {
                PIndexEntryShow(_pDisplayEntry.Value);
                return;
            }

            PLibraryClear();
            return;
        }

        if (_pDisplayEntry is null)
        {
            PLibraryClear();
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry.Value);
        PLibraryScribeShow(true);
    }

    private void PLibraryScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PLibraryViewer.IsChecked = !editing;
        PLibraryScribe.IsChecked = editing;
    }

    internal void PLibraryScribeRestore(bool editing)
    {
        if (editing && _pDisplayEntry is null)
        {
            return;
        }

        if (editing)
        {
            PLibraryMode.IsEnabled = true;
        }

        PLibraryScribeShow(editing);
    }

    internal bool PLibraryLeaveConfirm()
    {
        return _pLibraryHost.PWindowDiscardConfirm(PLibraryChangeCheck(), PLibraryDraftFinish);
    }

    private void PLibraryEntryShow(long id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PLibraryMode.IsEnabled = true;
        PLibraryCommandApply();
    }

    internal void PLibraryCommandApply()
    {
        PLibraryBin.IsEnabled = _pDisplayEntry is not null;
    }

    private void PLibraryClear()
    {
        _pDisplayEntry = null;
        PIndexSelect(null);
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PLibraryScribeShow(false);
        PLibraryMode.IsEnabled = false;
        PLibraryCommandApply();
    }

    private void PLibraryStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PLibraryBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayEntry is not long id)
        {
            return;
        }

        if (!_pLibraryHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lEngine.LEngineEntryDelete(id);
        }
        catch (Exception exception)
        {
            _pLibraryHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PLibraryClear();
    }
}
