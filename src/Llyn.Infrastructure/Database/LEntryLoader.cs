using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Infrastructure;

/// <summary>
/// Reads a stored entry back into the <see cref="LEntryDraft"/> the input form saved. It owns no SQL
/// of its own: it composes what the archives already read — the entry row, its meanings and
/// collocations, its note and pronunciation, and every Example, Situation and Tag each card
/// references, in stored order — into the one value shape <c>LEngineEntrySave</c> consumes, so a save and a load are
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
    /// A card's Examples, Situations and Tags are ordered sets: every row the card references fills the
    /// field, in the order the associations record, so a card referencing three tags loads back with
    /// three. Meanings arrive flat, as the draft has no nesting: a sub-meaning — which no save writes —
    /// would come back as a card of its own.
    /// </para>
    /// <para>
    /// Each card carries the id of the row it was read from, so a draft loaded, edited and handed to
    /// <c>LEngineEntryUpdate</c> names the Meaning or Collocation each card belongs to; a card the user
    /// adds afterwards has no id and is created.
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
            audio?.LPronunciationAudioSource);
    }

    // The one read path for the independents a card references. A Meaning card and a Collocation card
    // reference Examples, Situations and Tags on identical terms, so which owner side is being read is
    // the only thing that differs: the collocation flag says which side ownerId names, and the card's
    // own columns are handed in already read off its row.
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
            // Nothing stored feeds a card synonym: the save writes none, so the field comes back empty.
            string.Empty,
            LEntryTextRead(
                collocation ? tags.LTagCollocationRead(ownerId) : tags.LTagSenseRead(ownerId),
                tag => tag.LTagText),
            // The card says which stored row it is, so a draft handed back to the engine updates that
            // row rather than being read as a new card.
            ownerId);
    }

    // A card field is an ordered set, so every referenced row fills it and none is discarded. The rows
    // arrive ordered by the position each association carries, which is the order the save wrote them
    // in, so the field reads back as it was typed. A row whose text is null reads as the empty string
    // the draft uses for "nothing was typed".
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
