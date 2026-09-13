using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PPhonology
{
    private readonly ObservableCollection<PInventoryItem> _pInventoryList = [];

    private long? _pDisplayEntry;

    private LCatalogOrder _pSequenceChoice;

    private LCatalogFilter _pLensChoice = LCatalogFilter.LCatalogFilterEmpty;

    private async void PPhonologyBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PPhonologyReset();
            return;
        }

        PInventoryEntryUpdate(bulletin.LBulletinId);
    }

    private void PProbeHandle(object sender, TextChangedEventArgs e)
    {
        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    private void PSequenceHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pSequenceChoice = LCatalog.LCatalogOrderParse(choice, _pSequenceChoice);
        _lEngine.LEngineSequenceSave(_pSequenceChoice);
        PSequenceDropper.IsChecked = false;
        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    internal async void PSequenceRestore(LCatalogOrder order)
    {
        _pSequenceChoice = order;
        PChoice.PChoiceOrderApply(PSequenceDropdown, order);

        await PEnsign.PEnsignLoad(_lEngine);

        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    private void PLensHandle(object sender, RoutedEventArgs e)
    {
        _pLensChoice = PChoice.PChoiceFilterRead(PLensList);
        _lEngine.LEngineLensSave(_pLensChoice);
        PLensMark.Visibility = _pLensChoice.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;
        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    internal async void PLensRestore(LCatalogFilter filter)
    {
        _pLensChoice = filter;
        PLensMark.Visibility = filter.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PLensList, _lEngine.LEngineLanguageRead(), filter, PLensHandle);
    }

    private void PInventorySelect(long? id)
    {
        foreach (PInventoryItem item in _pInventoryList)
        {
            item.PInventoryItemChosen = id is not null
                && item.PInventoryItemId == id;
        }
    }

    private void PInventoryFind(string query)
    {
        _pInventoryList.Clear();
        foreach (LCatalogPronunciation row in _lEngine.LEnginePronunciationFind(query, _pSequenceChoice, _pLensChoice))
        {
            _pInventoryList.Add(new PInventoryItem(
                row.LCatalogPronunciationEntry.LEntryId,
                row.LCatalogPronunciationEntry.LEntryHeadword,
                row.LCatalogPronunciationEntry.LEntryLanguage,
                row.LCatalogPronunciationSound));
        }

        PTwin.PTwinNameApply(
            _pInventoryList,
            row => row.PInventoryItemHeadword,
            (row, name) => row.PInventoryItemName = name,
            row => row.PInventoryItemId);

        PInventoryEmpty.Visibility = _pInventoryList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PInventorySelect(_pDisplayEntry);
    }

    private void PInventoryHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PInventoryItem item)
        {
            return;
        }

        if (!PPhonologyLeaveConfirm())
        {
            return;
        }

        PInventoryEntryShow(item.PInventoryItemId);
    }

    private void PInventoryEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pPhonologyHost.PWindowFailureShow("Sound.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PPhonologyClear();
            PInventoryFind(PProbe.Text ?? string.Empty);
            return;
        }

        _pDisplayEntry = id;
        PInventorySelect(id);
        PPhonologyBin.IsEnabled = true;
        PPhonologyEntryShow(id, draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PInventoryEntryUpdate(long id)
    {
        if (id > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pDisplayEntry = id;
            PInventorySelect(id);
        PPhonologyBin.IsEnabled = true;
        }

        PInventoryFind(PProbe.Text ?? string.Empty);

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
            PPhonologyClear();
            return;
        }

        PPhonologyEntryShow(shown, draft);
    }

    private void PPhonologyFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PPhonologyLeaveConfirm())
        {
            return;
        }

        PPhonologyClear();
        PPhonologyMode.IsEnabled = true;
        PPhonologyScribeShow(true);
    }

    private void PPhonologyScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PPhonologyScribe);
        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PPhonologyLeaveConfirm())
            {
                PPhonologyScribeShow(true);
                return;
            }

            PPhonologyScribeShow(false);

            if (_pDisplayEntry is not null)
            {
                PInventoryEntryShow(_pDisplayEntry.Value);
                return;
            }

            PPhonologyClear();
            return;
        }

        if (_pDisplayEntry is null)
        {
            PPhonologyClear();
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry.Value);
        PPhonologyScribeShow(true);
    }

    private void PPhonologyScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PPhonologyViewer.IsChecked = !editing;
        PPhonologyScribe.IsChecked = editing;
    }

    internal void PPhonologyScribeRestore(bool editing)
    {
        if (editing && _pDisplayEntry is null)
        {
            return;
        }

        if (editing)
        {
            PPhonologyMode.IsEnabled = true;
        }

        PPhonologyScribeShow(editing);
    }

    private bool PPhonologyLeaveConfirm()
    {
        return _pPhonologyHost.PWindowDiscardConfirm(PPhonologyChangeCheck(), PPhonologyDraftFinish);
    }

    private void PPhonologyEntryShow(long id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PPhonologyMode.IsEnabled = true;
    }

    private void PPhonologyClear()
    {
        _pDisplayEntry = null;
        PInventorySelect(null);
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PPhonologyScribeShow(false);
        PPhonologyMode.IsEnabled = false;
        PPhonologyBin.IsEnabled = false;
    }

    private void PPhonologyStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PPhonologyBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayEntry is not long id)
        {
            return;
        }

        if (!_pPhonologyHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lEngine.LEngineEntryDelete(id);
        }
        catch (Exception exception)
        {
            _pPhonologyHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PPhonologyClear();
    }
}
