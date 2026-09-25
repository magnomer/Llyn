# LMentionArchive.cs

## `public sealed class LMentionArchive`

Persists Mentions, the words of an Example that stand for an Entry.
A Mention is the Example's own row, so it is read and written by Example.
`LExampleArchive` and `LSentenceArchive` call the static readers here to fill every Example they return.
Nothing in the engine or the UI reads the rows yet.

## `public LMentionArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LMention> LMentionExampleRead(long exampleId)`

Every Mention of the Example identified by `exampleId`, ordered by where each starts.

## `public IReadOnlyList<long> LMentionExampleSave(long exampleId, IReadOnlyList<LMention> mentions)`

Rewrites the Mention list of one Example and answers the written ids in input order.
A span past the end of the stored text is refused, and so is a pair of spans that overlap.
The refusal names the offending span and throws before any row is touched.
Existing rows are shelved by shifting their start far up, so the unique (example, start) index never trips.
A row whose positive id survives is rewritten in place, the rest are inserted, and the shelf is deleted.
This is the same shape as `LSentenceOwnerSave`.

## `public IReadOnlyList<long> LMentionSenseClear(long meaningId)`

Drops the sense from every Mention citing `meaningId`, so each lands on its entry alone.
The answer names each affected Example once, so a caller can report what lost its sense.
The markup Replace path uses it before the target's senses are rewritten.

## `public bool LMentionSenseSet(long mentionId, long senseId)`

Points the Mention `mentionId` at the sense `senseId`, answering whether a row changed.
A Mention with no entry is left alone, since a sense without an entry breaks the row's check.
The markup import uses it once every entry of a file is stored and the cited senses exist.

## Inline notes

### `internal static IReadOnlyDictionary<long, IReadOnlyList<LMention>> LMentionExampleRead(`

Every Mention of a set of Examples in one statement, keyed by Example.
The ids travel as one JSON parameter, so the whole corpus fits in one statement.
The sentence reader and the whole-table reader fill every row they return in one round trip.
An Example with no Mention has no key, and the caller reads that as an empty list.

### `internal static IReadOnlyList<long> LMentionExampleSave(`

The rewrite on a connection the caller already holds.
So an Example insert and its Mentions commit together.

### `internal static void LMentionExampleSweep(SqliteConnection connection, long exampleId)`

Deletes every Mention whose span no longer fits the stored text.
A text update calls it after the new text is written.
The rows that still fit are kept, and nothing is shifted here.

### `private static int LMentionTextRead(SqliteConnection connection, long exampleId)`

The length of the stored text in code points, or zero when the text is not stated.
Throws when no Example carries the id.
The count matches the span arithmetic, so a surrogate pair is one character on both sides.
