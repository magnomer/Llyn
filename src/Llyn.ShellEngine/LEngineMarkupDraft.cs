using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LEntryDraft LEngineMarkupResolve(
        LMarkupEntry entry,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions,
        List<(LMarkupMention, int)> held)
    {
        string language = entry.LMarkupEntryLanguage;
        LSpeechVault values = _lEngineSpeeches;

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
            inflections.Add(LEngineMarkupResolve(values, language, inflection, inflections.Count, omissions));
        }

        List<LPronunciationDraft> pronunciations = new(entry.LMarkupEntryPronunciation.Count);
        foreach (LPronunciationDraft pronunciation in entry.LMarkupEntryPronunciation)
        {
            pronunciations.Add(LEngineMarkupResolve(pronunciation.LPronunciationDraftAudio, omissions)
                ? pronunciation
                : pronunciation with { LPronunciationDraftAudio = string.Empty });
        }

        return new LEntryDraft(
            entry.LMarkupEntryHeadword,
            language,
            pronunciations,
            entry.LMarkupEntryNote,
            LEngineMarkupResolve(entry.LMarkupEntryMeaning, entry, prepared, omissions, held),
            LEngineMarkupResolve(entry.LMarkupEntryCollocation, entry, prepared, omissions, held),
            speeches,
            entry.LMarkupEntryForm,
            inflections,
            entry.LMarkupEntryTranscription,
            entry.LMarkupEntryReflex);
    }

    private static LSpeechValue? LEngineMarkupResolve(
        LSpeechVault values, string language, string name, List<LMarkupOmission> omissions)
    {
        LSpeechValue? value = string.IsNullOrWhiteSpace(language) ? null : values.LSpeechValueFind(language, name);
        if (value is null)
        {
            omissions.Add(new LMarkupOmission(0, $"speech \"{name}\""));
        }

        return value;
    }

    private LInflection LEngineMarkupResolve(
        LSpeechVault values,
        string language,
        LMarkupInflection inflection,
        int position,
        List<LMarkupOmission> omissions)
    {
        LSpeechValue? speech = inflection.LMarkupInflectionSpeech.Length == 0
            ? null
            : LEngineMarkupResolve(values, language, inflection.LMarkupInflectionSpeech, omissions);

        LMorphologyVault morphologies = _lEngineMorphologies;
        IReadOnlyList<LFeature> features = speech is null ? [] : morphologies.LFeatureRead(speech.LSpeechValueId);

        List<long> resolved = new(inflection.LMarkupInflectionMorphology.Count);
        foreach (string name in inflection.LMarkupInflectionMorphology)
        {
            LMorphology? found = null;
            foreach (LFeature feature in features)
            {
                found = morphologies.LMorphologyFind(feature.LFeatureId, name);
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

    private IReadOnlyList<LCardDraft> LEngineMarkupResolve(
        IReadOnlyList<LMarkupCard> cards,
        LMarkupEntry entry,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions,
        List<(LMarkupMention, int)> held)
    {
        List<LCardDraft> drafts = new(cards.Count);
        foreach (LMarkupCard card in cards)
        {
            List<LSentenceDraft> sentences = new(card.LMarkupCardSentence.Count);
            foreach (LMarkupSentence sentence in card.LMarkupCardSentence)
            {
                sentences.Add(new LSentenceDraft(
                    sentence.LMarkupSentenceExample is LMarkupExample example
                        ? LEngineMarkupResolve(example, entry.LMarkupEntryLine, prepared, omissions, held)
                        : null,
                    sentence.LMarkupSentenceParticle,
                    sentence.LMarkupSentenceDependence));
            }

            List<long> translations = new(card.LMarkupCardTranslation.Count);
            foreach (LMarkupTranslation translation in card.LMarkupCardTranslation)
            {
                long target = LEngineMarkupResolve(
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
                if (LEngineMarkupResolve(image.LImageDraftLocation.LStateValueShow(), omissions))
                {
                    images.Add(image);
                }
            }

            List<LVideoDraft> videos = new(card.LMarkupCardVideo.Count);
            foreach (LVideoDraft video in card.LMarkupCardVideo)
            {
                if (LEngineMarkupResolve(video.LVideoDraftLocation.LStateValueShow(), omissions))
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
                LEngineMarkupResolve(card.LMarkupCardChild, entry, prepared, omissions, held)));
        }

        return drafts;
    }
}
