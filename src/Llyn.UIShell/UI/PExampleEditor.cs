using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    private bool _pEditorLoading;

    public ObservableCollection<string> PExampleLanguage { get; } = [];

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
        PExampleLanguage.Clear();
        foreach (string language in languages)
        {
            _pLangcodeItem.Add(new PLangcodeItem(
                language, PLangcodeIndicator.PLangcodeIndicatorFind(language)));
            PExampleLanguage.Add(language);
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

    private void PTranslationFreshHandle(object sender, RoutedEventArgs e)
    {
        _pTranslationList.Add(new PTranslationItem(string.Empty, _pEditorLanguage, string.Empty));
    }

    private void PTranslationRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PTranslationItem row })
        {
            _pTranslationList.Remove(row);
        }
    }

    private void PTranslationMoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PTranslationItem row, Tag: string step })
        {
            return;
        }

        int index = _pTranslationList.IndexOf(row);
        int moved = index + int.Parse(step, CultureInfo.InvariantCulture);
        if (index < 0 || moved < 0 || moved >= _pTranslationList.Count)
        {
            return;
        }

        _pTranslationList.Move(index, moved);
    }

    private IReadOnlyList<LTranslation> PTranslationRead()
    {
        List<LTranslation> translations = [];
        foreach (PTranslationItem row in _pTranslationList)
        {
            if (string.IsNullOrWhiteSpace(row.PTranslationItemText)
                || string.IsNullOrWhiteSpace(row.PTranslationItemLanguage))
            {
                continue;
            }

            translations.Add(new LTranslation(
                row.PTranslationItemId,
                row.PTranslationItemLanguage,
                row.PTranslationItemText,
                translations.Count));
        }

        return translations;
    }

    private void PEditorApply(LExample? example)
    {
        _pEditorLoading = true;

        string unreadable = _pExampleHost.PLocalizationTextRead("Display.Unreadable");

        PEditorText.Text = example?.LExampleText.LStateValueShow() ?? string.Empty;
        _pEditorTextUnreadable = example?.LExampleText.LStateValueState == LState.LStateUnknown;
        PEditorText.Tag = _pEditorTextUnreadable ? unreadable : string.Empty;

        PEditorLocal.Text = example?.LExampleLocal ?? string.Empty;

        if (example is not null && example.LExampleLanguage.Length > 0)
        {
            _pEditorLanguage = example.LExampleLanguage;
        }

        PLangcodeShow();

        _pTranslationList.Clear();
        foreach (LTranslation translation in example?.LExampleTranslations ?? [])
        {
            _pTranslationList.Add(new PTranslationItem(
                translation.LTranslationId,
                translation.LTranslationLanguage,
                translation.LTranslationText));
        }

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
            string.IsNullOrWhiteSpace(PEditorLocal.Text) ? null : PEditorLocal.Text,
            PEditorValueRead(_pEditorCitation, false),
            PTranslationRead());
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
                || written.LExampleLocal is not null
                || !written.LExampleSource.LStateValueEmpty
                || written.LExampleTranslations.Count > 0;
        }

        return written.LExampleText != _pEditorExample.LExampleText
            || written.LExampleSource != _pEditorExample.LExampleSource
            || !string.Equals(
                written.LExampleLanguage, _pEditorExample.LExampleLanguage, StringComparison.Ordinal)
            || !string.Equals(
                written.LExampleLocal ?? string.Empty,
                _pEditorExample.LExampleLocal ?? string.Empty,
                StringComparison.Ordinal)
            || PTranslationChangeCheck(written.LExampleTranslations, _pEditorExample.LExampleTranslations);
    }

    private static bool PTranslationChangeCheck(
        IReadOnlyList<LTranslation> written, IReadOnlyList<LTranslation> stored)
    {
        if (written.Count != stored.Count)
        {
            return true;
        }

        for (int index = 0; index < written.Count; index++)
        {
            if (!string.Equals(
                    written[index].LTranslationLanguage,
                    stored[index].LTranslationLanguage,
                    StringComparison.Ordinal)
                || !string.Equals(
                    written[index].LTranslationText,
                    stored[index].LTranslationText,
                    StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
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
