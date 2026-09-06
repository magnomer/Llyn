# LEngineVocabulary.cs

## `public sealed partial class LEngine`

The controlled-vocabulary half of the engine.
It covers the parts of speech a language uses.
It also covers the morphology features each of them takes.
Lexical rows store only stable ids.
An Entry's part of speech is `noun`, not the word "noun".
What an id is called in a language is resolved here.
So a display name is never copied onto an entry's rows.
Renaming one renames it everywhere.
The one exception is deliberate.
The part-of-speech field is editable, so text no preset declares is stored as text.
A part of speech a pack has not thought of is still the one the user meant.
Such a row resolves against nothing and is displayed as it was typed.

The vocabulary is seeded from the language packs, not from anything compiled in.
Every pack on disk declaring a `vocabulary.json` is written into the workspace.
It happens when the engine binds to it, in one session.
The rows are keyed by `(language, value_id)`.
So a second run rewrites the same rows rather than adding to them.
That is why the tables are no longer empty on a fresh workspace.
It is also why adding a language is still a folder rather than a change here.

## `public void LEngineSpeechCreate(LSpeechValue value)`

Adds or replaces one part-of-speech vocabulary entry, keyed by its language and value id.
It carries its display name and display order.

## `public string? LEngineSpeechRead(string language, string valueId)`

Resolves the display name of the part of speech `valueId` names in `language`.
It returns `null` when that language declares no such part of speech.

## `public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language)`

Reads the parts of speech `language` declares, in the order its pack lists them.
Those are the presets the shell's part-of-speech field offers in its dropdown.
A language whose pack declares none returns an empty list.
The field is still editable.
Nothing to choose from is a language to type into, not a failure.

## `public string? LEngineSpeechFind(string language, string name)`

Resolves the display `name` of a part of speech back to the stable id `language` declares.
It returns `null` when no preset carries that name.
That is what makes the typed text a custom part of speech rather than a preset.

## `public void LEngineMorphologyCreate(LMorphology morphology)`

Adds or replaces one morphology vocabulary row.
It is keyed by its language, part of speech, feature and value.
It carries the display names and order those hold.

## `public LMorphology? LEngineMorphologyRead(`

Resolves one morphology vocabulary row, the feature and value display names and their order.
It returns `null` when the language declares no such row.

## `public LSentenceOrder LEngineOrderRead(string language)`

Reads which of the two Example frame fields `language` writes first, from that language's pack.
A language naming neither, or naming no pack at all, reads back the marker first.

## `public IReadOnlyList<string> LEngineParticleRead(string language)`

Reads the markers already saved under Entries written in `language`.
Nothing ships a marker, so an untouched workspace offers none and the field is a plain box.
An unnamed language reads back nothing rather than every marker of every language.

## `public IReadOnlyList<string> LEngineDependenceRead(string language)`

Reads the roles already saved under Entries written in `language`, on the same terms as a marker.

## Inline notes

### `private IReadOnlyList<LSpeech> LEngineSpeechResolve(string entryId, string language, string text)`

One typed part of speech as the row (or no row) an entry stores for it.
Blank text is no assignment at all.
Text a preset names is stored as that preset's stable id.
So renaming the preset renames it on every entry.
Text nothing names is stored as typed, because the field is editable.
Losing what a user wrote is not an option the form offers.

### `private void LEngineLanguageImport()`

Every language pack's vocabulary written into the workspace, in one session.
The parts of speech go first, then the morphology rows that name them.
It runs whenever the engine binds to a workspace.
A workspace may be new.
It may have been created by an older version with fewer packs installed.
It may have had a pack's wording corrected since it was last opened.
Each row is keyed by its ids, so writing it again updates the wording.

A pack that declares no vocabulary contributes nothing and is not an error.
A language with no morphology to declare is an ordinary language, not a broken pack.
