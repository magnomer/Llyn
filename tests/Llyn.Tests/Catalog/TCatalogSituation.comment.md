# TCatalogSituation.cs

## `public sealed class TCatalogSituation`

Covers the Situation catalog: what a Situation is ordered by and what it answers.
It covers title order and kind order.
It covers usage order, which counts the Meanings and Collocations referencing the Situation.
It covers the query, over the title, the description and the kind.

## Inline notes

### `private static LEntry TCatalogEntryCreate(`

One saved entry whose Meaning and Collocation reference the stored Situations given for each.

### `private static List<LSituationDraft> TCatalogDraftRead(IReadOnlyList<LSituation> situations)`

The chips a form holds for stored Situations, so the save picks them rather than minting new ones.
