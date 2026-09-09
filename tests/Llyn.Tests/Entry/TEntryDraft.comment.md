# TEntryDraft.cs

## `public sealed class TEntryDraft`

Covers what the draft model can carry between the store and the app.
A save and a load are inverses, so anything a card holds must make the trip in both directions.
The tests here are about that trip and not about any one archive.
Every later stage of the markup work leans on it.

## `public void EntryDraft_FrameWithoutExample_SurvivesSave()`

A card row stating a frame and no sentence is real data and is stored as one.
It comes back naming no Example, and saving it again neither invents one nor drops the row.

## `public void EntryDraft_ExampleSharedByTwoCards_SavesOneRow()`

Two cards quoting one sentence are two rows naming one Example.
Saving them writes one Example row, because the id the rows carry is what names it.
A sense and a collocation quote on identical terms, so the test uses one of each.

## `public void EntryDraft_NestedSense_KeepsTree()`

A Meaning nested under a Meaning nested under a Meaning loads as the tree it is stored as.
Position is per sibling group, which is what the store's unique index counts.
Saving the loaded draft back leaves every parent and every position as it was.

## `public void EntryDraft_NestedCollocation_Refuses()`

A Collocation carrying a card inside it is refused rather than saved with the card dropped.
Only a Meaning nests, and the format cannot write a nested Collocation either.

## `public void EntryDraft_CardFields_SurviveSave()`

The fields the card draft used to collapse or drop make the trip whole.
Those are a Situation's three fields, a Register's language, a card's Translations, and a video's span.
They also include a Meaning's gloss, its definition language and its labels.
No control on the card shows most of them, so the trip is the only thing keeping them.

## `public void EntryDraft_UntouchedEntry_WritesNoChange()`

A load followed by a save with no edit in between leaves every stored row exactly as it was.
The entry used carries one of everything a card can hold, including a nested Meaning.
Row identity counts and not merely row content, because an id is what other rows point at.
Bookkeeping the format deliberately does not carry is left out of the comparison.

## Inline notes

### `private static readonly string[] TEntryDraftQuery =`

Every table a card's content reaches, read whole rather than counted.
A count would pass while a column quietly changed under it.
The entry row is read by its named columns, because its timestamps move on every save by design.

### `private static IReadOnlyList<string> TEntryTableRead(TWorkspace workspace)`

The workspace as one comparable value, one table at a time.
Rows are sorted within their own table, so a renumber that changed nothing reads as nothing.
The query stands in the list beside its rows, so a difference names the table it is in.
