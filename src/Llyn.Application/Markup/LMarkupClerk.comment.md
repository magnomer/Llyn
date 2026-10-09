# LMarkupClerk.cs
Hash: `ee9d329322a31da0`

## `public sealed class LMarkupClerk`

An entry is translated to and from markup here.
The clerk holds the cargo read and the export.
The per-entry load lives in `LMarkupClerkEntry`, and the import in `LMarkupClerkIntake`.

## `public LMarkupClerk(LRig rig, LMarkupClerkEntry entry)`

Reads the vault, markup and language ports out of `rig`, the same instances the engine holds.
`entry` is the clerk that loads each exported entry as markup.
It opens no database of its own, so a test can hand it in-memory ports.

## `public LMarkupCargo LMarkupClerkRead(string path)`

The file at `path` parsed, entries in a language the workspace cannot load losing it with an omission.
Twin names are applied so two entries with one headword are told apart in the intake list.
Only entries of one language are twins.

## `public void LMarkupClerkExport(IReadOnlyList<long> ids, string path)`

The entries loaded in one session, formatted as one tree and written through the markup port.
An id no entry carries refuses the export.
