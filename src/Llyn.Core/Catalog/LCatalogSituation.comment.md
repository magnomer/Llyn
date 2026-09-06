# LCatalogSituation.cs

## `public sealed record LCatalogSituation(`

One Situation as a browsed row: the stored record and how many places reference it.
The count travels with the row because the ordering reads it and the row shows it.

**Parameters**

- `LCatalogSituationStored` — The stored Situation the row stands for.
- `LCatalogSituationUsage` — How many Meanings and Collocations reference it.

## `public static LCatalogSituation LCatalogSituationCreate(LSituation situation, int usage)`

Builds the row from the stored Situation and the number of places referencing it.

## `public static IReadOnlyList<LCatalogSituation> LCatalogSituationSort(IReadOnlyList<LCatalogSituation> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the title where the ordering is not one a Situation answers to.
A Situation with no stated kind is ordered as empty, which puts it before every stated kind.

## `public bool LCatalogSituationMatch(string query)`

Whether one Situation answers the query, over its title, its description and its kind.
