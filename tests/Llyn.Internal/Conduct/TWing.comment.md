# TWing.cs
Hash: `63643cb3bae7acf3`

## `public sealed class TWing`

Covers one side of the duplex panel end to end on a real workspace.
Opening an entry loads it, raises the loaded event and saves it under the side's own slot.
An open workspace restores each side's saved entry, loads nothing for an empty slot and saves nothing.
The restore closes the shown entry before it loads, and no row opens nothing.
A load that throws reports its key through the envoy and raises nothing.
A fresh side with nothing typed lists nothing and orders by headword.
A workspace change hands the side a fresh vista, and closing the atelier stops the side's playback.
A typed query lists the match and announces the change.
No order keeps the ordering, and a hidden language marks the side filtered.
A key move marks the row without loading or saving, and announces the re-marked rows.
The move starts on the first row with none chosen, steps one row, and stops at either end.
An empty list moves nothing and announces nothing.
The empty notice shows only for a held query that matched nothing.
The shown entry's incoming usages arrive in their Conduct shape, each worded by its owner key.

## `private static CWing TWingQueryPrepare(CAtelier atelier, string query)`

Builds a left side holding `query`, with its rows read as the driver reads them.

## `private static IReadOnlyList<LEntry> TWingTrioSave(LEngine engine)`

Stores three English entries that one query lists in headword order.

## `private static LEntry TWingWaterSave(LEngine engine)`

Stores one English entry the side can find, open and restore.
