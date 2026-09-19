using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

internal sealed class LMarkupLoader
{
    private readonly LDatabase _lMarkupLoaderDatabase;
    private readonly LEntryVault _lMarkupLoaderEntries;

    public LMarkupLoader(LDatabase database, LEntryVault entries)
    {
        ArgumentNullException.ThrowIfNull(database);
        ArgumentNullException.ThrowIfNull(entries);
        _lMarkupLoaderDatabase = database;
        _lMarkupLoaderEntries = entries;
    }

    public LMarkupEntry? LMarkupLoad(long id)
    {
        LEntryDraft? draft = _lMarkupLoaderEntries.LEntryLoad(id);
        if (draft is null)
        {
            return null;
        }

        List<string> speeches = new(draft.LEntryDraftSpeeches.Count);
        foreach (LSpeechDraft speech in draft.LEntryDraftSpeeches)
        {
            speeches.Add(speech.LSpeechDraftName);
        }

        List<LMarkupInflection> inflections = new(draft.LEntryDraftInflections.Count);
        foreach (LInflection inflection in draft.LEntryDraftInflections)
        {
            inflections.Add(LMarkupInflectionCreate(inflection));
        }

        List<LPronunciationDraft> pronunciations = new(draft.LEntryDraftPronunciations.Count);
        foreach (LPronunciationDraft pronunciation in draft.LEntryDraftPronunciations)
        {
            pronunciations.Add(pronunciation with { LPronunciationDraftId = 0, LPronunciationDraftSeeded = false });
        }

        List<LTranscriptionDraft> transcriptions = new(draft.LEntryDraftTranscriptions.Count);
        foreach (LTranscriptionDraft transcription in draft.LEntryDraftTranscriptions)
        {
            transcriptions.Add(transcription with { LTranscriptionDraftId = 0, LTranscriptionDraftSeeded = false });
        }

        List<LReflexDraft> reflexes = new(draft.LEntryDraftReflexes.Count);
        foreach (LReflexDraft reflex in draft.LEntryDraftReflexes)
        {
            reflexes.Add(reflex with
            {
                LReflexDraftId = 0,
                LReflexDraftAnatomy = LAnatomy.LAnatomyEmpty,
                LReflexDraftAnchors = [],
            });
        }

        return new LMarkupEntry(
            draft.LEntryDraftHeadword,
            draft.LEntryDraftLanguage,
            speeches,
            draft.LEntryDraftForms,
            inflections,
            pronunciations,
            transcriptions,
            reflexes,
            LMarkupCardCreate(draft.LEntryDraftMeanings),
            LMarkupCardCreate(draft.LEntryDraftCollocations),
            draft.LEntryDraftNote);
    }

    private LMarkupInflection LMarkupInflectionCreate(LInflection inflection)
    {
        string speech = inflection.LInflectionSpeechId is long speechId
            ? new LSpeechArchive(_lMarkupLoaderDatabase).LSpeechValueRead(speechId)?.LSpeechValueName ?? string.Empty
            : string.Empty;

        LMorphologyArchive morphologies = new(_lMarkupLoaderDatabase);
        List<string> names = new(inflection.LInflectionMorphology.Count);
        foreach (long morphologyId in inflection.LInflectionMorphology)
        {
            if (morphologies.LMorphologyRead(morphologyId) is LMorphology morphology)
            {
                names.Add(morphology.LMorphologyName);
            }
        }

        return new LMarkupInflection(inflection.LInflectionText, inflection.LInflectionLocal, speech, names);
    }

    private IReadOnlyList<LMarkupCard> LMarkupCardCreate(IReadOnlyList<LCardDraft> cards)
    {
        List<LMarkupCard> written = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            List<LMarkupSentence> sentences = new(card.LCardDraftSentence.Count);
            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                sentences.Add(new LMarkupSentence(
                    sentence.LSentenceDraftExample is LExampleDraft example
                        ? LMarkupExampleCreate(example)
                        : null,
                    sentence.LSentenceDraftParticle,
                    sentence.LSentenceDraftDependence));
            }

