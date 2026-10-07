# LCatalogRegister.cs
Hash: `6738e8767039f0da`

## `public sealed record LCatalogRegister(LRegister LCatalogRegisterStored, int LCatalogRegisterUsage, bool LCatalogRegisterChosen = false)`

One Register as a browsed row: the stored record and how many cards are marked with it.
The count travels with the row because the ordering reads it and the row shows it.

**Parameters**

- `LCatalogRegisterStored` — The stored Register the row stands for.
- `LCatalogRegisterUsage` — How many Meanings and Collocations are marked with it.
- `LCatalogRegisterChosen` — True on the row of the Register the vista stands on, false until the vista find fills it.

## `private static readonly Dictionary<string, string> LCatalogRegisterIcons`

The Register names that wear an icon of their own, keyed without regard to case.
The icon follows the name alone, because a Register belongs to no language.

## `public string LCatalogRegisterIcon`

The icon key the row wears, ready for a driver to resolve.
The name is read without its outer spaces.
A name with no icon of its own wears the plain register icon.

## `public static LCatalogRegister LCatalogRegisterCreate(LRegister register, int usage)`

Builds the row from the stored Register and the number of cards marked with it.

## `public static IReadOnlyList<LCatalogRegister> LCatalogRegisterSort(IReadOnlyList<LCatalogRegister> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the name where the ordering is not one a Register answers to.
A Register nothing is marked with counts as zero, which puts it last under the usage ordering.
Equal counts fall back to the name through the shared rule `LCatalog.LCatalogUsageSort`.
Rows still equal go by id, which only keeps the result stable, so storage order never decides a place.
This is the one owner of the Register order, for the tenor panel and for a card's offer alike.

## `public bool LCatalogRegisterMatch(string query)`

Whether the row answers a typed query by its name.
An empty query is answered by every row.
