# LFanqieVault.cs

## `public interface LFanqieVault`

The persistence port for the Fanqie rows the engine reads and writes.
It lists exactly what the engine asks of fanqie storage, and nothing about how rows are kept.
`LFanqieArchive` in Infrastructure is its adapter over the workspace database.

## `void LFanqieSave(string language, string character, IReadOnlyList<LFanqieRow> rows);`

Makes the given rows the whole set of the character, in one transaction.
A row already stored under the same book, source and position is rewritten in place under its id.
A row not stored yet is inserted, and a stored row the fetch no longer gives is deleted.
The books arrive in pack order and the positions in table order, and a first save's ids keep that order.

## `IReadOnlyList<LFanqieRow> LFanqieRead(string language, string character);`

Every stored row of the character, in insertion order, with the parts the line was built from.
The reading and tone class come back as the placement stored them, so no hypothesis runs on read.
An empty list means the character was never fetched, or nothing was found and nothing stored.
