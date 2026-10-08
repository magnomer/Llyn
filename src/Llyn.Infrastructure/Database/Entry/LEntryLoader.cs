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

        LEntryLoaderCard cards = new(_lEntryLoaderDatabase);
        LEntryLoaderReading readings = new(_lEntryLoaderDatabase);
        LEntryArchive entries = new(_lEntryLoaderDatabase);
        IReadOnlyList<LSpeechDraft> speeches = LEntrySpeechFormat(entries.LEntrySpeechRead(id));
        IReadOnlyList<LForm> forms = entries.LEntryFormRead(id);
        IReadOnlyList<LInflection> inflections =
            new LInflectionArchive(_lEntryLoaderDatabase).LInflectionRead(id);

        LNote? note = new LNoteArchive(_lEntryLoaderDatabase).LNoteRead(id);
        IReadOnlyList<LPronunciationDraft> pronunciations = readings.LEntrySoundRead(id);
        IReadOnlyList<LTranscriptionDraft> transcriptions = readings.LEntrySpellingRead(id);
        IReadOnlyList<LReflexDraft> reflexes = readings.LEntryReflexRead(id);
        LEtymologyDraft etymology = LEntryEtymologyRead(id);

        IReadOnlyList<LCardDraft> meaningCards = cards.LEntryMeaningRead(id);
        IReadOnlyList<LCardDraft> collocationCards = cards.LEntryCollocationRead(id);

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
            reflexes,
            etymology,
            entry.LEntryUnit);
    }

    private LEtymologyDraft LEntryEtymologyRead(long id)
    {
        LEtymologyArchive etymologies = new(_lEntryLoaderDatabase);
        List<long> targets = [];
        foreach (LEtymon etymon in etymologies.LEtymologyEtymonRead(id))
        {
            targets.Add(etymon.LEtymonTargetId);
        }

        return LEtymologyDraft.LEtymologyDraftCreate(etymologies.LEtymologyRead(id), targets);
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
}
