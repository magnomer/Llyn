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

        List<LCardDraft> senseCards = [];
        foreach (LSense sense in new LSenseArchive(_lEntryLoaderDatabase).LSenseRead(id))
        {
            senseCards.Add(LEntryCardRead(
                sense.LSenseId,
                collocation: false,
                sense.LSenseTitle,
                LStateValue.LStateValueUnspecified,
                sense.LSenseDefinition));
        }

        List<LCardDraft> collocationCards = [];
        foreach (LCollocation collocation in new LCollocationArchive(_lEntryLoaderDatabase).LCollocationRead(id))
        {
            collocationCards.Add(LEntryCardRead(
                collocation.LCollocationId,
                collocation: true,
                collocation.LCollocationTitle,
                collocation.LCollocationExpression,
                collocation.LCollocationMeaning));
        }

        return new LEntryDraft(
            entry.LEntryHeadword,
            entry.LEntryLanguage,
            pronunciation?.LPronunciationIpa ?? string.Empty,
            note?.LNoteText ?? string.Empty,
            senseCards,
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
        LStateValue title,
        LStateValue expression,
        LStateValue meaning)
    {
        LExampleLink examples = new(_lEntryLoaderDatabase);
        LSituationArchive situations = new(_lEntryLoaderDatabase);
        LTagArchive tags = new(_lEntryLoaderDatabase);

        return new LCardDraft(
            title,
            expression,
            meaning,
            LEntryExampleRead(
                collocation ? examples.LExampleCollocationRead(ownerId) : examples.LExampleSenseRead(ownerId)),
            LEntrySituationRead(
                collocation ? situations.LSituationCollocationRead(ownerId) : situations.LSituationSenseRead(ownerId)),
            string.Empty,
            LEntryTagRead(
                collocation ? tags.LTagCollocationRead(ownerId) : tags.LTagSenseRead(ownerId)),
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
                situation.LSituationTitle, situation.LSituationId, situation.LSituationSource));
        }

        return drafts;
    }

    private static IReadOnlyList<LStateValue> LEntryTagRead(IReadOnlyList<LTag> tags)
    {
        List<LStateValue> texts = new(tags.Count);
        foreach (LTag tag in tags)
        {
            texts.Add(tag.LTagText);
        }

        return texts;
    }
}
