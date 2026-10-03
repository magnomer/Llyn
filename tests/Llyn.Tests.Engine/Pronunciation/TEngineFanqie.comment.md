# TEngineFanqie.cs
Hash: `e5892350f0571261`

## `public sealed class TEngineFanqie`

Covers the engine's fanqie fetch over a fixture pack with two books behind stubbed rime tables.
A start posts each book's form and reads every cell carrying the marked character into one line.
The line joins the row's initial, the cell's rime with its rime heading, and the column's heading.
A cell parted into groups yields one row per marked group.
Each carries its own rime heading, tone, rounding and 反切.
A book without the part patterns fills the line and leaves the parts empty.
A book that answers with its busy text is skipped for now, and the other book is still read.
A read of an entry with nothing stored returns empty, and a start fetches every character.
It stores the rows and raises the fanqie bulletin.
A character every book was busy for still raises the bulletin and is asked again on the next start.
A character every book answered for yet none placed is asked once per session and stores nothing.
A language whose pack lists no book reads empty and asks nothing.
The line-book sources live in `TEngineFanqieSource` and the rank presses in `TEngineFanqieRank`.
Both build through the helpers kept here.

## Inline notes

`TFanqieFetchRead` saves an entry, starts its fetch, waits for it to settle and reads what it stored.
`TFanqieSettle` waits until no fetch runs for the entry, since a bulletin is raised per character.
`TFanqieCountCheck` waits for the stub to see the given number of requests.
`TFanqiePartRead` and `TFanqieRowRead` flatten a row for one-line assertions.
`TFanqieDraftCreate` builds the entry draft the fetch starts from.
`TEngineFanqieBroad` is the Broad book's page, shared with the source and rank classes.
