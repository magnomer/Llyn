# LCatalogSituation.cs
Hash: `110dfe7ad39010ee`

## `public sealed record LCatalogSituation(LSituation LCatalogSituationStored, int LCatalogSituationUsage, bool LCatalogSituationChosen = false)`

One Situation as a browsed row: the stored record and how many places reference it.
The count travels with the row because the ordering reads it and the row shows it.

**Parameters**

- `LCatalogSituationStored` — The stored Situation the row stands for.
- `LCatalogSituationUsage` — How many Meanings and Collocations reference it.
- `LCatalogSituationChosen` — True on the row of the Situation the vista stands on, false until the vista find fills it.

## `public string LCatalogSituationName { get; init; }`

The title as the row shows it, which the vista find words and numbers apart from its twins.
Sort and match read the stored title instead.

## `public string LCatalogSituationKind { get; init; }`

The kind as the row shows it, empty when none is stated.
The vista find words an unknown kind with the wording the panel hands in.

## `public string LCatalogSituationCount => LCatalog.LCatalogUsageFormat(LCatalogSituationUsage)`

The usage worded through the one shared rule, blank for an unused Situation.

## `public static LCatalogSituation LCatalogSituationCreate(LSituation situation, int usage)`

Builds the row from the stored Situation and the number of places referencing it.

## `public static IReadOnlyList<LCatalogSituation> LCatalogSituationSort(IReadOnlyList<LCatalogSituation> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the title where the ordering is not one a Situation answers to.
A Situation with no stated kind is ordered as empty, which puts it before every stated kind.
Rows equal under the kind go by title, then id.
Rows equal under the usage go by the shared rule `LCatalog.LCatalogUsageSort`, the one the Register catalog uses.
A typed field's offer reads this usage ordering, so equal counts there are ordered by title too.
The id comes last and only keeps the result stable, so storage order never decides a place.

## `public bool LCatalogSituationMatch(string query)`

Whether one Situation answers the query, over its title, its description and its kind.
