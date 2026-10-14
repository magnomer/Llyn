using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LStencilReader
{
    private readonly LTranslationClerk _lStencilReaderTranslations;
    private readonly LReferenceClerk _lStencilReaderReferences;
    private readonly LTrailClerk _lStencilReaderTrail;
    private readonly LPortraitLabel _lStencilReaderLabel;

    public LStencilReader(
        LTranslationClerk translations,
        LReferenceClerk references,
        LTrailClerk trail,
        LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(translations);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(trail);
        ArgumentNullException.ThrowIfNull(label);
        _lStencilReaderTranslations = translations;
        _lStencilReaderReferences = references;
        _lStencilReaderTrail = trail;
        _lStencilReaderLabel = label;
    }

    public IReadOnlyList<string> LStencilReaderRead(LEntryDraft draft, LCue cue, LStencilField field)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(cue);

        string mark = _lStencilReaderLabel.LPortraitLabelUnknown;
        IReadOnlyList<LCardDraft> meanings = LStencilCardScan(draft.LEntryDraftMeanings, cue, cue.LCueMeaningId);
        IReadOnlyList<LCardDraft> collocations =
            LStencilCardScan(draft.LEntryDraftCollocations, cue, cue.LCueCollocationId);
        IReadOnlyList<LCardDraft> cards = [.. meanings, .. collocations];

        return field switch
        {
            LStencilField.LStencilFieldHeadword => LStencilTextScan([draft.LEntryDraftHeadword]),
            LStencilField.LStencilFieldForms =>
                LStencilTextScan(draft.LEntryDraftForms.Select(static form => form.LFormText)),
            LStencilField.LStencilFieldPronunciation => LStencilTextScan(
                draft.LEntryDraftPronunciations.Select(static spoken => spoken.LPronunciationDraftIpa)),
            LStencilField.LStencilFieldRespelling => LStencilTextScan(
                draft.LEntryDraftPronunciations.Select(static spoken => spoken.LPronunciationDraftRespelling)),
            LStencilField.LStencilFieldRecording => LStencilTextScan(
                draft.LEntryDraftPronunciations.Select(static spoken => spoken.LPronunciationDraftAudio)),
            LStencilField.LStencilFieldSpeech => LVocabularyClerk.LSpeechShow(draft.LEntryDraftSpeeches),
            LStencilField.LStencilFieldUnit => LStencilTextScan(
                [_lStencilReaderLabel.LPortraitLabelUnit.GetValueOrDefault(draft.LEntryDraftUnit, string.Empty)]),
            LStencilField.LStencilFieldFreedom => LStencilTextScan(
                [_lStencilReaderLabel.LPortraitLabelFreedom.GetValueOrDefault(draft.LEntryDraftFreedom, string.Empty)]),
            LStencilField.LStencilFieldDefinition => LStencilTextScan(
                meanings.Select(card => LPortraitText.LPortraitTextRead(card.LCardDraftMeaning, mark))),
            LStencilField.LStencilFieldExpression => LStencilTextScan(
                collocations.Select(card => LPortraitText.LPortraitTextRead(card.LCardDraftExpression, mark))),
            LStencilField.LStencilFieldParaphrase => LStencilTextScan(
                collocations.Select(card => LPortraitText.LPortraitTextRead(card.LCardDraftMeaning, mark))),
            LStencilField.LStencilFieldSentence => LStencilTextScan(LStencilExampleScan(cards, cue)
                .Select(example => LPortraitText.LPortraitTextRead(example.LExampleDraftText, mark))),
            LStencilField.LStencilFieldGloss => LStencilTextScan(LStencilExampleScan(cards, cue)
                .SelectMany(static example => example.LExampleDraftGloss)
                .Select(gloss => LPortraitText.LPortraitTextRead(gloss.LGlossDraftText, mark))),
            LStencilField.LStencilFieldTarget => LStencilTargetScan(cards, cue),
            LStencilField.LStencilFieldImage => LStencilImageScan(cards),
            LStencilField.LStencilFieldTag => LStencilTextScan(cards
                .SelectMany(static card => card.LCardDraftTag)
                .Select(static tag => tag.LTagDraftText)
                .Distinct(StringComparer.Ordinal)),
            LStencilField.LStencilFieldRegister => LStencilTextScan(cards
                .SelectMany(static card => card.LCardDraftRegister)
                .Select(register => LPortraitText.LPortraitTextRead(register.LRegisterDraftName, mark))),
            LStencilField.LStencilFieldSituation => LStencilTextScan(cards
                .SelectMany(static card => card.LCardDraftSituation)
                .Select(situation => LPortraitText.LPortraitTextRead(situation.LSituationDraftTitle, mark))),
            LStencilField.LStencilFieldSource => LStencilSourceScan(LStencilExampleScan(cards, cue), mark),
            _ => [],
        };
    }

    private static IReadOnlyList<LCardDraft> LStencilCardScan(IReadOnlyList<LCardDraft> roots, LCue cue, long? cardId)
    {
        List<LCardDraft> cards = [];
        Stack<LCardDraft> pending = new(roots.Reverse());
        while (pending.Count > 0)
        {
            LCardDraft card = pending.Pop();
            cards.Add(card);
            for (int place = card.LCardDraftChild.Count - 1; place >= 0; place--)
            {
                pending.Push(card.LCardDraftChild[place]);
            }
        }

        return cue.LCueGrain switch
        {
            LGrain.LGrainEntry => cards,
            LGrain.LGrainExample => cards
                .Where(card => card.LCardDraftSentence.Any(
                    sentence => sentence.LSentenceDraftExample?.LExampleDraftId == cue.LCueExampleId))
                .ToList(),
            LGrain.LGrainMeaning or LGrain.LGrainCollocation or LGrain.LGrainPair =>
                cards.Where(card => card.LCardDraftId == cardId).ToList(),
            _ => [],
        };
    }

    private static IReadOnlyList<LExampleDraft> LStencilExampleScan(IReadOnlyList<LCardDraft> cards, LCue cue)
    {
        List<LExampleDraft> examples = [];
        foreach (LCardDraft card in cards)
        {
            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                if (sentence.LSentenceDraftExample is LExampleDraft example
                    && (cue.LCueGrain != LGrain.LGrainExample || example.LExampleDraftId == cue.LCueExampleId))
                {
                    examples.Add(example);
                }
            }
        }

        return examples;
    }

    private IReadOnlyList<string> LStencilTargetScan(IReadOnlyList<LCardDraft> cards, LCue cue)
    {
        List<long> ids = cards
            .SelectMany(static card => card.LCardDraftTranslation)
            .Where(static id => id > 0)
            .Distinct()
            .ToList();
        if (cue.LCueGrain == LGrain.LGrainPair)
        {
            ids = cue.LCueTargetId is long target && ids.Contains(target) ? [target] : [];
        }

        if (ids.Count == 0)
        {
            return [];
        }

        Dictionary<long, string> headwords = [];
        foreach (LTranslationTarget target in _lStencilReaderTranslations.LTranslationTargetRead(ids))
        {
            headwords[target.LTranslationTargetId] = target.LTranslationTargetHeadword;
        }

        return LStencilTextScan(ids.Select(id => headwords.GetValueOrDefault(id, string.Empty)));
    }

    private IReadOnlyList<string> LStencilImageScan(IReadOnlyList<LCardDraft> cards)
    {
        List<string> images = [];
        foreach (LImageDraft image in cards.SelectMany(static card => card.LCardDraftImage))
        {
            if (_lStencilReaderTrail.LTrailClerkResolve(image.LImageDraftLocation.LStateValueShow()) is Uri address)
            {
                images.Add(address.IsFile ? address.LocalPath : address.OriginalString);
            }
        }

        return images;
    }

    private IReadOnlyList<string> LStencilSourceScan(IReadOnlyList<LExampleDraft> examples, string mark)
    {
        if (examples.Count == 0)
        {
            return [];
        }

        IReadOnlyDictionary<long, string> citations = _lStencilReaderReferences.LCitationRead();
        List<string> sources = [];
        foreach (LExampleDraft example in examples)
        {
            LStateAnchor reference = example.LExampleDraftReference;
            if (reference.LStateAnchorState == LState.LStateSpecified
                && citations.TryGetValue(reference.LStateAnchorShow(), out string? source))
            {
                sources.Add(source);
            }
            else if (reference.LStateAnchorState == LState.LStateUnknown)
            {
                sources.Add(mark);
            }
        }

        return LStencilTextScan(sources);
    }

    private static IReadOnlyList<string> LStencilTextScan(IEnumerable<string> texts)
    {
        return texts.Where(static text => text.Length > 0).ToList();
    }
}
