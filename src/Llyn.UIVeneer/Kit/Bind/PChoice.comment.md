# PChoice.cs

## `internal static class PChoice`

The marking of a dropdown's options against a stored value.
A sorting dropdown opens with one of its rows marked.
A filter dropdown opens with one row per loaded language, ticked unless the stored filter hides it.
The row marked in the file is the one the user last clicked.
A panel restoring a stored ordering therefore has to move the mark as well as reorder its list.

## `private static readonly LReferenceKind[] PChoiceMenuOrder =`

The order the kind chip menu lists the kinds in, with the unspecified row first and the unknown row last.

## `private static readonly IReadOnlyDictionary<LCatalogOrder, string> PChoiceOrderName =`

The resource word of each ordering, joined to a panel's prefix to name the row's label.
The words are the veneer's own resource keys, so no panel spells an ordering in markup.

## `internal static void PChoiceOrderBuild(Panel list, string prefix, RoutedEventHandler handler, IReadOnlyList<LCatalogOrder> orders)`

Fills `list` with one row per ordering in `orders`, tagged with the ordering itself.
Each row's label is the resource `prefix` names joined to the ordering's word.
Every row reports its click to `handler`, which is the panel's own order handler.
The rows are built here so eleven panels share one row shape.
Each hands the deportment the enum, not a word.

## `internal static void PChoiceOrderApply(Popup dropdown, LCatalogOrder order)`

Marks the row of `dropdown` whose tag is `order`, and unmarks every other row.
Nothing is marked when no row offers that ordering.

## `internal static void PChoiceFilterBuild(Panel list, IReadOnlyList<string> languages, LCatalogFilter filter, RoutedEventHandler handler)`

Fills `list` with one ticked row per language in `languages`, unticking those `filter` hides.
Every row reports its click to `handler`, which is the panel's own filter handler.
The rows are built here so six panels share one row shape and one reading of a stored filter.
`LChoice` reads the ticked rows back, so the deportment owns the filter a click stands for.

## `internal static void PChoiceKindBuild(Panel list, LCatalogFilter filter, RoutedEventHandler handler)`

Builds one ticked box per Source kind, its stored word as the tag and its localized name as the content.
The kind's word is read twice rather than held, so no local carries it into the filter match.

## `internal static void PChoiceMenuBuild(Panel list, RoutedEventHandler handler)`

Builds one option row per Source kind for the imprint's kind chip, unknown last as the menu has always read.
The stored word is the tag and the localized name the content, as the kind filter builds them.

## `internal static void PChoiceMenuApply(Panel list, string tag)`

Marks the kind row whose tag is `tag` and unmarks every other, so the chip menu shows the held kind.

## `private static Grid PChoiceRowBuild(string language)`

The content of one language row, shaped like a row of the language menu.
That is its flag, or a bare ring where the pack has none, then its name.

## `private static IEnumerable<RadioButton> PChoiceButtonScan(DependencyObject? root)`

Walks the logical tree under `root` and returns every option row it holds.
A dropdown wraps its rows in a border and a stack.
The walk spares each panel from knowing that shape.
