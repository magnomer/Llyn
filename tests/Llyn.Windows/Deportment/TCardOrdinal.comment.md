# TCardOrdinal.cs

## `public sealed class TCardOrdinal`

Covers the number box on a card's position badge as the user sees it.
The box repaints from the card's row painter, reached through the real item watcher of a list.
The card is built on its own STA thread, since the box and the list are WPF objects.
The commit gate that reads the typed text is covered in `TCardList` of the internal suite.
So these cases stand in for the engine's answer with a position change or none.

## `public void CardRowApply_BadgeOpened_ShowsStoredNumber()`

Opening the badge repaints the box with the stored number, whatever text it held before.

## `public void CardPositionHide_TypedText_ShowsStoredNumber()`

Typed text stays in the box while the badge is open.
Hiding the badge, as a cancel does, brings the stored number back.

## `public void CardPositionHide_RefusedMove_ShowsStoredNumber(string text)`

A commit sends the typed text, here past the end or unreadable.
The list refuses it, so the position stays unchanged before the badge hides.
The box then shows the stored number again.

## `public void CardPositionHide_AcceptedMove_ShowsNewNumber()`

A commit sends the typed number and the list moves the card there.
The box then shows the new number after the badge hides.

## `public void CardRowApply_MoveWhileOpen_RepaintsNewNumber()`

A position change while the badge is open repaints the box over the typed text.

## `private static void TCardOrdinalRun(Action<object, TextBox> act)`

Builds a blank card at position three, hangs it in a list on an STA thread, and runs the case.
