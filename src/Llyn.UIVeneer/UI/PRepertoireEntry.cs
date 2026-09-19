using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PRepertoire
{
    private readonly ObservableCollection<POccurrenceItem> _pOccurrenceList = [];

    private LVista? _pOccurrenceVista;

    private void POccurrenceFind()
    {
        IReadOnlyList<LVistaRow> read;
        try
        {
            read = _lEngine.LEngineEntryFind(_pRepertoireVista, _pOccurrenceVista);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        List<POccurrenceItem> fresh = [];
        foreach (LVistaRow entry in read)
        {
            fresh.Add(new POccurrenceItem(
                entry.LVistaRowId,
                entry.LVistaRowHeadword,
                entry.LVistaRowLanguage,
                entry.LVistaRowEpithet ?? string.Empty,
                entry.LVistaRowChosen)
            {
                POccurrenceItemName = entry.LVistaRowName,
            });
        }

        PSplice.PSpliceApply(
            _pOccurrenceList, fresh, POccurrenceItem.POccurrenceItemMatch, POccurrenceItem.POccurrenceItemSync);

        POccurrenceEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PSortie.Text) ? "Situation.Vacant" : "Situation.Unmatched");
        POccurrenceEmpty.Visibility = _pOccurrenceList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

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
            _pOccurrenceVista?.LVistaSelect(id);
            draft = _pOccurrenceVista?.LVistaLoad()?.LDraftContent;
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

        _pOccurrenceVista?.LVistaSelect(id);
        POccurrenceFind();
        PDisplay.PDisplayShow(draft);
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
        long? situation = _pRepertoireVista?.LVistaChosen;

        PScenarioDraftCancel();
        PScenario.Visibility = Visibility.Collapsed;
        PVignette.Visibility = Visibility.Collapsed;

        _pOccurrenceVista?.LVistaSelect(null);
        POccurrenceFind();
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
        if (_pOccurrenceVista?.LVistaChosen is not long id)
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

        if (_pOccurrenceVista?.LVistaChosen is long stored)
        {
            POccurrenceEntryShow(stored);
            return;
        }

        PEditor.PEditorReset();

        if (_pRepertoireVista?.LVistaChosen is long kept)
        {
            PRepertoireShow(kept);
            return;
        }

        PRepertoireClear();
    }

    private void POccurrenceScribeShow(bool editing)
    {
        _pOccurrenceVista?.LVistaEditingSet(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PRepertoireViewer.IsChecked = !editing;
        PRepertoireScribe.IsChecked = editing;
        PChronicleUpdate();
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

    private void POccurrenceEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinStored
            && _pOccurrenceVista?.LVistaChosen is null
            && IsVisible
            && PEditor.Visibility == Visibility.Visible)
        {
            _pOccurrenceVista?.LVistaSelect(bulletin.LBulletinId);
        }

        PAtlasFind();
    }

    private void PRepertoireEntryUpdate()
    {
        LEntryDraft? draft;
        try
        {
            draft = _pOccurrenceVista?.LVistaLoad()?.LDraftContent;
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            if (_pRepertoireVista?.LVistaChosen is long kept)
            {
                PRepertoireShow(kept);
                return;
            }

            PRepertoireClear();
            return;
        }

        PDisplay.PDisplayShow(draft);
    }

    private void POccurrenceEntryHide()
    {
        bool editing = PScenario.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorReset();
        }

        _pOccurrenceVista?.LVistaSelect(null);
        POccurrenceFind();
        PDisplay.PDisplayClear();
        PDisplay.Visibility = Visibility.Collapsed;
        PEditor.Visibility = Visibility.Collapsed;
        PRepertoireScribeShow(editing);
        PRepertoireMode.IsEnabled = _pRepertoireVista?.LVistaChosen is not null;
        PRepertoireBin.IsEnabled = _pRepertoireVista?.LVistaChosen is not null;
    }

    private void PRepertoireFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PRepertoireLeaveConfirm())
        {
            return;
        }

        if (_pRepertoireVista?.LVistaChosen is not null || _pOccurrenceVista?.LVistaChosen is not null)
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
