# LSituationUsage.cs
Hash: `233d2ef33f66d4b3`

## `public sealed class LSituationUsage`

Reads the referring side of a Situation: which Meanings and Collocations point at it.
It lives beside `LSituationArchive` rather than inside it, as `LAuthorUsage` lives beside `LAuthorArchive`.
Naming the referring cards is a different responsibility from storing a Situation.
Nothing here creates, changes, or deletes anything.

## `public LSituationUsage(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LUsage> LSituationUsageRead(long id)`

The referring sides of one Situation, named rather than counted.
A Meaning is named by its title and, standing without one, by its definition.
A Collocation is named by its title and, standing without one, by its expression.
The Entry each side belongs to is read with it, so a row is legible without a second query.
Card rows sharing a headword go by language, then entry id, then place on the Entry.
That is the default catalog order, so storage order never decides a place.
