using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private readonly ObservableCollection<PAnthologyItem> _pAnthologyList = [];

    private readonly ObservableCollection<PCitationItem> _pCitationCatalog = [];

    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private IReadOnlyDictionary<long, int> _pAnthologyCount = new Dictionary<long, int>();

    private LVista? _pCorpusVista;

    private async void PCorpusBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectVista)
        {
            if (_pCorpusVista is not null && bulletin.LBulletinId == _pCorpusVista.LVistaId)
            {
                PAnthologyFind();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
        {
            if (bulletin.LBulletinId == PTranscriptDraft)
            {
                PTranscriptDraftRestore();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectTenure)
        {
            if (bulletin.LBulletinId == PTranscriptDraft)
            {
                PTranscriptChangeUpdate();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PSpeakerLoad();
            PCorpusReset();
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectEntry
            && bulletin.LBulletinId > 0
            && _pDisplayEntry is null
            && IsVisible
            && PEditor.Visibility == Visibility.Visible)
        {
            _pDisplayEntry = bulletin.LBulletinId;
        }

        if (!PBulletin.PBulletinEntryCheck(bulletin.LBulletinSubject))
        {
            return;
        }

        PCitationFind();
        PAnthologyFind();
        PQuotationEntryUpdate(bulletin.LBulletinId);
    }

    private void PQueryHandle(object sender, TextChangedEventArgs e)
    {
        _pCorpusVista?.LVistaQuerySet(PQuery.Text ?? string.Empty);
    }

    private void PRankHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pCorpusVista is null)
        {
            return;
        }

        PRankDropper.IsChecked = false;
        _pCorpusVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pCorpusVista.LVistaOrder));
    }

    internal async void PCorpusVistaRestore(LVista vista)
    {
        _pCorpusVista = vista;
        PRankRestore();
        PGauzeRestore();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PGauzeList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PGauzeHandle);
        vista.LVistaQuerySet(PQuery.Text ?? string.Empty);
        PSpeakerLoad();
        PCitationFind();
        PAnthologyFind();
    }

    private void PRankRestore()
    {
        if (_pCorpusVista is not null)
        {
            PChoice.PChoiceOrderApply(PRankDropdown, _pCorpusVista.LVistaOrder);
        }
    }

    private void PGauzeRestore()
    {
        bool active = _pCorpusVista?.LVistaFilter.LCatalogFilterActive == true;
        PGauzeMark.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PDredgeHandle(object sender, TextChangedEventArgs e)
    {
        PQuotationFind();
    }

    private void PGauzeHandle(object sender, RoutedEventArgs e)
    {
        if (_pCorpusVista is null)
        {
            return;
        }

        _pCorpusVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PGauzeList));
        PGauzeRestore();
    }

    private void PAnthologySelect(long? id)
    {
        foreach (PAnthologyItem item in _pAnthologyList)
        {
            item.PAnthologyItemChosen = id is not null
                && item.PAnthologyItemId == id;
        }
    }

    private void PAnthologyFind()
    {
        if (_pCorpusVista is null)
        {
            return;
        }

        IReadOnlyList<LCatalogExample> read;
        try
        {
            read = _lEngine.LEngineExampleFind(_pCorpusVista);
            _pAnthologyCount = _lEngine.LEngineUsageRead(LOwner.LOwnerExample);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        string unknown = _pCorpusHost.PLocalizationTextRead("Display.Unknown");
        string unwritten = _pCorpusHost.PLocalizationTextRead("Example.Unwritten");

        _pAnthologyList.Clear();
        bool kept = false;
        foreach (LCatalogExample row in read)
        {
            kept |= row.LCatalogExampleStored.LExampleId == _pExcerptExample;
            _pAnthologyList.Add(new PAnthologyItem(
                row.LCatalogExampleStored,
                row.LCatalogExampleUsage,
                unknown,
                unwritten));
        }

        LTwin.LTwinNameApply(
            _pAnthologyList, row => row.PAnthologyItemText, (row, name) => row.PAnthologyItemName = name);

        PAnthologyEmpty.Visibility = _pAnthologyList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PAnthologySelect(_pExcerptExample);
        PExcerptTally.Text = PCorpusTallyRead(_pExcerptExample);
        PTranscriptTally.Text = PCorpusTallyRead(PTranscriptExampleRead());

        if (!kept && _pExcerptExample is not null && PTranscript.Visibility != Visibility.Visible)
        {
            PCorpusClear();
        }

        PQuotationFind();
    }

    private void PAnthologyHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PAnthologyItem item)
        {
            return;
        }

        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        PAnthologyExampleShow(item.PAnthologyItemId);
    }

    internal void PAnthologyExampleShow(long id)
    {
        LExample? example;
        try
        {
            example = _lEngine.LEngineExampleRead(id);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        if (example is null)
        {
            PCorpusClear();
            PAnthologyFind();
            return;
        }

        _pExcerptExample = id;
        PAnthologySelect(id);
        PQuotationEntryHide();

        PExcerptSentenceShow(example);
        PExcerptLanguage.Text = example.LExampleLanguage;
        PExcerptFlag.Source = PEnsign.PEnsignFind(example.LExampleLanguage);
        PExcerptGlossShow(example.LExampleGloss);
        PExcerptCitationShow(example.LExampleSource);
        PExcerptTally.Text = PCorpusTallyRead(id);

        PQuotationFind();

        PExcerptBody.Visibility = Visibility.Visible;
        PExcerptUnselected.Visibility = Visibility.Collapsed;
        PCorpusMode.IsEnabled = true;
        PCorpusBin.IsEnabled = true;

        if (PTranscript.Visibility == Visibility.Visible)
        {
            PTranscriptDraftShow(PTranscriptDraftStart(id));
        }
    }

    private void PCorpusBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayEntry is not null || _pExcerptExample is not long id)
        {
            return;
        }

        int usage = _pAnthologyCount.TryGetValue(id, out int count) ? count : 0;

        if (!_pCorpusHost.PWindowRemovalConfirm(usage, "Example"))
        {
            return;
        }

        try
        {
            _lEngine.LEngineExampleDelete(id, usage > 0);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.DeleteFailed", exception);
            return;
        }

        PCorpusScribeShow(false);
        PCorpusClear();
        PAnthologyFind();
    }

    private string PCorpusTallyRead(long? id)
    {
        int count = id is long stored && _pAnthologyCount.TryGetValue(stored, out int usage) ? usage : 0;

        return count switch
        {
            0 => _pCorpusHost.PLocalizationTextRead("Example.UsageNone"),
            1 => _pCorpusHost.PLocalizationTextRead("Example.UsageOne"),
            _ => $"{count.ToString(CultureInfo.CurrentCulture)} "
                + _pCorpusHost.PLocalizationTextRead("Example.UsageMany"),
        };
    }

    private void PCorpusStoreHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntrySave();
            return;
        }

        PTranscriptStoreRun();
    }

    private void PCorpusScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PCorpusScribe);
        if (_pDisplayEntry is not null || PEditor.Visibility == Visibility.Visible)
        {
            PQuotationScribeHandle(editing);
            return;
        }

        if (editing == (PTranscript.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PCorpusLeaveConfirm())
            {
                PCorpusScribeShow(true);
                return;
            }

            PTranscriptDraftCancel();
            PCorpusScribeShow(false);

            if (_pExcerptExample is not null)
            {
                PAnthologyExampleShow(_pExcerptExample.Value);
                return;
            }

            PCorpusClear();
            return;
        }

        if (_pExcerptExample is null)
        {
            PCorpusClear();
            return;
        }

        PCorpusScribeShow(true);
        PTranscriptDraftShow(PTranscriptDraftStart(_pExcerptExample));
    }

    private void PCorpusScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PTranscript.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PExcerpt.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PCorpusViewer.IsChecked = !editing;
        PCorpusScribe.IsChecked = editing;
        PTranscriptChangeUpdate();
    }

    internal void PCorpusScribeRestore(bool editing)
    {
        if (editing && _pExcerptExample is null)
        {
            return;
        }

        if (editing)
        {
            PCorpusMode.IsEnabled = true;
        }

        PCorpusScribeShow(editing);
    }

    internal bool PCorpusLeaveConfirm()
    {
        return _pCorpusHost.PWindowDiscardConfirm(PCorpusChangeCheck(), PCorpusDraftFinish);
    }

    internal long PCorpusVoyageRead()
    {
        return _pExcerptExample ?? 0;
    }

    private void PCorpusClear()
    {
        PTranscriptDraftCancel();

        _pExcerptExample = null;
        PAnthologySelect(null);
        PQuotationEntryHide();
        PQuotationFind();

        PExcerptBody.Visibility = Visibility.Collapsed;
        PExcerptUnselected.Visibility = Visibility.Visible;
        PTranscriptApply(null);
        PCorpusScribeShow(false);
        PCorpusMode.IsEnabled = false;
        PCorpusBin.IsEnabled = false;
    }
}
