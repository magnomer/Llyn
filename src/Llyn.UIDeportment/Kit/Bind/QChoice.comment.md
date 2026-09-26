# QChoice.cs

## `internal static class QChoice`

The marking of a dropdown's options against a stored value.
A sorting dropdown opens with one of its rows marked.
A filter dropdown opens with one row per loaded language, ticked unless the stored filter hides it.
The row marked in the file is the one the user last clicked.
A panel restoring a stored ordering therefore has to move the mark as well as reorder its list.
It also reads the rows back into the filter or ordering they stand for.
So no veneer handler holds the loop.

## `internal static void QChoiceOrderBuild(Panel list, string prefix, RoutedEventHandler handler, IReadOnlyList<LCatalogOrder> orders)`

Fills `list` with one row per ordering in `orders`, tagged with the ordering's stored word.
The word comes from `LCatalog`, so the surface holds a plain string and never the ordering.
Each row's label is the resource `prefix` names joined to that word.
Every row reports its click to `handler`, which is the panel's own order handler.
The rows are built here so eleven panels share one row shape.
Each hands the deportment the enum, not a word.

## `internal static void QChoiceDropperAttach(ToggleButton dropper, Popup dropdown, UIElement anchor)`

Ties a dropper to its popup, which markup bindings did before.
Checking the dropper opens the popup and unchecking closes it.
A popup that closes itself on an outside click unchecks the dropper.
The anchor is where the popup is placed, the search bar or the dropper itself.

## `internal static void QChoiceOrderApply(Popup dropdown, LCatalogOrder order)`

Marks the row of `dropdown` whose tag is the stored word of `order`, and unmarks every other row.
Nothing is marked when no row offers that ordering.

## `internal static void QChoiceFilterBuild(Panel list, IReadOnlyList<string> languages, LCatalogFilter filter, RoutedEventHandler handler)`

Fills `list` with one ticked row per language in `languages`, unticking those `filter` hides.
Every row reports its click to `handler`, which is the panel's own filter handler.
The rows are built here so six panels share one row shape and one reading of a stored filter.
`QChoiceFilterRead` reads the ticked rows back, so the deportment owns the filter a click stands for.

## `internal static void QChoiceKindBuild(Panel list, LCatalogFilter filter, RoutedEventHandler handler)`

Builds one ticked box per Source kind, its stored word as the tag and its localized name as the content.
The kind's word is read twice rather than held, so no local carries it into the filter match.

## `internal static void QChoiceMenuBuild(Panel list, RoutedEventHandler handler, IReadOnlyList<LReferenceKind> kinds)`

Builds one option row per kind in `kinds` for the imprint's kind chip, in the order given.
The imprint hands in `LReference.LReferenceKindMenu`, so the order lives with the kinds.
The stored word is the tag and the localized name the content, as the kind filter builds them.

## `internal static void QChoiceMenuApply(Panel list, string tag)`

Marks the kind row whose tag is `tag` and unmarks every other, so the chip menu shows the held kind.

## `internal static LCatalogFilter QChoiceFilterRead(Panel list)`

The filter the rows of `list` now stand for: every unticked language is hidden.
A list with nothing unticked reads as the shared empty filter.

## `internal static LCatalogOrder? QChoiceOrderRead(object sender)`

The ordering an order row's stored word names, or null when the sender is not such a row.
The deportment takes the null and keeps the ordering it has, so the handler holds no branch.
Every browse panel reads its order rows here, so the read has one home.

## `private static string QChoiceKeyRead(string prefix, string word)`

The resource key of an order row's label, the panel's prefix joined to the ordering's word.

## `private static void QChoiceWordApply(Popup dropdown, string word)`

Marks the row whose tag is `word` and unmarks every other.

## `private static Grid QChoiceRowBuild(string language)`

The content of one language row, shaped like a row of the language menu.
That is its flag, or a bare ring where the pack has none, then its name.

## `private static IEnumerable<RadioButton> QChoiceButtonScan(DependencyObject? root)`

Walks the logical tree under `root` and returns every option row it holds.
A dropdown wraps its rows in a border and a stack.
The walk spares each panel from knowing that shape.
