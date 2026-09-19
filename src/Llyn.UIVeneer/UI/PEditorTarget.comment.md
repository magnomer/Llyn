# PEditorTarget.cs

## `public partial class PEditor`

The Entries an entry's cards link to, resolved once for the whole form.
A card stores link ids and its field shows words.
Something has to join the two before a card is built.
The engine is asked, because the panel reaches no database of its own.

## Inline notes

### `private IReadOnlyDictionary<long, LTranslationTarget> PEditorTargetRead(LEntryDraft draft)`

Every card's link target across both lists, read once by the held draft's id.
The engine reads the ids off the draft itself, so the panel hands none over.

