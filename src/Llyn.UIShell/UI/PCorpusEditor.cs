using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

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
            languages = _lEngine.LEngineLanguageRead();
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

        _pTranscriptLanguage = item.PLanguageItemName;
        PSpeaker.IsChecked = false;
        PSpeakerShow();
        PTranscriptChangeDefer();
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
        PTranscriptChangeDefer();
    }

    private string PTranscriptHintRead(bool unknown)
    {
        return _pCorpusHost.PLocalizationTextRead(unknown ? "Display.Unknown" : "Example.Text");
    }

    private void PTranscriptApply(LExample? example)
    {
        _pTranscriptLoading = true;

        PTranscriptText.Text = example?.LExampleText.LStateValueShow() ?? string.Empty;
        _pTranscriptTextUnknown = PStateConverter.PStateConverterCheck(example?.LExampleText);
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

        long citation = example.LExampleSource.LStateAnchorShow();
        if (_pTranscriptCitation != citation)
        {
            _pTranscriptCitation = citation;
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
        held = PStateConverter.PStateConverterCheck(value);
        field.Tag = PTranscriptHintRead(held);
    }

    private LRequestExampleBody PTranscriptRead(long draft)
    {
        return new LRequestExampleBody(
            draft,
            _pTranscriptLanguage,
            new LStateWritten(PTranscriptText.Text, _pTranscriptTextUnknown),
            _pTranscriptCitation);
    }

    private void PCorpusFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        if (_pCorpusVista?.LVistaChosen is not null || _pQuotationVista?.LVistaChosen is not null)
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
        if (_pTranscriptTenure is not LTenure held)
        {
            return;
        }

        held.LTenurePersist();
        if (!held.LTenureStateRead().LTenureStateChanged)
        {
            return;
        }

        long? stored;
        try
        {
            stored = _pCorpusHost.PWindowCommitRun(held, true);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.SaveFailed", exception);
            return;
        }

        _pTranscriptTenure = null;
        if (stored is not long example)
        {
            return;
        }

        _pCorpusVista?.LVistaSelect(example);

        PAnthologyFind();
        PCorpusScribeShow(false);
        PAnthologyExampleShow(example);
    }
}
