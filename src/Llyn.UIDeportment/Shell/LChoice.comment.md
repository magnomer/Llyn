# LChoice.cs

## `public static class LChoice`

Reads a dropdown's rows back into the filter or ordering they stand for.
The veneer builds the rows, and the deportment reads them, so no veneer handler holds the loop.

## `public static LCatalogFilter LChoiceFilterRead(Panel list)`

The filter the rows of `list` now stand for: every unticked language is hidden.
A list with nothing unticked reads as the shared empty filter.

## `public static LCatalogOrder? LChoiceOrderRead(object sender)`

The ordering a dropdown row carries as its tag, or null when the sender is not such a row.
The deportment takes the null and keeps the ordering it has, so the handler holds no branch.
Every browse panel reads its order rows here, so the read has one home.
