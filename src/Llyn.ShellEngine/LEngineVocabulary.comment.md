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
The rows are keyed by `(language, pack_id)` and their parents.
So a second run rewrites the same rows, keeps their ids, and adds nothing.
That is why the tables are no longer empty on a fresh workspace.
It is also why adding a language is still a folder rather than a change here.

## `public LSpeechValue LEngineSpeechCreate(LSpeechValue value)`

Adds or replaces one part-of-speech value, keyed by its language and code, and returns it with its row id.

## `public LSpeechValue? LEngineSpeechRead(long id)`

Reads the part-of-speech value with row id `id`, or `null` when no row has it.

## `public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language)`

Reads the parts of speech `language` declares, in the order its pack lists them.
Those are the presets the shell's part-of-speech field offers in its dropdown.
A language whose pack declares none returns an empty list.
The field is still editable.
Nothing to choose from is a language to type into, not a failure.

## `public LSpeechValue? LEngineSpeechAdd(string language, string name)`

Takes a part of speech no preset declares and makes it one of `language`'s presets.
A name a preset already carries is returned as it stands rather than declared twice.
Matching is on the trimmed name, without case, because the field is typed into by hand.
A new preset takes the next free order and a negative code, outside the space a pack owns.
It returns the preset the name now stands for, or `null` when either argument is blank.
This is what lets the dropdown grow with what a user writes.

## `public LSpeechValue? LEngineSpeechFind(string language, string name)`

Resolves the display `name` of a part of speech back to the row `language` declares.
It returns `null` when no preset carries that name.
That is what makes the typed text a custom part of speech rather than a preset.

## `public LFeature LEngineFeatureCreate(LFeature feature)`

Adds or replaces one feature under its part of speech, keyed by code.
It returns the feature with its row id.

## `public LMorphology LEngineMorphologyCreate(LMorphology value)`

Adds or replaces one value under its feature, keyed by code, and returns it with its row id.

## `public IReadOnlyList<LFeature> LEngineFeatureRead(long speechValueId)`

Reads the features the part of speech takes, in display order.

## `public LFeature? LEngineFeatureFind(long speechValueId, string name)`

Resolves the feature `name` names under the part of speech, or `null` when none does.

## `public LMorphology? LEngineMorphologyRead(long id)`

Reads the morphology value with row id `id`, or `null` when no row has it.

## `public IReadOnlyList<LMorphology> LEngineMorphologyScan(long featureId)`

Reads the values the feature takes, in display order.

## `public LMorphology? LEngineMorphologyFind(long featureId, string name)`

Resolves the value `name` names under the feature, or `null` when none does.

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

### `private static IReadOnlyList<string> LEngineSpeechShow(IReadOnlyList<LSpeechDraft> drafts)`

The names of the parts of speech, for the surfaces that show words rather than ids.
A draft that names nothing shows nothing.

### `private IReadOnlyList<LSpeech> LEngineSpeechResolve(long entryId, string language, IReadOnlyList<LSpeechDraft>? drafts)`

The parts of speech a draft carries, as the rows the store keeps.
A draft linking a value row that still exists is stored under that link untouched.
A draft carrying only typed text is looked up in the language's vocabulary first.
Text that names a value is filed under that row.
Text that names none is kept as it was typed.
So renaming the value renames it on every entry, and a user's own wording is never lost.

### `private void LEngineLanguageImport()`

Every language pack's vocabulary written into the workspace, in one session.
The parts of speech go first, then the features under them, then the values under those.
A pack names its parents by code, and the import maps each to the row id it was given.
A feature or value whose parent the pack never declared is skipped.
It runs whenever the engine binds to a workspace.
A workspace may be new.
It may have been created by an older version with fewer packs installed.
It may have had a pack's wording corrected since it was last opened.
Each row is keyed by its parent and code.
So writing it again updates the wording and keeps the row id.

A pack that declares no vocabulary contributes nothing and is not an error.
A language with no morphology to declare is an ordinary language, not a broken pack.
