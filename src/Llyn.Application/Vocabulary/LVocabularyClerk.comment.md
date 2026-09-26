# LVocabularyClerk.cs

## `public sealed class LVocabularyClerk`

The clerk over the controlled vocabulary.
It covers the parts of speech a language uses and the morphology features each of them takes.
Lexical rows store only stable ids.
An Entry's part of speech is `noun`, not the word "noun".
What an id is called in a language is resolved here.
So a display name is never copied onto an entry's rows, and renaming one renames it everywhere.
The one exception is deliberate.
The part-of-speech field is editable, so text no preset declares is stored as text.
A part of speech a pack has not thought of is still the one the user meant.

The vocabulary is seeded from the language packs, not from anything compiled in.
Every pack on disk declaring a `vocabulary.json` is written into the workspace when the engine binds to it.
The rows are keyed by `(language, pack_code)` and their parents.
So a second run rewrites the same rows, keeps their ids, and adds nothing.

## `public LVocabularyClerk(LRig rig)`

Reads the root, entry, language, morphology, sentence and speech ports out of `rig`.

## `public IReadOnlyList<LSpeechValue> LSpeechRead(string language)`

Reads the parts of speech `language` declares, in the order its pack lists them.
Those are the presets the shell's part-of-speech field offers in its dropdown.
A language whose pack declares none returns an empty list, and the field is still editable.

## `public LSpeechValue? LSpeechAdd(string language, string name)`

Takes a part of speech no preset declares and makes it one of `language`'s presets.
A name a preset already carries is returned as it stands rather than declared twice.
Matching is on the trimmed name, without case, because the field is typed into by hand.
A new preset takes the next free order and a negative code, outside the space a pack owns.
It returns the preset the name now stands for, or `null` when either argument is blank.

## `public static IReadOnlyList<string> LSpeechShow(IReadOnlyList<LSpeechDraft> drafts)`

The names of the parts of speech, for the surfaces that show words rather than ids.
A draft that names nothing shows nothing.

## `public IReadOnlyList<LSpeech> LSpeechResolve(string language, IReadOnlyList<LSpeechDraft>? drafts)`

The parts of speech a draft carries, as the rows the store keeps.
A draft linking a value row that still exists is stored under that link untouched.
A draft linking a value row that is gone is refused rather than quietly dropped or rebound to its text.
A draft carrying only typed text is looked up in the language's vocabulary first.
Text that names a value is filed under that row, and text that names none is kept as typed.

## `public void LSpeechUpdate(long entryId, LEntryDraft draft, List<LRevisionDelta> changes)`

The entry's part of speech reconciled to the draft.
The assignments are one owned set with an (entry_parent, position) identity and nothing referencing them.
So they are replaced wholesale rather than matched row by row.
A field cleared leaves the entry with no assignment, which is the set replaced by an empty one.
A field that comes back the same row writes nothing and records no change.

## `public string LSpeechFormat(IReadOnlyList<LSpeech> speeches)`

The parts of speech as words, for the revision change text.
A linked value shows its current name, and custom text shows itself.

## `public static bool LSpeechMatch(IReadOnlyList<LSpeech> one, IReadOnlyList<LSpeech> other)`

Two sets of assignments compared as they are stored.
A row is the same row when it links the same value row at the same position.
It is also the same when it carries the same typed text at the same position.
The generated record equality would compare the entry id too, which is empty on the way in.

## `public LSentenceOrder LSentenceOrderRead(string language)`

Reads which of the two Example frame fields `language` writes first, from that language's pack.
A language naming neither, or naming no pack at all, reads back the marker first.

## `public IReadOnlyList<string> LSentenceParticleRead(string language)`

Reads the markers already saved under Entries written in `language`.
An unnamed language reads back nothing rather than every marker of every language.

## `public IReadOnlyList<string> LSentenceDependenceRead(string language)`

Reads the roles already saved under Entries written in `language`, on the same terms as a marker.

## `public void LLanguageImport()`

Every language pack's vocabulary written into the workspace, in one session.
The parts of speech go first, then the features under them, then the values under those.
A pack names its parents by code, and the import maps each to the row id it was given.
A feature or value whose parent the pack never declared is skipped.
Each row is keyed by its parent and code.
So writing it again updates the wording and keeps the row id.
A pack that declares no vocabulary contributes nothing and is not an error.
