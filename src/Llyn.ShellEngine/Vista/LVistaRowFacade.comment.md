# LVistaRowFacade.cs
Hash: `c984c1184a7c7741`

## `internal sealed class LVistaRowFacade`

Builds the rows every catalog of entries shows, so no facade numbers twins or reads epithets itself.
It holds only the engine's hearth, so the facades that build rows need not reach the vista facade.

## `internal LVistaRowFacade(LEngineHearth hearth)`

Keeps the hearth, whose gate, staff and settings the row build reads.

## `internal IReadOnlyList<LVistaRow> LEngineVistaBuild(IReadOnlyList<LEntry> entries, long? chosen)`

Turns entries in their listed order into rows ready to show: twin name, epithet and chosen mark.
An entry with no epithet carries an empty one, so no reader of a row falls back on its own.
Twins are numbered by entry id, so the older entry is `(1)` in every view.
Only entries of one language are twins.
The epithets come from one scan, so a long list costs one statement rather than one session per row.
The chosen row is the one whose id equals `chosen`.
Every catalog of entries builds its rows here, so no panel numbers twins or reads epithets itself.
It takes the lock for the epithet scan, which reads the hearth's settings and staff.

