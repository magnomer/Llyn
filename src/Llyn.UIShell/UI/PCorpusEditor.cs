using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

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

    private void PSpeakerHandle(object sender, RoutedEventArgs e)
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

    private void PCitationFind()
    {
        _pCitationCatalog.Clear();

        IReadOnlyList<LReference> read;
        try
        {
            read = _lEngine.LEngineReferenceRead();
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Reference.LoadFailed", exception);
            read = [];
        }

        foreach (LReference reference in read)
        {
            _pCitationCatalog.Add(PCitationItem.PCitationItemCreate(reference));
        }

        PCitationEmpty.Visibility = _pCitationCatalog.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private string PCitationNameRead(long id)
    {
        if (id == 0)
        {
            return string.Empty;
        }

        foreach (PCitationItem row in _pCitationCatalog)
        {
            if (row.PCitationItemId == id)
            {
                return row.PCitationItemName;
            }
        }

        return id.ToString(CultureInfo.InvariantCulture);
    }

    private void PCitationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (_pTranscriptLoading || PCitationList.SelectedValue is not long id)
        {
            return;
        }

        _pTranscriptCitation = id;
        PCitation.IsChecked = false;
        PCitationUpdate();
        PTranscriptChangeDefer();
    }

    private void PCitationClearHandle(object sender, RoutedEventArgs e)
    {
        _pTranscriptCitation = 0;
        PCitationList.SelectedValue = null;
        PCitation.IsChecked = false;
        PCitationUpdate();
        PTranscriptChangeDefer();
    }

    private void PCitationUpdate()
    {
        PCitationName.Text = _pTranscriptCitation == 0
            ? _pCorpusHost.PLocalizationTextRead("Reference.Assign")
            : PCitationNameRead(_pTranscriptCitation);
        PCitationName.SetResourceReference(
            TextBlock.ForegroundProperty,
            _pTranscriptCitation == 0 ? "Theme.Muted" : "Theme.Ink");
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
        _pTranscriptTextUnknown = example?.LExampleText.LStateValueState == LState.LStateUnknown;
        PTranscriptText.Tag = PTranscriptHintRead(_pTranscriptTextUnknown);

        _pTranscriptGlossDirty.Clear();
        PTranscriptGlossShow(example);

        _pTranscriptLanguage = example?.LExampleLanguage ?? string.Empty;

        PSpeakerShow();

        _pTranscriptCitation = example?.LExampleSource.LStateAnchorShow() ?? 0;
        PCitationList.SelectedValue = _pTranscriptCitation == 0 ? null : _pTranscriptCitation;
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
            PCitationList.SelectedValue = citation == 0 ? null : citation;
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
        held = value.LStateValueState == LState.LStateUnknown;
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

        PCorpusClear();
        PCorpusScribeShow(true);
        PTranscriptDraftShow(PTranscriptDraftStart(null));
        PCorpusMode.IsEnabled = true;
    }

    private void PTranscriptStoreRun()
    {
        PTranscriptChangeSave();

        long held = _pTranscriptDraft;
        if (held == 0)
        {
            return;
        }

        LExample stored;
        try
        {
            stored = _pCorpusHost.PWindowCommitRun(held, _lEngine.LEngineExampleCommit);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.SaveFailed", exception);
            return;
        }

        _pTranscriptDraft = 0;
        _pExcerptExample = stored.LExampleId;

        PAnthologyFind(PQuery.Text ?? string.Empty);
        PCorpusScribeShow(false);
        PAnthologyExampleShow(stored.LExampleId);
    }
}
