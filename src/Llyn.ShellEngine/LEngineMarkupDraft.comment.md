# LEngineMarkupDraft.cs

## `public sealed partial class LEngine`

The turn from one parsed `LMarkupEntry` into the `LEntryDraft` the update path consumes.
Every name the file carries is settled here against the workspace, or dropped with an omission.
The four overloads share one name because each settles one kind of link of the same entry.

## `private LEntryDraft LEngineMarkupResolve(LMarkupEntry entry, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions, List<(LMarkupMention, int)> held)`

Turns one parsed entry into the draft the update path consumes, every id 0 except links.
Names that resolve to nothing are dropped and named in an omission.
A speech the pack does not know is kept as a custom speech, as the editor keeps a typed one.
A local file that does not exist is kept as written and named in an omission.
An audio location the location rule refuses is blanked, and an image or video row so refused is dropped.
A mention that names a sense is added to `held` with the entry's line, for the sense pass.
The overloads below each settle one kind of link.

## Inline notes

### `private static LSpeechValue? LEngineMarkupResolve(LSpeechArchive values, string language, string name, List<LMarkupOmission> omissions)`

A part of speech by name within the entry's language, or nothing with an omission.
An inflection's speech goes through here, because a paradigm slot needs a pack value.

### `private LInflection LEngineMarkupResolve(LSpeechArchive values, string language, LMarkupInflection inflection, int position, List<LMarkupOmission> omissions)`

An inflection with its speech and morphology names turned into ids.
A morphology is searched through every feature of the resolved speech.
No speech means no feature to search, so every morphology name is an omission.

### `private IReadOnlyList<LCardDraft> LEngineMarkupResolve(IReadOnlyList<LMarkupCard> cards, LMarkupEntry entry, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions, List<(LMarkupMention, int)> held)`

Cards in file order, numbered from 1, with children resolved the same way.
Situations, registers, tags, images and videos pass through whole, since the update path settles them by text.
