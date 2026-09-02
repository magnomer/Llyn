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
            PSpeechRead());
    }

    private void PEditorDraftShow(LEntryDraft draft)
    {
        PHeadword.Text = draft.LEntryDraftHeadword;
        PPronunciation.Text = draft.LEntryDraftPronunciation;
        PSpeechShow(draft.LEntryDraftSpeeches);
        PEditorLangcodeShow(draft.LEntryDraftLanguage);

        PCardShow(_pSenseList, "Meaning", draft.LEntryDraftSenses);
        PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations);

        PEditorNoteShow(draft.LEntryDraftNote);
        PEditorRecordingShow(draft);

        _pStateDraft = PEditorDraftRead();
    }

    private void PCardShow(
        ObservableCollection<PCard> cards,
        string prefix,
        IReadOnlyList<LCardDraft> drafts)
    {
        cards.Clear();
        foreach (LCardDraft draft in drafts)
        {
            PCard card = new(prefix, cards.Count + 1, _pExampleReference)
            {
                PCardTag = PEditorTagFormat(draft.LCardDraftTag),
                PCardId = draft.LCardDraftId
            };

            card.PCardTitleShow(draft.LCardDraftTitle);
            card.PCardExpressionShow(draft.LCardDraftExpression);
            card.PCardDefinitionShow(draft.LCardDraftMeaning);
            card.PCardExampleShow(draft.LCardDraftExample);
            card.PCardSituationShow(draft.LCardDraftSituation);
            cards.Add(card);
        }

        if (cards.Count == 0)
        {
            cards.Add(new PCard(prefix, 1, _pExampleReference));
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
        PSpeechShow(null);
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
            && PEditorTextMatch(one.LEntryDraftSpeeches ?? [], other.LEntryDraftSpeeches ?? [])
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
            if (first.LCardDraftTitle != second.LCardDraftTitle
                || first.LCardDraftExpression != second.LCardDraftExpression
                || first.LCardDraftMeaning != second.LCardDraftMeaning
                || !string.Equals(first.LCardDraftId, second.LCardDraftId, StringComparison.Ordinal)
                || !PEditorExampleMatch(first.LCardDraftExample, second.LCardDraftExample)
                || !PEditorSituationMatch(first.LCardDraftSituation, second.LCardDraftSituation)
                || !PEditorValueMatch(first.LCardDraftTag, second.LCardDraftTag))
            {
                return false;
            }
        }

        return true;
    }

    private static bool PEditorExampleMatch(
        IReadOnlyList<LExampleDraft> one, IReadOnlyList<LExampleDraft> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LExampleDraftText != other[index].LExampleDraftText
                || !string.Equals(
                    one[index].LExampleDraftId, other[index].LExampleDraftId, StringComparison.Ordinal)
                || one[index].LExampleDraftReference != other[index].LExampleDraftReference)
            {
                return false;
            }
        }

        return true;
    }

    private static bool PEditorSituationMatch(
        IReadOnlyList<LSituationDraft> one, IReadOnlyList<LSituationDraft> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LSituationDraftText != other[index].LSituationDraftText
                || !string.Equals(
                    one[index].LSituationDraftId, other[index].LSituationDraftId, StringComparison.Ordinal)
                || one[index].LSituationDraftReference != other[index].LSituationDraftReference)
            {
                return false;
            }
        }

        return true;
    }

    private static bool PEditorValueMatch(
        IReadOnlyList<LStateValue> one, IReadOnlyList<LStateValue> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index] != other[index])
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
                card.PCardTitleRead(),
                card.PCardExpressionRead(),
                card.PCardDefinitionRead(),
                card.PCardExampleRead(),
                card.PCardSituationRead(),
                string.Empty,
                PEditorTagParse(card.PCardTag),
                card.PCardId));
        }

        return drafts;
    }

    private static IReadOnlyList<LStateValue> PEditorTagParse(string text)
    {
        List<LStateValue> tags = [];
        foreach (string part in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            tags.Add(string.Equals(part, PCard.PCardUnreadableMark, StringComparison.Ordinal)
                ? LStateValue.LStateValueUnknown
                : LStateValue.LStateValueCreate(part));
        }

        return tags;
    }

    private static string PEditorTagFormat(IReadOnlyList<LStateValue> tags)
    {
        List<string> texts = new(tags.Count);
        foreach (LStateValue tag in tags)
        {
            if (tag.LStateValueEmpty)
            {
                continue;
            }

            texts.Add(tag.LStateValueState == LState.LStateUnknown
                ? PCard.PCardUnreadableMark
                : tag.LStateValueShow());
        }

        return string.Join(", ", texts);
    }

    private string PEditorNoteRead()
    {
        return (PNoteContents.Text ?? string.Empty).TrimEnd('\r', '\n');
    }
}
