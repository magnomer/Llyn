using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PCorpus
{
    private long _pTranscriptCitation;

    private string _pTranscriptLanguage = string.Empty;

    private bool _pTranscriptTextUnknown;

    private bool _pTranscriptLoading;

    private void PSpeakerLoad()
    {
        IReadOnlyList<string> languages;
        try
        {
            languages = _lCorpus.LCorpusLanguageRead();
        }
        catch (Exception)
        {
            languages = [];
        }

        _pLanguageItem.Clear();
        foreach (string language in languages)
        {
            _pLanguageItem.Add(new PLanguageItem(
                language, PEnsign.PEnsignFind(language)));
        }

        PSpeakerShow();
    }

    internal void PSpeakerHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PLanguageItem item })
        {
            return;
        }

        PSpeaker.IsChecked = false;
        PTranscriptRequestSend(new LRequestExampleLanguage(PTranscriptDraft, item.PLanguageItemName));
    }

    private void PSpeakerShow()
    {
        PSpeakerName.Text = _pTranscriptLanguage;
        PSpeakerFlag.Source = PEnsign.PEnsignFind(_pTranscriptLanguage);
    }

    private void PTranscriptTextHandle(object sender, TextChangedEventArgs e)
    {
        if (_pTranscriptLoading)
        {
            return;
        }

        _pTranscriptTextUnknown = false;
        PTranscriptText.Tag = PTranscriptHintRead(false);
        PTranscriptRequestDefer(
            new LRequestExampleText(PTranscriptDraft, new LStateWritten(PTranscriptText.Text, false)));
    }

    private string PTranscriptHintRead(bool unknown)
    {
        return PLocalizationCatalog.PLocalizationTextRead(unknown ? "Display.Unknown" : "Example.Text");
    }

    private void PTranscriptApply(LExample? example)
    {
        _pTranscriptLoading = true;

        PTranscriptText.Text = example?.LExampleText.LStateValueShow() ?? string.Empty;
        _pTranscriptTextUnknown = example?.LExampleText.LStateValueUncertain ?? false;
        PTranscriptText.Tag = PTranscriptHintRead(_pTranscriptTextUnknown);

        PTranscriptGlossShow(example);

        _pTranscriptLanguage = example?.LExampleLanguage ?? string.Empty;

        PSpeakerShow();

        _pTranscriptCitation = example?.LExampleSource.LStateAnchorShow() ?? 0;
        PCitationUpdate();

        PTranscriptMentionShow(example);

        PTranscriptTally.Text = PCorpusTallyRead(PTranscriptExampleRead());

        _pTranscriptLoading = false;

        PTranscriptChangeUpdate();
    }

    private void PTranscriptShow(LExample example)
    {
        _pTranscriptLoading = true;

        PTranscriptFieldShow(PTranscriptText, example.LExampleText, ref _pTranscriptTextUnknown);
        PTranscriptGlossShow(example);

        if (!string.Equals(_pTranscriptLanguage, example.LExampleLanguage, StringComparison.Ordinal))
        {
            _pTranscriptLanguage = example.LExampleLanguage;
            PSpeakerShow();
        }

        long shown = _pTranscriptCitation;
        _pTranscriptCitation = example.LExampleSource.LStateAnchorShow();
        if (shown != _pTranscriptCitation)
        {
            PCitationUpdate();
        }

        PTranscriptMentionShow(example);

        _pTranscriptLoading = false;

        PTranscriptChangeUpdate();
    }

    private void PTranscriptFieldShow(TextBox field, LStateValue value, ref bool held)
    {
        if (new LStateWritten(field.Text, held).LStateWrittenMatch(value))
        {
            return;
        }

        field.Text = value.LStateValueShow();
        bool uncertain = value.LStateValueUncertain;
        held = uncertain;
        field.Tag = PTranscriptHintRead(held);
    }

    private void PCorpusFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        if (_lCorpus.LCorpusChosen is not null || _lCorpus.LCorpusQuotationChosen is not null)
        {
            PQuotationEntryCreate();
            return;
        }

        PCorpusClear();
        PCorpusScribeShow(true);
        PTranscriptDraftShow(PTranscriptDraftStart(null));
        PCorpusMode.IsEnabled = true;
    }

    private void PTranscriptStoreRun()
    {
        if (!PTranscriptDesk.LDeskChangeCheck())
        {
            return;
        }

        PTranscriptDesk.LDeskFinish(true);
    }

    private void PTranscriptStoredShow(long example)
    {
        _lCorpus.LCorpusSelect(example);

        PAnthologyFind();
        PCorpusScribeShow(false);
        PAnthologyExampleShow(example);
    }
}
