using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PPhonology
{
    private readonly ObservableCollection<PInventoryItem> _pInventoryList = [];

    private long? _pDisplayEntry;

    private LVista? _pPhonologyVista;

    private async void PPhonologyBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectVista)
        {
            if (_pPhonologyVista is not null && bulletin.LBulletinId == _pPhonologyVista.LVistaId)
            {
                PInventoryFind();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PPhonologyReset();
            return;
        }

        if (!PBulletin.PBulletinEntryCheck(bulletin.LBulletinSubject))
        {
            return;
        }

        PInventoryEntryUpdate(bulletin.LBulletinId);
    }

    private void PProbeHandle(object sender, TextChangedEventArgs e)
    {
        _pPhonologyVista?.LVistaQuerySet(PProbe.Text ?? string.Empty);
    }

    private void PSequenceHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pPhonologyVista is null)
        {
            return;
        }

        PSequenceDropper.IsChecked = false;
        _pPhonologyVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pPhonologyVista.LVistaOrder));
    }

    private void PLensHandle(object sender, RoutedEventArgs e)
    {
        if (_pPhonologyVista is null)
        {
            return;
        }

        _pPhonologyVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PLensList));
        PLensRestore();
    }

    internal async void PPhonologyVistaRestore(LVista vista)
    {
        _pPhonologyVista = vista;
        PSequenceRestore();
        PLensRestore();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PLensList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PLensHandle);
        vista.LVistaQuerySet(PProbe.Text ?? string.Empty);
        PInventoryFind();
    }

    private void PSequenceRestore()
    {
        if (_pPhonologyVista is not null)
        {
            PChoice.PChoiceOrderApply(PSequenceDropdown, _pPhonologyVista.LVistaOrder);
        }
    }

    private void PLensRestore()
    {
        bool active = _pPhonologyVista?.LVistaFilter.LCatalogFilterActive == true;
        PLensMark.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PInventorySelect(long? id)
    {
        foreach (PInventoryItem item in _pInventoryList)
        {
            item.PInventoryItemChosen = id is not null
                && item.PInventoryItemId == id;
        }
    }

    private void PInventoryFind()
    {
        _pInventoryList.Clear();
        if (_pPhonologyVista is null)
        {
            return;
        }

        foreach (LCatalogPronunciation row in _lEngine.LEnginePronunciationFind(_pPhonologyVista))
        {
            _pInventoryList.Add(new PInventoryItem(
                row.LCatalogPronunciationEntry.LEntryId,
                row.LCatalogPronunciationEntry.LEntryHeadword,
                row.LCatalogPronunciationEntry.LEntryLanguage,
                row.LCatalogPronunciationSound,
                row.LCatalogPronunciationEpithet ?? string.Empty)
            {
                PInventoryItemName = row.LCatalogPronunciationName,
            });
        }

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
            PInventoryFind();
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

        PInventoryFind();

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
