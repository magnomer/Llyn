using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private string _pEditorDraft = string.Empty;

    private bool _pEditorHalted;

    private LEntryDraft PEditorDraftRead()
    {
        return new LEntryDraft(
            PHeadword.Text ?? string.Empty,
            _pSpeakerChoice,
            PPronunciation.Text ?? string.Empty,
            PEditorNoteRead(),
            PCardRead(_pMeaningList),
            PCardRead(_pCollocationList),
            _pRecording ?? string.Empty,
            _pRecordingSource,
            PMarkerRead());
    }

    private void PEditorDraftShow(LEntryDraft draft)
    {
        _pEditorFill = true;
        PSentenceFrameLoad(draft.LEntryDraftLanguage);

        PHeadword.Text = draft.LEntryDraftHeadword;
        PPronunciation.Text = draft.LEntryDraftPronunciation;
        PMarkerShow(draft.LEntryDraftSpeeches);
        PEditorLanguageShow(draft.LEntryDraftLanguage);

        IReadOnlyDictionary<string, LTranslationTarget> targets = PEditorTargetRead(draft);
        PCardShow(_pMeaningList, "Meaning", draft.LEntryDraftMeanings, targets);
        PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations, targets);

        PEditorNoteShow(draft.LEntryDraftNote);
        PEditorRecordingShow(draft);

        _pEditorFill = false;
    }

    private static IReadOnlyDictionary<string, LTranslationTarget> PEditorTargetEmpty =>
        new Dictionary<string, LTranslationTarget>(StringComparer.Ordinal);

    private IReadOnlyDictionary<string, LTranslationTarget> PEditorTargetRead(LEntryDraft draft)
    {
        List<string> ids = [];
        PEditorTargetRead(draft.LEntryDraftMeanings, ids);
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
            PCard card = new(prefix, draft.LCardDraftPosition, _pEditorCitation, _pEditorParticle, _pEditorDependence)
            {
                PCardId = draft.LCardDraftId
            };

            card.PCardSentenceApply(_pEditorSentenceOrder);
            card.PCardTitleShow(draft.LCardDraftTitle);
            card.PCardExpressionShow(draft.LCardDraftExpression);
            card.PCardDefinitionShow(draft.LCardDraftMeaning);
            card.PCardSentenceShow(draft.LCardDraftExample);
            card.PCardContextShow(draft.LCardDraftSituation);
            card.PCardLinkShow(PCardTargetRead(targets, draft.LCardDraftTranslation));
            PLinkAttach(card);
            PEditorChangeAttach(card);
            card.PCardLabelShow(draft.LCardDraftTag);
            card.PCardImageShow(draft.LCardDraftImage);
            card.PCardVideoShow(draft.LCardDraftVideo);
            cards.Add(card);
        }

        if (cards.Count == 0)
        {
            cards.Add(PCardCreate(prefix, 1));
        }
    }

    private void PEditorNoteShow(string note)
    {
        PNoteContents.Text = note;
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
        PVolumeLoad();
    }

    private void PEditorLanguageShow(string language)
    {
        PEditorExampleShow(language);

        if (language.Length == 0)
        {
            return;
        }

        _pSpeakerEntry = true;
        PHeadwordFontApply(language);

        if (string.Equals(_pSpeakerChoice, language, StringComparison.Ordinal))
        {
            return;
        }

        _pSpeakerChoice = language;
        PSpeakerName.Text = language;
        PSpeakerFlagUpdate();
    }

    private void PEditorExampleShow(string language)
    {
        LFont font = PFont.PFontExampleRead(_lEngine, language);

        if (font.LFontFamily is string named)
        {
            Resources["Theme.Card.ExampleFamily"] = new FontFamily(named);
        }
        else
        {
            Resources.Remove("Theme.Card.ExampleFamily");
        }

        if (font.LFontSize > 0)
        {
            Resources["Theme.Card.ExampleSize"] = font.LFontSize;
        }
        else
        {
            Resources.Remove("Theme.Card.ExampleSize");
        }
    }

    internal void PEditorReset()
    {
        PEditorDraftStart(null);

        _pEditorFill = true;

        PHeadword.Text = string.Empty;
        PPronunciation.Text = string.Empty;
        PMarkerShow(null);
        PRecordingClear();
        _pSpeakerEntry = false;
        PHeadwordFontApply(_pSpeakerChoice);
        PSentenceFrameLoad(_pSpeakerChoice);

        PCardShow(_pMeaningList, "Meaning", [], PEditorTargetEmpty);
        PCardShow(_pCollocationList, "Collocation", [], PEditorTargetEmpty);

        PNoteContents.Text = string.Empty;

        _pEditorFill = false;

        PEditorChangeUpdate();
    }

    private LDraft? PEditorDraftStart(string? entry)
    {
        PEditorChangeStop();
        PEditorDraftCancel();

        try
        {
            LDraft started = _lEngine.LEngineDraftStart(_pEditorOrigin, entry);
            _pEditorDraft = started.LDraftId;
            PEditorHoldResume();
            return started;
        }
        catch (Exception exception)
        {
            _pEditorDraft = string.Empty;
            PEditorHoldSuspend(exception);
            return null;
        }
    }

    private void PEditorHoldSuspend(Exception exception)
    {
        if (_pEditorHalted)
        {
            return;
        }

        _pEditorHalted = true;
        IsEnabled = false;
        _pEditorHost.PWindowFailureShow("Input.HoldFailed", exception);
    }

    private void PEditorHoldResume()
    {
        if (!_pEditorHalted)
        {
            return;
        }

        _pEditorHalted = false;
        IsEnabled = true;
    }

    private void PEditorDraftCancel()
    {
        if (_pEditorDraft.Length == 0)
        {
            return;
        }

        string held = _pEditorDraft;
        _pEditorDraft = string.Empty;

        try
        {
            _lEngine.LEngineDraftCancel(held);
        }
        catch (Exception)
        {
        }
    }

    private void PEditorDraftSave()
    {
        if (_pEditorDraft.Length == 0)
        {
            return;
        }

        try
        {
            LDraft? held = _lEngine.LEngineDraftRead(_pEditorDraft);
            if (held is null)
            {
                return;
            }

            LEntryDraft sent = PEditorDraftRead();
            LEntryDraft stored = _lEngine.LEngineDraftSave(held with { LDraftContent = sent });

            if (!ReferenceEquals(stored, sent))
            {
                PEditorDraftShow(stored);
            }
        }
        catch (Exception exception)
        {
            PEditorHoldSuspend(exception);
        }
    }

    private void PEditorDraftRestore()
    {
        if (_pEditorDraft.Length == 0)
        {
            return;
        }

        try
        {
            if (_lEngine.LEngineDraftRead(_pEditorDraft) is LDraft held)
            {
                PEditorDraftShow(held.LDraftContent);
            }
        }
        catch (Exception exception)
        {
            PEditorHoldSuspend(exception);
        }
    }

    internal bool PEditorDraftFinish(bool store)
    {
        if (_pEditorPending is not null)
        {
            PEditorChangeSave();
        }

        if (!store || !PEditorDraftCheck())
        {
            PEditorDraftCancel();
            return true;
        }

        string held = _pEditorDraft;
        if (held.Length == 0)
        {
            return true;
        }

        _pEditorDraft = string.Empty;

        try
        {
            _lEngine.LEngineDraftCommit(held);
        }
        catch (Exception exception)
        {
            _pEditorDraft = held;
            string? entry = PEditorEntryRead();
            _pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);
            return false;
        }

        return true;
    }

    private bool PEditorDraftCheck()
    {
        if (_pEditorDraft.Length == 0)
        {
            return false;
        }

        try
        {
            return _lEngine.LEngineDraftCheck(_pEditorDraft);
        }
        catch (Exception exception)
        {
            PEditorHoldSuspend(exception);
            return false;
        }
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
                card.PCardContextRead(),
                card.PCardLinkRead(),
                string.Empty,
                card.PCardLabelRead(),
                card.PCardImageRead(),
                card.PCardVideoRead(),
                card.PCardPosition,
                card.PCardId));
        }

        return drafts;
    }

    private string PEditorNoteRead()
    {
        return (PNoteContents.Text ?? string.Empty).TrimEnd('\r', '\n');
    }
}
