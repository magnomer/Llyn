# QDrawer.cs

## `internal sealed class QDrawer`

The citation drawer under the corpus transcript's citation field.
It holds the offered rows and the row the arrow keys stand on, apart from the driver.
The driver reads its state as arguments, so no driver field decides a request.

## `internal QDrawer(Popup popup, Border sheet, ListBox view, MouseButtonEventHandler press)`

Binds the list to the drawer's own rows and places the popup by its own callback.
Each row's press is the driver's handler, subscribed by the row fill.

## `internal bool QDrawerShown`

Whether the drawer stands open.

## `internal void QDrawerShow(IReadOnlyList<CCitationRow> rows, TextBox field)`

Lists the rows the deportment answered, or shuts the drawer when there are none.
The drawer opens under the field's frame and is at least as wide as it.
No row is chosen on opening, so Enter commits the typed line until an arrow is pressed.

## `internal void QDrawerHide()`

Shuts the drawer, drops the chosen row and empties the list.

## `internal long? QDrawerChosenRead()`

The Source of the row the arrow keys stand on, or null while none is chosen.

## `internal void QDrawerMove(bool down)`

Steps the chosen row one down or up, wrapping at either end.
The first step down from no choice lands on the first row, and the first step up on the last.

## `private void QDrawerChosenSet(int chosen)`

Keeps the chosen index and shows it as the list's selection.
The drawer owns the index, so no step is computed over the control's own selection.

## `private static CustomPopupPlacement[] QDrawerPlace(Size popup, Size target, Point offset)`

The list hangs under the field's frame, and flips above it when the window ends first.
The shade is pulled back so the frame edges line up with the field.

## `private static FrameworkElement? QDrawerFrameFind(TextBox box)`

The frame the field's template draws, so the drawer lines up with it rather than with the bare text.
