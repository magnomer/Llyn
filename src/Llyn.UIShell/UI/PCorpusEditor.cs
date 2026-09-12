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

    private bool _pTranscriptTranslationUnknown;

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

    private string PCitationNameRead(LStateAnchor source)
    {
        long id = source.LStateAnchorShow();
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
            : PCitationNameRead(LStateAnchor.LStateAnchorCreate(_pTranscriptCitation));
    }

    private void PTranscriptTextHandle(object sender, TextChangedEventArgs e)
    {
        if (_pTranscriptLoading)
        {
            return;
        }

        _pTranscriptTextUnknown = false;
        PTranscriptText.Tag = string.Empty;
        PTranscriptChangeDefer();
    }

    private void PTranscriptTranslationHandle(object sender, TextChangedEventArgs e)
    {
        if (_pTranscriptLoading)
        {
            return;
        }

        _pTranscriptTranslationUnknown = false;
        PTranscriptTranslation.Tag = string.Empty;
        PTranscriptChangeDefer();
    }

    private void PTranscriptApply(LExample? example)
    {
        _pTranscriptLoading = true;

        string unknown = _pCorpusHost.PLocalizationTextRead("Display.Unknown");

        PTranscriptText.Text = example?.LExampleText.LStateValueShow() ?? string.Empty;
        _pTranscriptTextUnknown = example?.LExampleText.LStateValueState == LState.LStateUnknown;
        PTranscriptText.Tag = _pTranscriptTextUnknown ? unknown : string.Empty;

        PTranscriptTranslation.Text = example?.LExampleTranslation.LStateValueShow() ?? string.Empty;
        _pTranscriptTranslationUnknown =
            example?.LExampleTranslation.LStateValueState == LState.LStateUnknown;
        PTranscriptTranslation.Tag = _pTranscriptTranslationUnknown ? unknown : string.Empty;

        _pTranscriptLanguage = example?.LExampleLanguage ?? string.Empty;

        PSpeakerShow();

        _pTranscriptCitation = example?.LExampleSource.LStateAnchorShow() ?? 0;
        PCitationList.SelectedValue = _pTranscriptCitation == 0 ? null : _pTranscriptCitation;
        PCitationUpdate();

        long? stored = PTranscriptExampleRead();
        PTranscriptRemoval.IsEnabled = stored is not null;
        PTranscriptCountShow(stored);

        _pTranscriptLoading = false;

        PTranscriptChangeUpdate();
    }

    private void PTranscriptCountShow(long? id)
    {
        if (id is null)
        {
            PTranscriptCount.Text = string.Empty;
            return;
        }

        int usage = _pAnthologyCount.TryGetValue(id.Value, out int count) ? count : 0;
        PTranscriptCount.Text = $"{_pCorpusHost.PLocalizationTextRead("Example.DetachCount")} "
            + usage.ToString(CultureInfo.CurrentCulture);
    }

    private void PTranscriptShow(LExample example)
    {
        _pTranscriptLoading = true;

        string unknown = _pCorpusHost.PLocalizationTextRead("Display.Unknown");

        PTranscriptFieldShow(PTranscriptText, example.LExampleText, unknown, ref _pTranscriptTextUnknown);
        PTranscriptFieldShow(
            PTranscriptTranslation, example.LExampleTranslation, unknown, ref _pTranscriptTranslationUnknown);

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

        _pTranscriptLoading = false;

        PTranscriptChangeUpdate();
    }

    private static void PTranscriptFieldShow(TextBox field, LStateValue value, string unknown, ref bool held)
    {
        if (LStateValue.LStateValueResolve(field.Text, held) == value)
        {
            return;
        }

        field.Text = value.LStateValueShow();
        held = value.LStateValueState == LState.LStateUnknown;
        field.Tag = held ? unknown : string.Empty;
    }

    private LExample PTranscriptRead()
    {
        return new LExample(
            0,
            _pTranscriptLanguage,
            LStateValue.LStateValueResolve(PTranscriptText.Text, _pTranscriptTextUnknown),
            LStateValue.LStateValueResolve(PTranscriptTranslation.Text, _pTranscriptTranslationUnknown),
            LStateAnchor.LStateAnchorRead(_pTranscriptCitation));
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
        PCorpusScribe.IsEnabled = true;
    }

    private void PTranscriptDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        long? example = PTranscriptExampleRead();
        PTranscriptDraftShow(PTranscriptDraftStart(example));
    }

    private void PTranscriptStoreHandle(object sender, RoutedEventArgs e)
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

    private void PTranscriptRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (PTranscriptExampleRead() is not long id)
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
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }
}
