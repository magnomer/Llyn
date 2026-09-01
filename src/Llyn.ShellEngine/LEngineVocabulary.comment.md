# LEngineVocabulary.cs

## `public sealed partial class LEngine`

The controlled-vocabulary half of the engine: the parts of speech a language uses and the morphology features each of them takes. Lexical rows store only stable ids — an Entry's part of speech is `noun`, not the word "noun" — and what an id is called in a language is resolved here, so a display name is never copied onto an entry's rows and renaming one renames it everywhere. The one exception is deliberate: the part-of-speech field is editable, so text no preset declares is stored as text on the entry's row, because a part of speech a pack has not thought of is still the one the user meant. Such a row resolves against nothing and is displayed as it was typed.

The vocabulary is seeded from the language packs, not from anything compiled in. Every pack on disk declaring a `vocabulary.json` is written into the workspace when the engine binds to it, in one session, keyed by `(language, value_id)` so a second run rewrites the same rows rather than adding to them. That is why the tables are no longer empty on a fresh workspace, and why adding a language is still a folder rather than a change here.

## `public void LEngineSpeechCreate(LSpeechValue value)`

Adds or replaces one part-of-speech vocabulary entry, keyed by its language and value id, with its display name and display order.

## `public string? LEngineSpeechRead(string language, string valueId)`

Resolves the display name of the part of speech `valueId` names in `language`, or `null` when that language declares no such part of speech.

## `public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language)`

Reads the parts of speech `language` declares, in the order its pack lists them — the presets the shell's part-of-speech field offers in its dropdown. A language whose pack declares none returns an empty list: the field is still editable, so nothing to choose from is a language to type into, not a failure.

## `public string? LEngineSpeechFind(string language, string name)`

Resolves the display `name` of a part of speech back to the stable id `language` declares it under, or `null` when no preset carries that name — which is what makes the typed text a custom part of speech rather than a preset.

## `public void LEngineMorphologyCreate(LMorphology morphology)`

Adds or replaces one morphology vocabulary row, keyed by its language, part of speech, feature and value, with the display names and order those carry.

## `public LMorphology? LEngineMorphologyRead(`

Resolves one morphology vocabulary row — the feature and value display names and their order — or `null` when the language declares no such row.

## Inline notes

### `private IReadOnlyList<LSpeech> LEngineSpeechResolve(string entryId, string language, string text)`

One typed part of speech as the row (or no row) an entry stores for it. Blank text is no assignment at all; text a preset names is stored as that preset's stable id, so renaming the preset renames it on every entry; text nothing names is stored as typed, because the field is editable and losing what a user wrote is not an option the form offers.

### `private void LEngineLanguageImport()`

Every language pack's vocabulary written into the workspace, in one session: the parts of speech first, then the morphology rows that name them. It runs whenever the engine binds to a workspace, because a workspace may be new, may have been created by an older version with fewer packs installed, or may have had a pack's wording corrected since it was last opened — and each row is keyed by its ids, so writing it again updates the wording instead of duplicating the row.

A pack that declares no vocabulary contributes nothing and is not an error: a language with no morphology to declare is an ordinary language, not a broken pack.
