# PChoice.cs

## `internal static class PChoice`

The marking of a dropdown's options against a stored value.
A sorting dropdown opens with one of its rows marked, and the row marked in the file is the one the user last clicked.
A panel restoring a stored ordering therefore has to move the mark as well as reorder its list.

## `internal static void PChoiceOrderApply(Popup dropdown, LCatalogOrder order)`

Marks the row of `dropdown` whose tag names `order`, and unmarks every other row.
The tag is the stored word for the ordering, so the dropdown and the workspace file agree on one spelling.
Nothing is marked when no row offers that ordering.

## `private static IEnumerable<RadioButton> PChoiceButtonScan(DependencyObject? root)`

Walks the logical tree under `root` and returns every option row it holds.
A dropdown wraps its rows in a border and a stack, and the walk spares each panel from knowing that shape.
