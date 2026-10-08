using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LEntryLoaderCard
{
    private readonly LDatabase _lEntryCardDatabase;

    public LEntryLoaderCard(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lEntryCardDatabase = database;
    }

    public IReadOnlyList<LCardDraft> LEntryMeaningRead(long id)
    {
        Dictionary<long, List<LMeaning>> senses = [];
        foreach (LMeaning meaning in new LMeaningArchive(_lEntryCardDatabase).LMeaningRead(id))
        {
            long parent = meaning.LMeaningParentId ?? 0;
            if (!senses.TryGetValue(parent, out List<LMeaning>? group))
            {
                group = [];
                senses[parent] = group;
            }

            group.Add(meaning);
        }

        return LEntryChildRead(senses, 0);
    }

    public IReadOnlyList<LCardDraft> LEntryCollocationRead(long id)
    {
        List<LCardDraft> collocationCards = [];
        foreach (LCollocation collocation in new LCollocationArchive(_lEntryCardDatabase).LCollocationRead(id))
        {
            collocationCards.Add(LEntryCardRead(
                collocation.LCollocationId,
                collocation: true,
                collocation.LCollocationPosition + 1,
                collocation.LCollocationTitle,
                collocation.LCollocationExpression,
                collocation.LCollocationMeaning));
        }

        return collocationCards;
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

    private LCardDraft LEntryCardRead(
        long ownerId,
        bool collocation,
        int position,
        LStateValue title,
        LStateValue expression,
        LStateValue meaning)
    {
        LSituationArchive situations = new(_lEntryCardDatabase);
        LRegisterArchive registers = new(_lEntryCardDatabase);
        LTagArchive tags = new(_lEntryCardDatabase);
        LTranslationArchive translations = new(_lEntryCardDatabase);
        LImageArchive images = new(_lEntryCardDatabase);
        LVideoArchive videos = new(_lEntryCardDatabase);
        LSentenceArchive sentences = new(_lEntryCardDatabase);

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
