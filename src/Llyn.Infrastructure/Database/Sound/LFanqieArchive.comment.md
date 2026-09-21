# LFanqieArchive.cs

## `public sealed class LFanqieArchive`

Stores and reads the rime-book placements of a character, per language.
A character's rows are written as one set and read back in the order they were first written.
A row keeps its id across refetches, so the anchors reflex rows hold on it stand.

## `public void LFanqieSave(string language, string character, IReadOnlyList<LFanqieRow> rows)`

Makes the given rows the whole set of the character, in one transaction.
A row already stored under the same book, source and position is rewritten in place under its id.
A row not stored yet is inserted, and a stored row the fetch no longer gives is deleted.
The books arrive in pack order and the positions in table order, and a first save's ids keep that order.
A representative rank survives the rewrite, and the ranks are closed up when a ranked row is deleted.

## `private static void LFanqieLeftoverDelete(`

Deletes the character's rows whose ids the save did not write, and their diwei links and anchors by cascade.

## `public IReadOnlyList<LFanqieRow> LFanqieRead(string language, string character)`

Every stored row of the character, in insertion order, with the parts the line was built from.
The reading and tone class come back as the placement stored them, so no hypothesis runs on read.
An empty list means the character was never fetched, or nothing was found and nothing stored.

## `public void LFanqieRepresentativeSet(long fanqieId, int rank)`

Places the row at the asked rank among the character's representative readings, zero unmarking it.
Every other ranked row of that character shifts, so the ranks always run from one without a gap.
A rank past the end lands the row last, which is how a fresh mark is asked for.

## `private static (string, string)? LFanqiePlaceRead(SqliteConnection connection, long fanqieId)`

The language and character the row belongs to, or nothing when the id is not stored.

## `private static List<long> LFanqieMarkedRead(SqliteConnection connection, string language, string character)`

The character's ranked row ids in rank order, the id breaking a tie two equal ranks leave.

## `private static void LFanqieRankApply(SqliteConnection connection, IReadOnlyList<long> marked)`

Numbers the given ids from one in the order they arrive.

## `private static void LFanqieRankSave(SqliteConnection connection, long fanqieId, int rank)`

Writes one row's rank, zero meaning the row is not representative.
