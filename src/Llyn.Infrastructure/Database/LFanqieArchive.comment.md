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

## `private static void LFanqieLeftoverDelete(`

Deletes the character's rows whose ids the save did not write, and their diwei links and anchors by cascade.

## `public IReadOnlyList<LFanqieRow> LFanqieRead(string language, string character)`

Every stored row of the character, in insertion order, with the parts the line was built from.
The reading and tone class come back as the placement stored them, so no hypothesis runs on read.
An empty list means the character was never fetched, or nothing was found and nothing stored.
