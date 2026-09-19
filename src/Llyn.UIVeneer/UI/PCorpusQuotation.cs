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

    private LVista? _pQuotationVista;

    private void PQuotationFind()
    {
        IReadOnlyList<LVistaRow> read;
        try
        {
            read = _lEngine.LEngineEntryFind(_pCorpusVista, _pQuotationVista);
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
            _pQuotationVista?.LVistaSelect(id);
            draft = _pQuotationVista?.LVistaLoad()?.LDraftContent;
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

        _pQuotationVista?.LVistaSelect(id);
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
        long? example = _pCorpusVista?.LVistaChosen;

        PTranscriptDraftCancel();
        PTranscript.Visibility = Visibility.Collapsed;
        PExcerpt.Visibility = Visibility.Collapsed;

        _pQuotationVista?.LVistaSelect(null);
        PQuotationFind();
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PCorpusMode.IsEnabled = true;
        PCorpusBin.IsEnabled = false;
        PQuotationScribeShow(true);

        if (example is long id)
        {
            PEditor.PEditorExampleAdd(id);
        }
    }

    private void PQuotationScribeHandle(bool editing)
    {
        if (_pQuotationVista?.LVistaChosen is not long id)
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

        if (_pQuotationVista?.LVistaChosen is long stored)
        {
            PQuotationEntryShow(stored);
            return;
        }

        PEditor.PEditorReset();

        if (_pCorpusVista?.LVistaChosen is long kept)
        {
            PAnthologyExampleShow(kept);
            return;
        }

        PCorpusClear();
    }

    private void PQuotationScribeShow(bool editing)
    {
        _pQuotationVista?.LVistaEditingSet(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PCorpusViewer.IsChecked = !editing;
        PCorpusScribe.IsChecked = editing;
        PChronicleUpdate();
    }

    private void PQuotationEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinStored
            && _pQuotationVista?.LVistaChosen is null
            && IsVisible
            && PEditor.Visibility == Visibility.Visible)
        {
            _pQuotationVista?.LVistaSelect(bulletin.LBulletinId);
        }

        PCitationFind();
        PAnthologyFind();
    }

    private void PCorpusEntryUpdate()
    {
        LEntryDraft? draft;
        try
        {
            draft = _pQuotationVista?.LVistaLoad()?.LDraftContent;
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            if (_pCorpusVista?.LVistaChosen is long kept)
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

        _pQuotationVista?.LVistaSelect(null);
        PQuotationFind();
        PDisplay.PDisplayClear();
        PDisplay.Visibility = Visibility.Collapsed;
        PEditor.Visibility = Visibility.Collapsed;
        PCorpusScribeShow(editing);
        PCorpusMode.IsEnabled = _pCorpusVista?.LVistaChosen is not null;
        PCorpusBin.IsEnabled = _pCorpusVista?.LVistaChosen is not null;
    }
}
