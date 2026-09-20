using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PCorpus
{
    private readonly ObservableCollection<PQuotationItem> _pQuotationList = [];

    private void PQuotationFind()
    {
        IReadOnlyList<LVistaRow> read;
        try
        {
            read = _lCorpus.LCorpusQuotationRead();
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        List<PQuotationItem> fresh = [];
        foreach (LVistaRow entry in read)
        {
            fresh.Add(new PQuotationItem(
                entry.LVistaRowId,
                entry.LVistaRowHeadword,
                entry.LVistaRowLanguage,
                entry.LVistaRowEpithet ?? string.Empty,
                entry.LVistaRowChosen)
            {
                PQuotationItemName = entry.LVistaRowName,
            });
        }

        PSplice.PSpliceApply(
            _pQuotationList, fresh, PQuotationItem.PQuotationItemMatch, PQuotationItem.PQuotationItemSync);

        PQuotationEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PDredge.Text) ? "Example.Vacant" : "Example.Unmatched");
        PQuotationEmpty.Visibility = _pQuotationList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    }

    private void PQuotationHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PQuotationItem item)
        {
            return;
        }

        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        PQuotationEntryShow(item.PQuotationItemId);
    }

    private void PQuotationEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            _lCorpus.LCorpusQuotationSelect(id);
            draft = _lCorpus.LCorpusQuotationLoad();
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PQuotationEntryHide();
            PQuotationFind();
            return;
        }

        bool editing = PTranscript.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        PTranscriptDraftCancel();
        PTranscript.Visibility = Visibility.Collapsed;
        PExcerpt.Visibility = Visibility.Collapsed;

        _lCorpus.LCorpusQuotationSelect(id);
        PQuotationFind();
        PDisplay.PDisplayShow(draft);
        PCorpusMode.IsEnabled = true;
        PCorpusBin.IsEnabled = false;

        if (editing)
        {
            PEditor.PEditorEntryShow(id);
        }

        PQuotationScribeShow(editing);
    }

    private void PQuotationEntryCreate()
    {
        long? example = _lCorpus.LCorpusChosen;

        PTranscriptDraftCancel();
        PTranscript.Visibility = Visibility.Collapsed;
        PExcerpt.Visibility = Visibility.Collapsed;

        _lCorpus.LCorpusQuotationSelect(null);
        PQuotationFind();
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PCorpusMode.IsEnabled = true;
        PCorpusBin.IsEnabled = false;
        PQuotationScribeShow(true);

        if (example is long id)
        {
            _lEditor.LEditorExampleAdd(id);
        }
    }

    private void PQuotationScribeHandle(bool editing)
    {
        if (_lCorpus.LCorpusQuotationChosen is not long id)
        {
            if (!editing)
            {
                PQuotationScribeReset();
            }

            return;
        }

        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PCorpusLeaveConfirm())
            {
                PQuotationScribeShow(true);
                return;
            }

            PQuotationScribeShow(false);
            PQuotationEntryShow(id);
            return;
        }

        PEditor.PEditorEntryShow(id);
        PQuotationScribeShow(true);
    }

    private void PQuotationScribeReset()
    {
        if (!PCorpusLeaveConfirm())
        {
            PQuotationScribeShow(true);
            return;
        }

        PQuotationScribeShow(false);

        if (_lCorpus.LCorpusQuotationChosen is long stored)
        {
            PQuotationEntryShow(stored);
            return;
        }

        PEditor.PEditorReset();

        if (_lCorpus.LCorpusChosen is long kept)
        {
            PAnthologyExampleShow(kept);
            return;
        }

        PCorpusClear();
    }

    private void PQuotationScribeShow(bool editing)
    {
        _lCorpus.LCorpusEditorSet(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PCorpusViewer.IsChecked = !editing;
        PCorpusScribe.IsChecked = editing;
        PChronicleUpdate();
    }

    private void PQuotationEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinStored
            && _lCorpus.LCorpusQuotationChosen is null
            && IsVisible
            && PEditor.Visibility == Visibility.Visible)
        {
            _lCorpus.LCorpusQuotationSelect(bulletin.LBulletinId);
        }

        PCitationFind();
        PAnthologyFind();
    }

    private void PCorpusEntryUpdate()
    {
        LEntryDraft? draft;
        try
        {
            draft = _lCorpus.LCorpusQuotationLoad();
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            if (_lCorpus.LCorpusChosen is long kept)
            {
                PAnthologyExampleShow(kept);
                return;
            }

            PCorpusClear();
            return;
        }

        PDisplay.PDisplayShow(draft);
    }

    private void PQuotationEntryHide()
    {
        bool editing = PTranscript.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorReset();
        }

        _lCorpus.LCorpusQuotationSelect(null);
        PQuotationFind();
        PDisplay.PDisplayClear();
        PDisplay.Visibility = Visibility.Collapsed;
        PEditor.Visibility = Visibility.Collapsed;
        PCorpusScribeShow(editing);
        PCorpusMode.IsEnabled = _lCorpus.LCorpusChosen is not null;
        PCorpusBin.IsEnabled = _lCorpus.LCorpusChosen is not null;
    }
}
