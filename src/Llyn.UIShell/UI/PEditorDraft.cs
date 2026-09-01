using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private LEntryDraft? _pStateDraft;

    private LEntryDraft PEditorDraftRead()
    {
        return new LEntryDraft(
            PHeadword.Text ?? string.Empty,
            _pLangcodeChoice,
            PPronunciation.Text ?? string.Empty,
            PEditorNoteRead(),
            PCardRead(_pSenseList),
            PCardRead(_pCollocationList),
            _pRecording ?? string.Empty,
            _pRecordingSource,
            PSpeechContents.Text ?? string.Empty);
    }

    private void PEditorDraftShow(LEntryDraft draft)
    {
        PHeadword.Text = draft.LEntryDraftHeadword;
        PPronunciation.Text = draft.LEntryDraftPronunciation;
        PSpeechContents.Text = draft.LEntryDraftSpeech;
        PEditorLangcodeShow(draft.LEntryDraftLanguage);

        PCardShow(_pSenseList, "Meaning", draft.LEntryDraftSenses);
        PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations);

        PEditorNoteShow(draft.LEntryDraftNote);
        PEditorRecordingShow(draft);

        _pStateDraft = PEditorDraftRead();
    }

    private static void PCardShow(
        ObservableCollection<PCard> cards,
        string prefix,
        IReadOnlyList<LCardDraft> drafts)
    {
        cards.Clear();
        foreach (LCardDraft draft in drafts)
        {
            cards.Add(new PCard(prefix, cards.Count + 1)
            {
                PTitle = draft.LCardDraftTitle,
                PCardExpression = draft.LCardDraftExpression,
                PCardDefinition = draft.LCardDraftMeaning,
                PCardExample = PEditorFieldFormat(draft.LCardDraftExample),
                PCardSituation = PEditorFieldFormat(draft.LCardDraftSituation),
                PCardTag = PEditorTagFormat(draft.LCardDraftTag),
                PCardId = draft.LCardDraftId
            });
        }

        if (cards.Count == 0)
        {
            cards.Add(new PCard(prefix, 1));
        }
    }

    private void PEditorNoteShow(string note)
    {
        PNoteContents.Text = note;
        PNotePlaceholder.Visibility = note.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PEditorRecordingShow(LEntryDraft draft)
    {
        PRecordingClear();
        if (draft.LEntryDraftAudio.Length == 0 || !File.Exists(draft.LEntryDraftAudio))
        {
            return;
        }

        _pRecording = draft.LEntryDraftAudio;
        _pRecordingSource = draft.LEntryDraftSource;
        _pRecordingStored = true;
        PPlayback.Visibility = Visibility.Visible;
    }

    private void PEditorLangcodeShow(string language)
    {
        if (language.Length == 0)
        {
            return;
        }

        _pLangcodeEntry = true;

        if (string.Equals(_pLangcodeChoice, language, StringComparison.Ordinal))
        {
            return;
        }

        _pLangcodeChoice = language;
        PLangcodeBaseName.Text = language;
        PLangcodeFlagUpdate();
    }

    internal void PEditorReset()
    {
        _pEditorEntry = null;

        PHeadword.Text = string.Empty;
        PPronunciation.Text = string.Empty;
        PSpeechContents.Text = string.Empty;
        PRecordingClear();
        _pLangcodeEntry = false;

        PCardShow(_pSenseList, "Meaning", []);
        PCardShow(_pCollocationList, "Collocation", []);

        PNoteContents.Text = string.Empty;
        PNotePlaceholder.Visibility = Visibility.Visible;

        _pStateDraft = PEditorDraftRead();
    }

    internal bool PEditorChangeCheck()
    {
        return _pStateDraft is not null && !PEditorDraftMatch(_pStateDraft, PEditorDraftRead());
    }

    private static bool PEditorDraftMatch(LEntryDraft one, LEntryDraft other)
    {
        return string.Equals(one.LEntryDraftHeadword, other.LEntryDraftHeadword, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftLanguage, other.LEntryDraftLanguage, StringComparison.Ordinal)
            && string.Equals(
                one.LEntryDraftPronunciation, other.LEntryDraftPronunciation, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftNote, other.LEntryDraftNote, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftSpeech, other.LEntryDraftSpeech, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftAudio, other.LEntryDraftAudio, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftSource, other.LEntryDraftSource, StringComparison.Ordinal)
            && PCardMatch(one.LEntryDraftSenses, other.LEntryDraftSenses)
            && PCardMatch(one.LEntryDraftCollocations, other.LEntryDraftCollocations);
    }

    private static bool PCardMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            LCardDraft first = one[index];
            LCardDraft second = other[index];
            if (!string.Equals(first.LCardDraftTitle, second.LCardDraftTitle, StringComparison.Ordinal)
                || !string.Equals(
                    first.LCardDraftExpression, second.LCardDraftExpression, StringComparison.Ordinal)
                || !string.Equals(first.LCardDraftMeaning, second.LCardDraftMeaning, StringComparison.Ordinal)
                || !string.Equals(first.LCardDraftId, second.LCardDraftId, StringComparison.Ordinal)
                || !PEditorTextMatch(first.LCardDraftExample, second.LCardDraftExample)
                || !PEditorTextMatch(first.LCardDraftSituation, second.LCardDraftSituation)
                || !PEditorTextMatch(first.LCardDraftTag, second.LCardDraftTag))
            {
                return false;
            }
        }

        return true;
    }

    private static bool PEditorTextMatch(IReadOnlyList<string> one, IReadOnlyList<string> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (!string.Equals(one[index], other[index], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private IReadOnlyList<LCardDraft> PCardRead(IReadOnlyList<PCard> cards)
    {
        List<LCardDraft> drafts = new(cards.Count);
        foreach (PCard card in cards)
        {
            drafts.Add(new LCardDraft(
                card.PTitle,
                card.PCardExpression,
                card.PCardDefinition,
                PEditorFieldRead(card.PCardExample),
                PEditorFieldRead(card.PCardSituation),
                string.Empty,
                PEditorTagParse(card.PCardTag),
                card.PCardId));
        }

        return drafts;
    }

    private static IReadOnlyList<string> PEditorFieldRead(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? [] : [text];
    }

    private static string PEditorFieldFormat(IReadOnlyList<string> texts)
    {
        return texts.Count == 0 ? string.Empty : texts[0];
    }

    private static IReadOnlyList<string> PEditorTagParse(string text)
    {
        List<string> tags = [];
        foreach (string part in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            tags.Add(part);
        }

        return tags;
    }

    private static string PEditorTagFormat(IReadOnlyList<string> tags)
    {
        return string.Join(", ", tags);
    }

    private string PEditorNoteRead()
    {
        return (PNoteContents.Text ?? string.Empty).TrimEnd('\r', '\n');
    }
}
