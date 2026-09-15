# TEngineFanqie.cs

## `public sealed class TEngineFanqie`

Covers the engine's fanqie fetch over a fixture pack with two books behind stubbed rime tables.
A find posts each book's form and reads every cell carrying the marked character into one line.
The line joins the row's initial, the cell's rime with its rime heading, and the column's heading.
A cell parted into groups yields one row per marked group.
Each carries its own rime heading, tone, rounding and 反切.
A book without the part patterns fills the line and leaves the parts empty.
A book with a line pattern is fetched by GET and read as plain text, one row per match.
Its rows carry its own source label.
Two sources of one book both store, since each counts its positions from zero under its own source.
A not-found answer from such a book counts as reached with nothing, so the character is not asked again.
A book that answers with its busy text is skipped for now, and the other book is still read.
A read of an entry with nothing stored returns empty, and a start fetches every character.
It stores the rows and raises the fanqie bulletin.
A character every book was busy for still raises the bulletin and is asked again on the next start.
A character every book answered for yet none placed is asked once per session and stores nothing.
A language whose pack lists no book reads empty and asks nothing.

## Inline notes

`TFanqieSettle` waits until no fetch runs for the entry, since a bulletin is raised per character.
`TFanqieCountCheck` waits for the stub to see the given number of requests.
`TFanqiePartRead` and `TFanqieRowRead` flatten a row for one-line assertions.
