# LTranslationArchive.cs

## `public sealed class LTranslationArchive`

Persists the Translation links a Meaning or a Collocation makes to other Entries.
A Translation carries no words of its own.
It is the id of the Entry that renders the card, and the place that id takes on the card.
The words are read back from that Entry, so a headword renamed once reads renamed everywhere.
The two association tables are therefore the whole of the storage.

A card's Translation line is written as a whole rather than edited a link at a time.
The old rows go and the new ones are numbered from zero in the order given.
Blank ids and repeats are dropped on the way.
So saving the same list twice leaves the same rows, and positions stay contiguous by construction.

Links are stored one way, from the card to the Entry.
The Entry finds them by looking back through the same rows.
Deleting an Entry takes its incoming rows with it, leaving the cards that pointed at it standing.

## `public LTranslationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LTranslation> LTranslationMeaningRead(long meaningId)`

Reads the links a Meaning makes, in the order that Meaning gives them.

## `public IReadOnlyList<LTranslation> LTranslationCollocationRead(long collocationId)`

Reads the links a Collocation makes, in the order that Collocation gives them.

## `public void LTranslationMeaningSave(long meaningId, IReadOnlyList<LTranslation> translations)`

Writes a Meaning's whole Translation line, replacing whatever it carried.
Ids are trimmed, blanks and repeats are dropped, and what survives is numbered from zero.

## `public void LTranslationCollocationSave(long collocationId, IReadOnlyList<LTranslation> translations)`

The same write for a Collocation.

## `public IReadOnlyList<LTranslationTarget> LTranslationTargetRead(IReadOnlyList<string> ids)`

Reads the headword and language of every Entry named by `ids`, in the order the ids were given.
A whole card's links resolve in one query rather than one query each.
An id no Entry answers is skipped rather than reported, because a caller wants what it can show.

## `public IReadOnlyList<LUsage> LTranslationIncomingRead(long entryId)`

Reads the Meanings and Collocations that point at this Entry.
Each one is a Usage, so an Entry shows what renders it the way an Example shows what quotes it.
Meanings come before Collocations, each group ordered by headword and then by place on its Entry.

## Inline notes

### `private void LTranslationReferrerSave(string table, string column, string referrerId, IReadOnlyList<LTranslation> translations)`

The two association tables differ only in their name and their card column, so the write is one implementation.
Both identifiers are store-owned literals chosen by the methods above, never caller input.
So composing them into the statement text opens no injection seam.
Every value still travels as a parameter.

### `public IReadOnlyList<LTranslationTarget> LTranslationTargetRead(IReadOnlyList<string> ids)`

The ids travel as one JSON parameter and the statement unpacks them with `json_each`.
So no id is ever composed into the statement, and the list has no ceiling.
