# LFanqieArchive.cs

## `public sealed class LFanqieArchive`

Stores and reads the rime-book placements of a character, per language.
A character's rows are written as one set and read back in the order they were written.

## `public void LFanqieSave(string language, string character, IReadOnlyList<LFanqieRow> rows)`

Replaces whatever the character held with the given rows, in one transaction.
The books arrive in pack order and the positions in table order, and the row ids keep that order.

## `public IReadOnlyList<LFanqieRow> LFanqieRead(string language, string character)`

Every stored row of the character, in insertion order, with the parts the line was built from.
The reading and tone class come back as the placement stored them, so no hypothesis runs on read.
An empty list means the character was never fetched, or nothing was found and nothing stored.
