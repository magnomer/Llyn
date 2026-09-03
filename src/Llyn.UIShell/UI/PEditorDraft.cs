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
            _pLanguageChoice,
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
        PEditorLanguageShow(draft.LEntryDraftLanguage);

        IReadOnlyDictionary<string, LTranslationTarget> targets = PEditorTargetRead(draft);
        PCardShow(_pSenseList, "Meaning", draft.LEntryDraftSenses, targets);
        PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations, targets);

        PEditorNoteShow(draft.LEntryDraftNote);
        PEditorRecordingShow(draft);

        _pStateDraft = PEditorDraftRead();
    }

    private static IReadOnlyDictionary<string, LTranslationTarget> PEditorTargetEmpty =>
        new Dictionary<string, LTranslationTarget>(StringComparer.Ordinal);

    private IReadOnlyDictionary<string, LTranslationTarget> PEditorTargetRead(LEntryDraft draft)
    {
        List<string> ids = [];
        PEditorTargetRead(draft.LEntryDraftSenses, ids);
        PEditorTargetRead(draft.LEntryDraftCollocations, ids);

        Dictionary<string, LTranslationTarget> targets = new(StringComparer.Ordinal);
        if (ids.Count == 0)
        {
            return targets;
        }

        try
        {
            foreach (LTranslationTarget target in _lEngine.LEngineTargetRead(ids))
            {
                targets[target.LTranslationTargetId] = target;
            }
        }
        catch (Exception)
        {
            targets.Clear();
        }

        return targets;
    }

    private static IReadOnlyList<LTranslationTarget> PCardTargetRead(
        IReadOnlyDictionary<string, LTranslationTarget> targets, IReadOnlyList<string> ids)
    {
        List<LTranslationTarget> found = [];
        foreach (string id in ids)
        {
            if (targets.TryGetValue(id, out LTranslationTarget? target))
            {
                found.Add(target);
            }
        }

        return found;
    }

    private static void PEditorTargetRead(IReadOnlyList<LCardDraft> cards, List<string> ids)
    {
        foreach (LCardDraft card in cards)
        {
            foreach (string id in card.LCardDraftTranslation)
            {
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                }
            }
        }
    }

    private void PCardShow(
        ObservableCollection<PCard> cards,
        string prefix,
        IReadOnlyList<LCardDraft> drafts,
        IReadOnlyDictionary<string, LTranslationTarget> targets)
    {
        cards.Clear();
        foreach (LCardDraft draft in drafts)
        {
            PCard card = new(prefix, cards.Count + 1, _pSentenceReference)
            {
                PCardId = draft.LCardDraftId
            };

            card.PCardTitleShow(draft.LCardDraftTitle);
            card.PCardExpressionShow(draft.LCardDraftExpression);
            card.PCardDefinitionShow(draft.LCardDraftMeaning);
            card.PCardSentenceShow(draft.LCardDraftExample);
            card.PCardSituationShow(draft.LCardDraftSituation);
            card.PCardLinkShow(PCardTargetRead(targets, draft.LCardDraftTranslation));
            PLinkAttach(card);
            card.PCardLabelShow(draft.LCardDraftTag);
            card.PCardImageShow(draft.LCardDraftImage);
            cards.Add(card);
        }

        if (cards.Count == 0)
        {
            PCard card = new(prefix, 1, _pSentenceReference);
            PLinkAttach(card);
            cards.Add(card);
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

    private void PEditorLanguageShow(string language)
    {
        if (language.Length == 0)
        {
            return;
        }

        _pLanguageEntry = true;

        if (string.Equals(_pLanguageChoice, language, StringComparison.Ordinal))
        {
            return;
        }

        _pLanguageChoice = language;
        PLanguageName.Text = language;
        PLanguageFlagUpdate();
    }

    internal void PEditorReset()
    {
        _pEditorEntry = null;

        PHeadword.Text = string.Empty;
        PPronunciation.Text = string.Empty;
        PSpeechShow(null);
        PRecordingClear();
        _pLanguageEntry = false;

        PCardShow(_pSenseList, "Meaning", [], PEditorTargetEmpty);
        PCardShow(_pCollocationList, "Collocation", [], PEditorTargetEmpty);

        PNoteContents.Text = string.Empty;
        PNotePlaceholder.Visibility = Visibility.Visible;

        _pStateDraft = PEditorDraftRead();
    }

    private void PEditorStateUpdate()
    {
        if (_pStateDraft is null)
        {
            return;
        }

        _pStateDraft = _pStateDraft with { LEntryDraftLanguage = _pLanguageChoice };
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
                || !PEditorTextMatch(first.LCardDraftTranslation, second.LCardDraftTranslation)
                || !PEditorTextMatch(first.LCardDraftTag, second.LCardDraftTag)
                || !PEditorValueMatch(first.LCardDraftImage, second.LCardDraftImage))
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
                card.PCardSentenceRead(),
                card.PCardSituationRead(),
                card.PCardLinkRead(),
                string.Empty,
                card.PCardLabelRead(),
                card.PCardImageRead(),
                card.PCardId));
        }

        return drafts;
    }

    private string PEditorNoteRead()
    {
        return (PNoteContents.Text ?? string.Empty).TrimEnd('\r', '\n');
    }
}
