# TCatalogSituation.cs
Hash: `2b1d3667214be533`

## `public sealed class TCatalogSituation`

Covers what the Situation catalog orders by and what it answers.
It covers title order and kind order.
It covers usage order, which counts the Meanings and Collocations referencing the Situation.
It covers the query, over the title, the description and the kind.
It covers equal usage and equal kind, which go by title without case and then id.
So rows stored out of order never surface in storage order.

## Inline notes

### `private static LEntry TCatalogEntryCreate(`

One saved entry whose Meaning and Collocation reference the stored Situations given for each.

### `private static List<LSituationDraft> TCatalogDraftRead(IReadOnlyList<LSituation> situations)`

The chips a form holds for stored Situations, so the save picks them rather than minting new ones.

### `private static IReadOnlyList<LCatalogSituation> TCatalogSituationBuild()`

Three rows of one kind and one usage, stored out of title order, two titles differing only in case.
