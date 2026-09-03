using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PExample
{
    private LExample? _pTranscriptExample;

    private string _pTranscriptCitation = string.Empty;

    private string _pTranscriptLanguage = string.Empty;

    private bool _pTranscriptTextUnreadable;

    private bool _pTranscriptTranslationUnreadable;

    private bool _pTranscriptLoading;

    private void PLanguageLoad()
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

        _pTongueItem.Clear();
        foreach (string language in languages)
        {
            _pTongueItem.Add(new PTongueItem(
                language, PEnsign.PEnsignFind(language)));
        }

        if (_pTranscriptLanguage.Length == 0 && languages.Count > 0)
        {
            _pTranscriptLanguage = languages[0];
        }

        PLanguageShow();
    }

    private void PLanguageHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PTongueItem item })
        {
            return;
        }

        _pTranscriptLanguage = item.PTongueItemName;
        PLanguage.IsChecked = false;
        PLanguageShow();
    }

    private void PLanguageShow()
    {
        PLanguageName.Text = _pTranscriptLanguage;
        PLanguageFlag.Source = PEnsign.PEnsignFind(_pTranscriptLanguage);
    }

    private void PCitationFind()
    {
        IReadOnlyList<LReference> read;
        try
        {
            read = _lEngine.LEngineReferenceRead();
        }
        catch (Exception exception)
        {
            _pExampleHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        _pCitationCatalog.Clear();
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

    private void PCitationFreshHandle(object sender, RoutedEventArgs e)
    {
        string name = (PCitationDraft.Text ?? string.Empty).Trim();
        if (name.Length == 0)
        {
            return;
        }

        LReference created;
        try
        {
            created = _lEngine.LEngineReferenceCreate(new LReference(
                string.Empty,
                LStateValue.LStateValueCreate(name),
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                LState.LStateUnspecified));
        }
        catch (Exception exception)
        {
            _pExampleHost.PWindowFailureShow("Reference.AddFailed", exception);
            return;
        }

        _pCitationCatalog.Add(PCitationItem.PCitationItemCreate(created));
        PCitationEmpty.Visibility = Visibility.Collapsed;
        PCitationDraft.Text = string.Empty;

        _pTranscriptCitation = created.LReferenceId;
        PCitationList.SelectedValue = created.LReferenceId;
        PCitation.IsChecked = false;
        PCitationUpdate();
    }

    private void PCitationUpdate()
    {
        PCitationName.Text = _pTranscriptCitation.Length == 0
            ? _pExampleHost.PLocalizationTextRead("Reference.Assign")
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

        string unreadable = _pExampleHost.PLocalizationTextRead("Display.Unreadable");

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

        PLanguageShow();

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
        PTranscriptCount.Text = $"{_pExampleHost.PLocalizationTextRead("Example.DetachCount")} "
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

    private void PExampleFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PExampleLeaveConfirm())
        {
            return;
        }

        PExampleClear();
        _pTranscriptExample = null;
        PTranscriptApply(null);
        PExampleScribe.IsEnabled = true;
        PExampleScribeShow(true);
    }

    private void PTranscriptDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!PExampleLeaveConfirm())
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
            _pExampleHost.PWindowFailureShow("Example.SaveFailed", exception);
            return;
        }

        _pTranscriptExample = written;
        _pExcerptExample = written.LExampleId;

        PAnthologyFind(PQuery.Text ?? string.Empty);
        PExampleScribeShow(false);
        PExampleShow(written.LExampleId);
    }

    private void PTranscriptRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (_pTranscriptExample is null)
        {
            return;
        }

        string id = _pTranscriptExample.LExampleId;
        int usage = _pAnthologyCount.TryGetValue(id, out int count) ? count : 0;

        if (!_pExampleHost.PWindowRemovalConfirm(usage, "Example"))
        {
            return;
        }

        try
        {
            _lEngine.LEngineExampleDelete(id, usage > 0);
        }
        catch (Exception exception)
        {
            _pExampleHost.PWindowFailureShow("Example.DeleteFailed", exception);
            return;
        }

        PExampleScribeShow(false);
        PExampleClear();
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }
}
