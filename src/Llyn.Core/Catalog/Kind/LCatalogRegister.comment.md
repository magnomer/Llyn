# LCatalogRegister.cs

## `public sealed record LCatalogRegister(`

One Register as a browsed row: the stored record and how many cards are marked with it.
The count travels with the row because the ordering reads it and the row shows it.

**Parameters**

- `LCatalogRegisterStored` — The stored Register the row stands for.
- `LCatalogRegisterUsage` — How many Meanings and Collocations are marked with it.

## `public static LCatalogRegister LCatalogRegisterCreate(LRegister register, int usage)`

Builds the row from the stored Register and the number of cards marked with it.

## `public static IReadOnlyList<LCatalogRegister> LCatalogRegisterSort(IReadOnlyList<LCatalogRegister> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the name where the ordering is not one a Register answers to.
A Register nothing is marked with counts as zero, which puts it last under the usage ordering.

## `public bool LCatalogRegisterMatch(string query)`

Whether the row answers a typed query by its name.
An empty query is answered by every row.
