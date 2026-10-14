using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Tests;

internal static class TExemplarText
{
    internal static IReadOnlyList<string> TExemplarTextRead(LEntryDraft draft)
    {
        List<string> texts = [draft.LEntryDraftHeadword, draft.LEntryDraftLanguage, draft.LEntryDraftNote];

        foreach (LSpeechDraft speech in draft.LEntryDraftSpeeches)
        {
            texts.Add(speech.LSpeechDraftName);
        }

        foreach (LForm form in draft.LEntryDraftForms)
        {
            TExemplarTextAdd(texts, form.LFormText, form.LFormLocal, form.LFormRole);
        }

        foreach (LInflection inflection in draft.LEntryDraftInflections)
        {
            TExemplarTextAdd(texts, inflection.LInflectionText, inflection.LInflectionLocal);
        }

        foreach (LAncestorDraft box in draft.LEntryDraftAncestors)
        {
            TExemplarTextAdd(texts, box.LAncestorDraftText, box.LAncestorDraftLanguage, box.LAncestorDraftGloss);
        }

        foreach (LPronunciationDraft pronunciation in draft.LEntryDraftPronunciations)
        {
            TExemplarTextAdd(
                texts,
                pronunciation.LPronunciationDraftIpa,
                pronunciation.LPronunciationDraftRespelling,
                pronunciation.LPronunciationDraftVariety,
                pronunciation.LPronunciationDraftAudio,
                pronunciation.LPronunciationDraftSource);
            foreach (LSyllable syllable in pronunciation.LPronunciationDraftSyllables)
            {
                TExemplarTextAdd(
                    texts,
                    syllable.LSyllableOnset,
                    syllable.LSyllableMedial,
                    syllable.LSyllableNucleus,
                    syllable.LSyllableCoda,
                    syllable.LSyllableTonePoints);
            }
        }

        foreach (LTranscriptionDraft transcription in draft.LEntryDraftTranscriptions)
        {
            TExemplarTextAdd(texts, transcription.LTranscriptionDraftScheme, transcription.LTranscriptionDraftText);
        }

        foreach (LReflexDraft reflex in draft.LEntryDraftReflexes)
        {
            TExemplarTextAdd(
                texts,
                reflex.LReflexDraftLanguage,
                reflex.LReflexDraftKind,
                reflex.LReflexDraftText,
                reflex.LReflexDraftRespelling,
                reflex.LReflexDraftRomanization,
                reflex.LReflexDraftMeaning,
                reflex.LReflexDraftNote,
                reflex.LReflexDraftRegion);
        }

        TExemplarTextAdd(texts, draft.LEntryDraftMeanings);
        TExemplarTextAdd(texts, draft.LEntryDraftCollocations);
        return texts;
    }

    private static void TExemplarTextAdd(List<string> texts, IReadOnlyList<LCardDraft> cards)
    {
        foreach (LCardDraft card in cards)
        {
            TExemplarTextAdd(texts, card.LCardDraftTitle, card.LCardDraftExpression, card.LCardDraftMeaning);

            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                TExemplarTextAdd(texts, sentence.LSentenceDraftParticle, sentence.LSentenceDraftDependence);
                if (sentence.LSentenceDraftExample is not LExampleDraft example)
                {
                    continue;
                }

                TExemplarTextAdd(texts, example.LExampleDraftText);
                TExemplarTextAdd(texts, example.LExampleDraftLanguage);
                foreach (LGlossDraft gloss in example.LExampleDraftGloss)
                {
                    TExemplarTextAdd(texts, gloss.LGlossDraftLanguage);
                    TExemplarTextAdd(texts, gloss.LGlossDraftText);
                }
            }

            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                TExemplarTextAdd(
                    texts,
                    situation.LSituationDraftTitle,
                    situation.LSituationDraftDescription,
                    situation.LSituationDraftKind);
            }

            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                TExemplarTextAdd(texts, register.LRegisterDraftName);
            }

            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                TExemplarTextAdd(texts, tag.LTagDraftText);
            }

            foreach (LImageDraft image in card.LCardDraftImage)
            {
                TExemplarTextAdd(texts, image.LImageDraftLocation);
            }

            foreach (LVideoDraft video in card.LCardDraftVideo)
            {
                TExemplarTextAdd(texts, video.LVideoDraftLocation, video.LVideoDraftSpan);
            }

            TExemplarTextAdd(texts, card.LCardDraftChild);
        }
    }

    private static void TExemplarTextAdd(List<string> texts, params string?[] values)
    {
        foreach (string? value in values)
        {
            if (!string.IsNullOrEmpty(value))
            {
                texts.Add(value);
            }
        }
    }

    private static void TExemplarTextAdd(List<string> texts, params LStateValue[] values)
    {
        foreach (LStateValue value in values)
        {
            if (value.LStateValueState == LState.LStateSpecified)
            {
                texts.Add(value.LStateValueShow());
            }
        }
    }
}
