# QVita.cs

## `internal sealed class QVita`

The driver of the author reading, a veneer page the guild page places.
It draws the vita sheet it is handed and asks the engine for nothing.
The guild driver decides what is read and when the page is shown.

## `internal QVita(UserControl surface)`

Takes the vita page as its surface.
Subscribes the clicks of both lists and attaches the co-author and citation row fills.

## `private StackPanel QVitaBody`

Each part of the page is pulled by its contract ID through `QContract.QContractFind`.

## `internal void QVitaAttach(QWindow host, CGuild guild)`

Keeps the window and the panel the guild driver built, for the two row clicks.

## `internal void QVitaShow(CVita vita, bool held)`

Writes the vita from its sheet: name, counts, fellows and citing places, and shows or hides the body.

## `private void QFellowObserve(object sender, RoutedEventArgs e)`

A co-author row selects that Author in the same panel.

## `private void QVitaCitationObserve(object sender, RoutedEventArgs e)`

A clicked citing place goes to the navigation's gate `CNavigationUsageOpen`, which opens its Example or its Entry.
