# LSpeechArchive.cs

## `public sealed class LSpeechArchive`

Persists and resolves the language-controlled part-of-speech display vocabulary.
An entry's POS rows store only the stable value id.
The display name for a language lives here and is resolved by `(language, value_id)`.
So a name is never copied onto an entry's rows.

## `public LSpeechArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public void LSpeechValueCreate(LSpeechValue value)`

Adds or replaces one vocabulary entry, keyed by `(language, value_id)`, with its display name and display order.

## `public string? LSpeechValueRead(string language, string valueId)`

Resolves the display name for `valueId` in `language`, or `null` when the vocabulary has no such entry.

## `public IReadOnlyList<LSpeechValue> LSpeechValueRead(string language)`

Reads every part of speech `language` declares.
They come in the display order the language pack listed them in.
Those are the presets the part-of-speech field offers.
A language with no pack on disk declares none, which is an empty list rather than a failure.

## `public string? LSpeechValueFind(string language, string name)`

Resolves the part of speech `name` names in `language` back to its stable id.
It returns `null` when the language declares no preset by that name.
Matching is a trimmed, case-insensitive comparison of the display name.
The name arrives as the user typed it.
Picking "Verb, transitive" from the dropdown and typing "verb, transitive" mean the same thing.
Only text that names no preset at all is stored as typed.

## Inline notes

### `command.CommandText =`

Compared in the store rather than in memory so the whole vocabulary is not read to answer one lookup.
SQLite's NOCASE collation folds ASCII only.
The display names are the pack's.
So a name it declares outside that range still matches when it is typed as declared.
