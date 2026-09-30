# QUsageItem.cs

## `internal sealed class QUsageItem`

Presentation item for one citing place in the guild's citations and the lectern's incoming usages.
It carries the Conduct shape of the place for its gate, and plain copies of what it prints.
The owner and title texts are looked up under the keys Conduct chose.
The row says a Meaning or a Collocation carries the Situation, never the whole Entry.
The Entry id is the way from this row to the panel holding that Entry.

## `internal QUsageItem(CUsage usage)`

Builds the row from one citing place in its shape.
The owner and the unknown mark are looked up under the keys Conduct chose.
A title that is not uncertain shows its own text, which is empty when nothing names the side.
The row finds its own flag once for the language through `QEnsignImage`.

## `internal static IReadOnlyList<QUsageItem> QUsageItemBuild(IReadOnlyList<CUsage> usages)`

A plain copy loop over the citing places, one row each.

## `internal static void QUsageItemRefine(FrameworkElement container, object item, string? _)`

Fills one citing place with its flag, name, epithet, title and kind.
The epithet is led by an en space, as the other catalog rows set it apart.