            List<LSituationDraft> situations = new(card.LCardDraftSituation.Count);
            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                situations.Add(situation with { LSituationDraftId = 0 });
            }

            List<LRegisterDraft> registers = new(card.LCardDraftRegister.Count);
            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                registers.Add(register with { LRegisterDraftId = 0 });
            }

            List<LTagDraft> tags = new(card.LCardDraftTag.Count);
            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                tags.Add(tag with { LTagDraftId = 0 });
            }

            List<LImageDraft> images = new(card.LCardDraftImage.Count);
            foreach (LImageDraft image in card.LCardDraftImage)
            {
                images.Add(image with { LImageDraftId = 0 });
            }

            List<LVideoDraft> videos = new(card.LCardDraftVideo.Count);
            foreach (LVideoDraft video in card.LCardDraftVideo)
            {
                videos.Add(video with { LVideoDraftId = 0 });
            }

            written.Add(new LMarkupCard(
                card.LCardDraftTitle,
                card.LCardDraftExpression,
                card.LCardDraftMeaning,
                sentences,
                situations,
                registers,
                LMarkupTranslationCreate(card.LCardDraftTranslation),
                tags,
                images,
                videos,
                LMarkupCardCreate(card.LCardDraftChild)));
        }

        return written;
    }

    private IReadOnlyList<LMarkupTranslation> LMarkupTranslationCreate(IReadOnlyList<long> ids)
    {
        List<LMarkupTranslation> translations = new(ids.Count);
        foreach (long id in ids)
        {
            if (_lMarkupLoaderEntries.LEntryRead(id) is LEntry entry)
            {
                translations.Add(new LMarkupTranslation(entry.LEntryHeadword, entry.LEntryLanguage));
            }
        }

        return translations;
    }

    private LMarkupExample LMarkupExampleCreate(LExampleDraft example)
    {
        List<LGlossDraft> glosses = new(example.LExampleDraftGloss.Count);
        foreach (LGlossDraft gloss in example.LExampleDraftGloss)
        {
            glosses.Add(gloss with { LGlossDraftId = 0 });
        }

        List<LMarkupMention> mentions = new(example.LExampleDraftMention.Count);
        foreach (LMentionDraft mention in example.LExampleDraftMention)
        {
            mentions.Add(LMarkupMentionCreate(mention));
        }

        return new LMarkupExample(
            example.LExampleDraftText,
            example.LExampleDraftLanguage,
            glosses,
            mentions,
            LMarkupReferenceCreate(example.LExampleDraftReference));
    }

    private LMarkupMention LMarkupMentionCreate(LMentionDraft mention)
    {
        if (mention.LMentionDraftEntry <= 0
            || _lMarkupLoaderEntries.LEntryRead(mention.LMentionDraftEntry) is not LEntry entry)
        {
            return new LMarkupMention(
                mention.LMentionDraftOffset, mention.LMentionDraftLength, string.Empty, string.Empty);
        }

        return new LMarkupMention(
            mention.LMentionDraftOffset,
            mention.LMentionDraftLength,
            entry.LEntryHeadword,
            entry.LEntryLanguage,
            LMarkupMeaningResolve(entry.LEntryId, mention.LMentionDraftSense));
    }

    private string LMarkupMeaningResolve(long entryId, long meaningId)
    {
        if (meaningId <= 0)
        {
            return string.Empty;
        }

        Dictionary<long, LMeaning> meanings = [];
        foreach (LMeaning meaning in new LMeaningArchive(_lMarkupLoaderDatabase).LMeaningRead(entryId))
        {
            meanings[meaning.LMeaningId] = meaning;
        }

        List<string> steps = [];
        long? current = meaningId;
        while (current is long step && meanings.TryGetValue(step, out LMeaning? found))
        {
            steps.Insert(0, (found.LMeaningPosition + 1).ToString(CultureInfo.InvariantCulture));
            current = found.LMeaningParentId;
        }

        return string.Join('.', steps);
    }

    private LMarkupReference? LMarkupReferenceCreate(LStateAnchor anchor)
    {
        if (anchor.LStateAnchorState != LState.LStateSpecified || anchor.LStateAnchorId is not long referenceId)
        {
            return null;
        }

        if (new LReferenceArchive(_lMarkupLoaderDatabase).LReferenceRead(referenceId) is not LReference reference)
        {
            return null;
        }

        IReadOnlyList<LAuthor> credited = new LAuthorArchive(_lMarkupLoaderDatabase).LAuthorReferenceRead(referenceId);
        List<string> authors = new(credited.Count);
        foreach (LAuthor author in credited)
        {
            authors.Add(author.LAuthorName);
        }

        return new LMarkupReference(
            reference.LReferenceTitle,
            reference.LReferenceYear,
            reference.LReferenceKind,
            reference.LReferenceUrl,
            reference.LReferenceNote,
            authors);
    }
}
