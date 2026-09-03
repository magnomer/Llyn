using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PExample
{
    private LExample? _pEditorExample;

    private string _pEditorCitation = string.Empty;

    private string _pEditorLanguage = string.Empty;

    private bool _pEditorTextUnreadable;

    private bool _pEditorTranslationUnreadable;

    private bool _pEditorLoading;

    private void PLangcodeLoad()
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

        _pLangcodeItem.Clear();
        foreach (string language in languages)
        {
            _pLangcodeItem.Add(new PLangcodeItem(
                language, PLangcodeIndicator.PLangcodeIndicatorFind(language)));
        }

        if (_pEditorLanguage.Length == 0 && languages.Count > 0)
        {
            _pEditorLanguage = languages[0];
        }

        PLangcodeShow();
    }

    private void PLangcodeHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PLangcodeItem item })
        {
            return;
        }

        _pEditorLanguage = item.PLangcodeItemName;
        PLangcode.IsChecked = false;
        PLangcodeShow();
    }

    private void PLangcodeShow()
    {
        PLangcodeName.Text = _pEditorLanguage;
        PLangcodeFlag.Source = PLangcodeIndicator.PLangcodeIndicatorFind(_pEditorLanguage);
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
            _pCitationCatalog.Add(PReference.PReferenceCreate(reference));
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

        foreach (PReference row in _pCitationCatalog)
        {
            if (string.Equals(row.PReferenceId, id, StringComparison.Ordinal))
            {
                return row.PReferenceName;
            }
        }

        return id;
    }

    private void PCitationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (_pEditorLoading || PCitationList.SelectedValue is not string id)
        {
            return;
        }

        _pEditorCitation = id;
        PCitation.IsChecked = false;
        PCitationUpdate();
    }

    private void PCitationClearHandle(object sender, RoutedEventArgs e)
    {
        _pEditorCitation = string.Empty;
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

        _pCitationCatalog.Add(PReference.PReferenceCreate(created));
        PCitationEmpty.Visibility = Visibility.Collapsed;
        PCitationDraft.Text = string.Empty;

        _pEditorCitation = created.LReferenceId;
        PCitationList.SelectedValue = created.LReferenceId;
        PCitation.IsChecked = false;
        PCitationUpdate();
    }

    private void PCitationUpdate()
    {
        PCitationName.Text = _pEditorCitation.Length == 0
            ? _pExampleHost.PLocalizationTextRead("Reference.Assign")
            : PCitationNameRead(LStateValue.LStateValueCreate(_pEditorCitation));
    }

    private void PEditorTextHandle(object sender, TextChangedEventArgs e)
    {
        if (_pEditorLoading)
        {
            return;
        }

        _pEditorTextUnreadable = false;
        PEditorText.Tag = string.Empty;
    }

    private void PEditorTranslationHandle(object sender, TextChangedEventArgs e)
    {
        if (_pEditorLoading)
        {
            return;
        }

        _pEditorTranslationUnreadable = false;
        PEditorTranslation.Tag = string.Empty;
    }

    private void PEditorApply(LExample? example)
    {
        _pEditorLoading = true;

        string unreadable = _pExampleHost.PLocalizationTextRead("Display.Unreadable");

        PEditorText.Text = example?.LExampleText.LStateValueShow() ?? string.Empty;
        _pEditorTextUnreadable = example?.LExampleText.LStateValueState == LState.LStateUnknown;
        PEditorText.Tag = _pEditorTextUnreadable ? unreadable : string.Empty;

        PEditorTranslation.Text = example?.LExampleTranslation.LStateValueShow() ?? string.Empty;
        _pEditorTranslationUnreadable =
            example?.LExampleTranslation.LStateValueState == LState.LStateUnknown;
        PEditorTranslation.Tag = _pEditorTranslationUnreadable ? unreadable : string.Empty;

        if (example is not null && example.LExampleLanguage.Length > 0)
        {
            _pEditorLanguage = example.LExampleLanguage;
        }

        PLangcodeShow();

        _pEditorCitation = example?.LExampleSource.LStateValueShow() ?? string.Empty;
        PCitationList.SelectedValue = _pEditorCitation.Length == 0 ? null : _pEditorCitation;
        PCitationUpdate();

        PRemoval.IsEnabled = example is not null;
        PEditorUsageShow(example);

        _pEditorLoading = false;
    }

    private void PEditorUsageShow(LExample? example)
    {
        if (example is null)
        {
            PEditorUsage.Text = string.Empty;
            return;
        }

        int usage = _pAnthologyUsage.TryGetValue(example.LExampleId, out int count) ? count : 0;
        PEditorUsage.Text = $"{_pExampleHost.PLocalizationTextRead("Example.DetachCount")} "
            + usage.ToString(CultureInfo.CurrentCulture);
    }

    private LExample PEditorRead()
    {
        return new LExample(
            _pEditorExample?.LExampleId ?? string.Empty,
            _pEditorLanguage,
            PEditorValueRead(PEditorText.Text, _pEditorTextUnreadable),
            PEditorValueRead(PEditorTranslation.Text, _pEditorTranslationUnreadable),
            PEditorValueRead(_pEditorCitation, false));
    }

    private static LStateValue PEditorValueRead(string text, bool unreadable)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            return LStateValue.LStateValueCreate(text);
        }

        return unreadable ? LStateValue.LStateValueUnknown : LStateValue.LStateValueUnspecified;
    }

    private bool PEditorChangeCheck()
    {
        LExample written = PEditorRead();

        if (_pEditorExample is null)
        {
            return !written.LExampleText.LStateValueEmpty
                || !written.LExampleTranslation.LStateValueEmpty
                || !written.LExampleSource.LStateValueEmpty;
        }

        return written.LExampleText != _pEditorExample.LExampleText
            || written.LExampleTranslation != _pEditorExample.LExampleTranslation
            || written.LExampleSource != _pEditorExample.LExampleSource
            || !string.Equals(
                written.LExampleLanguage, _pEditorExample.LExampleLanguage, StringComparison.Ordinal);
    }

    private void PFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PExampleLeaveConfirm())
        {
            return;
        }

        PExampleClear();
        _pEditorExample = null;
        PEditorApply(null);
        PScribe.IsEnabled = true;
        PScribeShow(true);
    }

    private void PDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!PExampleLeaveConfirm())
        {
            return;
        }

        PEditorApply(_pEditorExample);
    }

    private void PStoreHandle(object sender, RoutedEventArgs e)
    {
        LExample written = PEditorRead();

        try
        {
            if (_pEditorExample is null)
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

        _pEditorExample = written;
        _pDisplayExample = written.LExampleId;

        PAnthologyFind(PQuery.Text ?? string.Empty);
        PScribeShow(false);
        PExampleShow(written.LExampleId);
    }

    private void PRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (_pEditorExample is null)
        {
            return;
        }

        string id = _pEditorExample.LExampleId;
        int usage = _pAnthologyUsage.TryGetValue(id, out int count) ? count : 0;

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

        PScribeShow(false);
        PExampleClear();
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }
}
