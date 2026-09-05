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

    public LEntryDraft? LEntryLoad(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lEntryLoaderDatabase.LDatabaseSessionStart();

        LEntry? entry = new LEntryArchive(_lEntryLoaderDatabase).LEntryRead(id);
        if (entry is null)
        {
            return null;
        }

        LEntryArchive entries = new(_lEntryLoaderDatabase);
        IReadOnlyList<string> speeches = LEntrySpeechFormat(
            entry.LEntryLanguage, entries.LEntrySpeechRead(id));

        LNote? note = new LNoteArchive(_lEntryLoaderDatabase).LNoteRead(id);
        LPronunciationArchive pronunciations = new(_lEntryLoaderDatabase);
        LPronunciation? pronunciation = pronunciations.LPronunciationRead(id);
        LPronunciationAudio? audio = pronunciation is null
            ? null
            : pronunciations.LPronunciationAudioRead(pronunciation.LPronunciationId);

        List<LCardDraft> meaningCards = [];
        foreach (LMeaning meaning in new LMeaningArchive(_lEntryLoaderDatabase).LMeaningRead(id))
        {
            meaningCards.Add(LEntryCardRead(
                meaning.LMeaningId,
                collocation: false,
                meaning.LMeaningPosition + 1,
                meaning.LMeaningTitle,
                LStateValue.LStateValueUnspecified,
                meaning.LMeaningDefinition));
        }

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
            pronunciation?.LPronunciationIpa ?? string.Empty,
            note?.LNoteText ?? string.Empty,
            meaningCards,
            collocationCards,
            audio?.LPronunciationAudioFile ?? string.Empty,
            audio?.LPronunciationAudioSource,
            speeches);
    }

    private IReadOnlyList<string> LEntrySpeechFormat(string language, IReadOnlyList<LSpeech> speeches)
    {
        if (speeches.Count == 0)
        {
            return [];
        }

        LSpeechArchive values = new(_lEntryLoaderDatabase);
        List<string> names = new(speeches.Count);
        foreach (LSpeech speech in speeches)
        {
            names.Add(speech.LSpeechValueId is not string value
                ? speech.LSpeechCustom ?? string.Empty
                : values.LSpeechValueRead(language, value) ?? value);
        }

        return names;
    }

    private LCardDraft LEntryCardRead(
        string ownerId,
        bool collocation,
        int position,
        LStateValue title,
        LStateValue expression,
        LStateValue meaning)
    {
        LExampleLink examples = new(_lEntryLoaderDatabase);
        LSituationArchive situations = new(_lEntryLoaderDatabase);
        LTagArchive tags = new(_lEntryLoaderDatabase);
        LTranslationArchive translations = new(_lEntryLoaderDatabase);
        LImageArchive images = new(_lEntryLoaderDatabase);

        return new LCardDraft(
            title,
            expression,
            meaning,
            LEntryExampleRead(
                collocation ? examples.LExampleCollocationRead(ownerId) : examples.LExampleMeaningRead(ownerId)),
            LEntrySituationRead(
                collocation ? situations.LSituationCollocationRead(ownerId) : situations.LSituationMeaningRead(ownerId)),
            LEntryTranslationRead(
                collocation
                    ? translations.LTranslationCollocationRead(ownerId)
                    : translations.LTranslationMeaningRead(ownerId)),
            string.Empty,
            LEntryTagRead(
                collocation ? tags.LTagCollocationRead(ownerId) : tags.LTagMeaningRead(ownerId)),
            LEntryImageRead(
                collocation ? images.LImageCollocationRead(ownerId) : images.LImageMeaningRead(ownerId)),
            position,
            ownerId);
    }

    private static IReadOnlyList<LExampleDraft> LEntryExampleRead(IReadOnlyList<LExample> examples)
    {
        List<LExampleDraft> drafts = new(examples.Count);
        foreach (LExample example in examples)
        {
            drafts.Add(new LExampleDraft(
                example.LExampleText, example.LExampleId, example.LExampleSource));
        }

        return drafts;
    }

    private static IReadOnlyList<LSituationDraft> LEntrySituationRead(IReadOnlyList<LSituation> situations)
    {
        List<LSituationDraft> drafts = new(situations.Count);
        foreach (LSituation situation in situations)
        {
            drafts.Add(new LSituationDraft(
                situation.LSituationTitle, situation.LSituationId));
        }

        return drafts;
    }

    private static IReadOnlyList<LStateValue> LEntryImageRead(IReadOnlyList<LImage> images)
    {
        List<LStateValue> locations = new(images.Count);
        foreach (LImage image in images)
        {
            locations.Add(image.LImageLocation);
        }

        return locations;
    }

    private static IReadOnlyList<string> LEntryTranslationRead(
        IReadOnlyList<LTranslation> translations)
    {
        List<string> ids = new(translations.Count);
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
