# QBond.cs
Hash: `d88c00794d12fc9c`

## `internal sealed class QBond`

The driver of a collocation card's Meanings field, split by role.
Each Observe hears one event and calls at most one card-field gate, and each Refine only changes the look.
Attaching, detaching, ordering and the home rule live below Conduct, not here.
`QCard` owns one instance and hands it each card container, which keeps chip work out of `QCard`.

## `internal void QBondIntroduce(CCardField field)`

Holds the Conduct card field whose three sense gates the handlers call.

## `internal void QBondApply(FrameworkElement container, object item, string? _)`

Serves two kinds of container with the item-fill signature of `QLookItem`.
For a card it finds `PCardBond`, binds the card's chips and wires `PBondChooser` and `PBondList`.
A meaning card has no `PCardBond`, so it gets nothing.
For a chip it fills the path and the name and wires the eraser.
The handlers are removed before they are added, since containers are filled again on every change.

## `internal static void QBondRefine(PCard card, IReadOnlyList<CMeaning> senses)`

Shows the card's linked meanings as chips, in link order.
The first chip is the home, and a new link arrives last.
Each meaning becomes a `QBondItem` carrying the card's id.
A chip with the same meaning id, path and name keeps its row, so an unchanged chip is not rebuilt.

## `private static void QBondMenuRefine(ItemsControl bond, IReadOnlyList<QBondItem> rows)`

Shows `rows` in the card's `PBondMenu` and opens it while there is a row.
An empty list shuts it, so opening, picking and Escape all paint through this one method.
The popup and its list are template parts of `PCardBond`, so each card owns its own menu.
The anchor is set here to the card's `PBondChooser`, because markup holds no binding.

## `private static void QBondRowRefine(FrameworkElement container, object item, string? _)`

Fills one menu row with the meaning's path and name.
The row's indent goes on the path, so a sub-meaning's whole line steps in by depth.
Hover comes from the row style and the pointer from markup, so no row gets a handler.

## `private void QBondChooserObserve(object sender, RoutedEventArgs e)`

Asks the card field for every meaning of the entry, drafts included, and hands the answer to the menu.
The answer reaches the menu as `QBondItem` rows, so no menu row holds a Conduct meaning.

## `private void QBondMenuObserve(object sender, MouseButtonEventArgs e)`

Links the picked meaning to the card and shuts the menu first.
The list hears the click once for all rows, and the row is the item under the pointer.
The card comes from the picked row's own card id, never from a remembered field or the menu's anchor.

## `private void QBondKeyObserve(object sender, KeyEventArgs e)`

Shuts an open menu on Escape, since keyboard focus stays on the chooser while the popup shows.
An Escape with the menu shut passes on untouched.
An outside click needs no handler, because the popup does not stay open.

## `private void QBondEraserObserve(object sender, RoutedEventArgs e)`

Unlinks the chip's meaning from the card.
The chip carries both its card's id and its meaning's id, so no control is searched for the card.
