# LFellow.cs

## `public sealed record LFellow(`

One co-author of a read Author as the engine hands it to the vita, counted and ordered.
The vita copies it into its list and decides nothing.

**Parameters**

- `LFellowId` — The id of the Author credited beside the read one.
- `LFellowName` — The stored name of that Author.
- `LFellowShared` — How many Sources credit the two together.

## `public string LFellowCount`

The shared count as shown, by `LCatalog.LCatalogUsageFormat`.
It is read off the count on every access, because the author clerk raises the count on a copy.
