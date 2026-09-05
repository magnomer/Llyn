using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private LExample? _pTranscriptExample;

    private string _pTranscriptCitation = string.Empty;

    private string _pTranscriptLanguage = string.Empty;

    private bool _pTranscriptTextUnreadable;

    private bool _pTranscriptTranslationUnreadable;

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

        if (_pTranscriptLanguage.Length == 0 && languages.Count > 0)
        {
            _pTranscriptLanguage = languages[0];
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

    private string PCitationNameRead(LStateValue source)
    {
        string id = source.LStateValueShow();
        if (id.Length == 0)
        {
            return string.Empty;
        }

        foreach (PCitationItem row in _pCitationCatalog)
        {
            if (string.Equals(row.PCitationItemId, id, StringComparison.Ordinal))
            {
                return row.PCitationItemName;
            }
        }

        return id;
    }

    private void PCitationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (_pTranscriptLoading || PCitationList.SelectedValue is not string id)
        {
            return;
        }

        _pTranscriptCitation = id;
        PCitation.IsChecked = false;
        PCitationUpdate();
    }

    private void PCitationClearHandle(object sender, RoutedEventArgs e)
    {
        _pTranscriptCitation = string.Empty;
        PCitationList.SelectedValue = null;
        PCitation.IsChecked = false;
        PCitationUpdate();
    }

    private void PCitationUpdate()
    {
        PCitationName.Text = _pTranscriptCitation.Length == 0
            ? _pCorpusHost.PLocalizationTextRead("Reference.Assign")
            : PCitationNameRead(LStateValue.LStateValueCreate(_pTranscriptCitation));
    }

    private void PTranscriptTextHandle(object sender, TextChangedEventArgs e)
    {
        if (_pTranscriptLoading)
        {
            return;
        }

        _pTranscriptTextUnreadable = false;
        PTranscriptText.Tag = string.Empty;
    }

    private void PTranscriptTranslationHandle(object sender, TextChangedEventArgs e)
    {
        if (_pTranscriptLoading)
        {
            return;
        }

        _pTranscriptTranslationUnreadable = false;
        PTranscriptTranslation.Tag = string.Empty;
    }

    private void PTranscriptApply(LExample? example)
    {
        _pTranscriptLoading = true;

        string unreadable = _pCorpusHost.PLocalizationTextRead("Display.Unreadable");

        PTranscriptText.Text = example?.LExampleText.LStateValueShow() ?? string.Empty;
        _pTranscriptTextUnreadable = example?.LExampleText.LStateValueState == LState.LStateUnknown;
        PTranscriptText.Tag = _pTranscriptTextUnreadable ? unreadable : string.Empty;

        PTranscriptTranslation.Text = example?.LExampleTranslation.LStateValueShow() ?? string.Empty;
        _pTranscriptTranslationUnreadable =
            example?.LExampleTranslation.LStateValueState == LState.LStateUnknown;
        PTranscriptTranslation.Tag = _pTranscriptTranslationUnreadable ? unreadable : string.Empty;

        if (example is not null && example.LExampleLanguage.Length > 0)
        {
            _pTranscriptLanguage = example.LExampleLanguage;
        }

        PSpeakerShow();

        _pTranscriptCitation = example?.LExampleSource.LStateValueShow() ?? string.Empty;
        PCitationList.SelectedValue = _pTranscriptCitation.Length == 0 ? null : _pTranscriptCitation;
        PCitationUpdate();

        PTranscriptRemoval.IsEnabled = example is not null;
        PTranscriptCountShow(example);

        _pTranscriptLoading = false;
    }

    private void PTranscriptCountShow(LExample? example)
    {
        if (example is null)
        {
            PTranscriptCount.Text = string.Empty;
            return;
        }

        int usage = _pAnthologyCount.TryGetValue(example.LExampleId, out int count) ? count : 0;
        PTranscriptCount.Text = $"{_pCorpusHost.PLocalizationTextRead("Example.DetachCount")} "
            + usage.ToString(CultureInfo.CurrentCulture);
    }

    private LExample PTranscriptRead()
    {
        return new LExample(
            _pTranscriptExample?.LExampleId ?? string.Empty,
            _pTranscriptLanguage,
            PTranscriptValueRead(PTranscriptText.Text, _pTranscriptTextUnreadable),
            PTranscriptValueRead(PTranscriptTranslation.Text, _pTranscriptTranslationUnreadable),
            PTranscriptValueRead(_pTranscriptCitation, false));
    }

    private static LStateValue PTranscriptValueRead(string text, bool unreadable)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            return LStateValue.LStateValueCreate(text);
        }

        return unreadable ? LStateValue.LStateValueUnknown : LStateValue.LStateValueUnspecified;
    }

    private bool PTranscriptChangeCheck()
    {
        LExample written = PTranscriptRead();

        if (_pTranscriptExample is null)
        {
            return !written.LExampleText.LStateValueEmpty
                || !written.LExampleTranslation.LStateValueEmpty
                || !written.LExampleSource.LStateValueEmpty;
        }

        return written.LExampleText != _pTranscriptExample.LExampleText
            || written.LExampleTranslation != _pTranscriptExample.LExampleTranslation
            || written.LExampleSource != _pTranscriptExample.LExampleSource
            || !string.Equals(
                written.LExampleLanguage, _pTranscriptExample.LExampleLanguage, StringComparison.Ordinal);
    }

    private void PCorpusFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        PCorpusClear();
        _pTranscriptExample = null;
        PTranscriptApply(null);
        PCorpusScribe.IsEnabled = true;
        PCorpusScribeShow(true);
    }

    private void PTranscriptDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        PTranscriptApply(_pTranscriptExample);
    }

    private void PTranscriptStoreHandle(object sender, RoutedEventArgs e)
    {
        LExample written = PTranscriptRead();

        try
        {
            if (_pTranscriptExample is null)
            {
                written = _lEngine.LEngineExampleCreate(written);
            }
            else
            {
                _lEngine.LEngineExampleUpdate(written);
            }
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.SaveFailed", exception);
            return;
        }

        _pTranscriptExample = written;
        _pExcerptExample = written.LExampleId;

        PAnthologyFind(PQuery.Text ?? string.Empty);
        PCorpusScribeShow(false);
        PCorpusShow(written.LExampleId);
    }

    private void PTranscriptRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (_pTranscriptExample is null)
        {
            return;
        }

        string id = _pTranscriptExample.LExampleId;
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
