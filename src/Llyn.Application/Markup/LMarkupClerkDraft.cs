using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LMarkupClerkDraft
{
    private readonly LSpeechVault _lMarkupDraftSpeeches;
    private readonly LMorphologyVault _lMarkupDraftMorphologies;
    private readonly LMarkupClerkLink _lMarkupDraftLink;

    public LMarkupClerkDraft(LRig rig, LMarkupClerkLink link)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(link);
        _lMarkupDraftSpeeches = rig.LRigLexicon.LRigLexiconSpeeches;
        _lMarkupDraftMorphologies = rig.LRigLexicon.LRigLexiconMorphologies;
        _lMarkupDraftLink = link;
    }

    public LEntryDraft LMarkupDraftResolve(
        LMarkupEntry entry,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions,
        List<(LMarkupMention, int, long)> held,
        List<(LMarkupReference, long)> cited)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(prepared);
        ArgumentNullException.ThrowIfNull(omissions);
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(cited);

        string language = entry.LMarkupEntryLanguage;
        LSpeechVault values = _lMarkupDraftSpeeches;

        List<LSpeechDraft> speeches = new(entry.LMarkupEntrySpeech.Count);
        foreach (string name in entry.LMarkupEntrySpeech)
        {
            LSpeechValue? value = string.IsNullOrWhiteSpace(language) ? null : values.LSpeechValueFind(language, name);
            speeches.Add(value is null
                ? LSpeechDraft.LSpeechDraftCreate(name)
                : LSpeechDraft.LSpeechDraftCreate(value.LSpeechValueId, value.LSpeechValueName));
        }

        List<LInflection> inflections = new(entry.LMarkupEntryInflection.Count);
        foreach (LMarkupInflection inflection in entry.LMarkupEntryInflection)
        {
            inflections.Add(LMarkupInflectionResolve(language, inflection, inflections.Count, omissions));
        }

        List<LPronunciationDraft> pronunciations = new(entry.LMarkupEntryPronunciation.Count);
        foreach (LPronunciationDraft pronunciation in entry.LMarkupEntryPronunciation)
        {
            pronunciations.Add(
                _lMarkupDraftLink.LMarkupLocationResolve(pronunciation.LPronunciationDraftAudio, omissions)
                    ? pronunciation
                    : pronunciation with { LPronunciationDraftAudio = string.Empty });
        }

        IReadOnlyList<LCardDraft> meanings =
            LMarkupCardResolve(entry.LMarkupEntryMeaning, entry, prepared, omissions, held, cited);
        IReadOnlyList<LCardDraft> collocations =
            LMarkupCardResolve(entry.LMarkupEntryCollocation, entry, prepared, omissions, held, cited);
        if (entry.LMarkupEntryCollocation.Any(static card => card.LMarkupCardSense.Count > 0))
        {
            long next = -(held.Count + cited.Count + 1);
            meanings = LMarkupCardPrepare(meanings, ref next);
            List<LCardDraft> linked = new(collocations.Count);
            for (int index = 0; index < collocations.Count; index++)
            {
                LMarkupCard card = entry.LMarkupEntryCollocation[index];
                List<long> senses = new(card.LMarkupCardSense.Count);
                foreach (string path in card.LMarkupCardSense)
                {
                    long senseId = LMeaning.LMeaningPathFind(meanings, path);
                    if (senseId == 0)
                    {
                        omissions.Add(new LMarkupOmission(
                            0, $"sense \"{path}\" of \"{card.LMarkupCardExpression.LStateValueShow()}\""));
                        continue;
                    }

                    if (!senses.Contains(senseId))
                    {
                        senses.Add(senseId);
                    }
                }

                linked.Add(collocations[index] with { LCardDraftSense = senses });
            }

            collocations = linked;
        }

        return new LEntryDraft(
            entry.LMarkupEntryHeadword,
            language,
            pronunciations,
            entry.LMarkupEntryNote,
            meanings,
            collocations,
            speeches,
            entry.LMarkupEntryForm,
            inflections,
            entry.LMarkupEntryTranscription,
            entry.LMarkupEntryReflex,
            _lMarkupDraftLink.LMarkupEtymologyResolve(entry, prepared, omissions),
            entry.LMarkupEntryUnit,
            entry.LMarkupEntryFreedom);
    }

    private LSpeechValue? LMarkupSpeechResolve(string language, string name, List<LMarkupOmission> omissions)
    {
        LSpeechValue? value = string.IsNullOrWhiteSpace(language)
            ? null
            : _lMarkupDraftSpeeches.LSpeechValueFind(language, name.Trim());
        if (value is null)
        {
            omissions.Add(new LMarkupOmission(0, $"speech \"{name}\""));
        }

        return value;
    }

    private LInflection LMarkupInflectionResolve(
        string language, LMarkupInflection inflection, int position, List<LMarkupOmission> omissions)
    {
        LSpeechValue? speech = inflection.LMarkupInflectionSpeech.Length == 0
            ? null
            : LMarkupSpeechResolve(language, inflection.LMarkupInflectionSpeech, omissions);

        LMorphologyVault morphologies = _lMarkupDraftMorphologies;
        IReadOnlyList<LFeature> features = speech is null ? [] : morphologies.LFeatureRead(speech.LSpeechValueId);

        List<long> resolved = new(inflection.LMarkupInflectionMorphology.Count);
        foreach (string name in inflection.LMarkupInflectionMorphology)
        {
            string typed = name.Trim();
            LMorphology? found = null;
            foreach (LFeature feature in features)
            {
                found = morphologies.LMorphologyFind(feature.LFeatureId, typed);
                if (found is not null)
                {
                    break;
                }
            }

            if (found is null)
            {
                omissions.Add(new LMarkupOmission(0, $"morphology \"{name}\""));
                continue;
            }

            resolved.Add(found.LMorphologyId);
        }

        return new LInflection(
            0,
            0,
            position,
            inflection.LMarkupInflectionText,
            inflection.LMarkupInflectionLocal,
            speech?.LSpeechValueId,
            resolved);
    }

    private IReadOnlyList<LCardDraft> LMarkupCardResolve(
        IReadOnlyList<LMarkupCard> cards,
        LMarkupEntry entry,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions,
        List<(LMarkupMention, int, long)> held,
        List<(LMarkupReference, long)> cited)
    {
        LMarkupClerkLink link = _lMarkupDraftLink;
        List<LCardDraft> drafts = new(cards.Count);
        foreach (LMarkupCard card in cards)
        {
            List<LSentenceDraft> sentences = new(card.LMarkupCardSentence.Count);
            foreach (LMarkupSentence sentence in card.LMarkupCardSentence)
            {
                sentences.Add(new LSentenceDraft(
                    sentence.LMarkupSentenceExample is LMarkupExample example
                        ? link.LMarkupExampleResolve(example, entry.LMarkupEntryLine, prepared, omissions, held, cited)
                        : null,
                    sentence.LMarkupSentenceParticle,
                    sentence.LMarkupSentenceDependence));
            }

            List<long> translations = new(card.LMarkupCardTranslation.Count);
            foreach (LMarkupTranslation translation in card.LMarkupCardTranslation)
            {
                long target = link.LMarkupEntryResolve(
                    translation.LMarkupTranslationHeadword, translation.LMarkupTranslationLanguage, prepared);
                if (target == 0)
                {
                    omissions.Add(new LMarkupOmission(0, $"translation \"{translation.LMarkupTranslationHeadword}\""));
                    continue;
                }

                translations.Add(target);
            }

            List<LImageDraft> images = new(card.LMarkupCardImage.Count);
            foreach (LImageDraft image in card.LMarkupCardImage)
            {
                if (link.LMarkupLocationResolve(image.LImageDraftLocation.LStateValueShow(), omissions))
                {
                    images.Add(image);
                }
            }

            List<LVideoDraft> videos = new(card.LMarkupCardVideo.Count);
            foreach (LVideoDraft video in card.LMarkupCardVideo)
            {
                if (link.LMarkupLocationResolve(video.LVideoDraftLocation.LStateValueShow(), omissions))
                {
                    videos.Add(video);
                }
            }

            drafts.Add(new LCardDraft(
                card.LMarkupCardTitle,
                card.LMarkupCardExpression,
                card.LMarkupCardMeaning,
                sentences,
                card.LMarkupCardSituation,
                card.LMarkupCardRegister,
                translations,
                card.LMarkupCardTag,
                images,
                videos,
                drafts.Count + 1,
                0,
                LMarkupCardResolve(card.LMarkupCardChild, entry, prepared, omissions, held, cited)));
        }

        return drafts;
    }

    private static IReadOnlyList<LCardDraft> LMarkupCardPrepare(IReadOnlyList<LCardDraft> cards, ref long next)
    {
        List<LCardDraft> numbered = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            long id = next--;
            numbered.Add(card with
            {
                LCardDraftId = id,
                LCardDraftChild = LMarkupCardPrepare(card.LCardDraftChild, ref next),
            });
        }

        return numbered;
    }
}
