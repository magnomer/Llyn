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
                sense.LSenseTitle ?? string.Empty,
                string.Empty,
                sense.LSenseDefinition ?? string.Empty));
        }

        List<LCardDraft> collocationCards = [];
        foreach (LCollocation collocation in new LCollocationArchive(_lEntryLoaderDatabase).LCollocationRead(id))
        {
            collocationCards.Add(LEntryCardRead(
                collocation.LCollocationId,
                collocation: true,
                collocation.LCollocationTitle ?? string.Empty,
                collocation.LCollocationExpression ?? string.Empty,
                collocation.LCollocationMeaning ?? string.Empty));
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
        string title,
        string expression,
        string meaning)
    {
        LExampleLink examples = new(_lEntryLoaderDatabase);
        LSituationArchive situations = new(_lEntryLoaderDatabase);
        LTagArchive tags = new(_lEntryLoaderDatabase);

        return new LCardDraft(
            title,
            expression,
            meaning,
            LEntryTextRead(
                collocation ? examples.LExampleCollocationRead(ownerId) : examples.LExampleSenseRead(ownerId),
                example => example.LExampleText),
            LEntryTextRead(
                collocation ? situations.LSituationCollocationRead(ownerId) : situations.LSituationSenseRead(ownerId),
                situation => situation.LSituationTitle),
            string.Empty,
            LEntryTextRead(
                collocation ? tags.LTagCollocationRead(ownerId) : tags.LTagSenseRead(ownerId),
                tag => tag.LTagText),
            ownerId);
    }

    private static IReadOnlyList<string> LEntryTextRead<TRow>(
        IReadOnlyList<TRow> rows,
        Func<TRow, string?> text)
    {
        List<string> texts = new(rows.Count);
        foreach (TRow row in rows)
        {
            texts.Add(text(row) ?? string.Empty);
        }

        return texts;
    }
}
