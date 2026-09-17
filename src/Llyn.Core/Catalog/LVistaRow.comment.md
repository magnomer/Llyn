# LVistaRow.cs

## `public sealed record LVistaRow(`

One row of a catalog list as the engine hands it to a panel, ready to show.
The engine has already filtered, sorted, numbered and marked it.
The panel copies it into its list and decides nothing.

**Parameters**

- `LVistaRowId` — The id of the record the row stands for.
- `LVistaRowHeadword` — The stored headword, untouched.
- `LVistaRowLanguage` — The language the record is in, which the panel turns into a flag.
- `LVistaRowEpithet` — The reflex epithet shown beside the headword, or `null` when the workspace shows none.
- `LVistaRowName` — The headword as displayed, numbered when another listed row shares it.
- `LVistaRowChosen` — Whether this row is the one the vista stands on.
