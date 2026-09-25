# LSpeechVault.cs

## `public interface LSpeechVault`

The persistence port for the Speech rows the engine reads and writes.
It lists exactly what the engine asks of speech storage, and nothing about how rows are kept.
`LSpeechArchive` in Infrastructure is its adapter over the workspace database.

## `LSpeechValue LSpeechValueCreate(LSpeechValue value);`

Adds or replaces one vocabulary row, keyed by `(language, pack_code)`, and returns it with its row id.
A pack that renames a value keeps the row id, so every entry that links it follows the rename.
A value with code `0` is user-added and is given a negative code no pack can collide with.

## `LSpeechValue? LSpeechValueRead(long id);`

Reads one value by row id, or `null` when no row has it.

## `IReadOnlyList<LSpeechValue> LSpeechValueRead(string language);`

Reads every part of speech `language` declares.
They come in the display order the language pack listed them in.
Those are the presets the part-of-speech field offers.
A language with no pack on disk declares none, which is an empty list rather than a failure.

## `LSpeechValue? LSpeechValueFind(string language, string name);`

Resolves the part of speech `name` names in `language` back to its row.
It returns `null` when the language declares no value by that name.
Matching is a trimmed, case-insensitive comparison of the display name.
The name arrives as the user typed it.
Picking "Verb, transitive" from the dropdown and typing "verb, transitive" mean the same thing.
Only text that names no value at all is stored as typed.

## `LSpeechPack LSpeechLoad(string language);`

Reads the parts of speech and their features the language pack of `language` declares.
A missing pack yields an empty pack.
