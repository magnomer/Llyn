using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PCorpus : UserControl, PChronicleHost
{
    private PWindow _pCorpusHost = null!;

    private LCorpus _lCorpus = null!;

    private LEditor _lEditor = null!;

    public PCorpus()
    {
        InitializeComponent();
        Resources.MergedDictionaries.Add(new PCorpusTranscript(this));
    }

    internal void PCorpusAttach(PWindow host, LEngine engine)
    {
        _pCorpusHost = host;
        _lCorpus = new LCorpus(engine, engine, engine, engine, host.PWindowUnreadableConfirm);
        PTranscriptDeskAttach();

        PAnthology.ItemsSource = _pAnthologyList;
        PQuotation.ItemsSource = _pQuotationList;
        PCitationList.ItemsSource = _pCitationItem;
        PCitationDrawer.CustomPopupPlacementCallback = PCitationPlace;
        PLanguageList.ItemsSource = _pLanguageItem;
        PTranscriptMentionLine.ItemsSource = _pTranscriptChip.PMentionLineChip;
        PTranscriptGlossLine.ItemsSource = _pTranscriptGloss;
        PExcerptGloss.ItemsSource = _pExcerptGloss;

        _lEditor = new LEditor(engine, engine, engine, engine, host.PWindowUnreadableConfirm);
        PDisplay.PDisplayAttach(host, _lEditor.LEditorDisplay);
        _lEditor.LEditorStateChanged += PCorpusStoreUpdate;
        _lEditor.LEditorStateChanged += PChronicleUpdate;
        PEditor.PEditorAttach(host, _lEditor);
    }

    private void PCorpusStoreUpdate()
    {
        PCorpusStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal void PCorpusReset()
    {
        PCorpusClear();
        PAnthologyFind();
    }

    internal bool PCorpusChangeCheck()
    {
        return PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorChangeCheck()
            : PTranscriptChangeCheck();
    }

    internal bool PCorpusDraftFinish(bool store)
    {
        return PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorDraftFinish(store)
            : PTranscriptDraftFinish(store);
    }

    internal void PCorpusClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
        PCitationDrawer.IsOpen = false;
        PLanguage.IsOpen = false;
        PRankDropdown.IsOpen = false;
        PGauzeDropdown.IsOpen = false;
    }

    private void PCorpusPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = (_lCorpus.LCorpusQuotationChosen is not null && PDisplay.Visibility == Visibility.Visible)
            || (_lCorpus.LCorpusChosen is not null && PExcerpt.Visibility == Visibility.Visible);
    }

    private async void PCorpusPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lCorpus.LCorpusQuotationChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pCorpusHost.PWindowPressRun(
                    ticket => _lCorpus.LCorpusPortraitPrint(_pCorpusHost.PWindowLabelRead(), ticket));
                return;
            }
        }

        if (_lCorpus.LCorpusChosen is not null)
        {
            if (PExcerpt.Visibility == Visibility.Visible)
            {
                await _pCorpusHost.PWindowPressRun(
                    ticket => _lCorpus.LCorpusPortraitPrint(_pCorpusHost.PWindowLegendRead("Example"), ticket));
            }
        }
    }

    private void PCorpusPortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lCorpus.LCorpusQuotationChosen is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PCorpusPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lCorpus.LCorpusQuotationChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pCorpusHost.PWindowPortraitExport(_lCorpus.LCorpusFileRead(), _lCorpus.LCorpusPortraitExport);
            }
        }
    }
}
