# PChoice.cs

## `internal static class PChoice`

The marking of a dropdown's options against a stored value.
A sorting dropdown opens with one of its rows marked.
A filter dropdown opens with one row per loaded language, ticked unless the stored filter hides it.
The row marked in the file is the one the user last clicked.
A panel restoring a stored ordering therefore has to move the mark as well as reorder its list.

## `private static readonly LReferenceKind[] PChoiceMenuOrder =`

The order the kind chip menu lists the kinds in, with the unspecified row first and the unknown row last.

## `internal static void PChoiceOrderApply(Popup dropdown, LCatalogOrder order)`

Marks the row of `dropdown` whose tag names `order`, and unmarks every other row.
The tag is the stored word for the ordering, so the dropdown and the workspace file agree on one spelling.
Nothing is marked when no row offers that ordering.

## `internal static void PChoiceFilterBuild(Panel list, IReadOnlyList<string> languages, LCatalogFilter filter, RoutedEventHandler handler)`

Fills `list` with one ticked row per language in `languages`, unticking those `filter` hides.
Every row reports its click to `handler`, which is the panel's own filter handler.
The rows are built here so six panels share one row shape and one reading of a stored filter.

## `internal static void PChoiceKindBuild(Panel list, LCatalogFilter filter, RoutedEventHandler handler)`

Builds one ticked box per Source kind, its stored word as the tag and its localized name as the content.
The kind's word is read twice rather than held, so no local carries it into the filter match.

## `internal static void PChoiceMenuBuild(Panel list, RoutedEventHandler handler)`

Builds one option row per Source kind for the imprint's kind chip, unknown last as the menu has always read.
The stored word is the tag and the localized name the content, as the kind filter builds them.

## `internal static void PChoiceMenuApply(Panel list, string tag)`

Marks the kind row whose tag is `tag` and unmarks every other, so the chip menu shows the held kind.

## `internal static LCatalogFilter PChoiceFilterRead(Panel list)`

The filter the rows of `list` now stand for: every unticked language is hidden.
A list with nothing unticked reads as the shared empty filter.

## `private static Grid PChoiceRowBuild(string language)`

The content of one language row, shaped like a row of the language menu.
That is its flag, or a bare ring where the pack has none, then its name.

## `private static IEnumerable<RadioButton> PChoiceButtonScan(DependencyObject? root)`

Walks the logical tree under `root` and returns every option row it holds.
A dropdown wraps its rows in a border and a stack.
The walk spares each panel from knowing that shape.
