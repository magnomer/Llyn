# PCategory.cs

## `public partial class PEditor`

The parts of speech offered under the part-of-speech field, as a menu the editor opens.
The rows are what the draft language's catalog lists, in its order.
Picking one commits its name as a chip.

The menu is an offer, not the field itself.
A pack cannot have thought of everything a user will want to write down.
So the field still takes anything typed.
What is typed anew joins the catalog, so the pack's list is a starting point rather than a ceiling.

The engine narrows the rows to the typed text and marks the taken ones.
Conduct hands the menu ready, so the driver only paints it.

## `private readonly PCategoryTemplate _pCategoryTemplate`

The category dictionary, merged into the editor's resources so its rows draw.

## `private void PCategoryAttach()`

Hands the category list its rows and fill, and ties the marker switch to the category menu.
The menu hangs under the marker field, where a binding named its target.

## `private void PCategoryApply(FrameworkElement container, object item, string? _)`

Fills one category row: its name, its check icon shown only when taken, and its click.

## `private void PCategoryRefine(CCategory category)`

Paints the menu Conduct handed over: its rows, and the notice when a hint key stands in their place.
The key is Conduct's choice, so the notice only looks it up.
The marker's own repaint and its typing hand the menu in, so it follows every draft and every keystroke.

## `private void PCategoryObserve(object sender, RoutedEventArgs e)`

A row clicked in the menu hands its name to the speech gate.
The gate declares the name, so the chip carries the catalog's id.

## `private void PCategoryPickRefine()`

Clears the field and closes the menu after a pick, as a committed Enter clears it.
