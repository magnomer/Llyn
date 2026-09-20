# LSpeechArchive.cs

## `public sealed class LSpeechArchive`

Persists and resolves a language's part-of-speech vocabulary.
An entry's POS rows link a `speech_value` row by id.
The display name lives here and is read by that id.
So a name is never copied onto an entry's rows.

## `public LSpeechArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LSpeechValue LSpeechValueCreate(LSpeechValue value)`

Adds or replaces one vocabulary row, keyed by `(language, pack_code)`, and returns it with its row id.
A pack that renames a value keeps the row id, so every entry that links it follows the rename.
A value with code `0` is user-added and is given a negative code no pack can collide with.

## `public LSpeechValue? LSpeechValueRead(long id)`

Reads one value by row id, or `null` when no row has it.

## `public IReadOnlyList<LSpeechValue> LSpeechValueRead(string language)`

Reads every part of speech `language` declares.
They come in the display order the language pack listed them in.
Those are the presets the part-of-speech field offers.
A language with no pack on disk declares none, which is an empty list rather than a failure.

## `public LSpeechValue? LSpeechValueFind(string language, string name)`

Resolves the part of speech `name` names in `language` back to its row.
It returns `null` when the language declares no value by that name.
Matching is a trimmed, case-insensitive comparison of the display name.
The name arrives as the user typed it.
Picking "Verb, transitive" from the dropdown and typing "verb, transitive" mean the same thing.
Only text that names no value at all is stored as typed.

## `public LSpeechPack LSpeechLoad(string language)`

The parts of speech and features the language pack declares, read through `LSpeechLoader`.

## Inline notes

### `private static long LSpeechCodeCreate(SqliteConnection connection, string language)`

The next free negative code for `language`, counting down from `-1`.

### `command.CommandText =`

Compared in the store rather than in memory so the whole vocabulary is not read to answer one lookup.
SQLite's NOCASE collation folds ASCII only.
The display names are the pack's.
So a name it declares outside that range still matches when it is typed as declared.
