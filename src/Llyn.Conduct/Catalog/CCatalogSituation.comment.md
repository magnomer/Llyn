# CCatalogSituation.cs
Hash: `2be1f43f5ab654e1`

## `public sealed record CCatalogSituation(`

One situation a search found, as the Proffer dropdown or the atlas lists it.

**Parameters**

- `CCatalogSituationId`: the stored situation.
- `CCatalogSituationTitle`: the situation's title as shown.
- `CCatalogSituationCount`: how many cards use the situation, worded ready, blank when none.
- `CCatalogSituationKind`: the kind as shown, the unknown wording for an unknown kind.
- `CCatalogSituationChosen`: whether the vista has this situation chosen.
