using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private readonly ObservableCollection<POccurrenceItem> _pOccurrenceList = [];

    private long? _pDisplayEntry;

    private void POccurrenceFind()
    {
        IReadOnlyList<LEntry> read;
        try
        {
            read = _lEngine.LEngineEntryFind(
                new LSituation(
                    _pVignetteSituation ?? 0,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified),
                PSortie.Text ?? string.Empty,
                _pMeshChoice);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        _pOccurrenceList.Clear();
        foreach (LEntry entry in read)
        {
            _pOccurrenceList.Add(new POccurrenceItem(
                entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
        }

        PTwin.PTwinNameApply(
            _pOccurrenceList,
            row => row.POccurrenceItemHeadword,
            (row, name) => row.POccurrenceItemName = name,
            row => row.POccurrenceItemId);

        POccurrenceEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PSortie.Text) ? "Situation.Vacant" : "Situation.Unmatched");
        POccurrenceEmpty.Visibility = _pOccurrenceList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        POccurrenceSelect(_pDisplayEntry);
    }

    private void POccurrenceSelect(long? id)
    {
        foreach (POccurrenceItem item in _pOccurrenceList)
        {
            item.POccurrenceItemChosen = id is not null
                && item.POccurrenceItemId == id;
        }
    }

    private void POccurrenceHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not POccurrenceItem item)
        {
            return;
        }

        if (!PRepertoireLeaveConfirm())
        {
            return;
        }

        POccurrenceEntryShow(item.POccurrenceItemId);
    }

    private void POccurrenceEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            POccurrenceEntryHide();
            POccurrenceFind();
            return;
        }

        bool editing = PScenario.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        PScenarioDraftCancel();
        PScenario.Visibility = Visibility.Collapsed;
        PVignette.Visibility = Visibility.Collapsed;

        _pDisplayEntry = id;
        POccurrenceSelect(id);
        PDisplay.PDisplayShow(id, draft);
        PRepertoireMode.IsEnabled = true;
        PRepertoireBin.IsEnabled = false;

        if (editing)
        {
            PEditor.PEditorEntryShow(id);
        }

        POccurrenceScribeShow(editing);
    }

    private void POccurrenceEntryCreate()
    {
        long? situation = _pVignetteSituation;

        PScenarioDraftCancel();
        PScenario.Visibility = Visibility.Collapsed;
        PVignette.Visibility = Visibility.Collapsed;

        _pDisplayEntry = null;
        POccurrenceSelect(null);
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PRepertoireMode.IsEnabled = true;
        PRepertoireBin.IsEnabled = false;
        POccurrenceScribeShow(true);

        if (situation is long id)
        {
            PEditor.PEditorSituationAdd(id);
        }
    }

    private void POccurrenceScribeHandle(bool editing)
    {
        if (_pDisplayEntry is not long id)
        {
            if (!editing)
            {
                POccurrenceScribeReset();
            }

            return;
        }

        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PRepertoireLeaveConfirm())
            {
                POccurrenceScribeShow(true);
                return;
            }

            POccurrenceScribeShow(false);
            POccurrenceEntryShow(id);
            return;
        }

        PEditor.PEditorEntryShow(id);
        POccurrenceScribeShow(true);
    }

    private void POccurrenceScribeReset()
    {
        if (!PRepertoireLeaveConfirm())
        {
            POccurrenceScribeShow(true);
            return;
        }

        POccurrenceScribeShow(false);

        if (_pDisplayEntry is long stored)
        {
            POccurrenceEntryShow(stored);
            return;
        }

        PEditor.PEditorReset();

        if (_pVignetteSituation is long kept)
        {
            PRepertoireShow(kept);
            return;
        }

        PRepertoireClear();
    }

    private void POccurrenceScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PRepertoireViewer.IsChecked = !editing;
        PRepertoireScribe.IsChecked = editing;
    }

    private void PRepertoireStoreHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntrySave();
            return;
        }

        PScenarioStoreRun();
    }

    private void POccurrenceEntryUpdate(long id)
    {
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
            if (_pVignetteSituation is long kept)
            {
                PRepertoireShow(kept);
                return;
            }

            PRepertoireClear();
            return;
        }

        PDisplay.PDisplayShow(shown, draft);
    }

    private void POccurrenceEntryHide()
    {
        bool editing = PScenario.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorReset();
        }

        _pDisplayEntry = null;
        POccurrenceSelect(null);
        PDisplay.PDisplayClear();
        PDisplay.Visibility = Visibility.Collapsed;
        PEditor.Visibility = Visibility.Collapsed;
        PRepertoireScribeShow(editing);
        PRepertoireMode.IsEnabled = _pVignetteSituation is not null;
        PRepertoireBin.IsEnabled = _pVignetteSituation is not null;
    }

    private void PRepertoireFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PRepertoireLeaveConfirm())
        {
            return;
        }

        if (_pVignetteSituation is not null || _pDisplayEntry is not null)
        {
            POccurrenceEntryCreate();
            return;
        }

        PRepertoireClear();
        PRepertoireScribeShow(true);
        PScenarioDraftShow(PScenarioDraftStart(null));
        PRepertoireMode.IsEnabled = true;
    }
}
