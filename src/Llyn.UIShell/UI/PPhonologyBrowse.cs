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

    private LVista? _pPhonologyVista;

    private async void PPhonologyWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_lEngine);
        PPhonologyReset();
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
        vista.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PInventoryFind));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PPhonologyWorkspaceUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, PInventoryEntryUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PInventoryFind));
        vista.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PInventoryFind));
        vista.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PPhonologyEntryUpdate));
        PDisplay.PDisplayVistaRestore(vista);
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

    private void PInventoryChosenApply()
    {
        long? chosen = _pPhonologyVista?.LVistaChosen;
        foreach (PInventoryItem item in _pInventoryList)
        {
            item.PInventoryItemChosen = chosen is not null
                && item.PInventoryItemId == chosen;
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
                PInventoryItemChosen = row.LCatalogPronunciationChosen,
            });
        }

        PInventoryEmpty.Visibility = _pInventoryList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
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

        _pPhonologyVista?.LVistaSelect(id);
        PInventoryChosenApply();
        PPhonologyEntryShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PInventoryEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pPhonologyVista?.LVistaSelect(bulletin.LBulletinId);
        }

        PPhonologyCommandApply();
        PInventoryFind();
    }

    private void PPhonologyEntryUpdate(LBulletin bulletin)
    {
        if (_pPhonologyVista?.LVistaChosen is not long shown)
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

        PPhonologyEntryShow(draft);
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

            if (_pPhonologyVista?.LVistaChosen is long chosen)
            {
                PInventoryEntryShow(chosen);
                return;
            }

            PPhonologyClear();
            return;
        }

        if (_pPhonologyVista?.LVistaChosen is not long shown)
        {
            PPhonologyClear();
            return;
        }

        PEditor.PEditorEntryShow(shown);
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
        if (editing && _pPhonologyVista?.LVistaChosen is null)
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

    private void PPhonologyEntryShow(LEntryDraft draft)
    {
        PDisplay.PDisplayShow(draft);
        PPhonologyMode.IsEnabled = true;
        PPhonologyCommandApply();
    }

    private void PPhonologyCommandApply()
    {
        PPhonologyBin.IsEnabled = _pPhonologyVista?.LVistaChosen is not null;
    }

    private void PPhonologyClear()
    {
        _pPhonologyVista?.LVistaSelect(null);
        PInventoryChosenApply();
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PPhonologyScribeShow(false);
        PPhonologyMode.IsEnabled = false;
        PPhonologyCommandApply();
    }

    private void PPhonologyStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PPhonologyBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pPhonologyVista?.LVistaChosen is not long id)
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
