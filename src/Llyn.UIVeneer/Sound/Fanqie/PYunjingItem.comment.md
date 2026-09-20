# PYunjingItem.cs

## `internal sealed class PYunjingItem`

One category as a row of the onset or rime column: its key, 來 or 寒, and its entry count.
Both columns share the row, since an initial and a rime are the same kind of thing to choose from.
The row remembers its side, so a click can say which column it came from.
The mark saying whether the row is chosen is the one thing that changes after the row is built.

## `internal static IReadOnlyList<PYunjingItem> PYunjingItemBuild(IReadOnlyList<LDiwei> rows)`

A plain copy loop over one column's rows.

## `internal static bool PYunjingItemMatch(PYunjingItem held, PYunjingItem fresh)`

Whether a held row is the same row as a fresh one, so the splice keeps it.

## `internal static void PYunjingItemSync(PYunjingItem held, PYunjingItem fresh)`

Carries the chosen mark from the fresh row onto the kept one.
