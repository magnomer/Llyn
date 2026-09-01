using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Infrastructure;

/// <summary>
/// Reads a stored entry back into the <see cref="LEntryDraft"/> the input form saved. It owns no SQL
/// of its own: it composes what the archives already read — the entry row, its meanings and
/// collocations, its note and pronunciation, and the Examples, Situations and Tags each card
/// references — into the one value shape <c>LEngineEntrySave</c> consumes, so a save and a load are
/// inverses of each other.
/// <para>
/// The whole composition runs inside one <see cref="LDatabaseSession"/>. Every archive call it makes
/// nests into that session, so the draft is a single consistent snapshot rather than a dozen
/// independently-timed queries that could disagree with each other.
/// </para>
/// <para>
/// The audio path comes back exactly as stored, relative to the workspace folder. Resolving it
/// against the folder in use is the engine's job, because the workspace root is what the engine owns.
/// </para>
/// </summary>
public sealed class LEntryLoader
{
    private readonly LDatabase _lEntryLoaderDatabase;

    /// <summary>Binds the loader to the workspace <paramref name="database"/> it reads through.</summary>
    public LEntryLoader(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lEntryLoaderDatabase = database;
    }

    /// <summary>
    /// Returns the draft for the entry identified by <paramref name="id"/>, or <c>null</c> when no
    /// entry has that id.
    /// <para>
    /// A card carries one Example, one Situation and one Tag, because the input form gives it one field
    /// for each; the save writes at most one row per field, so the first referenced row fills the field
    /// back in and a card the save produced round-trips exactly. Meanings arrive flat, as the draft has
    /// no nesting: a sub-meaning — which no save writes — would come back as a card of its own.
    /// </para>
    /// <para>
    /// Synonyms come back empty. The save writes none (the card's field is free text and a relation
    /// needs a target id), so there is nothing stored to read, and the collocation card has no Synonym
    /// control feeding it at all.
    /// </para>
    /// </summary>
    public LEntryDraft? LEntryLoad(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lEntryLoaderDatabase.LDatabaseSessionStart();

        LEntry? entry = new LEntryArchive(_lEntryLoaderDatabase).LEntryRead(id);
        if (entry is null)
        {
            return null;
        }

        LNote? note = new LNoteArchive(_lEntryLoaderDatabase).LNoteRead(id);
        LPronunciationArchive pronunciations = new(_lEntryLoaderDatabase);
        LPronunciation? pronunciation = pronunciations.LPronunciationRead(id);
        LPronunciationAudio? audio = pronunciation is null
            ? null
            : pronunciations.LPronunciationAudioRead(pronunciation.LPronunciationId);

        LExampleLink examples = new(_lEntryLoaderDatabase);
        LSituationArchive situations = new(_lEntryLoaderDatabase);
        LTagArchive tags = new(_lEntryLoaderDatabase);

        List<LSenseDraft> senseCards = [];
        foreach (LSense sense in new LSenseArchive(_lEntryLoaderDatabase).LSenseRead(id))
        {
            senseCards.Add(new LSenseDraft(
                senseCards.Count + 1,
                sense.LSenseDefinition ?? string.Empty,
                LEntryFirstRead(examples.LExampleSenseRead(sense.LSenseId), example => example.LExampleText),
                LEntryFirstRead(situations.LSituationSenseRead(sense.LSenseId), situation => situation.LSituationTitle),
                string.Empty,
                LEntryFirstRead(tags.LTagSenseRead(sense.LSenseId), tag => tag.LTagText)));
        }

        List<LCollocationDraft> collocationCards = [];
        foreach (LCollocation collocation in new LCollocationArchive(_lEntryLoaderDatabase).LCollocationRead(id))
        {
            string cardId = collocation.LCollocationId;
            collocationCards.Add(new LCollocationDraft(
                collocationCards.Count + 1,
                collocation.LCollocationExpression ?? string.Empty,
                collocation.LCollocationMeaning ?? string.Empty,
                LEntryFirstRead(examples.LExampleCollocationRead(cardId), example => example.LExampleText),
                LEntryFirstRead(situations.LSituationCollocationRead(cardId), situation => situation.LSituationTitle),
                string.Empty,
                LEntryFirstRead(tags.LTagCollocationRead(cardId), tag => tag.LTagText)));
        }

        return new LEntryDraft(
            entry.LEntryHeadword,
            entry.LEntryLanguage,
            pronunciation?.LPronunciationIpa ?? string.Empty,
            note?.LNoteText ?? string.Empty,
            senseCards,
            collocationCards,
            audio?.LPronunciationAudioFile ?? string.Empty,
            audio?.LPronunciationAudioSource);
    }

    // A card field holds one value, so a referenced set fills it from its first row and an empty set
    // leaves the empty string the draft uses for "nothing was typed".
    private static string LEntryFirstRead<TRow>(IReadOnlyList<TRow> rows, Func<TRow, string?> text)
    {
        return rows.Count == 0 ? string.Empty : text(rows[0]) ?? string.Empty;
    }
}
