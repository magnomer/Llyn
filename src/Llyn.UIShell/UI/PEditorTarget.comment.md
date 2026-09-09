# PEditorTarget.cs

## `public partial class PEditor`

The Entries an entry's cards link to, resolved once for the whole form.
A card stores link ids and its field shows words, so something has to join the two before a card is built.
The engine is asked, because the panel reaches no database of its own.

## Inline notes

### `private static IReadOnlyDictionary<string, LTranslationTarget> PEditorTargetEmpty =>`

What a form with nothing to resolve is shown from.
An empty form links to nothing, so it asks nothing rather than asking for an empty list.

### `private IReadOnlyDictionary<string, LTranslationTarget> PEditorTargetRead(LEntryDraft draft)`

Every card's link ids across both lists, asked for in one read.
An entry of many cards still asks once.
A workspace that refuses the read leaves the chips off rather than stopping the load.

### `private static void PEditorTargetRead(IReadOnlyList<LCardDraft> cards, List<string> ids)`

Collects the ids one list of cards holds, skipping any the other list already named.
Two cards linking to the same Entry are one id in the read.
