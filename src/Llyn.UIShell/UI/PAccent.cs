using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PAccentItem> _pAccentItem = [];
    private string _pAccentLanguage = string.Empty;
    private bool _pAccentFlagged;
    private string _pAccentPrimary = string.Empty;

    private static string PAccentRequestFormat(long id)
    {
        return string.Concat("Accent:", id.ToString(CultureInfo.InvariantCulture));
    }

    internal void PAccentRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PAccentItem row)
        {
            PEditorRequestSend(new LRequestPronunciationRemoval(_pEditorDraft, row.PAccentItemId));
        }
    }

    private void PAccentChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not PAccentItem row
            || !string.Equals(e.PropertyName, nameof(PAccentItem.PAccentItemIpa), StringComparison.Ordinal))
        {
            return;
        }

        PEditorRequestDefer(
            PAccentRequestFormat(row.PAccentItemId),
            new LRequestPronunciationIpa(_pEditorDraft, row.PAccentItemId, row.PAccentItemIpa));
    }

    private void PAccentShow(LEntryDraft draft)
    {
        string language = draft.LEntryDraftLanguage;
        bool flagged = language.Length > 0 && _lEngine.LEngineFlaggedCheck(language);
        _pAccentLanguage = language;
        _pAccentFlagged = flagged;
        _pAccentPrimary = draft.LEntryDraftPronunciation?.LPronunciationDraftVariety ?? string.Empty;

        PCard.PCardRowShow(
            _pAccentItem,
            draft.LEntryDraftPronunciations.Skip(1).ToList(),
            static row => row.PAccentItemId,
            static spoken => spoken.LPronunciationDraftId,
            PAccentCreate,
            PAccentUpdate);

        PAccentPrimaryShow();
        _ = PAccentFlagLoad(language, flagged);
    }

    private PAccentItem PAccentCreate(LPronunciationDraft spoken)
    {
        PAccentItem row = PAccentItem.PAccentItemCreate(_pEditorHost, _pAccentLanguage, _pAccentFlagged, spoken);
        row.PropertyChanged += PAccentChangeHandle;
        return row;
    }

    private PAccentItem PAccentUpdate(PAccentItem row, LPronunciationDraft spoken)
    {
        if (!string.Equals(row.PAccentItemVariety, spoken.LPronunciationDraftVariety, StringComparison.Ordinal))
        {
            row.PropertyChanged -= PAccentChangeHandle;
            return PAccentCreate(spoken);
        }

        if (!PEditorRequestCheck(PAccentRequestFormat(row.PAccentItemId)))
        {
            row.PAccentItemIpa = spoken.LPronunciationDraftIpa;
        }

        return row;
    }

    private void PAccentPrimaryShow()
    {
        PPronunciationFlag.Source = PAccentItem.PAccentFlagFind(_pAccentLanguage, _pAccentFlagged, _pAccentPrimary);
        PPronunciationLabel.Text = PPronunciationFlag.Source is null
            ? PAccentItem.PAccentLabelFormat(_pEditorHost, _pAccentPrimary)
            : string.Empty;
    }

    private async Task PAccentFlagLoad(string language, bool flagged)
    {
        if (!flagged)
        {
            return;
        }

        List<string> varieties = _pAccentItem.Select(static row => row.PAccentItemVariety).ToList();
        varieties.Add(_pAccentPrimary);

        try
        {
            await PEnsign.PEnsignVarietyLoad(
                _lEngine, language, varieties.Where(static variety => variety.Length > 0));
        }
        catch (Exception)
        {
            return;
        }

        if (!string.Equals(_pAccentLanguage, language, StringComparison.Ordinal) || !_pAccentFlagged)
        {
            return;
        }

        foreach (PAccentItem row in _pAccentItem)
        {
            row.PAccentFlagUpdate(language, flagged);
        }

        PAccentPrimaryShow();
    }

    private void PAccentClear()
    {
        foreach (PAccentItem row in _pAccentItem)
        {
            row.PropertyChanged -= PAccentChangeHandle;
        }

        _pAccentItem.Clear();
        _pAccentLanguage = string.Empty;
        _pAccentFlagged = false;
        _pAccentPrimary = string.Empty;
        PAccentPrimaryShow();
    }
}
