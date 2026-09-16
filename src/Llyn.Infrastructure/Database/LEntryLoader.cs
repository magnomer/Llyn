using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LEntryLoader
{
    private readonly LDatabase _lEntryLoaderDatabase;

    public LEntryLoader(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lEntryLoaderDatabase = database;
    }

    public LEntryDraft? LEntryLoad(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lEntryLoaderDatabase.LDatabaseSessionStart();

        LEntry? entry = new LEntryArchive(_lEntryLoaderDatabase).LEntryRead(id);
        if (entry is null)
        {
            return null;
        }

        LEntryArchive entries = new(_lEntryLoaderDatabase);
        IReadOnlyList<LSpeechDraft> speeches = LEntrySpeechFormat(entries.LEntrySpeechRead(id));
        IReadOnlyList<LForm> forms = entries.LEntryFormRead(id);
        IReadOnlyList<LInflection> inflections =
            new LInflectionArchive(_lEntryLoaderDatabase).LInflectionRead(id);

        LNote? note = new LNoteArchive(_lEntryLoaderDatabase).LNoteRead(id);
        IReadOnlyList<LPronunciationDraft> pronunciations = LEntrySoundRead(id);
        IReadOnlyList<LTranscriptionDraft> transcriptions = LEntrySpellingRead(id);
        IReadOnlyList<LReflexDraft> reflexes = LEntryReflexRead(id);

        Dictionary<long, List<LMeaning>> senses = [];
        foreach (LMeaning meaning in new LMeaningArchive(_lEntryLoaderDatabase).LMeaningRead(id))
        {
            long parent = meaning.LMeaningParentId ?? 0;
            if (!senses.TryGetValue(parent, out List<LMeaning>? group))
            {
                group = [];
                senses[parent] = group;
            }

            group.Add(meaning);
        }

        IReadOnlyList<LCardDraft> meaningCards = LEntryChildRead(senses, 0);

        List<LCardDraft> collocationCards = [];
        foreach (LCollocation collocation in new LCollocationArchive(_lEntryLoaderDatabase).LCollocationRead(id))
        {
            collocationCards.Add(LEntryCardRead(
                collocation.LCollocationId,
                collocation: true,
                collocation.LCollocationPosition + 1,
                collocation.LCollocationTitle,
                collocation.LCollocationExpression,
                collocation.LCollocationMeaning));
        }

        return new LEntryDraft(
            entry.LEntryHeadword,
            entry.LEntryLanguage,
            pronunciations,
            note?.LNoteText ?? string.Empty,
            meaningCards,
            collocationCards,
            speeches,
            forms,
            inflections,
            transcriptions,
            reflexes);
    }

    private IReadOnlyList<LCardDraft> LEntryChildRead(
        IReadOnlyDictionary<long, List<LMeaning>> senses, long parentId)
    {
        if (!senses.TryGetValue(parentId, out List<LMeaning>? group))
        {
            return [];
        }

        List<LCardDraft> cards = new(group.Count);
        foreach (LMeaning meaning in group)
        {
            cards.Add(LEntryCardRead(
                meaning.LMeaningId,
                collocation: false,
                meaning.LMeaningPosition + 1,
                meaning.LMeaningTitle,
                LStateValue.LStateValueUnspecified,
                meaning.LMeaningDefinition) with
            {
                LCardDraftChild = LEntryChildRead(senses, meaning.LMeaningId),
            });
        }

        return cards;
    }

    private IReadOnlyList<LPronunciationDraft> LEntrySoundRead(long id)
    {
        LPronunciationArchive pronunciations = new(_lEntryLoaderDatabase);
        List<LPronunciationDraft> drafts = [];
        foreach (LPronunciation pronunciation in pronunciations.LPronunciationRead(id))
        {
            LPronunciationAudio? audio = pronunciations.LPronunciationAudioRead(pronunciation.LPronunciationId);
            drafts.Add(new LPronunciationDraft(
                pronunciation.LPronunciationIpa ?? string.Empty,
                pronunciation.LPronunciationSyllables,
                audio?.LPronunciationAudioFile ?? string.Empty,
                audio?.LPronunciationAudioSource,
                pronunciation.LPronunciationId,
                pronunciation.LPronunciationVariety ?? string.Empty,
                LPronunciationDraftRespelling: pronunciation.LPronunciationRespelling ?? string.Empty));
        }

        return drafts;
    }

    private IReadOnlyList<LTranscriptionDraft> LEntrySpellingRead(long id)
    {
        List<LTranscriptionDraft> drafts = [];
        LTranscriptionArchive transcriptions = new(_lEntryLoaderDatabase);
        foreach (LTranscription transcription in transcriptions.LTranscriptionRead(id))
        {
            drafts.Add(new LTranscriptionDraft(
                transcription.LTranscriptionScheme,
                transcription.LTranscriptionText,
                transcription.LTranscriptionId));
        }

        return drafts;
    }

    private IReadOnlyList<LReflexDraft> LEntryReflexRead(long id)
    {
        List<LReflexDraft> drafts = [];
        foreach (LReflex reflex in new LReflexArchive(_lEntryLoaderDatabase).LReflexRead(id))
        {
            drafts.Add(new LReflexDraft(
                reflex.LReflexLanguage,
                reflex.LReflexKind,
                reflex.LReflexText,
                reflex.LReflexMain,
                reflex.LReflexId,
                reflex.LReflexNote,
                reflex.LReflexRespelling,
                reflex.LReflexRegion,
                reflex.LReflexRemark));
        }

        return drafts;
    }

    private IReadOnlyList<LSpeechDraft> LEntrySpeechFormat(IReadOnlyList<LSpeech> speeches)
    {
        if (speeches.Count == 0)
        {
            return [];
        }

        LSpeechArchive values = new(_lEntryLoaderDatabase);
        List<LSpeechDraft> named = new(speeches.Count);
        foreach (LSpeech speech in speeches)
        {
            if (speech.LSpeechValueId is not long valueId)
            {
                named.Add(new LSpeechDraft(0, speech.LSpeechCustom, speech.LSpeechCustom ?? string.Empty));
                continue;
            }

            string shown = values.LSpeechValueRead(valueId)?.LSpeechValueName ?? string.Empty;
            named.Add(new LSpeechDraft(valueId, null, shown));
        }

        return named;
    }

    private LCardDraft LEntryCardRead(
        long ownerId,
        bool collocation,
        int position,
        LStateValue title,
        LStateValue expression,
        LStateValue meaning)
    {
        LSituationArchive situations = new(_lEntryLoaderDatabase);
        LRegisterArchive registers = new(_lEntryLoaderDatabase);
        LTagArchive tags = new(_lEntryLoaderDatabase);
        LTranslationArchive translations = new(_lEntryLoaderDatabase);
        LImageArchive images = new(_lEntryLoaderDatabase);
        LVideoArchive videos = new(_lEntryLoaderDatabase);
        LSentenceArchive sentences = new(_lEntryLoaderDatabase);

        return new LCardDraft(
            title,
            expression,
            meaning,
            LEntrySentenceRead(
                collocation
                    ? sentences.LSentenceCollocationRead(ownerId)
                    : sentences.LSentenceMeaningRead(ownerId)),
            LEntrySituationRead(
                collocation
                    ? situations.LSituationCollocationRead(ownerId)
                    : situations.LSituationMeaningRead(ownerId)),
            LEntryRegisterRead(
                collocation ? registers.LRegisterCollocationRead(ownerId) : registers.LRegisterMeaningRead(ownerId)),
            LEntryTranslationRead(
                collocation
                    ? translations.LTranslationCollocationRead(ownerId)
                    : translations.LTranslationMeaningRead(ownerId)),
            LEntryTagRead(
                collocation ? tags.LTagCollocationRead(ownerId) : tags.LTagMeaningRead(ownerId)),
            LEntryImageRead(
                collocation ? images.LImageCollocationRead(ownerId) : images.LImageMeaningRead(ownerId)),
            LEntryVideoRead(
                collocation ? videos.LVideoCollocationRead(ownerId) : videos.LVideoMeaningRead(ownerId)),
            position,
            ownerId);
    }

    private static IReadOnlyList<LSentenceDraft> LEntrySentenceRead(IReadOnlyList<LSentence> sentences)
    {
        List<LSentenceDraft> drafts = new(sentences.Count);
        foreach (LSentence sentence in sentences)
        {
            drafts.Add(new LSentenceDraft(
                LEntryExampleRead(sentence.LSentenceExample),
                sentence.LSentenceParticle,
                sentence.LSentenceDependence,
                sentence.LSentenceId));
        }

        return drafts;
    }

    private static LExampleDraft? LEntryExampleRead(LExample? example)
    {
        return example is null ? null : LExampleDraft.LExampleDraftCreate(example);
    }

    private static IReadOnlyList<LSituationDraft> LEntrySituationRead(IReadOnlyList<LSituation> situations)
    {
        List<LSituationDraft> drafts = new(situations.Count);
        foreach (LSituation situation in situations)
        {
            drafts.Add(new LSituationDraft(
                situation.LSituationTitle,
                situation.LSituationId,
                situation.LSituationDescription,
                situation.LSituationKind));
        }

        return drafts;
    }

    private static IReadOnlyList<LRegisterDraft> LEntryRegisterRead(IReadOnlyList<LRegister> registers)
    {
        List<LRegisterDraft> drafts = new(registers.Count);
        foreach (LRegister register in registers)
        {
            drafts.Add(new LRegisterDraft(register.LRegisterName, register.LRegisterId));
        }

        return drafts;
    }

    private static IReadOnlyList<LImageDraft> LEntryImageRead(IReadOnlyList<LImage> images)
    {
        List<LImageDraft> rows = new(images.Count);
        foreach (LImage image in images)
        {
            rows.Add(new LImageDraft(image.LImageLocation, image.LImageId));
        }

        return rows;
    }

    private static IReadOnlyList<LVideoDraft> LEntryVideoRead(IReadOnlyList<LVideo> videos)
    {
        List<LVideoDraft> rows = new(videos.Count);
        foreach (LVideo video in videos)
        {
            rows.Add(new LVideoDraft(video.LVideoLocation, video.LVideoSpan, video.LVideoId));
        }

        return rows;
    }

    private static IReadOnlyList<long> LEntryTranslationRead(
        IReadOnlyList<LTranslation> translations)
    {
        List<long> ids = new(translations.Count);
        foreach (LTranslation translation in translations)
        {
            ids.Add(translation.LTranslationEntryId);
        }

        return ids;
    }

    private static IReadOnlyList<LTagDraft> LEntryTagRead(IReadOnlyList<LTag> tags)
    {
        List<LTagDraft> drafts = new(tags.Count);
        foreach (LTag tag in tags)
        {
            drafts.Add(new LTagDraft(tag.LTagId, tag.LTagText));
        }

        return drafts;
    }
}
