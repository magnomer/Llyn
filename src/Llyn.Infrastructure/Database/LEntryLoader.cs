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
        LPronunciationArchive pronunciations = new(_lEntryLoaderDatabase);
        LPronunciation? pronunciation = pronunciations.LPronunciationRead(id);
        LPronunciationAudio? audio = pronunciation is null
            ? null
            : pronunciations.LPronunciationAudioRead(pronunciation.LPronunciationId);

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
            LEntrySoundFormat(pronunciation, audio),
            note?.LNoteText ?? string.Empty,
            meaningCards,
            collocationCards,
            speeches,
            forms,
            inflections);
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
                LCardDraftGloss = meaning.LMeaningGloss,
                LCardDraftLanguage = meaning.LMeaningDefinitionLanguage,
                LCardDraftLabels = meaning.LMeaningLabels,
            });
        }

        return cards;
    }

    private static LPronunciationDraft? LEntrySoundFormat(
        LPronunciation? pronunciation, LPronunciationAudio? audio)
    {
        if (pronunciation is null)
        {
            return null;
        }

        return new LPronunciationDraft(
            pronunciation.LPronunciationIpa ?? string.Empty,
            pronunciation.LPronunciationLevel,
            pronunciation.LPronunciationSyllables,
            pronunciation.LPronunciationRepresentations,
            audio?.LPronunciationAudioFile ?? string.Empty,
            audio?.LPronunciationAudioSource,
            pronunciation.LPronunciationId);
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
                collocation ? situations.LSituationCollocationRead(ownerId) : situations.LSituationMeaningRead(ownerId)),
            LEntryRegisterRead(
                collocation ? registers.LRegisterCollocationRead(ownerId) : registers.LRegisterMeaningRead(ownerId)),
            LEntryTranslationRead(
                collocation
                    ? translations.LTranslationCollocationRead(ownerId)
                    : translations.LTranslationMeaningRead(ownerId)),
            string.Empty,
            LEntryTagRead(
                collocation ? tags.LTagCollocationRead(ownerId) : tags.LTagMeaningRead(ownerId)),
            LEntryImageRead(
                collocation ? images.LImageCollocationRead(ownerId) : images.LImageMeaningRead(ownerId)),
            LEntryVideoRead(
                collocation ? videos.LVideoCollocationRead(ownerId) : videos.LVideoMeaningRead(ownerId)),
            position,
            ownerId,
            LCardDraftRelation: collocation ? [] : LEntryRelationRead(ownerId),
            LCardDraftInterlink: collocation ? LEntrySynonymRead(ownerId) : []);
    }

    private IReadOnlyList<LRelationDraft> LEntryRelationRead(long meaningId)
    {
        IReadOnlyList<LRelation> stored =
            new LRelationArchive(_lEntryLoaderDatabase).LRelationRead(meaningId);

        List<LRelationDraft> drafts = new(stored.Count);
        foreach (LRelation relation in stored)
        {
            drafts.Add(new LRelationDraft(
                relation.LRelationType,
                relation.LRelationLabel,
                relation.LRelationLabels,
                relation.LRelationTargetEntry ?? 0,
                relation.LRelationTargetMeaning ?? 0));
        }

        return drafts;
    }

    private IReadOnlyList<LSynonymDraft> LEntrySynonymRead(long collocationId)
    {
        IReadOnlyList<LSynonym> stored =
            new LSynonymArchive(_lEntryLoaderDatabase).LSynonymRead(collocationId);

        List<LSynonymDraft> drafts = new(stored.Count);
        foreach (LSynonym synonym in stored)
        {
            drafts.Add(new LSynonymDraft(
                synonym.LSynonymTargetEntry ?? 0,
                synonym.LSynonymTargetMeaning ?? 0));
        }

        return drafts;
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
        return example is null
            ? null
            : new LExampleDraft(
                example.LExampleText,
                example.LExampleId,
                example.LExampleSource,
                example.LExampleTranslation,
                example.LExampleLanguage);
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
            drafts.Add(new LRegisterDraft(
                register.LRegisterName,
                register.LRegisterId,
                register.LRegisterLanguage,
                register.LRegisterBuiltin));
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

    private static IReadOnlyList<string> LEntryTagRead(IReadOnlyList<LTag> tags)
    {
        List<string> texts = new(tags.Count);
        foreach (LTag tag in tags)
        {
            texts.Add(tag.LTagText);
        }

        return texts;
    }
}
