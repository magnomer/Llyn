using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PReference
{
    private readonly ObservableCollection<PFootnoteItem> _pFootnoteList = [];

    private LVista? _pFootnoteVista;

    private void PFootnoteFind()
    {
        IReadOnlyList<LEntry> read;
        try
        {
            read = _lEngine.LEngineEntryFind(
                new LReference(
                    _pReferenceVista?.LVistaChosen ?? 0,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LReferenceKind.LReferenceKindUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LStateMark.LStateMarkUnspecified),
                PRummage.Text ?? string.Empty,
                _pReferenceVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.LoadFailed", exception);
            return;
        }

        _pFootnoteList.Clear();
        foreach (LEntry entry in read)
        {
            _pFootnoteList.Add(new PFootnoteItem(
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                _lEngine.LEngineEpithetRead(entry.LEntryId)));
        }

        LTwin.LTwinNameApply(
            _pFootnoteList,
            row => row.PFootnoteItemHeadword,
            (row, name) => row.PFootnoteItemName = name,
            row => row.PFootnoteItemId);

        PFootnoteEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PRummage.Text) ? "Source.Vacant" : "Source.Unmatched");
        PFootnoteEmpty.Visibility = _pFootnoteList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PFootnoteChosenApply();
    }

    private void PFootnoteChosenApply()
    {
        long? chosen = _pFootnoteVista?.LVistaChosen;
        foreach (PFootnoteItem item in _pFootnoteList)
        {
            item.PFootnoteItemChosen = chosen is not null
                && item.PFootnoteItemId == chosen;
        }
    }

    private void PFootnoteHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement row || row.DataContext is not PFootnoteItem item)
        {
            return;
        }

        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PFootnoteEntryShow(item.PFootnoteItemId);
    }

    private void PFootnoteEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PFootnoteEntryHide();
            PFootnoteFind();
            return;
        }

        bool editing = PImprint.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        PImprint.PImprintDraftCancel();
        PImprint.Visibility = Visibility.Collapsed;
        PColophon.Visibility = Visibility.Collapsed;

        _pFootnoteVista?.LVistaSelect(id);
        PFootnoteChosenApply();
        PDisplay.PDisplayShow(draft);
        PReferenceMode.IsEnabled = true;
        PReferenceBin.IsEnabled = false;

        if (editing)
        {
            PEditor.PEditorEntryShow(id);
        }

        PFootnoteScribeShow(editing);
    }

    private void PFootnoteEntryCreate()
    {
        long? reference = _pReferenceVista?.LVistaChosen;

        PImprint.PImprintDraftCancel();
        PImprint.Visibility = Visibility.Collapsed;
        PColophon.Visibility = Visibility.Collapsed;

        _pFootnoteVista?.LVistaSelect(null);
        PFootnoteChosenApply();
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PReferenceMode.IsEnabled = true;
        PReferenceBin.IsEnabled = false;
        PFootnoteScribeShow(true);

        if (reference is long id)
        {
            PEditor.PEditorReferenceAdd(id);
        }
    }

    private void PFootnoteScribeHandle(bool editing)
    {
        if (_pFootnoteVista?.LVistaChosen is not long id)
        {
            if (!editing)
            {
                PFootnoteScribeReset();
            }

            return;
        }

        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PReferenceLeaveConfirm())
            {
                PFootnoteScribeShow(true);
                return;
            }

            PFootnoteScribeShow(false);
            PFootnoteEntryShow(id);
            return;
        }

        PEditor.PEditorEntryShow(id);
        PFootnoteScribeShow(true);
    }

    private void PFootnoteScribeReset()
    {
        if (!PReferenceLeaveConfirm())
        {
            PFootnoteScribeShow(true);
            return;
        }

        PFootnoteScribeShow(false);

        if (_pFootnoteVista?.LVistaChosen is long stored)
        {
            PFootnoteEntryShow(stored);
            return;
        }

        PEditor.PEditorReset();

        if (_pReferenceVista?.LVistaChosen is long kept)
        {
            PReferenceShow(kept);
            return;
        }

        PReferenceClear();
    }

    private void PFootnoteScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PReferenceViewer.IsChecked = !editing;
        PReferenceScribe.IsChecked = editing;
        PReferenceChronicleUpdate();
    }

    private void PReferenceChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorChronicleRead()
            : PImprint.PImprintChronicleRead();
        PReferenceBackward.IsEnabled = undo;
        PReferenceForward.IsEnabled = redo;
    }

    private void PReferenceUndoHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PChronicleUndo();
            return;
        }

        PImprint.PChronicleUndo();
    }

    private void PReferenceRedoHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PChronicleRedo();
            return;
        }

        PImprint.PChronicleRedo();
    }

    private void PReferenceStoreHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntrySave();
            return;
        }

        PImprint.PImprintStoreRun();
    }

    private void PFootnoteEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId > 0
            && _pFootnoteVista?.LVistaChosen is null
            && IsVisible
            && PEditor.Visibility == Visibility.Visible)
        {
            _pFootnoteVista?.LVistaSelect(bulletin.LBulletinId);
        }

        PShelfFind();
    }

    private void PReferenceEntryUpdate()
    {
        if (_pFootnoteVista?.LVistaChosen is not long shown)
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
            if (_pReferenceVista?.LVistaChosen is long kept)
            {
                PReferenceShow(kept);
                return;
            }

            PReferenceClear();
            return;
        }

        PDisplay.PDisplayShow(draft);
    }

    private void PFootnoteEntryHide()
    {
        bool editing = PImprint.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorReset();
        }

        _pFootnoteVista?.LVistaSelect(null);
        PFootnoteChosenApply();
        PDisplay.PDisplayClear();
        PDisplay.Visibility = Visibility.Collapsed;
        PEditor.Visibility = Visibility.Collapsed;
        PReferenceScribeShow(editing);
        PReferenceMode.IsEnabled = _pReferenceVista?.LVistaChosen is not null;
        PReferenceBin.IsEnabled = _pReferenceVista?.LVistaChosen is not null;
    }

    private void PReferenceFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        if (_pReferenceVista?.LVistaChosen is not null || _pFootnoteVista?.LVistaChosen is not null)
        {
            PFootnoteEntryCreate();
            return;
        }

        PReferenceClear();
        PReferenceScribeShow(true);
        PImprint.PImprintDraftOpen(null);
        PReferenceMode.IsEnabled = true;
    }
}
