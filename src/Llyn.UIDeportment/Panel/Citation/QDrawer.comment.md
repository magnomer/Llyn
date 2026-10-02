# QDrawer.cs

## `internal sealed class QDrawer`

The citation drawer under the corpus transcript's citation field.
It holds the offered rows apart from the driver.
The row the arrow keys stand on is the list's own selection.
The pick reads the list's selected row, as the card row's dropdown does.

## `internal QDrawer(Popup popup, Border sheet, ListBox view, MouseButtonEventHandler press)`

Binds the list to the drawer's own rows and places the popup by its own callback.
Each row's press is the driver's handler, subscribed by the row fill.

## `internal bool QDrawerShown`

Whether the drawer stands open.

## `internal void QDrawerShow(CProffer offer, TextBox field)`

Lists the rows the offer carries, or shuts the drawer when the engine's verdict says it stays shut.
The drawer opens under the field's frame and is at least as wide as it.
No row is chosen on opening, so Enter commits the typed line until an arrow is pressed.

## `internal void QDrawerHide()`

Shuts the drawer, drops the chosen row and empties the list.

## `internal void QDrawerMove(bool down)`

Asks the lantern gate for the row one down or up, which wraps at either end.
The first step down from no choice lands on the first row, and the first step up on the last.
An empty list gets no row from the gate, so nothing moves.
The gate reads the list's selection and row count, and the drawer shows its answer.

## `private static CustomPopupPlacement[] QDrawerPlace(Size popup, Size target, Point offset)`

The list hangs under the field's frame, and flips above it when the window ends first.
The shade is pulled back so the frame edges line up with the field.

## `private static FrameworkElement? QDrawerFrameFind(TextBox box)`

The frame the field's template draws, so the drawer lines up with it rather than with the bare text.
